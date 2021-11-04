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
using UnityEngine;
using KSP.Localization;
using GUI = KSPe.UI.GUI;
using GUILayout = KSPe.UI.GUILayout;
using Asset = KSPe.IO.Asset<KSEA.Historian.Startup>;

namespace KSEA.Historian
{
    public class Editor
    {
        bool isOpen = false;
        Rect position;
        int windowId;
        static Texture2D nextButtonTexture = null;
        static Texture2D previousButtonTexture = null;


        public Editor()
        {
            float windowHeight = 580f;
            float windowWidth = 900f;
            //if (GameSettings.KERBIN_TIME)
            //    windowHeight += 100; // add extra height for Kerbin month/day name fields

            position = new Rect(0.5f * Screen.width - windowWidth / 2, 0.5f * Screen.height - windowHeight / 2, windowWidth, windowHeight);
            windowId = (new System.Random()).Next(876543210, 987654321); // 9 digit random number

            nextButtonTexture = nextButtonTexture ?? (nextButtonTexture = Asset.Texture2D.LoadFromFile(ToolbarController.ICON_DIR, "Historian_Button_Next"));
            previousButtonTexture = previousButtonTexture ?? (previousButtonTexture = Asset.Texture2D.LoadFromFile(ToolbarController.ICON_DIR, "Historian_Button_Previous"));

            if (!ToolbarController.Instance.IsRegistered)
            {
                ToolbarController.Instance.OnTrue += Toggle;
                ToolbarController.Instance.OnFalse += Toggle;
                ToolbarController.Instance.OnAlternateClick += Button_OnAlternateClick;

                ToolbarController.Instance.Register();
            }
        }

        public void Draw()
        {
            if (isOpen)
            {
                position = GUI.Window(windowId, position, OnWindowGUI, $"Historian: v {Historian.Instance.AssemblyFileVersion}", HighLogic.Skin.window);
            }
        }

