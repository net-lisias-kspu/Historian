using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSEA.Historian
{
    class TextList : Text
    {
        Dictionary<ExtendedSituation, List<string>> situationTexts = new Dictionary<ExtendedSituation, List<string>>();
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

            situationTexts = new Dictionary<ExtendedSituation, List<string>>();

            isRandom = node.GetBoolean("Random", false);
            resetOnLaunch = node.GetBoolean("ResetOnLaunch", false);

            evaOnly = node.GetEnum("EvaOnly", TriState.UseDefault);

            foreach (var section in node.GetNodes())
            {
                var name = section.name;
                try
                {
                    var situation = (ExtendedSituation)(object)ConfigNode.ParseEnum(typeof(ExtendedSituation), name);
                    
                    if (!situationTexts.ContainsKey(situation))
                    {
                        situationTexts.Add(situation, new List<string>());
                        messageIndices.Add(situation, -1);
                    }
                    situationTexts[situation].AddRange(section.GetValues("Text"));
                    // Historian.Print($"Adding text list for {situation} - total items {situationTexts[situation].Count}");
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

            var situation = FlightGlobals.ActiveVessel?.situation;
            var extendedSituation = situation.HasValue && situationTexts.ContainsKey((ExtendedSituation)situation.Value) 
                ? Extend(situation, isEva)
                : ExtendedSituation.Default;

            var texts = situationTexts[extendedSituation];

            // debug
            // Historian.Print($"Random text: {isRandom}, Reset: {resetOnLaunch}, Index: {messageIndices[extendedSituation]}, isEva: {isEva}, situation: {extendedSituation}, #Messages: {texts.Count}");

            if (texts.Count < 1)
                return;

            UpdateMessageIndex(extendedSituation);


            SetText(texts[messageIndices[extendedSituation]]);
            base.OnDraw(bounds);

            lastVessel = FlightGlobals.ActiveVessel;
            lastDraw = DateTime.Now;
        }

        ExtendedSituation Extend(Vessel.Situations? situation, TriState isEva)
        {
            if (isEva == TriState.True)
            {
                var kerbal = FlightGlobals.ActiveVessel.evaController;

                var ragDolled = kerbal.isRagdoll;
                var onLadder = kerbal.OnALadder;
                var clambering = kerbal.fsm.currentStateName.StartsWith("Clamber", StringComparison.InvariantCulture);

                // Historian.Print(kerbal.fsm.currentStateName);

                if (ragDolled && situationTexts.ContainsKey(ExtendedSituation.RagDolled))
                    return ExtendedSituation.RagDolled;

                if (clambering && situationTexts.ContainsKey(ExtendedSituation.Clambering))
                    return ExtendedSituation.Clambering;

                if (onLadder && situationTexts.ContainsKey(ExtendedSituation.OnLadder))
                    return ExtendedSituation.OnLadder;
            }

            return (ExtendedSituation)situation;
        }

        void UpdateMessageIndex(ExtendedSituation extendedSituation)
        {
            
            if (DateTime.Now - lastDraw < minimumInterval)
            {
                // Historian.Print("No index update");
            }
            else
            {
                messageIndices[extendedSituation]++;

                if (isRandom)
                {
                    messageIndices[extendedSituation] = rnd.Next(0, situationTexts[extendedSituation].Count - 1);
                }
                else
                {
                    if (resetOnLaunch && lastVessel != FlightGlobals.ActiveVessel)
                    {
                        // Historian.Print("Vessel changed - reseting messages");
                        messageIndices[extendedSituation] = 0;
                    }
                }

                if (messageIndices[extendedSituation] >= situationTexts[extendedSituation].Count)
                    messageIndices[extendedSituation] = 0; // wrap around after end of list
            }
        }

    }
}
