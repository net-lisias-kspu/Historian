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
using UnityEngine;

namespace KSEA.Historian
{
    public class Configuration
    {
        // defaults
        static readonly Version CurrentVersion = new Version("1.1.2");

        static Configuration Defaults = new Configuration {
            Layout = "Default",
            EnableLauncherButton = true,
            EnableToolbarButton = true,
            CustomText = "",
            PersistentConfigurationWindow = false,
            PersistentCustomText = false,
            AutoHideUI = true,
            TimeToRememberLastAction = 2000, // 2000ms = 2s
            DefaultSpaceCenterName = "KSC",
            KerbinMonthNames = new string[] { "Unnam", "Dosnam", "Trenam", "Cuatnam", "Cinqnam", "Seinam", "Sietnam", "Ocnam", "Nuevnam", "Diznam", "Oncnam", "Docenam" },
            KerbinDayNames = new string[] { "Akant", "Brant", "Casant", "Dovant", "Esant", "Flant" }
        };
        
        public string Layout { get; set; }

        public string DefaultSpaceCenterName { get; set; }

        public bool EnableLauncherButton { get; set; }

        public bool EnableToolbarButton { get; set; }

        public string CustomText { get; set; }

        public bool PersistentCustomText { get; set; }

        public bool PersistentConfigurationWindow { get; set; }

        public int TimeToRememberLastAction { get; set; }

        public bool AutoHideUI { get; set; }

        public string[] KerbinMonthNames;
        public string[] KerbinDayNames;

        public static Configuration Load(string file)
        {
            try
            {
                var node = ConfigNode.Load(file).GetNode("KSEA_HISTORIAN_CONFIGURATION");
                var configuration = new Configuration();

                var version = node.GetVersion("Version", new Version());

                configuration.Layout 
                    = node.GetString("Layout", Defaults.Layout);
                configuration.EnableLauncherButton 
                    = node.GetBoolean("EnableLauncherButton", Defaults.EnableLauncherButton);
                configuration.EnableToolbarButton 
                    = node.GetBoolean("EnableToolbarButton", Defaults.EnableToolbarButton);
                configuration.AutoHideUI
                    = node.GetBoolean("AutoHideUI", Defaults.AutoHideUI);
                configuration.CustomText 
                    = node.GetString("CustomText", Defaults.CustomText);
                configuration.DefaultSpaceCenterName 
                    = node.GetString("DefaultSpaceCenterName", Defaults.DefaultSpaceCenterName);
                configuration.PersistentCustomText 
                    = node.GetBoolean("PersistentCustomText", Defaults.PersistentCustomText);
                configuration.PersistentConfigurationWindow 
                    = node.GetBoolean("PersistentConfigurationWindow", Defaults.PersistentConfigurationWindow);
                configuration.TimeToRememberLastAction 
                    = node.GetInteger("TimeToRememberLastAction", Defaults.TimeToRememberLastAction);
                configuration.KerbinDayNames
                    = node.TryReadStringArray("KerbinDayNames", Defaults.KerbinDayNames);
                configuration.KerbinMonthNames
                    = node.TryReadStringArray("KerbinMonthNames", Defaults.KerbinMonthNames);

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

                configuration.Layout = Defaults.Layout;
                configuration.EnableLauncherButton = Defaults.EnableLauncherButton;
                configuration.EnableToolbarButton = Defaults.EnableToolbarButton;
                configuration.AutoHideUI = Defaults.AutoHideUI;
                configuration.CustomText = Defaults.CustomText;
                configuration.PersistentCustomText = Defaults.PersistentCustomText;
                configuration.PersistentConfigurationWindow = Defaults.PersistentConfigurationWindow;
                configuration.TimeToRememberLastAction = Defaults.TimeToRememberLastAction;
                configuration.KerbinDayNames = (string[])Defaults.KerbinDayNames.Clone();  
                configuration.KerbinMonthNames = (string[])Defaults.KerbinMonthNames.Clone();

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
                node.AddValue("AutoHideUI", AutoHideUI);
                node.AddValue("CustomText", CustomText);
                node.AddValue("PersistentCustomText", PersistentCustomText);
                node.AddValue("PersistentConfigurationWindow", PersistentConfigurationWindow);
                node.AddValue("DefaultSpaceCenterName", DefaultSpaceCenterName);
                node.AddValue("TimeToRememberLastAction", TimeToRememberLastAction);
                node.AddValue("KerbinDayNames", string.Join(";", KerbinDayNames));
                node.AddValue("KerbinMonthNames", string.Join(";", KerbinMonthNames));

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