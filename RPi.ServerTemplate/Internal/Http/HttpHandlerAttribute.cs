using System;

namespace RPiServerTemplate.Internal.Http
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class HttpHandlerAttribute : Attribute
    {
        public string Path {get; set;}


        public HttpHandlerAttribute(string path)
        {
            this.Path = path;
        }
    }
}
