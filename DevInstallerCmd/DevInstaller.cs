using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace DevInstallerCmd
{
    class DevInstaller
    {
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private const int WM_SETTINGCHANGE = 0x1a;
        private const int SMTO_ABORTIFHUNG = 0x0002;
                
        // the execution directory
        private string baseDirectory; 
        public string BaseDirectory { get => baseDirectory; set => baseDirectory = value; }

        // the installation ROOT folder
        private string installDirectory; 
        public string InstallDirectory { get => installDirectory; set => installDirectory = value; }

        public DevInstaller(string baseDirectory, string installDirectory)
        {
            this.BaseDirectory = baseDirectory;
            this.InstallDirectory = installDirectory;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);

        static void Main(string[] args)
        {
            // ask the user for the install directory
            Console.WriteLine("Define the install directory (Default is C:\\develop) :");
            String installDirectory = Console.ReadLine();
            if (string.IsNullOrEmpty(installDirectory)) {
                installDirectory = @"C:\develop";
            }
            
            // create instance of the program
            DevInstaller devInstaller = new DevInstaller(System.AppDomain.CurrentDomain.BaseDirectory, installDirectory);
            
            // install java
            //JavaInstaller javaInstaller = new JavaInstaller(devInstaller);
            //javaInstaller.install(args);

            // install Maven
            MavenInstaller mavenInstaller = new MavenInstaller(devInstaller);
            mavenInstaller.install();

            // complete
            Console.WriteLine("Press any key to continue . . .");
            Console.ReadLine();
        }
        
    }
}
