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
using UnityEngine;

namespace KSEA.Historian
{
    public class SituationText : Text
    {
        Dictionary<ExtendedSituation, List<Token>> situations = new Dictionary<ExtendedSituation, List<Token>>();
        TriState evaOnly = TriState.UseDefault;

        protected override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            foreach (ExtendedSituation key in System.Enum.GetValues(typeof(ExtendedSituation)))
            {
                situations.Add(key, Parser.GetTokens(node.GetString(key.ToString(), "")));
            }

            evaOnly = node.GetEnum("EvaOnly", TriState.UseDefault);
        }

        protected override void OnDraw(Rect bounds)
        {
            var isEva = (FlightGlobals.ActiveVessel?.isEVA).ToTriState();
            if (evaOnly != TriState.UseDefault && evaOnly != isEva)
                return;

            var situation = SituationExtensions.Extend(FlightGlobals.ActiveVessel?.situation, isEva, false);
            var fallback = SituationExtensions.Extend(FlightGlobals.ActiveVessel?.situation, isEva, true);

            TokenizedText = (situations.ContainsKey(situation)) ? situations[situation] : situations[fallback];
            base.OnDraw(bounds);
        }
    }
}