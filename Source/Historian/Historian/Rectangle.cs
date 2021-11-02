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
    public class Rectangle : Element
    {
        Color color = Color.black;
        Texture2D texture = null;

        protected override void OnDraw(Rect bounds) => GUI.DrawTexture(bounds, texture);

        protected override void OnLoad(ConfigNode node)
        {
            color = node.GetColor("Color", Color.black);

            int width = (int) (Screen.width * Size.x);
            int height = (int) (Screen.height * Size.y);

            texture = new Texture2D(width, height);
            Color[] pixels = texture.GetPixels();

            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply();
        }
    }
}