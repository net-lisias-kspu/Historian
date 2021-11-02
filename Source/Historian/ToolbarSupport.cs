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
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class ToolbarController:MonoBehaviour
	{
		private static ToolbarController instance;
		internal static ToolbarController Instance => instance;
		private Toolbar.Toolbar controller => Toolbar.Controller.Instance.Get<ToolbarController>();

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

		// State controller for the toobar button
		public class SupressedStatus:KSPe.UI.Toolbar.State.Status<bool> { protected SupressedStatus(bool v):base(v) { }  public static implicit operator SupressedStatus(bool v) => new SupressedStatus(v);   public static implicit operator bool(SupressedStatus s) => s.v; }
		private Toolbar.State.Control supressedController;
		internal SupressedStatus supressedStatus = false;

		private Toolbar.Button button = null;
		private bool state = false;
		internal bool IsRegistered { get; private set; }
		internal event Callback OnTrue = delegate { };
		internal event Callback OnFalse = delegate { };
		internal event Callback OnAlternateClick = delegate { };
		internal const string ICON_DIR = "Icons";
		private static UnityEngine.Texture2D launcher = null;
		private static UnityEngine.Texture2D launcherSupressed = null;
		private static UnityEngine.Texture2D toolbar = null;
		private static UnityEngine.Texture2D toolbarSupressed = null;
		internal void Register()
		{
			launcher			= launcher			?? (launcher = Asset.Texture2D.LoadFromFile(ICON_DIR, "Historian_Launcher"));
			launcherSupressed	= launcherSupressed	?? (launcherSupressed= Asset.Texture2D.LoadFromFile(ICON_DIR, "Historian_Launcher_Suppressed"));
			toolbar				= toolbar			?? (toolbar = Asset.Texture2D.LoadFromFile(ICON_DIR, "Historian_Toolbar"));
			toolbarSupressed	= toolbarSupressed	?? (toolbarSupressed = Asset.Texture2D.LoadFromFile(ICON_DIR, "Historian_Toolbar_Suppressed"));
			this.button = Toolbar.Button.Create(this
					, ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW
						| ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.SPH
						| ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.VAB
					, launcher
					, toolbar
					, Localizer.Format("#Historian_ButtonToolTip")??Version.FriendlyName
				);
			this.supressedController = button.State.Controller.Create<SupressedStatus>(
				new Dictionary<Toolbar.State.Status, Toolbar.State.Data> {
							{ (SupressedStatus)false, Toolbar.State.Data.Create(launcher, toolbar) }
							,{ (SupressedStatus)true, Toolbar.State.Data.Create(launcherSupressed, toolbarSupressed) }
				}
			);
			this.button.Mouse.Add(Toolbar.Button.MouseEvents.Kind.Left, this.Button_OnLeftClick);
			this.button.Mouse.Add(Toolbar.Button.MouseEvents.Kind.Right, this.Button_OnRightClick);
			this.controller.Add(this.button);
			ToolbarController.Instance.ButtonsActive(Configuration.Instance.EnableToolbarButton, Configuration.Instance.EnableLauncherButton);
			this.IsRegistered = true;
		}

		internal void ButtonsActive(bool enableBlizzy, bool enableStock)
		{
			this.controller.ButtonsActive(enableBlizzy, enableStock);
		}

		internal void UpdateButton()
		{
			if (!this.IsRegistered) return;
			Log.dbg("TollbarSupport.Update!!");
			this.button.Status = this.supressedStatus;
		}

		internal void Unregister()
		{
			this.OnTrue.Clear();
			this.OnFalse.Clear();
			this.OnAlternateClick.Clear();
			this.controller.Destroy();
			this.button = null;
			this.IsRegistered = false;
		}

		internal void Button_OnLeftClick()
		{
			Log.dbg("Left Click!!!");
			this.state = !this.state;

			if (state) this.OnTrue();
			else this.OnFalse();
		}

		internal void Button_OnRightClick()
		{
			Log.dbg("Right Click!!!");
			this.OnAlternateClick();
		}
	}
}
