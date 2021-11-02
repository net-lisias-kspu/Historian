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
using Toolbar = KSPe.UI.Toolbar;
using Asset = KSPe.IO.Asset<KSEA.Historian.Startup>;
namespace KSEA.Historian
{
    public class ToolbarButton
    {
        Toolbar.Button button = null;
        bool state = false;
        public delegate void Callback();

        public event Callback OnTrue = delegate { };
        public event Callback OnFalse = delegate { };
        public event Callback OnAlternateClick = delegate { };

        public bool IsRegistered { get; private set; }

        public void SetState(bool state) => this.state = state;

        private static UnityEngine.Texture2D toolbar = null;
        private static UnityEngine.Texture2D toolbarSupressed = null;
        public void Register()
        {
            if(null == toolbar) toolbar             = Asset.Texture2D.LoadFromFile("Historian_Toolbar");
            if(null == toolbar) toolbarSupressed    = Asset.Texture2D.LoadFromFile("Historian_Toolbar_Suppressed");
            this.button = Toolbar.Button.Create(this
                    , KSP.UI.Screens.ApplicationLauncher.AppScenes.ALWAYS
                    , toolbar, toolbarSupressed
                    , toolbar, toolbarSupressed
                    , Localizer.Format("#Historian_ButtonToolTip")
                );
            this.button.Mouse.Add(Toolbar.Button.MouseEvents.Kind.Left, this.Button_OnLeftClick);
            this.button.Mouse.Add(Toolbar.Button.MouseEvents.Kind.Right, this.Button_OnRightClick);
            ToolbarController.Instance.Add(this.button);
        }

        public void Unregister()
        {
            IsRegistered = false;

            ToolbarController.Instance.Destroy();
            this.button = null;
        }

        public void Update()
        {
            if (!IsRegistered) return;
            Historian historian = Historian.Instance;
            this.button.Enabled = !historian.Suppressed;
        }

		public void Button_OnLeftClick()
		{

			state = !state;

			if (state)
			{
				OnTrue();
			}
			else
			{
				OnFalse();
			}
		}

		public void Button_OnRightClick()
		{
			OnAlternateClick();
			Update();
		}
    }
}