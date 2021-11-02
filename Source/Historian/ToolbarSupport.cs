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
using System.Collections.Generic;

using UnityEngine;
using KSP.Localization;
using KSP.UI.Screens;

using KSPe.Annotations;
using Toolbar = KSPe.UI.Toolbar;
using GUI = KSPe.UI.GUI;
using GUILayout = KSPe.UI.GUILayout;
using Asset = KSPe.IO.Asset<KSEA.Historian.Startup>;

namespace KSEA.Historian
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class ToolbarController:MonoBehaviour
	{
		private static ToolbarController instance;
		internal static ToolbarController Instance => instance;

		[UsedImplicitly]
		private void Awake()
		{
			instance = this;
			DontDestroyOnLoad(this);
		}

		[UsedImplicitly]
		private void Start()
		{
			Toolbar.Controller.Instance.Register<ToolbarController>(Version.FriendlyName);
		}

		private Toolbar.Button button = null;
		private bool state = false;
		internal bool IsRegistered { get; private set; }
		internal event Callback OnTrue = delegate { };
		internal event Callback OnFalse = delegate { };
		internal event Callback OnAlternateClick = delegate { };
		private static UnityEngine.Texture2D toolbar = null;
		private static UnityEngine.Texture2D toolbarSupressed = null;
		internal void Register()
		{
			if (null == toolbar) toolbar = Asset.Texture2D.LoadFromFile("Historian_Toolbar");
			if (null == toolbar) toolbarSupressed = Asset.Texture2D.LoadFromFile("Historian_Toolbar_Suppressed");
			this.button = Toolbar.Button.Create(this
					, ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW
						| ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.SPH
						| ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.VAB
					, toolbar, toolbarSupressed
					, toolbar, toolbarSupressed
					, Localizer.Format("#Historian_ButtonToolTip")
				);
			this.button.Mouse.Add(Toolbar.Button.MouseEvents.Kind.Left, this.Button_OnLeftClick);
			this.button.Mouse.Add(Toolbar.Button.MouseEvents.Kind.Right, this.Button_OnRightClick);
			Toolbar.Controller.Instance.Get<ToolbarController>().Add(this.button);
		}

		internal void ButtonsActive(bool enableBlizzy, bool enableStock)
		{
			Toolbar.Controller.Instance.Get<ToolbarController>().ButtonsActive(enableBlizzy, enableStock);
		}

		internal void Update()
		{
			if (!this.IsRegistered) return;
			this.button.Enabled = !Historian.Instance.Suppressed;
		}

		internal void Unregister()
		{
			this.IsRegistered = false;

			Toolbar.Controller.Instance.Get<ToolbarController>().Destroy();
			this.button = null;
		}

		internal void Button_OnLeftClick()
		{
			this.state = !this.state;

			if (state) this.OnTrue();
			else this.OnFalse();
		}

		internal void Button_OnRightClick()
		{
			this.OnAlternateClick();
			this.Update();
		}
	}
}
