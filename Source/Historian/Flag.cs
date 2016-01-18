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
        Vector2 m_Scale = Vector2.zero;
        Texture m_Texture = null;
        Texture m_DefaultTexture = null;
        Texture2D m_BackgroundTexture;

        protected override void OnDraw(Rect bounds)
        {
            UpdateTexture();

            GUI.DrawTexture(bounds, m_BackgroundTexture);
            GUI.DrawTexture(bounds, m_Texture);
        }

        protected override void OnLoad(ConfigNode node)
        {
            m_Scale = node.GetVector2("Scale", Vector2.one);
            m_DefaultTexture = GameDatabase.Instance.GetTexture(node.GetString("DefaultTexture", ""), false);
            m_BackgroundTexture = GameDatabase.Instance.GetTexture(node.GetString("BackgroundTexture", ""), false);

            if (Size == Vector2.zero)
            {
                Size = new Vector2((float) m_DefaultTexture.width / Screen.width * m_Scale.x, (float) m_DefaultTexture.height / Screen.height * m_Scale.y);
            }
            else
            {
                Size = new Vector2(Size.x * m_Scale.x, Size.y * m_Scale.y);
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

                m_Texture = (string.IsNullOrEmpty(url)) ? m_DefaultTexture : GameDatabase.Instance.GetTexture(url, false);
            }
            else
            {
                m_Texture = m_DefaultTexture;
            }
        }
    }
}