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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSEA.Historian
{
    class TextList : Text
    {
        Dictionary<ExtendedSituation, List<List<Token>>> situationTokenizedTexts 
            = new Dictionary<ExtendedSituation, List<List<Token>>>();
        TriState evaOnly = TriState.UseDefault;

        bool isRandom = false;
        Dictionary<ExtendedSituation, int> messageIndices = new Dictionary<ExtendedSituation, int>();
        bool resetOnLaunch = false;
        Vessel lastVessel = null;
        System.Random rnd = new System.Random();
        DateTime lastDraw = DateTime.Now;
        TimeSpan minimumInterval = TimeSpan.FromMilliseconds(80);

        protected override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            situationTokenizedTexts = new Dictionary<ExtendedSituation, List<List<Token>>>();

            isRandom = node.GetBoolean("Random", false);
            resetOnLaunch = node.GetBoolean("ResetOnLaunch", false);

            evaOnly = node.GetEnum("EvaOnly", TriState.UseDefault);

            foreach (var section in node.GetNodes())
            {
                var name = section.name;
                try
                {
                    var situation = (ExtendedSituation)(object)ConfigNode.ParseEnum(typeof(ExtendedSituation), name);
                    
                    if (!situationTokenizedTexts.ContainsKey(situation))
                    {
                        situationTokenizedTexts.Add(situation, new List<List<Token>>());
                        messageIndices.Add(situation, -1);
                    }
                    situationTokenizedTexts[situation].AddTokenizedRange(section.GetValues("Text"));
                }
                catch
                {
                    Historian.Print($"Unrecognised situation block {name} in TEXT_LIST");
                }
            }
        }

        protected override void OnDraw(Rect bounds)
        {
            var isEva = (FlightGlobals.ActiveVessel?.isEVA).ToTriState();
            if (evaOnly != TriState.UseDefault && evaOnly != isEva)
                return;

            var situation = SituationExtensions.Extend(FlightGlobals.ActiveVessel?.situation, isEva, false);
            var fallback = SituationExtensions.Extend(FlightGlobals.ActiveVessel?.situation, isEva, true);

            var extendedSituation = situationTokenizedTexts.ContainsKey(situation) ? situation : fallback;
            var texts = situationTokenizedTexts[extendedSituation];

            // debug
            // Historian.Print($"Random text: {isRandom}, Reset: {resetOnLaunch}, Index: {messageIndices[extendedSituation]}, isEva: {isEva}, situation: {extendedSituation}, #Messages: {texts.Count}");

            if (texts != null && texts.Count < 1)
                return;

            UpdateMessageIndex(extendedSituation);

            try
            {
                TokenizedText = texts[messageIndices[extendedSituation]];
            }
            catch (Exception e)
            {
                TokenizedText = TokenizedError;
                Historian.Print($"TextList error: {e.Message}, situation: {extendedSituation}, index: {messageIndices[extendedSituation]}");
            }
            base.OnDraw(bounds);

            lastVessel = FlightGlobals.ActiveVessel;
            lastDraw = DateTime.Now;
        }

        void UpdateMessageIndex(ExtendedSituation extendedSituation)
        {
            
            if (DateTime.Now - lastDraw < minimumInterval && messageIndices[extendedSituation] > -1)
            {
                // Historian.Print("No index update");
            }
            else
            {
                messageIndices[extendedSituation]++;

                if (isRandom)
                {
                    messageIndices[extendedSituation] = rnd.Next(0, situationTokenizedTexts[extendedSituation].Count - 1);
                }
                else
                {
                    if (resetOnLaunch && lastVessel != FlightGlobals.ActiveVessel)
                    {
                        // Historian.Print("Vessel changed - reseting messages");
                        messageIndices[extendedSituation] = 0;
                    }
                }

                if (messageIndices[extendedSituation] >= situationTokenizedTexts[extendedSituation].Count)
                    messageIndices[extendedSituation] = 0; // wrap around after end of list
            }
        }

    }
}
