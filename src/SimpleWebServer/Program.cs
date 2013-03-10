using System.Diagnostics;
using System.Drawing;
using System.Net.Mime;
using System.Windows.Forms;

namespace SimpleWebServer
{
    class Program
    {
        static void Main()
        {
            var context = new ApplicationContext();
            var server = new SimpleHttpServer(Application.StartupPath);
            var icon = new NotifyIcon
                {
                    Icon = new Icon(SystemIcons.Exclamation, 40, 40),
                    Text = string.Format("Http server started on {0}", server.ListeningUrl),

                };
            icon.ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Stop", (sender, a) =>
                        {
                            server.Stop();
                            icon.Visible = false;
                            context.ExitThread();
                        })
                });

            icon.DoubleClick += (sender, a) => Process.Start(server.ListeningUrl);
            icon.Visible = true;
            Process.Start(server.ListeningUrl);
            Application.Run(context);
        }
    }
}
