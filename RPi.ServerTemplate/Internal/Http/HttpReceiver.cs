using System;
using System.Net;

namespace RPiServerTemplate.Internal.Http
{
    internal class HttpReceiver : IDisposable
    {
        public event EventHandler HttpError;

        private readonly HttpListener listener;

        public HttpRouteCollection Routes {get;}
        public string RootPath {get;}


        public HttpReceiver(string prefix, string root)
        {
            RootPath = root;

            listener = new HttpListener();
            listener.Prefixes.Add(prefix);

            Routes = new HttpRouteCollection();
        }

        public void Dispose()
        {
            Stop();
            listener?.Close();
        }

        public void Start()
        {
            listener.Start();
            Wait();
        }

        public void Stop()
        {
            listener.Stop();
        }

        private void Wait()
        {
            var state = new object();
            listener.BeginGetContext(OnContextReceived, state);
        }

        private void OnContextReceived(IAsyncResult result)
        {
            HttpListenerContext context;
            try {
                context = listener.EndGetContext(result);
            }
            catch (Exception error) {
                OnHttpError(error);
                return;
            }
            finally {
                if (listener.IsListening) Wait();
            }

            try {
                RouteRequest(context);
            }
            catch (Exception error) {
                OnHttpError(error);
            }
        }

        private void RouteRequest(HttpListenerContext context)
        {
            Console.WriteLine($"Request received from '{context.Request.RemoteEndPoint}'.");

            var path = context.Request.Url.AbsolutePath;

            if (path.StartsWith(RootPath))
                path = path.Substring(RootPath.Length);

            try {
                HttpHandlerResult result;
                if (Routes.TryFind(path, out RouteEvent routeAction)) {
                    try {
                        result = routeAction.Invoke(context);
                    }
                    catch (Exception error) {
                        result = HttpHandlerResult.Exception(error);
                    }
                }
                else {
                    result = HttpHandlerResult.NotFound()
                        .SetText($"No handler found matching path '{path}'!");
                }

                result.Apply(context);
            }
            finally {
                try {
                    context.Response.Close();
                }
                catch {}
            }
        }

        protected virtual void OnHttpError(Exception error)
        {
            try {
                HttpError?.Invoke(this, new EventArgs());
            }
            catch {}
        }
    }
}
