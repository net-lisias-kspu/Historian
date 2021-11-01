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
using KSP.Localization;
using System;
using UnityEngine;

namespace KSEA.Historian
{
    public class Editor
    {
        bool isOpen = false;
        static LauncherButton appLauncherButton = new LauncherButton();
        ToolbarButton toolbarButton = null;
        Rect position;
        int windowId;
        Texture nextButtonTexture = null;
        Texture previousButtonTexture = null;
        bool enableLauncherButton = true;
        bool enableToolberButton = true;
        

        public Editor(Configuration configuration)
        {
            //m_LauncherButton = new LauncherButton();
            toolbarButton = new ToolbarButton();

            var windowHeight = 580f;
            var windowWidth = 900f;
            //if (GameSettings.KERBIN_TIME)
            //    windowHeight += 100; // add extra height for Kerbin month/day name fields

            position = new Rect(0.5f * Screen.width - windowWidth / 2, 0.5f * Screen.height - windowHeight / 2, windowWidth, windowHeight);
            windowId = (new System.Random()).Next(876543210, 987654321); // 9 digit random number

            nextButtonTexture = GameDatabase.Instance.GetTexture("Historian/Historian_Button_Next", false);
            previousButtonTexture = GameDatabase.Instance.GetTexture("Historian/Historian_Button_Previous", false);

            enableLauncherButton = configuration.EnableLauncherButton;
            enableToolberButton = configuration.EnableToolbarButton;

            if (enableLauncherButton)
            {
                appLauncherButton.Click += Button_Click;

                if (!appLauncherButton.IsRegistered)
                    appLauncherButton.Register();
            }

            if (ToolbarManager.ToolbarAvailable && enableToolberButton)
            {
                toolbarButton.OnTrue += Toggle;
                toolbarButton.OnFalse += Toggle;
                toolbarButton.OnAlternateClick += Button_OnAlternateClick;

                toolbarButton.Register();
            }
        }

        public void Draw()
        {
            if (isOpen)
            {
                position = GUI.Window(windowId, position, OnWindowGUI, $"Historian: v {Historian.Instance.AssemblyFileVersion}", HighLogic.Skin.window);

                if (enableLauncherButton)
                {
                    appLauncherButton.Update();
                }

                if (enableToolberButton && ToolbarManager.ToolbarAvailable)
                {
                    toolbarButton.Update();
                }
            }
        }

