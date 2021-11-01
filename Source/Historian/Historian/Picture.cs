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
    public class Picture : Element
    {
        Vector2 scale = Vector2.zero;
        Texture texture = null;

        protected override void OnDraw(Rect bounds) => GUI.DrawTexture(bounds, texture);

        protected override void OnLoad(ConfigNode node)
        {
            scale = node.GetVector2("Scale", Vector2.one);
            texture = GameDatabase.Instance.GetTexture(node.GetString("Texture", ""), false);

            if (Size == Vector2.zero)
            {
                Size = new Vector2((float) texture.width / Screen.width * scale.x, (float) texture.height / Screen.height * scale.y);
            }
            else
            {
                Size = new Vector2(Size.x * scale.x, Size.y * scale.y);
            }
        }
    }
}