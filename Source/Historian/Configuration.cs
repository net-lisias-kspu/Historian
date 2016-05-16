/**
 * This file is part of Historian.
 * 
 * Historian is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Historian is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Historian. If not, see <http://www.gnu.org/licenses/>.
 **/

using System;
using System.IO;

namespace KSEA.Historian
{
    public class Configuration
    {
        static readonly Version CurrentVersion = new Version("1.1.2");

        public string Layout { get; set; } = "";

        public string DefaultSpaceCenterName { get; set; } = "KSC";

        public bool EnableLauncherButton { get; set; } = true;

        public bool EnableToolbarButton { get; set; } = true;

        public string CustomText { get; set; } = "";

        public bool PersistentCustomText { get; set; } = false;

        public bool PersistentConfigurationWindow { get; set; } = false;

        public static Configuration Load(string file)
        {
            try
            {
                var node = ConfigNode.Load(file).GetNode("KSEA_HISTORIAN_CONFIGURATION");
                var configuration = new Configuration();

                var version = node.GetVersion("Version", new Version());

                configuration.Layout = node.GetString("Layout", "Default");
                configuration.EnableLauncherButton = node.GetBoolean("EnableLauncherButton", true);
                configuration.EnableToolbarButton = node.GetBoolean("EnableToolbarButton", true);
                configuration.CustomText = node.GetString("CustomText", "");
                configuration.DefaultSpaceCenterName = node.GetString("DefaultSpaceCenterName", "KSC");
                configuration.PersistentCustomText = node.GetBoolean("PersistentCustomText", false);
                configuration.PersistentConfigurationWindow = node.GetBoolean("PersistentConfigurationWindow", true);

                if (version != CurrentVersion)
                {
                    configuration.Save(file);
                }

                return configuration;
            }
            catch
            {
                Historian.Print($"Failed to load configuration file '{file}'. Attempting recovery ...");

                if (File.Exists(file))
                    File.Delete(file);

                var configuration = new Configuration();

                configuration.Layout = "Default";
                configuration.EnableLauncherButton = true;
                configuration.EnableToolbarButton = true;
                configuration.CustomText = "";
                configuration.PersistentCustomText = false;
                configuration.PersistentConfigurationWindow = true;

                configuration.Save(file);

                return configuration;
            }
        }

        public void Save(string file)
        {
            try
            {
                // ensure save directory exists.
                var dir = Path.GetDirectoryName(file);
                Directory.CreateDirectory(dir);

                var root = new ConfigNode();
                var node = root.AddNode("KSEA_HISTORIAN_CONFIGURATION");

                node.AddValue("Version", CurrentVersion.ToString());
                node.AddValue("Layout", Layout);
                node.AddValue("EnableLauncherButton", EnableLauncherButton);
                node.AddValue("EnableToolbarButton", EnableToolbarButton);
                node.AddValue("CustomText", CustomText);
                node.AddValue("PersistentCustomText", PersistentCustomText);
                node.AddValue("PersistentConfigurationWindow", PersistentConfigurationWindow);
                node.AddValue("DefaultSpaceCenterName", DefaultSpaceCenterName);

                if (File.Exists(file))
                    File.Delete(file);

                root.Save(file);

                Historian.Print($"Configuration saved at '{file}'.");
            }
            catch
            {
                Historian.Print($"Failed to save configuration file '{file}'.");
            }
        }
    }
}