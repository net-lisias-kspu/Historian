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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KSEA.Historian
{
    public class Editor
    {
        bool isOpen = false;
        static LauncherButton appLauncherButton = new LauncherButton();
        ToolbarButton toolbarButton = null;
        Rect position;
        Texture nextButtonTexture = null;
        Texture previousButtonTexture = null;
        bool enableLauncherButton = true;
        bool enableToolberButton = true;

        public Editor(HistorianSettings configuration)
        {
            //m_LauncherButton = new LauncherButton();
            toolbarButton = new ToolbarButton();

            position = new Rect(0.5f * Screen.width - 200.0f, 0.5f * Screen.height - 250.0f, 400.0f, 500.0f);

            nextButtonTexture = GameDatabase.Instance.GetTexture("KSEA/Historian/Historian_Button_Next", false);
            previousButtonTexture = GameDatabase.Instance.GetTexture("KSEA/Historian/Historian_Button_Previous", false);

            enableLauncherButton = configuration.EnableLauncherButton;
            enableToolberButton = configuration.EnableToolbarButton;

            if (enableLauncherButton)
            {
                appLauncherButton.OnTrue += Button_OnTrue;
                appLauncherButton.OnFalse += Button_OnFalse;

                if (!appLauncherButton.IsRegistered)
                    appLauncherButton.Register();
            }

            if (ToolbarManager.ToolbarAvailable && enableToolberButton)
            {
                toolbarButton.OnTrue += Button_OnTrue;
                toolbarButton.OnFalse += Button_OnFalse;
                toolbarButton.OnAlternateClick += Button_OnAlternateClick;

                toolbarButton.Register();
            }
        }

        public void Open() => isOpen = true;

        public void Close() => isOpen = false;

        public void Draw()
        {
            if (isOpen)
            {
                
                position = GUI.Window(0, position, OnWindowGUI, $"Historian: v {Historian.Instance.AssemblyFileVersion}", HighLogic.Skin.window);

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
            var configuration = HistorianSettings.fetch;

            GUILayout.BeginVertical();

            historian.Suppressed = GUILayout.Toggle(historian.Suppressed, "Suppressed");
            historian.AlwaysActive = GUILayout.Toggle(historian.AlwaysActive, "Always Active");

            configuration.PersistentConfigurationWindow = GUILayout.Toggle(configuration.PersistentConfigurationWindow, "Always Display Configuration Window");

            enableLauncherButton = GUILayout.Toggle(enableLauncherButton, "Use Stock Launcher");
            enableToolberButton = GUILayout.Toggle(enableToolberButton, "Use Blizzy's Toolbar");

            if (enableLauncherButton && !appLauncherButton.IsRegistered)
            {
                appLauncherButton.OnTrue += Button_OnTrue;
                appLauncherButton.OnFalse += Button_OnFalse;

                appLauncherButton.Register();
            }
            else if (!enableLauncherButton && appLauncherButton.IsRegistered)
            {
                appLauncherButton.OnTrue -= Button_OnTrue;
                appLauncherButton.OnFalse -= Button_OnFalse;

                appLauncherButton.Unregister();
            }

            if (enableToolberButton && ToolbarManager.ToolbarAvailable && !toolbarButton.IsRegistered)
            {
                toolbarButton.OnTrue += Button_OnTrue;
                toolbarButton.OnFalse += Button_OnFalse;

                toolbarButton.SetState(isOpen);

                toolbarButton.Register();
            }
            else if (!enableToolberButton && ToolbarManager.ToolbarAvailable && toolbarButton.IsRegistered)
            {
                toolbarButton.OnTrue -= Button_OnTrue;
                toolbarButton.OnFalse -= Button_OnFalse;

                toolbarButton.SetState(isOpen);

                toolbarButton.Unregister();
            }

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Layout");
            GUILayout.Space(20);

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
            GUILayout.Label(historian.GetCurrentLayoutName(), GUI.skin.textField);

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.Label("Custom Text:");
            configuration.CustomText = GUILayout.TextArea(configuration.CustomText, GUILayout.Height(60));
            configuration.PersistentCustomText = GUILayout.Toggle(configuration.PersistentCustomText, "Persistent Custom Text");

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Default Space Center Name:");
            configuration.DefaultSpaceCenterName = GUILayout.TextField(configuration.DefaultSpaceCenterName, GUILayout.Width(150));
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Load", GUILayout.Width(100.0f)))
            {
                historian.Reload();
            }

            if (GUILayout.Button("Save", GUILayout.Width(100.0f)))
            {
                configuration.Layout = historian.GetCurrentLayoutName();
                configuration.EnableLauncherButton = enableLauncherButton;
                configuration.EnableToolbarButton = enableToolberButton;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        internal void RemoveButton() => appLauncherButton.Unregister();

        void Button_OnTrue() => Open();

        void Button_OnFalse() => Close();

        void Button_OnAlternateClick() => Historian.Instance.Suppressed = !Historian.Instance.Suppressed;
    }
}