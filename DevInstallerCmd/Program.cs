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
    class Program
    {
        private static String javaInstallDir = @"develop\software\java";
        public String currentDir = "";
        public String installDir = "";

        static void Main(string[] args)
        {
            // create instance of the program
            Program p = new Program();

            // set the install directory for java
            p.currentDir = System.AppDomain.CurrentDomain.BaseDirectory;
            p.installDir = @"C:\" + javaInstallDir;

            // lookup the java version
            String javaVersion = p.findJavaVersion();
            if (javaVersion == null)
            {
                // java version was not found find the java executable
                String javaExe = p.findJavaJDKs();
                
                // install java
                if (javaExe != null)
                {
                    if (p.installJavaJDK(javaExe)) {
                        // java was installed
                        p.setJavaHome();
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
                    p.setJavaHome();
                }
            }

            Console.WriteLine("Press any key to continue . . .");
            Console.ReadLine();
        }

        private String findJavaJDKs()
        {
            // search in the current folder for a JDK installation file
            Console.WriteLine("The execution path is : " + currentDir);

            String[] jdks = Directory.GetFiles(@currentDir, "jdk*");

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
            startInfo.Arguments = "/s INSTALLDIR=" + installDir + @"\jdk /INSTALLDIRPUBJRE=" + installDir + @"\jre";
            
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

        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private const int WM_SETTINGCHANGE = 0x1a;
        private const int SMTO_ABORTIFHUNG = 0x0002;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);

        private void setJavaHome() {
            
            String javaVersion = findJavaVersion();

            Console.WriteLine("The version is : " + javaVersion);

            // set the JAVA_HOME environment variable
            Environment.SetEnvironmentVariable("JAVA_HOME", installDir + @"\jdk", EnvironmentVariableTarget.Machine);

            // get the JAVA_HOME environment variable
            String javaHome = Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.Machine);

            if (javaHome != null)
            {
                Console.WriteLine("JAVA_HOME is : " + javaHome);

                // update the path environment variable via registry
                string keyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
                //get non-expanded PATH environment variable            
                string oldPath = (string) Registry.LocalMachine.CreateSubKey(keyName).GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);

                Console.WriteLine("PATH set to : " + oldPath);

                //set the path as an an expandable string
                Registry.LocalMachine.CreateSubKey(keyName).SetValue("Path", "%JAVA_HOME%\\bin;" + oldPath, RegistryValueKind.ExpandString);
                
                string newPath = (string)Registry.LocalMachine.CreateSubKey(keyName).GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);

                SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, "Environment", SMTO_ABORTIFHUNG, 100, IntPtr.Zero);

                /*
                String path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
                //Environment.SetEnvironmentVariable("path", "%JAVA_HOME%;" + path, EnvironmentVariableTarget.Machine);

                path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
                //Console.WriteLine("PATH set to : " + path);

                //Environment.SetEnvironmentVariable("path", path.Replace("%JAVA_HOME%", "%JAVA_HOME%\\bin"), EnvironmentVariableTarget.Machine);

                path = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine);
                   */

                Console.WriteLine("Updated PATH is : " + newPath);
            }
            else
            {
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
