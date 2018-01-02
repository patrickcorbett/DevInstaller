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
    internal class EnvironmentVariableUtil
    {
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private const int WM_SETTINGCHANGE = 0x1a;
        private const int SMTO_ABORTIFHUNG = 0x0002;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);

        public static Boolean setVariable(String pKey, String pValue)
        {
            return setVariable(pKey, pValue, false, null);
        }

        public static Boolean setVariable(String pKey, String pValue, Boolean pUpdatePath, String pPathValue) {
                        
            // set the environment variable
            Environment.SetEnvironmentVariable(pKey, pValue, EnvironmentVariableTarget.Machine);

            // get the environment variable
            String environmentVariable = Environment.GetEnvironmentVariable(pKey, EnvironmentVariableTarget.Machine);

            // output the new variable value
            Console.WriteLine("\"" + pKey + "\" is : " + environmentVariable);

            // should the PATH value be updated
            if (environmentVariable != null && pUpdatePath)
            {
                // update the path environment variable via registry
                string keyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
                //get non-expanded PATH environment variable            
                string oldPath = (string) Registry.LocalMachine.CreateSubKey(keyName).GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);

                Console.WriteLine("Current PATH is : " + oldPath);
                
                String valueForPath = null;
                if(null != pPathValue)
                {
                    valueForPath = pPathValue;
                }
                else
                {
                    valueForPath = pValue;
                }
                
                //set the path as an an expandable string
                Registry.LocalMachine.CreateSubKey(keyName).SetValue("Path", valueForPath + ";" + oldPath, RegistryValueKind.ExpandString);
                
                string newPath = (string)Registry.LocalMachine.CreateSubKey(keyName).GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);
                Console.WriteLine("Updated PATH is : " + newPath);
                
                // broadcast path change
                SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, "Environment", SMTO_ABORTIFHUNG, 100, IntPtr.Zero);
            }

            // return true if the variable was set
            return environmentVariable != null;
        }
    }
}
