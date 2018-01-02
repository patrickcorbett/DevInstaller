using System;
using System.IO;
using System.IO.Compression;

namespace DevInstallerCmd
{
    internal class TomcatInstaller
    {
        private DevInstaller devInstaller;
        private String extractPath;

        public TomcatInstaller(DevInstaller pInstaller)
        {
            this.devInstaller = pInstaller;

            // find the zipfile
            this.extractPath = devInstaller.InstallDirectory + @"\software\tomcat";
        }

        // https://tomcat.apache.org/tomcat-8.5-doc/RUNNING.txt
        public void install()
        {
            FileInfo zipInfo = FileSelector.selectFile(devInstaller.BaseDirectory, "Tomcat zip to install", "zip");
            if (zipInfo != null)
            {
                // unzip the archive
                ZipFile.ExtractToDirectory(zipInfo.FullName, extractPath);
                Console.WriteLine("Zip :"+ zipInfo.Name + " was extracted");
            }
            else
            {
                Console.WriteLine("Zip could not be extracted");
            }

            // select the tomcat version (CATALINA_HOME)
            FileInfo tomcatInfo = FileSelector.selectFile(extractPath, "CATALINA_HOME", "zip");
            if (tomcatInfo != null)
            {
                // set catalina_home
                setCatalinaHome(tomcatInfo);
            }

            // select the active tomcat version (CATALINA_BASE)
            FileInfo activeTomcatInfo = FileSelector.selectFile(extractPath, "CATALINA_BASE", "zip");
            if (tomcatInfo != null)
            {
                 // set catalina_base
                setCatalinaBase(activeTomcatInfo);
            }
        }

        private void setCatalinaHome(FileInfo pCatalinaInfo)
        {
            // set the JAVA_HOME variable and set it on the System PATH
            if (!EnvironmentVariableUtil.setVariable("CATALINA_HOME", extractPath + @"\tomcat\" + pCatalinaInfo.Name))
            {
                Console.WriteLine("CATALINA_HOME was not set");
            }
        }

        private void setCatalinaBase(FileInfo pCatalinaInfo)
        {
            // set the JAVA_HOME variable and set it on the System PATH
            if (!EnvironmentVariableUtil.setVariable("CATALINA_BASE", extractPath + @"\tomcat\" + pCatalinaInfo.Name))
            {
                Console.WriteLine("CATALINA_HOME was not set");
            }
        }
    }
}