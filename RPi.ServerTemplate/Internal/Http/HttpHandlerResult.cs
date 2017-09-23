using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace RPiServerTemplate.Internal.Http
{
    internal class HttpHandlerResult
    {
        private string Content {get; set;}

        public int StatusCode {get; set;}
        public string StatusDescription {get; set;}


        public HttpHandlerResult SetText(string text)
        {
            Content = text;
            return this;
        }

        public void Apply(HttpListenerContext context)
        {
            context.Response.StatusCode = StatusCode;
            context.Response.StatusDescription = StatusDescription;

            if (Content != null) {
                using (var writer = new StreamWriter(context.Response.OutputStream)) {
                    writer.Write(Content);
                }
            }
        }

        public static HttpHandlerResult Ok()
        {
            return new HttpHandlerResult {
                StatusCode = (int)HttpStatusCode.OK,
                StatusDescription = "OK.",
            };
        }

        public static HttpHandlerResult NotFound()
        {
            return new HttpHandlerResult {
                StatusCode = (int)HttpStatusCode.NotFound,
                StatusDescription = "Not Found!",
            };
        }

        public static HttpHandlerResult Exception(Exception error)
        {
            return new HttpHandlerResult {
                StatusCode = (int)HttpStatusCode.NotFound,
                StatusDescription = "Not Found!",
                Content = error.ToString(),
            };
        }

        public static HttpHandlerResult ExternalView(string name, object param)
        {
            if (Path.DirectorySeparatorChar != '\\')
                name = name.Replace("\\", Path.DirectorySeparatorChar.ToString());

            var filename = Path.GetFullPath(name);

            if (!File.Exists(filename))
                throw new ApplicationException($"External View '{filename}' could not be found!");

            string content;
            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream)) {
                content = reader.ReadToEnd();
            }

            var moustache = new HtmlEngine();
            content = moustache.Process(content, ToDictionary(param));

            return new HttpHandlerResult {
                StatusCode = (int)HttpStatusCode.OK,
                StatusDescription = "OK.",
                Content = content,
            };
        }

        public static HttpHandlerResult EmbeddedView(string name, object param)
        {
            var content = "?";

            return new HttpHandlerResult {
                StatusCode = (int)HttpStatusCode.OK,
                StatusDescription = "OK.",
                Content = content,
            };
        }

        private static IDictionary<string, object> ToDictionary(object parameters, IEqualityComparer<string> comparer = null)
        {
            if (parameters == null) return null;

            var dictionary = parameters as IDictionary<string, object>;
            if (dictionary != null) return dictionary;

            return parameters.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(property => new KeyValuePair<string, object>(property.Name, property.GetValue(parameters)))
                .ToDictionary(x => x.Key, x => x.Value, comparer);
        }
    }
}
