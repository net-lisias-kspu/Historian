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