

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
using UnityEngine;
using KSP.UI.Screens;

namespace KSEA.Historian
{
    public class LauncherButton
    {
        ApplicationLauncherButton button = null;
        Texture normalTexture = null;
        Texture suppressedTexture = null;
        public delegate void Callback();

        public event Callback Click = delegate { };

        public bool IsRegistered { get; private set; }

        public LauncherButton()
        {
            normalTexture = GameDatabase.Instance.GetTexture("KSEA/Historian/Historian_Launcher", false);
            suppressedTexture = GameDatabase.Instance.GetTexture("KSEA/Historian/Historian_Launcher_Suppressed", false);
        }

        public void Register()
        {
            var scenes = ApplicationLauncher.AppScenes.FLIGHT |
                         ApplicationLauncher.AppScenes.MAPVIEW |
                         ApplicationLauncher.AppScenes.SPACECENTER |
                         ApplicationLauncher.AppScenes.SPH |
                         ApplicationLauncher.AppScenes.TRACKSTATION |
                         ApplicationLauncher.AppScenes.VAB;

            button = ApplicationLauncher.Instance.AddModApplication(OnClick, OnClick, null, null, null, null, scenes, normalTexture);

            Update();

            IsRegistered = true;
        }

        public void Unregister()
        {
            if (button != null)
            {
                IsRegistered = false;
                ApplicationLauncher.Instance.RemoveModApplication(button);
                button = null;
            }
        }

        public void Set(bool value, bool call = true)
        {
            if (value)
            {
                button.SetTrue(call);
            }
            else
            {
                button.SetFalse(call);
            }
        }

        public void Update()
        {
            if (Historian.Instance.Suppressed)
            {
                button.SetTexture(suppressedTexture);
            }
            else
            {
                button.SetTexture(normalTexture);
            }
        }

        void OnClick()
        {
            Click();
        }
    }
}