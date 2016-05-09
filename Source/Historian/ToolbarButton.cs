using UnityEngine;

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