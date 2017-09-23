using RPiServerTemplate.Internal.Http;
using System.Net;

namespace RPiServerTemplate.Http
{
    [HttpHandler("/helloworld")]
    class HelloWorldHandler : HttpHandler
    {
        public override HttpHandlerResult Get(HttpListenerContext context)
        {
            return Ok().SetText("Hello World!");
        }
    }
}
