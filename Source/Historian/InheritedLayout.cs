using System;
using UnityEngine;

namespace KSEA.Historian
{
    public class InheritedLayout : Element
    {
        string layoutName;
        string[] excludedNodes = { };

        protected override void OnLoad(ConfigNode node)
        {
            layoutName = node.GetString("LayoutName", "");
            if (string.IsNullOrEmpty(layoutName))
                Historian.Print("No 'LayoutName' specified in 'INHERIT' element.");
            else
            {
                if (!layoutName.EndsWith(".layout"))
                {
                    Historian.Print($"Invalid LayoutName '{layoutName}' specified in 'INHERIT' element.");
                    return;
                }
                layoutName = layoutName.Split('.')[0];
            }

            if (node.HasNode("EXCLUDE"))
            {
                var exclude = node.GetNode("EXCLUDE");
                excludedNodes = exclude.GetValues("Element");
            }
        }
        protected override void OnDraw(Rect bounds)
        {
            var layout = Historian.Instance.GetLayout(layoutName);
            layout.DrawExcept(excludedNodes);
        }
    }
}