using System;
using System.Collections.Generic;
using System.Text;

namespace RPiServerTemplate.Internal
{
    public class HtmlEngine
    {
        public TagNotFoundBehavior NotFoundBehavior {get; set;}
        public string MoustacheStartChars {get; set;}
        public string MoustacheStopChars {get; set;}


        public HtmlEngine()
        {
            NotFoundBehavior = TagNotFoundBehavior.Source;
            MoustacheStartChars = "{{";
            MoustacheStopChars = "}}";
        }

        public string Process<T>(string text, IDictionary<string, T> valueCollection)
        {
            return ProcessBlock(text, valueCollection);
        }

        private string ProcessBlock<T>(string text, IDictionary<string, T> valueCollection)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (valueCollection == null) throw new ArgumentNullException(nameof(valueCollection));

            var read_pos = 0;
            var result = new StringBuilder();
            while (read_pos < text.Length) {
                string tag;
                int tagStart, tagEnd;
                if (!FindTag(text, read_pos, out tagStart, out tagEnd, out tag)) break;

                result.Append(text, read_pos, tagStart - read_pos);
                read_pos = tagEnd;

                if (tag.StartsWith("#")) {
                    if (!tag.StartsWith("#if ")) {
                        result.Append(text.Substring(tagStart, tagEnd - tagStart));
                        continue;

                        //throw new ApplicationException($"Invalid tag '{tag}'!");
                    }

                    var blockEndStart = text.IndexOf("{{#endif}}", read_pos);
                    if (blockEndStart < 0) continue;

                    var block = text.Substring(read_pos, blockEndStart - read_pos);
                    read_pos = blockEndStart + 10;

                    string trueBlock, falseBlock;

                    var blockElseStart = block.IndexOf("{{#else}}");
                    if (blockElseStart >= 0) {
                        trueBlock = block.Substring(0, blockElseStart);
                        falseBlock = block.Substring(blockElseStart + 9);
                    }
                    else {
                        trueBlock = block;
                        falseBlock = string.Empty;
                    }

                    bool conditionResult = false;

                    var conditionStart = tag.IndexOf(' ');
                    if (conditionStart >= 0) {
                        var condition = tag.Substring(conditionStart+1);

                        if (valueCollection.TryGetValue(condition, out T item_value))
                            conditionResult = GetTruthyValue(item_value);
                    }

                    var blockText = conditionResult ? trueBlock : falseBlock;
                    blockText = ProcessBlock(blockText, valueCollection);
                    result.Append(blockText);
                }
                else {
                    if (valueCollection.TryGetValue(tag, out T item_value)) {
                        result.Append(item_value);
                        continue;
                    }

                    switch (NotFoundBehavior) {
                        case TagNotFoundBehavior.Source:
                            result.Append(text.Substring(tagStart, tagEnd - tagStart));
                            break;
                    }
                }
            }

            if (read_pos < text.Length) {
                result.Append(text, read_pos, text.Length - read_pos);
            }

            return result.ToString();
        }

        private bool FindTag(string text, int startPos, out int tagStartPos, out int tagEndPos, out string tag)
        {
            var tagStart = text.IndexOf(MoustacheStartChars, startPos, StringComparison.Ordinal);
            if (tagStart < 0) {
                tagStartPos = tagEndPos = -1;
                tag = null;
                return false;
            }

            var tagEnd = text.IndexOf(MoustacheStopChars, tagStart, StringComparison.Ordinal);
            if (tagEnd < 0) {
                tagStartPos = tagEndPos = -1;
                tag = null;
                return false;
            }

            tagStartPos = tagStart;
            tagEndPos = tagEnd + MoustacheStopChars.Length;
            tag = text.Substring(tagStart + MoustacheStartChars.Length, tagEnd - tagStart - MoustacheStartChars.Length);
            return true;
        }

        private bool GetTruthyValue(object value)
        {
            var type = value.GetType();
            if (type == typeof(bool))
                return (bool)value;

            if (type == typeof(int))
                return (int)value > 0;

            if (type == typeof(string)) {
                var stringValue = (string)value;
                if (string.Equals(stringValue, "true", StringComparison.OrdinalIgnoreCase)) return true;
                if (string.Equals(stringValue, "false", StringComparison.OrdinalIgnoreCase)) return false;
                if (string.Equals(stringValue, "yes", StringComparison.OrdinalIgnoreCase)) return true;
                if (string.Equals(stringValue, "no", StringComparison.OrdinalIgnoreCase)) return false;

                return !string.IsNullOrEmpty(stringValue);
            }

            return value != null;
        }
    }

    public enum TagNotFoundBehavior
    {
        Empty,
        Source,
    }
}