        void OnWindowGUI(int id)
        {
            GUI.skin = HighLogic.Skin;
            var historian = Historian.Instance;
            var configuration = historian.GetConfiguration();

            // column one
            using (var columnOne = new GUILayout.AreaScope(new Rect(15, 20, 380, 550)))
            {
                using (var col = new GUILayout.VerticalScope())
                {
                    GUILayout.Space(20);
                    historian.Suppressed = GUILayout.Toggle(historian.Suppressed, Localizer.GetStringByTag("#Historian_Suppressed"));
                    historian.AlwaysActive = GUILayout.Toggle(historian.AlwaysActive, Localizer.GetStringByTag("#Historian_AlwaysActive"));
                    historian.AutoHideUI = GUILayout.Toggle(historian.AutoHideUI, Localizer.GetStringByTag("#Historian_AutoHideUI"));
                    configuration.AutoHideUI = historian.AutoHideUI;

                    configuration.PersistentConfigurationWindow = GUILayout.Toggle(configuration.PersistentConfigurationWindow, Localizer.GetStringByTag("#Historian_AlwaysShowConfigWindow"));
                    enableLauncherButton = GUILayout.Toggle(enableLauncherButton, Localizer.GetStringByTag("#Historian_UseAppLauncher"));
                    enableToolberButton = GUILayout.Toggle(enableToolberButton, Localizer.GetStringByTag("#Historian_UseToolbar"));
                    GUILayout.Space(10);

                    using (var layout = new GUILayout.HorizontalScope())
                    {
                        var rightClickOptionsCount = 4;
                        //GUILayout.Space(40);
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_RightClickAction"));
                        GUILayout.Space(10);
                        if (GUILayout.Button(previousButtonTexture, GUILayout.Width(20), GUILayout.Height(GUI.skin.label.lineHeight)))
                        {
                            configuration.RightClickAction = (RightClickAction)Mathf.Clamp((int)configuration.RightClickAction - 1, 0, rightClickOptionsCount - 1);
                        }
                        else if (GUILayout.Button(nextButtonTexture, GUILayout.Width(20), GUILayout.Height(GUI.skin.label.lineHeight)))
                        {
                            configuration.RightClickAction = (RightClickAction)Mathf.Clamp((int)configuration.RightClickAction + 1, 0, rightClickOptionsCount - 1);
                        }
                        GUILayout.Space(5);
                        GUILayout.Label(configuration.RightClickAction.ToString(), GUI.skin.textArea, GUILayout.ExpandWidth(true));
                    }

                    ManageButtons();

                    GUILayout.Space(10);
                    using (var layout = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_Layout"));
                        GUILayout.Space(10);
                        var layouts = historian.GetLayoutNames();
                        if (GUILayout.Button(previousButtonTexture, GUILayout.Width(20), GUILayout.Height(GUI.skin.label.lineHeight)))
                        {
                            historian.CurrentLayoutIndex = Mathf.Clamp(historian.CurrentLayoutIndex - 1, 0, layouts.Length - 1);
                        }
                        else if (GUILayout.Button(nextButtonTexture, GUILayout.Width(20), GUILayout.Height(GUI.skin.label.lineHeight)))
                        {
                            historian.CurrentLayoutIndex = Mathf.Clamp(historian.CurrentLayoutIndex + 1, 0, layouts.Length - 1);
                        }
                        GUILayout.Space(5);
                        GUILayout.Label(historian.GetCurrentLayoutName(), GUI.skin.textArea, GUILayout.ExpandWidth(true));
                    }

                    GUILayout.Space(10);
                    using (var customHead = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_CustomText"));
                        GUILayout.FlexibleSpace();
                        configuration.PersistentCustomText = GUILayout.Toggle(configuration.PersistentCustomText, Localizer.GetStringByTag("#Historian_Persistent"), GUILayout.Width(120));
                    }
                    configuration.CustomText = GUILayout.TextArea(configuration.CustomText, GUI.skin.textArea, GUILayout.Height(60));

                    GUILayout.Space(10);
                    using (var spaceCentre = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_DefaultSpaceCenterLabel"));
                        GUILayout.FlexibleSpace();
                        configuration.DefaultSpaceCenterName = GUILayout.TextField(configuration.DefaultSpaceCenterName, GUI.skin.textArea, GUILayout.Width(150));
                    }


                    GUILayout.Space(10);
                    GUILayout.Label($"{Localizer.GetStringByTag("#Historian_LastActionTime")}: {configuration.TimeToRememberLastAction} ms");
                    configuration.TimeToRememberLastAction = (int)GUILayout.HorizontalSlider(configuration.TimeToRememberLastAction, 250, 10000, GUILayout.ExpandWidth(true));

                }
            }

            // column two
            using (var columnTwo = new GUILayout.AreaScope(new Rect(410, 20, 220, 500)))
            {
                using (var col = new GUILayout.VerticalScope())
                {
                    GUILayout.Space(20);
                    GUILayout.Label(Localizer.GetStringByTag("#Historian_DayNames"));
                    for (int i = 0; i < configuration.KerbinDayNames.Length; i++)
                    {
                        using (var item = new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label($"{i + 1}:");
                            GUILayout.FlexibleSpace();
                            configuration.KerbinDayNames[i] = GUILayout.TextField(configuration.KerbinDayNames[i], GUI.skin.textArea, GUILayout.Width(190f));
                        }
                    }

                    GUILayout.Space(50);
                    GUILayout.Label(Localizer.GetStringByTag("#Historian_DefaultEmptyCrewSlot"));
                    GUILayout.Space(10);
                    using (var noCrewLabel = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_CrewedLabel"));
                        GUILayout.FlexibleSpace();
                        configuration.DefaultNoCrewLabel = GUILayout.TextField(configuration.DefaultNoCrewLabel, GUI.skin.textArea, GUILayout.Width(120));
                    }
                    using (var noCrewLabel = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_UncrewedLabel"));
                        GUILayout.FlexibleSpace();
                        configuration.DefaultUnmannedLabel = GUILayout.TextField(configuration.DefaultUnmannedLabel, GUI.skin.textArea, GUILayout.Width(120));
                    }

                }
            }

            // column three
            using (var columnThree = new GUILayout.AreaScope(new Rect(660, 20, 220, 480)))
            {
                using (var col = new GUILayout.VerticalScope())
                {
                    GUILayout.Space(20);
                    GUILayout.Label(Localizer.GetStringByTag("#Historian_MonthNames"));
                    for (int i = 0; i < configuration.KerbinMonthNames.Length; i++)
                    {
                        using (var item = new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label($"{i + 1}:");
                            GUILayout.FlexibleSpace();
                            configuration.KerbinMonthNames[i] = GUILayout.TextField(configuration.KerbinMonthNames[i], GUI.skin.textArea, GUILayout.Width(190f));
                        }
                    }
                }
            }

            // bottom bar
            using (var buttonBar = new GUILayout.AreaScope(new Rect(5, 525, 890, 30)))
            {
                using (var layout = new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(Localizer.GetStringByTag("#autoLOC_900539"), GUILayout.Width(100.0f))) // #autoLOC_900539 = Load
                    {
                        historian.Reload();
                    }
                    if (GUILayout.Button(Localizer.GetStringByTag("#autoLOC_174778"), GUILayout.Width(100.0f))) // #autoLOC_174778 = Save
                    {
                        configuration.Layout = historian.GetCurrentLayoutName();
                        configuration.EnableLauncherButton = enableLauncherButton;
                        configuration.EnableToolbarButton = enableToolberButton;

                        historian.SetConfiguration(configuration);
                        if (!configuration.PersistentConfigurationWindow) Toggle();

                        if (!String.IsNullOrEmpty(configuration.CustomText))
                            configuration.TokenizedCustomText = Parser.GetTokens(configuration.CustomText);
                    }
                    GUILayout.Space(20);
                    // GUILayout.FlexibleSpace();
                }
            }

            GUI.DragWindow();
        }

