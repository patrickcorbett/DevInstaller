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
            JavaInstaller javaInstaller = new JavaInstaller(devInstaller);
            javaInstaller.install(args);

            // install Maven
            MavenInstaller mavenInstaller = new MavenInstaller(devInstaller);
            mavenInstaller.install();
            
            // install Tomcat
            TomcatInstaller tomcatInstaller = new TomcatInstaller(devInstaller);
            tomcatInstaller.install();

            // install DiffMerge
            DiffMergeInstaller diffMergeInstaller = new DiffMergeInstaller(devInstaller);
            diffMergeInstaller.install();

            // complete
            Console.WriteLine("Press any key to continue . . .");
            Console.ReadLine();
        }
        
    }
}
