/*
	This file is part of Historian /L Unleashed
		© 2018-2021 Lisias T : http://lisias.net <support@lisias.net>
		© 2016-2018 Aelfhe1n
		© 2015-2016 Zeenobit

	Historian /L Unleashed is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Historian /L Unleashed is distributed in the hope that it will be
	useful, but WITHOUT ANY WARRANTY; without even the implied
	warranty of	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0 along
	with Historian /L Unleashed. If not, see <https://www.gnu.org/licenses/>.

*/
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
        bool autoHideUI = false;
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

        public bool AutoHideUI
        {
            get
            {
                return autoHideUI;
            }
            set
            {
                autoHideUI = value;
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

        public string AssemblyFileVersion => assemblyVersion;


        public void Reload()
        {
            layouts.Clear();
            Configuration.Load();
            Configuration.Instance.LoadLayouts(this.layouts);
            currentLayoutIndex = FindLayoutIndex(Configuration.Instance.Layout);
        }

        public string[] GetLayoutNames() => layouts.Select(item => item.Name).ToArray();

        public string GetLayoutName(int index) => GetLayout(index).Name;

        public string GetCurrentLayoutName() => GetCurrentLayout().Name;

		public void SetConfiguration(Configuration configuration)
		{
			Configuration.Set(configuration);
			actionTimeout = TimeSpan.FromMilliseconds(configuration.TimeToRememberLastAction);
		}

		void Awake()
        {
            DontDestroyOnLoad(this);

			Configuration.Load();
			Configuration.Instance.LoadLayouts(this.layouts);

            currentLayoutIndex = FindLayoutIndex(Configuration.Instance.Layout);
            Print("Current Layout Index {0}", currentLayoutIndex);

            actionTimeout = TimeSpan.FromMilliseconds(Configuration.Instance.TimeToRememberLastAction);

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
            feflectedMods.Add("kkLaunchSiteManager", Reflect.GetExternalType("KerbalKonstructs.Core.LaunchSiteManager"));
            feflectedMods.Add("kkSpaceCenterManager", Reflect.GetExternalType("KerbalKonstructs.Core.SpaceCenterManager"));
        }

        void RemoveButton()
        {
            if (editor != null)
                editor.RemoveButton();
        }

        void AddButton() => editor = new Editor();

        public void set_m_Active()
        {
            active = true;
            screenshotRequested = true;
        }

        void Update()
        {
            if (!suppressed)
            {
                if (lastAction != LastAction.None && (DateTime.Now - lastActionTime) > TimeSpan.FromSeconds(Configuration.Instance.TimeToRememberLastAction) )
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

                if (GameSettings.TAKE_SCREENSHOT.GetKeyDown() && Configuration.Instance.AutoHideUI && !UIHidden)
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
                    if (!Configuration.Instance.PersistentCustomText && !string.IsNullOrEmpty(Configuration.Instance.CustomText))
                    {
                        Configuration.Instance.CustomText = "";
                        Configuration.Instance.Save();
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
            suppressEditorWindow |= !Configuration.Instance.PersistentConfigurationWindow;
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

        public Layout GetLayout(string name)
        {
            var index = FindLayoutIndex(name);
            return (index < 0) ? Layout.Empty : GetLayout(index);
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

        public static void Print(string message) => Debug.Log("[Historian] " + message);
    }
}