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
    public class Rectangle : Element
    {
        Color color = Color.black;
        Texture2D texture = null;

        protected override void OnDraw(Rect bounds) => GUI.DrawTexture(bounds, texture);

        protected override void OnLoad(ConfigNode node)
        {
            color = node.GetColor("Color", Color.black);

            var width = (int) (Screen.width * Size.x);
            var height = (int) (Screen.height * Size.y);

            texture = new Texture2D(width, height);
            var pixels = texture.GetPixels();

            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply();
        }
    }
}