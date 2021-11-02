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
            normalTexture = GameDatabase.Instance.GetTexture("Historian/Historian_Launcher", false);
            suppressedTexture = GameDatabase.Instance.GetTexture("Historian/Historian_Launcher_Suppressed", false);
        }

        public void Register()
        {
            ApplicationLauncher.AppScenes scenes = ApplicationLauncher.AppScenes.FLIGHT |
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