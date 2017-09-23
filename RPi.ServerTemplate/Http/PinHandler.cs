using RPiServerTemplate.Internal.Http;
using System;
using System.Net;

namespace RPiServerTemplate.Http
{
    [HttpHandler("/pin")]
    class PinHandler : HttpHandler
    {
        public override HttpHandlerResult Get(HttpListenerContext context)
        {
            Program.PinMgr.Blink(TimeSpan.FromMilliseconds(600));

            return Ok().SetText("Blink Successful.");
        }
    }
}
