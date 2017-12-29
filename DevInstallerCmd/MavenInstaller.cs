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
            this.extractPath = devInstaller.InstallDirectory + @"\software";
        }

        public void install()
        {
            FileInfo zipInfo = findZIPs();
            if (zipInfo != null)
            {
                // unzip the archive
                ZipFile.ExtractToDirectory(zipInfo.FullName, extractPath);
                Console.WriteLine("Zip :"+ zipInfo.Name + " was extracted");

                // rename the created folder
                string folder = extractPath + "\\" + zipInfo.Name.Replace("-bin" + zipInfo.Extension, "");


                Directory.Move(folder, extractPath + "\\maven");
            }
            else
            {
                Console.WriteLine("Zip could not be extracted");
            }
        }

        private FileInfo findZIPs()
        {
            // search in the current folder for a JDK installation file
            Console.WriteLine("The execution path is : " + devInstaller.BaseDirectory);

            String[] zips = Directory.GetFiles(devInstaller.BaseDirectory, "*.zip");

            int index = 0;
            Console.WriteLine("Please select the correct zip file");
            foreach (String zip in zips)
            {
                Console.WriteLine("["+ (index++) +"]" + zip);
            }

            FileInfo zipFileInfo = null;
            while (zipFileInfo == null)
            {

                String indexString = Console.ReadLine();
                int parseResult;
                if (int.TryParse(indexString, out parseResult)) {

                    try
                    {
                        // get the string value input
                        string zipFile = (string) zips.GetValue(parseResult);

                        // get the file info
                        zipFileInfo = new FileInfo(zipFile);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Could not detect a valid selection");
                        zipFileInfo = null;
                    }
                }
            }
            
            return zipFileInfo;
        }
    }
}