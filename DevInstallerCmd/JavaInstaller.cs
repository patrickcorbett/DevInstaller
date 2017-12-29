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
    internal class JavaInstaller
    {
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private const int WM_SETTINGCHANGE = 0x1a;
        private const int SMTO_ABORTIFHUNG = 0x0002;
        
        private String javaInstallPath;

        private DevInstaller devInstaller;

        public JavaInstaller(DevInstaller pInstaller)
        {
            this.devInstaller = pInstaller;
            // set the java install path
            this.javaInstallPath = devInstaller.InstallDirectory + @"\software\java";
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);

        public void install(string[] args)
        {
            // lookup the java version
            String javaVersion = findJavaVersion();
            if (javaVersion == null)
            {
                // java version was not found find the java executable
                String javaExe = findJavaJDKs();
                
                // install java
                if (javaExe != null)
                {
                    if (installJavaJDK(javaExe)) {
                        // java was installed
                        setJavaHome();
                    }
                } else
                {
                    Console.WriteLine("Java Executable was not found, Skipping Installation");
                }
            }
            else
            {
                Console.WriteLine("Java '" + javaVersion + "' is installed");
                Console.WriteLine("Java is already installed");

                // get the JAVA_HOME environment variable
                String javaHome = Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.Machine);
                if (javaHome != null)
                {
                    Console.WriteLine("JAVA_HOME is : " + javaHome);
                }
                else
                {
                    setJavaHome();
                }
            }
        }

        private String findJavaJDKs()
        {
            // search in the current folder for a JDK installation file
            Console.WriteLine("The execution path is : " + devInstaller.BaseDirectory);

            String[] jdks = Directory.GetFiles(devInstaller.BaseDirectory, "jdk*");

            String javaExe = null;
            foreach (String jdk in jdks) {
                Console.WriteLine("JDK found : " + jdk);
                javaExe = jdk;
                break;
            }

            return javaExe;
        }

        private Boolean installJavaJDK(String pJavaExe) {
           
            // start the installation
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = pJavaExe;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "/s INSTALLDIR=" + javaInstallPath + @"\jdk /INSTALLDIRPUBJRE=" + javaInstallPath + @"\jre";
            
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();

                    Console.WriteLine("Java was installed");
                }

                return true;
            }
            catch
            {
                // Log error.
                Console.WriteLine("Java was not installed");
                return false;
            }
        }

        private void setJavaHome() {
            
            String javaVersion = findJavaVersion();

            Console.WriteLine("The version is : " + javaVersion);

            // set the JAVA_HOME variable and set it on the System PATH
            if (!EnvironmentVariableUtil.setVariable("JAVA_HOME", javaInstallPath + @"\jdk", true, @"%JAVA_HOME%\bin")) {
                Console.WriteLine("JAVA_HOME was not set");
            }
        }
        
        private String findJavaVersion()
        {
            try
            {
                // get the path environment variable
                String path = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine);

                String javapath = null;
                foreach (String pathPiece in path.Split(';')) {
                    // look for the javapath set by the public JRE that was just installed
                    if (pathPiece.Contains("javapath")) {
                        javapath = pathPiece;
                        break;  
                    }
                }

                if (javapath != null) {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = javapath + "\\java.exe";
                    psi.Arguments = " -version";
                    psi.RedirectStandardError = true;
                    psi.UseShellExecute = false;

                    Process pr = Process.Start(psi);
                    string strOutput = pr.StandardError.ReadLine().Split(' ')[2].Replace("\"", "");

                    return (strOutput);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception is " + ex.Message);
                return null;
            }
        }
    }
}
