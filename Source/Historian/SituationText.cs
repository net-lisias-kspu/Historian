using System.Collections.Generic;
using UnityEngine;

namespace KSEA.Historian
{
    public class SituationText : Text
    {
        Dictionary<Vessel.Situations, string> situations = new Dictionary<Vessel.Situations, string>();
        string defaultSituation = "";
        string ragDolledSituation = "";
        string climbingSituation = "";

        TriState evaOnly = TriState.UseDefault;

        protected override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            defaultSituation = node.GetString("Default", "");
            ragDolledSituation = node.GetString("RagDolled", "");
            climbingSituation = node.GetString("Climbing", "");

            evaOnly = node.GetEnum("EvaOnly", TriState.UseDefault);

            situations.Add(Vessel.Situations.LANDED, node.GetString("Landed", ""));
            situations.Add(Vessel.Situations.FLYING, node.GetString("Flying", ""));
            situations.Add(Vessel.Situations.SPLASHED, node.GetString("Splashed", ""));
            situations.Add(Vessel.Situations.PRELAUNCH, node.GetString("Prelaunch", ""));
            situations.Add(Vessel.Situations.SUB_ORBITAL, node.GetString("SubOrbital", ""));
            situations.Add(Vessel.Situations.ORBITING, node.GetString("Orbiting", ""));
            situations.Add(Vessel.Situations.ESCAPING, node.GetString("Escaping", ""));
            situations.Add(Vessel.Situations.DOCKED, node.GetString("Docked", ""));
        }

        protected override void OnDraw(Rect bounds)
        {
            var isEva = (FlightGlobals.ActiveVessel?.isEVA).ToTriState();
            if (evaOnly != TriState.UseDefault && evaOnly != isEva)
                return;

            var situation = FlightGlobals.ActiveVessel?.situation;
            var text = (situation.HasValue && situations.ContainsKey(situation.Value)) ? situations[situation.Value] : defaultSituation;

            if (isEva == TriState.True)
            {
                var kerbal = FlightGlobals.ActiveVessel.evaController;

                var ragDolled = kerbal.isRagdoll;
                var onLadder = kerbal.OnALadder;
                var clambering = kerbal.fsm.currentStateName.StartsWith("Clamber");

                if (ragDolled && !string.IsNullOrEmpty(ragDolledSituation))
                {
                    text = ragDolledSituation;
                }

                if ((onLadder || clambering) && !string.IsNullOrEmpty(climbingSituation))
                {
                    text = climbingSituation;
                }
            }

            SetText(text);
            base.OnDraw(bounds);
        }
    }
}