        void OnWindowGUI(int id)
        {
            GUI.skin = HighLogic.Skin;
            Historian historian = Historian.Instance;

            // column one
            using (GUILayout.AreaScope columnOne = new GUILayout.AreaScope(new Rect(15, 20, 380, 550)))
            {
                using (GUILayout.VerticalScope col = new GUILayout.VerticalScope())
                {
                    GUILayout.Space(20);
                    historian.Suppressed = GUILayout.Toggle(historian.Suppressed, Localizer.GetStringByTag("#Historian_Suppressed"));
                    historian.AlwaysActive = GUILayout.Toggle(historian.AlwaysActive, Localizer.GetStringByTag("#Historian_AlwaysActive"));
                    historian.AutoHideUI = GUILayout.Toggle(historian.AutoHideUI, Localizer.GetStringByTag("#Historian_AutoHideUI"));
                    Configuration.Instance.AutoHideUI = historian.AutoHideUI;

                    Configuration.Instance.PersistentConfigurationWindow = GUILayout.Toggle(Configuration.Instance.PersistentConfigurationWindow, Localizer.GetStringByTag("#Historian_AlwaysShowConfigWindow"));
                    Configuration.Instance.EnableLauncherButton = GUILayout.Toggle(Configuration.Instance.EnableLauncherButton, Localizer.GetStringByTag("#Historian_UseAppLauncher"));
                    Configuration.Instance.EnableToolbarButton = GUILayout.Toggle(Configuration.Instance.EnableToolbarButton, Localizer.GetStringByTag("#Historian_UseToolbar"));
                    GUILayout.Space(10);

                    using (GUILayout.HorizontalScope layout = new GUILayout.HorizontalScope())
                    {
                        int rightClickOptionsCount = 4;
                        //GUILayout.Space(40);
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_RightClickAction"));
                        GUILayout.Space(10);
                        if (GUILayout.Button(previousButtonTexture, GUILayout.Width(20), GUILayout.Height(GUI.skin.label.lineHeight)))
                        {
                            Configuration.Instance.RightClickAction = (RightClickAction)Mathf.Clamp((int)Configuration.Instance.RightClickAction - 1, 0, rightClickOptionsCount - 1);
                        }
                        else if (GUILayout.Button(nextButtonTexture, GUILayout.Width(20), GUILayout.Height(GUI.skin.label.lineHeight)))
                        {
                            Configuration.Instance.RightClickAction = (RightClickAction)Mathf.Clamp((int)Configuration.Instance.RightClickAction + 1, 0, rightClickOptionsCount - 1);
                        }
                        GUILayout.Space(5);
                        GUILayout.Label(Configuration.Instance.RightClickAction.ToString(), GUI.skin.textArea, GUILayout.ExpandWidth(true));
                    }

                    ManageButtons();

                    GUILayout.Space(10);
                    using (GUILayout.HorizontalScope layout = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_Layout"));
                        GUILayout.Space(10);
                        string[] layouts = historian.GetLayoutNames();
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
                    using (GUILayout.HorizontalScope customHead = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_CustomText"));
                        GUILayout.FlexibleSpace();
                        Configuration.Instance.PersistentCustomText = GUILayout.Toggle(Configuration.Instance.PersistentCustomText, Localizer.GetStringByTag("#Historian_Persistent"), GUILayout.Width(120));
                    }
                    Configuration.Instance.CustomText = GUILayout.TextArea(Configuration.Instance.CustomText, GUI.skin.textArea, GUILayout.Height(60));

                    GUILayout.Space(10);
                    using (GUILayout.HorizontalScope spaceCentre = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_DefaultSpaceCenterLabel"));
                        GUILayout.FlexibleSpace();
                        Configuration.Instance.DefaultSpaceCenterName = GUILayout.TextField(Configuration.Instance.DefaultSpaceCenterName, GUI.skin.textArea, GUILayout.Width(150));
                    }


                    GUILayout.Space(10);
                    GUILayout.Label($"{Localizer.GetStringByTag("#Historian_LastActionTime")}: {Configuration.Instance.TimeToRememberLastAction} ms");
                    Configuration.Instance.TimeToRememberLastAction = (int)GUILayout.HorizontalSlider(Configuration.Instance.TimeToRememberLastAction, 250, 10000, GUILayout.ExpandWidth(true));

                }
            }

            // column two
            using (GUILayout.AreaScope columnTwo = new GUILayout.AreaScope(new Rect(410, 20, 220, 500)))
            {
                using (GUILayout.VerticalScope col = new GUILayout.VerticalScope())
                {
                    GUILayout.Space(20);
                    GUILayout.Label(Localizer.GetStringByTag("#Historian_DayNames"));
                    for (int i = 0; i < Configuration.Instance.KerbinDayNames.Length; i++)
                    {
                        using (GUILayout.HorizontalScope item = new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label($"{i + 1}:");
                            GUILayout.FlexibleSpace();
                            Configuration.Instance.KerbinDayNames[i] = GUILayout.TextField(Configuration.Instance.KerbinDayNames[i], GUI.skin.textArea, GUILayout.Width(190f));
                        }
                    }

                    GUILayout.Space(50);
                    GUILayout.Label(Localizer.GetStringByTag("#Historian_DefaultEmptyCrewSlot"));
                    GUILayout.Space(10);
                    using (GUILayout.HorizontalScope noCrewLabel = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_CrewedLabel"));
                        GUILayout.FlexibleSpace();
                        Configuration.Instance.DefaultNoCrewLabel = GUILayout.TextField(Configuration.Instance.DefaultNoCrewLabel, GUI.skin.textArea, GUILayout.Width(120));
                    }
                    using (GUILayout.HorizontalScope noCrewLabel = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Localizer.GetStringByTag("#Historian_UncrewedLabel"));
                        GUILayout.FlexibleSpace();
                        Configuration.Instance.DefaultUnmannedLabel = GUILayout.TextField(Configuration.Instance.DefaultUnmannedLabel, GUI.skin.textArea, GUILayout.Width(120));
                    }

                }
            }

            // column three
            using (GUILayout.AreaScope columnThree = new GUILayout.AreaScope(new Rect(660, 20, 220, 480)))
            {
                using (GUILayout.VerticalScope col = new GUILayout.VerticalScope())
                {
                    GUILayout.Space(20);
                    GUILayout.Label(Localizer.GetStringByTag("#Historian_MonthNames"));
                    for (int i = 0; i < Configuration.Instance.KerbinMonthNames.Length; i++)
                    {
                        using (GUILayout.HorizontalScope item = new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label($"{i + 1}:");
                            GUILayout.FlexibleSpace();
                            Configuration.Instance.KerbinMonthNames[i] = GUILayout.TextField(Configuration.Instance.KerbinMonthNames[i], GUI.skin.textArea, GUILayout.Width(190f));
                        }
                    }
                }
            }

            // bottom bar
            using (GUILayout.AreaScope buttonBar = new GUILayout.AreaScope(new Rect(5, 525, 890, 30)))
            {
                using (GUILayout.HorizontalScope layout = new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(Localizer.GetStringByTag("#autoLOC_900539"), GUILayout.Width(100.0f))) // #autoLOC_900539 = Load
                    {
                        historian.Reload();
                    }
                    if (GUILayout.Button(Localizer.GetStringByTag("#autoLOC_174778"), GUILayout.Width(100.0f))) // #autoLOC_174778 = Save
                    {
                        Configuration.Instance.Layout = historian.GetCurrentLayoutName();

                        historian.SetConfiguration(Configuration.Instance);
                        if (!Configuration.Instance.PersistentConfigurationWindow) Toggle();

                        if (!String.IsNullOrEmpty(Configuration.Instance.CustomText))
                            Configuration.Instance.TokenizedCustomText = Parser.GetTokens(Configuration.Instance.CustomText);
                    }
                    GUILayout.Space(20);
                    // GUILayout.FlexibleSpace();
                }
            }

            GUI.DragWindow();
        }

        void ManageButtons()
        {
            ToolbarController.Instance.ButtonsActive(Configuration.Instance.EnableToolbarButton, Configuration.Instance.EnableLauncherButton);
        }

        internal void RemoveButton() => ToolbarController.Instance.Unregister();

        void Toggle() => isOpen = !isOpen;

        void Button_OnAlternateClick()
        {
            switch (Configuration.Instance.RightClickAction)
            {
                case RightClickAction.None:
                    break;
                case RightClickAction.Suppress:
                    Log.dbg("Button_OnAlternateClick : Supressed was {0}", Historian.Instance.Suppressed);
                    Historian.Instance.Suppressed = !Historian.Instance.Suppressed;
                    Log.dbg("Button_OnAlternateClick : Supressed is now {0}", Historian.Instance.Suppressed);
                    break;
                case RightClickAction.AlwaysActive:
                    Historian.Instance.AlwaysActive = !Historian.Instance.AlwaysActive;
                    Log.dbg("Button_OnAlternateClick : AlwaysActive = {0}", Historian.Instance.AlwaysActive);
                    break;
                case RightClickAction.AutoHideUI:
                    Historian.Instance.AutoHideUI = !Historian.Instance.AutoHideUI;
                    Log.dbg("Button_OnAlternateClick : AutoHideUI = {0}", Historian.Instance.AutoHideUI);
                    break;
                default:
                    break;
            }
        } 
    }
}