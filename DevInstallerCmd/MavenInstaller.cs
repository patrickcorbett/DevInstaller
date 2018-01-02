using System;
using System.IO;
using System.IO.Compression;

namespace DevInstallerCmd
{
    internal class MavenInstaller
    {
        private DevInstaller devInstaller;
        private String extractPath;

        public MavenInstaller(DevInstaller pInstaller)
        {
            this.devInstaller = pInstaller;

            // find the zipfile
            this.extractPath = devInstaller.InstallDirectory + @"\software\maven";
        }

        // https://maven.apache.org/install.html
        public void install()
        {
            FileInfo zipInfo = FileSelector.selectFile(devInstaller.BaseDirectory, "Maven zip to install", "zip");
            if (zipInfo != null)
            {
                // unzip the archive
                ZipFile.ExtractToDirectory(zipInfo.FullName, extractPath);
                Console.WriteLine("Zip :"+ zipInfo.Name + " was extracted");

                // rename the created folder
                // string folder = extractPath + "\\" + zipInfo.Name.Replace("-bin" + zipInfo.Extension, "");
                // Directory.Move(folder, extractPath + "\\maven");
            }
            else
            {
                Console.WriteLine("Zip could not be extracted");
            }


            // select the maven version
            FileInfo mavenInfo = FileSelector.selectFile(extractPath, "MAVEN_HOME");
            if (mavenInfo != null)
            {
                // set maven home
                setMavenHome(mavenInfo);
            }
            else
            {
                Console.WriteLine("MAVEN_HOME was not set");
            }
        }

        private void setMavenHome(FileInfo pMavenInfo)
        {
            // set the JAVA_HOME variable and set it on the System PATH
            if (!EnvironmentVariableUtil.setVariable("MAVEN_HOME", extractPath + @"\maven\" + pMavenInfo.Name, true, @"%MAVEN_HOME%\bin"))
            {
                Console.WriteLine("MAVEN_HOME was not set");
            }
        }
    }
}