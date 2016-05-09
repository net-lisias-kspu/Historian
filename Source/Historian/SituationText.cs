using System.Collections.Generic;
using UnityEngine;

namespace KSEA.Historian
{
    public class SituationText : Text
    {
        Dictionary<Vessel.Situations, string> situations = new Dictionary<Vessel.Situations, string>();
        string defaultSituation = "";

        protected override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            defaultSituation = node.GetString("Default", "");

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
            var situation = FlightGlobals.ActiveVessel?.situation;
            var text = (situation.HasValue && situations.ContainsKey(situation.Value)) ? situations[situation.Value] : defaultSituation;
            SetText(text);
            base.OnDraw(bounds);
        }
    }
}