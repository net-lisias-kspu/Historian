﻿

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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KSEA.Historian
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class Historian : Singleton<Historian>
    {
        List<Layout> layouts = new List<Layout>();
        int currentLayoutIndex = -1;
        bool active = false;
        bool alwaysActive = false;
        bool suppressed = false;
        Configuration configuration = null;
        Editor editor = null;
        bool suppressEditorWindow = false;
        Dictionary<string, Type> feflectedMods = new Dictionary<string, Type>();
        bool screenshotRequested = false;
        string assemblyVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public bool Suppressed
        {
            get
            {
                return suppressed;
            }

            set
            {
                suppressed = value;
            }
        }

        public bool AlwaysActive
        {
            get
            {
                return alwaysActive;
            }

            set
            {
                alwaysActive = value;
            }
        }

        public int CurrentLayoutIndex
        {
            get
            {
                return currentLayoutIndex;
            }

            set
            {
                currentLayoutIndex = value;
            }
        }

        public Type ReflectedClassType(string classNameKey) 
            => feflectedMods.ContainsKey(classNameKey) ? feflectedMods[classNameKey] : null;

        public string PluginDirectory
        {
            get
            {
                var path = Assembly.GetExecutingAssembly().Location;
                return Path.Combine(Path.GetDirectoryName(path), "Plugins");
            }
        }

        public string ModDirectory
        {
            get
            {
                var path = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(path);
            }
        }

        public string AssemblyFileVersion => assemblyVersion;

        public void Reload()
        {
            layouts.Clear();
            LoadLayouts();
            currentLayoutIndex = FindLayoutIndex(configuration.Layout);
        }

        public Configuration GetConfiguration() => configuration;

        public string[] GetLayoutNames() => layouts.Select(item => item.Name).ToArray();

        public string GetLayoutName(int index) => GetLayout(index).Name;

        public string GetCurrentLayoutName() => GetCurrentLayout().Name;

        public void SetConfiguration(Configuration configuration)
        {
            this.configuration = configuration;
            this.configuration.Save(Path.Combine(PluginDirectory, "Historian.cfg"));
        }

        private void RemoveOldConfig()
        {
            var fName = Path.Combine(ModDirectory, "Historian.cfg");
            if (File.Exists(fName)) File.Delete(fName);
        }

        void Awake()
        {
            DontDestroyOnLoad(this);

            RemoveOldConfig();

            configuration = Configuration.Load(Path.Combine(PluginDirectory, "Historian.cfg"));

            LoadLayouts();

            currentLayoutIndex = FindLayoutIndex(configuration.Layout);
            Print("Current Layout Index {0}", currentLayoutIndex);
            

            GameEvents.onHideUI.Add(Game_OnHideGUI);
            GameEvents.onShowUI.Add(Game_OnShowGUI);
            GameEvents.onGamePause.Add(Game_OnPause);
            GameEvents.onGameUnpause.Add(Game_OnUnpause);

            GameEvents.onGUIApplicationLauncherReady.Add(AddButton);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveButton);

            // get reference to KSC switcher if installed
            feflectedMods.Add("switcher",Reflect.GetExternalType("regexKSP.LastKSC"));
            feflectedMods.Add("switcherLoader",Reflect.GetExternalType("regexKSP.KSCLoader"));

            // Kerbal Konstructs
            feflectedMods.Add("kkLaunchSiteManager", Reflect.GetExternalType("KerbalKonstructs.LaunchSites.LaunchSiteManager"));
            feflectedMods.Add("kkSpaceCenterManager", Reflect.GetExternalType("KerbalKonstructs.SpaceCenters.SpaceCenterManager"));
        }


        private void RemoveButton()
        {
            if (editor != null)
            {
                editor.RemoveButton();
            }
        }

        private void AddButton()
        {
            editor = new Editor(configuration);
        }

        public void set_m_Active()
		{
			active = true;
            screenshotRequested = true;
		}

        void Update()
        {
            if (!suppressed)
            {
                if (!active)
                {
                    active |= GameSettings.TAKE_SCREENSHOT.GetKeyDown();
                }
                else
                {
                    if (!configuration.PersistentCustomText)
                    {
                        configuration.CustomText = "";
                        configuration.Save(Path.Combine(PluginDirectory, "Historian.cfg"));
                    }

                    if (!screenshotRequested) active = false;
                }
            }
        }

        void OnGUI()
        {
            if (!suppressed && (active || alwaysActive))
            {
                var layout = GetCurrentLayout();
                layout.Draw();

                if (screenshotRequested) screenshotRequested = false;
            }

            if (!suppressEditorWindow && editor != null)
            {
                editor.Draw();
            }
        }

        void Game_OnHideGUI()
        {
            suppressEditorWindow |= !configuration.PersistentConfigurationWindow;
        }

        void Game_OnShowGUI()
        {
            suppressEditorWindow = false;
        }

        void Game_OnUnpause()
        {
            suppressEditorWindow = false;
        }

        void Game_OnPause()
        {
            suppressEditorWindow = true;
        }

        int FindLayoutIndex(string layoutName) => layouts.FindIndex(layout => layout.Name == layoutName);

        Layout FindLayout(string layoutName) => GetLayout(FindLayoutIndex(layoutName));

        void LoadLayouts()
        {
            Print("Searching for layouts ...");

            var directory = Path.Combine(ModDirectory, "Layouts");
            var files = Directory.GetFiles(directory, "*.layout");

            foreach (var file in files)
            {
                LoadLayout(file);
            }
        }

        void LoadLayout(string file)
        {
            try
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var node = ConfigNode.Load(file).GetNode("KSEA_HISTORIAN_LAYOUT");

                if (layouts.FindIndex(layout => layout.Name == name) < 0)
                {
                    var layout = Layout.Load(name, node);
                    layouts.Add(layout);

                    Print("Found layout '{0}'.", name);
                }
                else
                {
                    Print("Layout with name '{0}' already exists. Unable to load duplicate.", name);
                }
            }
            catch
            {
                Print("Failed to load layout '{0}'.", name);
            }
        }

        Layout GetLayout(int index)
        {
            if (index >= 0 && index < layouts.Count)
            {
                return layouts[index];
            }

            return Layout.Empty;
        }

        Layout GetCurrentLayout() => GetLayout(currentLayoutIndex);

        public static void Print(string format, params object[] args) => Print(string.Format(format, args));

        public static void Print(string message) => Debug.Log("[KSEA.Historian] " + message);
    }
}