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