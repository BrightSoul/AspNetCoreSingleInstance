using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCoreSingleInstance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var mutex = new Mutex(false, "Global\\MyAppName");
            if (ShouldRun(mutex))
            {
                
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MyApplicationContext(args));
                mutex.ReleaseMutex();
            }
        }

        

        private static void ShowNotifyIcon()
        {
            var notifyIcon = new NotifyIcon();
            notifyIcon.Text = "hey";
            notifyIcon.Visible = true;
        }

        private static bool ShouldRun(Mutex mutex)
        {
            return true;
            return mutex.WaitOne(0);
        }
    }
}
