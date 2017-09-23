using System.Net;

namespace RPiServerTemplate.Internal.Http
{
    internal abstract class HttpHandler
    {
        public virtual HttpHandlerResult Get(HttpListenerContext context)
        {
            return NotFound();
        }

        public virtual HttpHandlerResult Post(HttpListenerContext context)
        {
            return NotFound();
        }

        public virtual HttpHandlerResult Head(HttpListenerContext context)
        {
            return NotFound();
        }

        public virtual HttpHandlerResult Options(HttpListenerContext context)
        {
            return NotFound();
        }

        public HttpHandlerResult Ok()
        {
            return HttpHandlerResult.Ok();
        }

        public HttpHandlerResult NotFound()
        {
            return HttpHandlerResult.NotFound();
        }

        public HttpHandlerResult ExternalView(string name, object param)
        {
            return HttpHandlerResult.ExternalView(name, param);
        }
    }
}
