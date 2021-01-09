using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AspNetCoreSingleInstance.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Do we want console output?
            bool hasConsole = args.Contains("--console");
            if (hasConsole) AllocConsole();

            // Run the winforms application (which in turn will run the web application)
            Application.Run(new WebHostApplicationContext(args));

            if (hasConsole) FreeConsole();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int FreeConsole();
    }
}
