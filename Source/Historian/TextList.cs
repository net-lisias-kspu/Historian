using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSEA.Historian
{
    class TextList : Text
    {
        List<string> texts = new List<string>();
        bool isRandom = false;
        int messageIndex = 0;
        bool resetOnLaunch = false;
        Vessel lastVessel = null;
        System.Random rnd = new System.Random();
        DateTime lastDraw = DateTime.Now;
        TimeSpan minimumInterval = TimeSpan.FromMilliseconds(200);

        protected override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            texts = new List<string>();

            isRandom = node.GetBoolean("Random", false);
            resetOnLaunch = node.GetBoolean("ResetOnLaunch", false);

            texts.AddRange(node.GetValues("Text"));
            messageIndex = -1;
        }

        protected override void OnDraw(Rect bounds)
        {
            
            Historian.Print($"Random text: {isRandom}, Reset: {resetOnLaunch}, Index: {messageIndex}");
            if (texts.Count < 1)
                return;

            if (DateTime.Now - lastDraw < minimumInterval)
            {
                Historian.Print("No index update");
            }
            else
            {
                messageIndex++;

                if (isRandom)
                {
                    messageIndex = rnd.Next(0, texts.Count - 1);
                }
                else
                {
                    if (resetOnLaunch && lastVessel != FlightGlobals.ActiveVessel)
                    {
                        Historian.Print("Vessel changed - reseting messages");
                        messageIndex = 0;
                    }
                }

                if (messageIndex >= texts.Count)
                    messageIndex = 0; // wrap around after end of list
            }

            SetText(texts[messageIndex]);
            base.OnDraw(bounds);

            lastVessel = FlightGlobals.ActiveVessel;
            lastDraw = DateTime.Now;
        }
    }
}