        void ManageButtons()
        {
            if (enableLauncherButton && !appLauncherButton.IsRegistered)
            {
                appLauncherButton.Click += Button_Click;
                appLauncherButton.Register();
            }
            else if (!enableLauncherButton && appLauncherButton.IsRegistered)
            {
                appLauncherButton.Click -= Button_Click;
                appLauncherButton.Unregister();
            }

            if (enableToolberButton && ToolbarManager.ToolbarAvailable && !toolbarButton.IsRegistered)
            {
                toolbarButton.OnTrue += Button_Click;
                toolbarButton.OnFalse += Button_Click;

                toolbarButton.SetState(isOpen);

                toolbarButton.Register();
            }
            else if (!enableToolberButton && ToolbarManager.ToolbarAvailable && toolbarButton.IsRegistered)
            {
                toolbarButton.OnTrue -= Button_Click;
                toolbarButton.OnFalse -= Button_Click;

                toolbarButton.SetState(isOpen);

                toolbarButton.Unregister();
            }
        }

        internal void RemoveButton() => appLauncherButton.Unregister();

        void Button_Click() => Toggle();

        void Toggle() => isOpen = !isOpen;

        void Button_OnAlternateClick()
        {
            switch (Historian.Instance.GetConfiguration().RightClickAction)
            {
                case RightClickAction.None:
                    break;
                case RightClickAction.Suppress:
                    Historian.Instance.Suppressed = !Historian.Instance.Suppressed;
                    break;
                case RightClickAction.AlwaysActive:
                    Historian.Instance.AlwaysActive = !Historian.Instance.AlwaysActive;
                    break;
                case RightClickAction.AutoHideUI:
                    Historian.Instance.AutoHideUI = !Historian.Instance.AutoHideUI;
                    break;
                default:
                    break;
            }
            
        } 
    }
}