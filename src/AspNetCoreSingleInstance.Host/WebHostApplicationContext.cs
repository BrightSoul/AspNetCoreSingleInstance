using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreSingleInstance.Host
{
    public class WebHostApplicationContext : ApplicationContext
    {
        private readonly IHost webHost;
        private readonly NotifyIcon notificationIcon;

        public WebHostApplicationContext(string[] args)
        {
            // Stop the web applicaton and perform cleanup on application exit
            Application.ApplicationExit += (sender, args) => StopWebApp();
            // Create a tray icon
            notificationIcon = CreateNotifyIcon();
            // Create the web host and start the web application
            webHost = StartWebApp(args);
            // Open in the browser, even if the start failed
            OpenWebAppInBrowser();
        }

        private IHost StartWebApp(string[] args)
        {
            var webHost = AspNetCoreSingleInstance.Web.Program.CreateHostBuilder(args).Build();
            // Attempt to start the web Host
            try
            {
                webHost.Start();
                Console.WriteLine("Web application started successfully");
            }
            catch
            {
                Console.WriteLine("Couldn't start the web application");
                // If it couldn't be started, then we exit the host application
                Application.Exit();
            }
            return webHost;
        }

        private void StopWebApp()
        {
            // Hide the notification icon on exit
            notificationIcon.Visible = false;

            // Stop the webhost (if it was running in the first place)
            try
            {
                Console.WriteLine("Stopping web application...");
                var stopTask = webHost.StopAsync(TimeSpan.FromSeconds(10));
                stopTask.ConfigureAwait(false);
                stopTask.Wait();
                Console.WriteLine("Web application was stopped successfully");
            }
            catch
            {
                Console.WriteLine("Couldn't stop the web application in the allotted time");
            }
            finally
            {
                // Web host should be properly disposed
                webHost.Dispose();
            }
        }

        private void OpenWebAppInBrowser()
        {
            var config = webHost.Services.GetService<IConfiguration>();
            var url = config["Kestrel:Endpoints:Http:Url"];
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            try
            {
                Process.Start(processStartInfo);
            }
            catch
            {
                Console.WriteLine("Couldn't launch the browser");
            }
        }

        private NotifyIcon CreateNotifyIcon()
        {
            // Create two commands for the tray icon menu
            var showCommand = new ToolStripMenuItem("Show in browser");
            showCommand.Click += (sender, args) => OpenWebAppInBrowser();

            var exitCommand = new ToolStripMenuItem("Exit");
            exitCommand.Click += (sender, args) => Application.Exit();

            // Create the tray icon itself
            var notifyIcon = new NotifyIcon
            {
                Text = "My ASP.NET Core Application",
                Icon = SystemIcons.Application,
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip
                {
                    Items = { showCommand, exitCommand }
                }
            };

            // If the user double clicks it, then show in the browser
            notifyIcon.DoubleClick += (sender, args) => OpenWebAppInBrowser();

            return notifyIcon;
        }
    }
}