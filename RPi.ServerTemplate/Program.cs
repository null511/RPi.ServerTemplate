using Raspberry;
using RPiServerTemplate.Internal;
using RPiServerTemplate.Internal.Http;
using System;
using System.Configuration;
using System.Reflection;

namespace RPiServerTemplate
{
    internal static class Program
    {
        public static PinManager PinMgr {get;}


        static Program()
        {
            PinMgr = new PinManager();
        }

        private static int Main(string[] args)
        {
            Console.ResetColor();
            Console.WriteLine("Connecting Pin Manager...");

            if (Board.Current.IsRaspberryPi) {
                try {
                    PinMgr.Connect();

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("  Pin Manager connected.");
                }
                catch (Exception error) {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"  Failed to connect Pin Manager! {error}");
                }
            }
            else {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"  No Raspberry Pi device detected.");
            }

            Console.ResetColor();
            Console.WriteLine("Starting Http Server...");

            var prefix = ConfigurationManager.AppSettings["http.prefix"];
            var root = ConfigurationManager.AppSettings["http.root"];
            var receiver = new HttpReceiver(prefix, root);

            try {
                receiver.Routes.Scan(Assembly.GetExecutingAssembly());

                receiver.Start();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("  Http Server started successfully.");
            }
            catch (Exception error) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  Failed to start Http Server! {error}");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
                return 1;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Http Server is running. Press any key to stop...");
            Console.WriteLine();
            Console.ReadKey(true);

            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Stopping Http Server...");

            try {
                receiver.Dispose();

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("  Http Server stopped.");
            }
            catch (Exception error) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"  Error while stopping Http Server! {error}");
            }

            Console.ResetColor();
            Console.WriteLine("Closing Pin Manager...");

            try {
                PinMgr.Dispose();

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("  Pin Manager disconnected.");
            }
            catch (Exception error) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"  Error while disconnecting Pin Manager! {error}");
            }

            Console.ResetColor();
            return 0;
        }
    }
}
