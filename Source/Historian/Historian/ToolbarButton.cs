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
namespace KSEA.Historian
{
    public class ToolbarButton
    {
        IButton button = null;
        bool state = false;
        public delegate void Callback();

        public event Callback OnTrue = delegate { };
        public event Callback OnFalse = delegate { };
        public event Callback OnAlternateClick = delegate { };

        public bool IsRegistered { get; private set; }

        public void SetState(bool state) => this.state = state;

        public void Register()
        {
            if (ToolbarManager.ToolbarAvailable)
            {
                var toolbar = ToolbarManager.Instance;

                button = toolbar.add("KSEA_Historian", "Button");

                button.Text = "Historian";
                button.ToolTip = Localizer.Format("#Historian_ButtonToolTip");
                button.TexturePath = "Historian/Historian_Toolbar";
                button.OnClick += Button_OnClick;

                IsRegistered = true;
            }
        }

        public void Unregister()
        {
            IsRegistered = false;

            if (button != null)
            {
                button.Destroy();
            }
        }

        public void Update()
        {
            if (!IsRegistered) return;
            var historian = Historian.Instance;

            if (historian.Suppressed)
            {
                button.TexturePath = "Historian/Historian_Toolbar_Suppressed";
            }
            else
            {
                button.TexturePath = "Historian/Historian_Toolbar";
            }
        }

        public void Button_OnClick(ClickEvent e)
        {
            switch (e.MouseButton)
            {
                case 0: // Left Click

                    state = !state;

                    if (state)
                    {
                        OnTrue();
                    }
                    else
                    {
                        OnFalse();
                    }

                    break;

                case 1: // Right Click

                    OnAlternateClick();
                    Update();

                    break;

                case 2: // Middle Click
                default:

                    break;
            }
        }
    }
}