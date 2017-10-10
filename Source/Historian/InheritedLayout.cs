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