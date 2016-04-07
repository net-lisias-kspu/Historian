using System.Collections.Generic;
using UnityEngine;

namespace KSEA.Historian
{
    public class SituationText : Text
    {
        Dictionary<Vessel.Situations, string> m_situations = new Dictionary<Vessel.Situations, string>();
        string m_Default = "";

        protected override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);


            m_Default = node.GetString("Default", "");

            m_situations.Add(Vessel.Situations.LANDED, node.GetString("Landed", ""));
            m_situations.Add(Vessel.Situations.FLYING, node.GetString("Flying", ""));
            m_situations.Add(Vessel.Situations.SPLASHED, node.GetString("Splashed", ""));
            m_situations.Add(Vessel.Situations.PRELAUNCH, node.GetString("Prelaunch", ""));
            m_situations.Add(Vessel.Situations.SUB_ORBITAL, node.GetString("SubOrbital", ""));
            m_situations.Add(Vessel.Situations.ORBITING, node.GetString("Orbiting", ""));
            m_situations.Add(Vessel.Situations.ESCAPING, node.GetString("Escaping", ""));
            m_situations.Add(Vessel.Situations.DOCKED, node.GetString("Docked", ""));
        }

        protected override void OnDraw(Rect bounds)
        {
            var situation = FlightGlobals.ActiveVessel?.situation;
            var text = (situation.HasValue && m_situations.ContainsKey(situation.Value)) ? m_situations[situation.Value] : m_Default;
            SetText(text);
            base.OnDraw(bounds);
        }
    }
}