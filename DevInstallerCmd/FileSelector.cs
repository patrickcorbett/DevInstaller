using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevInstallerCmd
{
    internal class FileSelector
    {

        public static FileInfo selectFile(string pSearchPath)
        {
            return selectFile(pSearchPath, null);
        }

        public static FileInfo selectFile(string pSearchPath, string pExtension)
        {
            // look up the files in the search folder
            String[] files = null;
            if (pExtension != null)
            {
                Console.WriteLine("The execution path is : " + pSearchPath + ", listing '" + pExtension + "' files");
                files = Directory.GetFiles(pSearchPath, "*." + pExtension);
            }
            else
            {
                Console.WriteLine("The execution path is : " + pSearchPath + ", listing all files");
                files = Directory.GetFiles(pSearchPath);
            }

            int index = 0;
            Console.WriteLine("Please select the correct file");
            foreach (String file in files)
            {
                Console.WriteLine("[" + (index++) + "] - " + file);
            }

            FileInfo fileInfo = null;
            while (fileInfo == null)
            {

                String indexString = Console.ReadLine();
                int parseResult;
                if (int.TryParse(indexString, out parseResult))
                {

                    try
                    {
                        // get the string value input
                        string selectedFile = (string)files.GetValue(parseResult);

                        // get the file info
                        fileInfo = new FileInfo(selectedFile);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Could not detect a valid selection");
                        fileInfo = null;
                    }
                }
            }

            return fileInfo;
        }

    }
}
