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
                button.ToolTip = "Click to open Historian configuration window.";
                button.TexturePath = "KSEA/Historian/Historian_Toolbar";
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
                button.TexturePath = "KSEA/Historian/Historian_Toolbar_Suppressed";
            }
            else
            {
                button.TexturePath = "KSEA/Historian/Historian_Toolbar";
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