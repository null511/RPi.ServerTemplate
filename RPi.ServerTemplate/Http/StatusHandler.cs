using Raspberry;
using RPiServerTemplate.Internal.Http;
using System.Net;

namespace RPiServerTemplate.Http
{
    [HttpHandler("/status")]
    class StatusHandler : HttpHandler
    {
        public override HttpHandlerResult Get(HttpListenerContext context)
        {
            var board = Board.Current;

            return ExternalView("Views\\Status.html", new {
                isPi = board.IsRaspberryPi,
                firmwareVersion = board.Firmware,
                modelNumber = board.Model,
                serialNumber = board.SerialNumber,
                processor = board.Processor.ToString(),
                processorName = board.ProcessorName,
                pinout = board.ConnectorPinout.ToString(),
                overclocked = board.IsOverclocked ? "yes" : "no",
            });
        }
    }
}
