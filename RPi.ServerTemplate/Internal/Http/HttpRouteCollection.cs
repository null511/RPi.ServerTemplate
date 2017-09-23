using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace RPiServerTemplate.Internal.Http
{
    internal delegate HttpHandlerResult RouteEvent(HttpListenerContext context);

    internal class HttpRouteCollection
    {
        public Dictionary<string, RouteEvent> RouteList {get;}


        public HttpRouteCollection()
        {
            RouteList = new Dictionary<string, RouteEvent>();
        }

        public bool TryFind(string path, out RouteEvent action)
        {
            return RouteList.TryGetValue(path, out action);
        }

        public void Scan(Assembly assembly)
        {
            var typeList = assembly.DefinedTypes
                .Where(t => t.IsClass && !t.IsAbstract);

            foreach (var classType in typeList) {
                var attr = classType.GetCustomAttribute<HttpHandlerAttribute>();
                if (attr == null) continue;

                RouteList[attr.Path] = (context) => {
                    var handler = Activator.CreateInstance(classType) as HttpHandler;
                    if (handler == null) throw new ApplicationException($"Unable to construct HttpHandler implementation '{classType.Name}'!");

                    HttpHandlerResult result = null;
                    switch (context.Request.HttpMethod.ToUpper()) {
                        case "GET":
                            result = handler.Get(context);
                            break;
                        case "POST":
                            result = handler.Post(context);
                            break;
                    }

                    return result;
                };
            }
        }
    }
}
