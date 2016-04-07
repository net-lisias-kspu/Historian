

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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace KSEA.Historian
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class Historian : Singleton<Historian>
    {
        List<Layout> m_Layouts = new List<Layout>();
        int m_CurrentLayoutIndex = -1;
        bool m_Active = false;
        bool m_AlwaysActive = false;
        bool m_Suppressed = false;
        Configuration m_Configuration = null;
        Editor m_Editor = null;
        bool m_SuppressEditorWindow = false;
        Type m_KscSwitcher = null;
        Type m_KscSwitcherLoader = null;

        bool m_screenshotRequested = false;

        public bool Suppressed
        {
            get
            {
                return m_Suppressed;
            }

            set
            {
                m_Suppressed = value;
            }
        }

        public bool AlwaysActive
        {
            get
            {
                return m_AlwaysActive;
            }

            set
            {
                m_AlwaysActive = value;
            }
        }

        public int CurrentLayoutIndex
        {
            get
            {
                return m_CurrentLayoutIndex;
            }

            set
            {
                m_CurrentLayoutIndex = value;
            }
        }

        public Type KscSwitcher { get { return m_KscSwitcher; } }

        public Type KscSwitcherLoader { get { return m_KscSwitcherLoader; } }

        string PluginDirectory
        {
            get
            {
                var path = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(path);
            }
        }

        public void Reload()
        {
            m_Layouts.Clear();
            LoadLayouts();
            m_CurrentLayoutIndex = FindLayoutIndex(m_Configuration.Layout);
        }

        public Configuration GetConfiguration()
        {
            return m_Configuration;
        }

        public string[] GetLayoutNames()
        {
            return m_Layouts.Select(item => item.Name).ToArray();
        }

        public string GetLayoutName(int index)
        {
            return GetLayout(index).Name;
        }

        public string GetCurrentLayoutName()
        {
            return GetCurrentLayout().Name;
        }

        public void SetConfiguration(Configuration configuration)
        {
            m_Configuration = configuration;
            m_Configuration.Save(Path.Combine(PluginDirectory, "Historian.cfg"));
        }

        void Awake()
        {
            DontDestroyOnLoad(this);

            m_Configuration = Configuration.Load(Path.Combine(PluginDirectory, "Historian.cfg"));

            LoadLayouts();

            m_CurrentLayoutIndex = FindLayoutIndex(m_Configuration.Layout);
            Print("Current Layout Index {0}", m_CurrentLayoutIndex);
            m_Editor = new Editor(m_Configuration);

            GameEvents.onHideUI.Add(Game_OnHideGUI);
            GameEvents.onShowUI.Add(Game_OnShowGUI);
            GameEvents.onGamePause.Add(Game_OnPause);
            GameEvents.onGameUnpause.Add(Game_OnUnpause);

            // get reference to KSC switcher if installed
            m_KscSwitcher = Reflect.GetExternalType("regexKSP.LastKSC");
            m_KscSwitcherLoader = Reflect.GetExternalType("regexKSP.KSCLoader");
        }
        public void set_m_Active()
        {
            m_Active = true;
            m_screenshotRequested = true;
        }

        void Update()
        {
            if (!m_Suppressed)
            {
                if (!m_Active)
                {
                    m_Active |= GameSettings.TAKE_SCREENSHOT.GetKeyDown();
                }
                else
                {
                    if (!m_Configuration.PersistentCustomText)
                    {
                        m_Configuration.CustomText = "";
                        m_Configuration.Save(Path.Combine(PluginDirectory, "Historian.cfg"));
                    }

                    if (!m_screenshotRequested) m_Active = false;
                }
            }
        }

        void OnGUI()
        {
            if (!m_Suppressed && (m_Active || m_AlwaysActive))
            {
                var layout = GetCurrentLayout();
                layout.Draw();

                if (m_screenshotRequested) m_screenshotRequested = false;
            }

            if (!m_SuppressEditorWindow)
            {
                m_Editor.Draw();
            }
        }

        void Game_OnHideGUI()
        {
            m_SuppressEditorWindow |= !m_Configuration.PersistentConfigurationWindow;
        }

        void Game_OnShowGUI()
        {
            m_SuppressEditorWindow = false;
        }

        void Game_OnUnpause()
        {
            m_SuppressEditorWindow = false;
        }

        void Game_OnPause()
        {
            m_SuppressEditorWindow = true;
        }

        int FindLayoutIndex(string name)
        {
            return m_Layouts.FindIndex(layout => layout.Name == name);
        }

        Layout FindLayout(string name)
        {
            var index = FindLayoutIndex(name);

            return GetLayout(index);
        }

        void LoadLayouts()
        {
            Print("Searching for layouts ...");

            var directory = Path.Combine(PluginDirectory, "Layouts");
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

                if (m_Layouts.FindIndex(layout => layout.Name == name) < 0)
                {
                    var layout = Layout.Load(name, node);
                    m_Layouts.Add(layout);

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
            if (index >= 0 && index < m_Layouts.Count)
            {
                return m_Layouts[index];
            }

            return Layout.Empty;
        }

        Layout GetCurrentLayout()
        {
            return GetLayout(m_CurrentLayoutIndex);
        }

        public static void Print(string format, params object[] args)
        {
            Print(string.Format(format, args));
        }

        public static void Print(string message)
        {
            UnityEngine.Debug.Log("[KSEA.Historian] " + message);
        }
    }
}