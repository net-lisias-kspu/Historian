

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
        LastAction lastAction = LastAction.None;
        DateTime lastActionTime = DateTime.Now;
        bool UIHidden;
        bool restoreUI;
        TimeSpan actionTimeout;

        public LastAction LastAction {
            get
            {
                return lastAction;
            }
            set
            {
                lastAction = value;
            }
        }

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
                return Path.Combine(Path.Combine(Path.GetDirectoryName(path), "Plugins"), "Plugindata");
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
            configuration = Configuration.Load(Path.Combine(PluginDirectory, "Historian.cfg"));
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

            actionTimeout = TimeSpan.FromMilliseconds(configuration.TimeToRememberLastAction);
        }

        void RemoveOldConfig()
        {
            var fName = Path.Combine(ModDirectory, "Historian.cfg");
            if (File.Exists(fName)) File.Delete(fName);
            fName = Path.Combine(Path.Combine(ModDirectory, "Plugins"), "Historian.cfg");
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

            actionTimeout = TimeSpan.FromMilliseconds(configuration.TimeToRememberLastAction);

            GameEvents.onHideUI.Add(Game_OnHideGUI);
            GameEvents.onShowUI.Add(Game_OnShowGUI);
            GameEvents.onGamePause.Add(Game_OnPause);
            GameEvents.onGameUnpause.Add(Game_OnUnpause);

            GameEvents.onGUIApplicationLauncherReady.Add(AddButton);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveButton);

            // get reference to KSC switcher if installed
            feflectedMods.Add("switcher", Reflect.GetExternalType("regexKSP.LastKSC"));
            feflectedMods.Add("switcherLoader", Reflect.GetExternalType("regexKSP.KSCLoader"));

            // Kerbal Konstructs
            feflectedMods.Add("kkLaunchSiteManager", Reflect.GetExternalType("KerbalKonstructs.LaunchSites.LaunchSiteManager"));
            feflectedMods.Add("kkSpaceCenterManager", Reflect.GetExternalType("KerbalKonstructs.SpaceCenters.SpaceCenterManager"));
        }


        void RemoveButton()
        {
            if (editor != null)
                editor.RemoveButton();
        }

        void AddButton() => editor = new Editor(configuration);

        public void set_m_Active()
        {
            active = true;
            screenshotRequested = true;
        }

        void Update()
        {
            if (!suppressed)
            {
                if (lastAction != LastAction.None && (DateTime.Now - lastActionTime) > TimeSpan.FromSeconds(configuration.TimeToRememberLastAction) )
                {
                    lastAction = LastAction.None;
                }
                CheckForEvents();

                if (UIHidden && restoreUI) // restore UI on next update after forcibly hiding it
                {
                    // Historian.Print("Restore UI");
                    GameEvents.onShowUI.Fire();
                    restoreUI = false;
                }

                if (GameSettings.TAKE_SCREENSHOT.GetKeyDown() && configuration.AutoHideUI && !UIHidden)
                {
                    // Historian.Print("Hide UI");
                    restoreUI = true;
                    GameEvents.onHideUI.Fire();
                }
                
                if (!active)
                {
                    active |= GameSettings.TAKE_SCREENSHOT.GetKeyDown();
                }
                else
                {
                    if (!configuration.PersistentCustomText && !string.IsNullOrEmpty(configuration.CustomText))
                    {
                        configuration.CustomText = "";
                        configuration.Save(Path.Combine(PluginDirectory, "Historian.cfg"));
                    }

                    if (!screenshotRequested) active = false;
                }
            }
        }

        void CheckForEvents()
        {
            var temp = LastAction.None;

            var vessel = FlightGlobals.ActiveVessel;
            if (GameSettings.AbortActionGroup.GetKeyDown())
                temp = LastAction.Abort;
            if (GameSettings.LAUNCH_STAGES.GetKeyDown())
            {
                // ignore stage unless a vessel is active & it's not an EVA kerbal or flag
                if (vessel != null && vessel.vesselType != VesselType.Flag && !vessel.isEVA)
                    temp = LastAction.Stage;
            }
                
            if (GameSettings.CustomActionGroup1.GetKeyDown())
                temp = LastAction.AG1;
            if (GameSettings.CustomActionGroup2.GetKeyDown())
                temp = LastAction.AG2;
            if (GameSettings.CustomActionGroup3.GetKeyDown())
                temp = LastAction.AG3;
            if (GameSettings.CustomActionGroup4.GetKeyDown())
                temp = LastAction.AG4;
            if (GameSettings.CustomActionGroup5.GetKeyDown())
                temp = LastAction.AG5;
            if (GameSettings.CustomActionGroup6.GetKeyDown())
                temp = LastAction.AG6;
            if (GameSettings.CustomActionGroup7.GetKeyDown())
                temp = LastAction.AG7;
            if (GameSettings.CustomActionGroup8.GetKeyDown())
                temp = LastAction.AG8;
            if (GameSettings.CustomActionGroup9.GetKeyDown())
                temp = LastAction.AG9;
            if (GameSettings.CustomActionGroup10.GetKeyDown())
                temp = LastAction.AG10;

            if (temp == LastAction.None && lastAction != LastAction.None && DateTime.Now - lastActionTime > actionTimeout)
            {
                // Historian.Print($"Clearing action after timeout. Now: {DateTime.Now}, LastActionTime: {lastActionTime}, timeout: {actionTimeout} ");
                lastAction = LastAction.None;
                lastActionTime = DateTime.Now;
                return;
            }

            if (temp != lastAction && temp != LastAction.None)
            {
                // Historian.Print($"Setting action {temp} at {DateTime.Now}");
                lastActionTime = DateTime.Now;
                lastAction = temp;
            }
        }

        void OnGUI()
        {
            var s = new ScreenShot();
            
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
            UIHidden = true;
        }

        void Game_OnShowGUI()
        {
            suppressEditorWindow = false;
            UIHidden = false;
        }
            

        void Game_OnUnpause() => suppressEditorWindow = false;

        void Game_OnPause() => suppressEditorWindow = true;

        int FindLayoutIndex(string layoutName) => layouts.FindIndex(layout => layout.Name == layoutName);

        Layout FindLayout(string layoutName) => GetLayout(FindLayoutIndex(layoutName));

        void LoadLayouts()
        {
            Print("Searching for layouts ...");
            var files = Directory.GetFiles(Path.Combine(ModDirectory, "Layouts"), "*.layout");
            foreach (var file in files)
            {
                LoadLayout(file);
            }
        }

        void LoadLayout(string file)
        {
            string layoutName = Path.GetFileNameWithoutExtension(file); 
            try
            {
                var node = ConfigNode.Load(file).GetNode("KSEA_HISTORIAN_LAYOUT");

                if (layouts.FindIndex(layout => layout.Name == layoutName) < 0)
                {
                    var layout = Layout.Load(layoutName, node);
                    layouts.Add(layout);

                    Print($"Found layout '{layoutName}'.");
                }
                else
                {
                    Print($"Layout with name '{layoutName}' already exists. Unable to load duplicate.");
                }
            }
            catch
            {
                Print($"Failed to load layout '{layoutName}'.");
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