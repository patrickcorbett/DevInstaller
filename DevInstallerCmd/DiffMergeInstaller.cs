using System;
using System.IO;
using System.IO.Compression;

namespace DevInstallerCmd
{
    internal class DiffMergeInstaller
    {
        private DevInstaller devInstaller;
        private String extractPath;

        public DiffMergeInstaller(DevInstaller pInstaller)
        {
            this.devInstaller = pInstaller;

            // find the zipfile
            this.extractPath = devInstaller.InstallDirectory + @"\software\DiffMerge";
        }

        // https://sourcegear.com/diffmerge/webhelp/sec__inst__windows.html
        public void install()
        {
            FileInfo zipInfo = FileSelector.selectFile(devInstaller.BaseDirectory, "Diff Merge zip to install", "zip");
            if (zipInfo != null)
            {
                // unzip the archive
                ZipFile.ExtractToDirectory(zipInfo.FullName, extractPath);
                Console.WriteLine("Zip :"+ zipInfo.Name + " was extracted");

                // rename the created folder
                string folder = zipInfo.Name.Replace(zipInfo.Extension, "");
                string folderPath = extractPath + "\\" + folder;
                string newFolderName = extractPath + "\\" + folder.Replace("DiffMerge_", "").Replace("_stable_x64", "");

                Directory.Move(folderPath, newFolderName);
            }
            else
            {
                Console.WriteLine("Zip could not be extracted");
            }
        }
    }
}