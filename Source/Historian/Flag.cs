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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSEA.Historian
{
    public class Flag : Element
    {
        Vector2 scale = Vector2.zero;
        Texture texture = null;
        Texture defaultTexture = null;
        Texture2D backgroundTexture;

        protected override void OnDraw(Rect bounds)
        {
            UpdateTexture();

            if (backgroundTexture != null) GUI.DrawTexture(bounds, backgroundTexture);
            GUI.DrawTexture(bounds, texture);
        }

        protected override void OnLoad(ConfigNode node)
        {
            scale = node.GetVector2("Scale", Vector2.one);
            defaultTexture = GameDatabase.Instance.GetTexture(node.GetString("DefaultTexture", ""), false);
            backgroundTexture = GameDatabase.Instance.GetTexture(node.GetString("BackgroundTexture", ""), false);

            if (Size == Vector2.zero)
            {
                Size = new Vector2((float) defaultTexture.width / Screen.width * scale.x, (float) defaultTexture.height / Screen.height * scale.y);
            }
            else
            {
                Size = new Vector2(Size.x * scale.x, Size.y * scale.y);
            }
        }

        void UpdateTexture()
        {
            var vessel = FlightGlobals.ActiveVessel;

            if (vessel != null)
            {
                var flags = new List<string>();
                flags.AddRange(vessel.Parts.Select(p => p.flagURL));

                // Find the flag with the highest occurrance in the entire vessel

                var url = flags.GroupBy(item => item).OrderByDescending(item => item.Count()).First().Key;

                texture = (string.IsNullOrEmpty(url)) ? defaultTexture : GameDatabase.Instance.GetTexture(url, false);
            }
            else
            {
                texture = defaultTexture;
            }
        }
    }
}