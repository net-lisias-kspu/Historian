using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSEA.Historian
{
    class ActionText : Text
    {
        Dictionary<LastAction, string> actionMessages = new Dictionary<LastAction, string>();

        protected override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            actionMessages.Add(LastAction.None, node.GetString("Default", ""));

            actionMessages.Add(LastAction.Abort, node.GetString("Abort", ""));
            actionMessages.Add(LastAction.Stage, node.GetString("Stage", ""));

            actionMessages.Add(LastAction.AG1, node.GetString("AG1", ""));
            actionMessages.Add(LastAction.AG2, node.GetString("AG2", ""));
            actionMessages.Add(LastAction.AG3, node.GetString("AG3", ""));
            actionMessages.Add(LastAction.AG4, node.GetString("AG4", ""));
            actionMessages.Add(LastAction.AG5, node.GetString("AG5", ""));
            actionMessages.Add(LastAction.AG6, node.GetString("AG6", ""));
            actionMessages.Add(LastAction.AG7, node.GetString("AG7", ""));
            actionMessages.Add(LastAction.AG8, node.GetString("AG8", ""));
            actionMessages.Add(LastAction.AG9, node.GetString("AG9", ""));
            actionMessages.Add(LastAction.AG10, node.GetString("AG10", ""));
        }

        protected override void OnDraw(Rect bounds)
        {
            var action = Historian.Instance.LastAction;
            var text = actionMessages[action];
            SetText(text);
            base.OnDraw(bounds);
        }

    }
}
