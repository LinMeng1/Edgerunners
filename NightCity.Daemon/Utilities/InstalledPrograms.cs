using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace NightCity.Daemon.Utilities
{
    public static class InstalledPrograms
    {
        const string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public static List<LocalInstallInformation> GetInstalledPrograms()
        {
            var result = new List<LocalInstallInformation>();
            result.AddRange(GetInstalledProgramsFromRegistry(RegistryView.Registry32));
            result.AddRange(GetInstalledProgramsFromRegistry(RegistryView.Registry64));
            return result;
        }
        private static IEnumerable<LocalInstallInformation> GetInstalledProgramsFromRegistry(RegistryView registryView)
        {
            var result = new List<LocalInstallInformation>();

            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        if (IsProgramVisible(subkey))
                        {
                            result.Add(new LocalInstallInformation
                            {
                                DisplayIcon = (string)subkey.GetValue("DisplayIcon"),
                                DisplayName = (string)subkey.GetValue("DisplayName"),
                                DisplayVersion = (string)subkey.GetValue("DisplayVersion"),
                                Publisher = (string)subkey.GetValue("Publisher"),
                                UninstallString = (string)subkey.GetValue("UninstallString"),
                            });
                        }
                    }
                }
            }

            return result;
        }
        private static bool IsProgramVisible(RegistryKey subkey)
        {
            var name = (string)subkey.GetValue("DisplayName");
            var releaseType = (string)subkey.GetValue("ReleaseType");
            //var unistallString = (string)subkey.GetValue("UninstallString");
            var systemComponent = subkey.GetValue("SystemComponent");
            var parentName = (string)subkey.GetValue("ParentDisplayName");

            return
                !string.IsNullOrEmpty(name)
                && string.IsNullOrEmpty(releaseType)
                && string.IsNullOrEmpty(parentName)
                && (systemComponent == null);
        }
        public static void CreateUninstallInRegistry(LocalInstallInformation installInfo)
        {
            try
            {
                var productKey = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{installInfo.DisplayName}");
                productKey.SetValue("DisplayIcon", installInfo.DisplayIcon, RegistryValueKind.String);
                productKey.SetValue("DisplayName", installInfo.DisplayName, RegistryValueKind.String);
                productKey.SetValue("DisplayVersion", installInfo.DisplayVersion, RegistryValueKind.String);
                productKey.SetValue("Publisher", installInfo.Publisher, RegistryValueKind.String);
                productKey.SetValue("UninstallString", installInfo.UninstallString, RegistryValueKind.String);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void RemoveUninstallInRegistry(LocalInstallInformation installInfo)
        {
            try
            {
                Registry.LocalMachine.DeleteSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{installInfo.DisplayName}");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
