using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SimpleWebServer.Extensions;

namespace SimpleWebServer
{
    public class SimpleHttpServer
    {
        private readonly string _rootFolder;
        private readonly HttpListener _listener;

        public SimpleHttpServer(string rootFolder)
        {
            _rootFolder = rootFolder;

            _listener = StartListener(54556);

            Listen();
        }
        public static HttpListener StartListener(int port)
        {
            try
            {
                var l = new HttpListener();
                l.Prefixes.Add(string.Format("http://localhost:{0}/", port));
                l.Start();
                return l;
            }
            catch
            {
                return StartListener(port + 1);
            }
        }
        private async void Listen()
        {
            var context = await Task.Factory.FromAsync<HttpListenerContext>(_listener.BeginGetContext, _listener.EndGetContext, null).Catch(null);
            if (_listener.IsListening)
                Listen();
            else return;

            var path = context.Request.Url.AbsolutePath.Replace(_listener.Prefixes.First(), "");
            if (path == "/")
                path = "/index.html";

            path = Path.Combine(_rootFolder, path.Remove(0, 1));
            context.Response.Headers[HttpResponseHeader.CacheControl] = "NO-CACHE";
            context.Response.Headers[HttpResponseHeader.Expires] = "Sat, 26 Jul 1997 05:00:00 GMT";
            
            if (!File.Exists(path))
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
                return;
            }
            using (var file = File.Open(path, FileMode.Open))
            {
                file.CopyTo(context.Response.OutputStream);
                context.Response.Close();
            }

        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        public string ListeningUrl { get { return _listener.Prefixes.First(); } }
    }
}