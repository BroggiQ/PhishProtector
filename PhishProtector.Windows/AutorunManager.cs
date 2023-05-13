using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace PhishProtector.Windows
{
    /// <summary>
    /// In progress
    /// The goal is to run the windows apps at the start of Windows 
    /// </summary>
    public class AutorunManager
    {
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "PhishProtector";

        public static void EnableAutorun()
        {
            string appPath = Assembly.GetExecutingAssembly().Location;
            SetAutorunValue(appPath);
        }

        public static void DisableAutorun()
        {
            SetAutorunValue(null);
        }

        private static void SetAutorunValue(string value)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey, true))
            {
                if (value == null)
                {
                    key.DeleteValue(AppName, false);
                }
                else
                {
                    key.SetValue(AppName, value);
                }
            }
        }
    }
}
