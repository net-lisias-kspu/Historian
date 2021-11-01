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