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
    class ActionText : Text
    {
        Dictionary<LastAction, List<Token>> actionMessages = new Dictionary<LastAction, List<Token>>();

        protected override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            actionMessages.Add(LastAction.None, Parser.GetTokens( node.GetString("Default", "")));

            actionMessages.Add(LastAction.Abort, Parser.GetTokens(node.GetString("Abort", "")));
            actionMessages.Add(LastAction.Stage, Parser.GetTokens(node.GetString("Stage", "")));

            actionMessages.Add(LastAction.AG1, Parser.GetTokens(node.GetString("AG1", "")));
            actionMessages.Add(LastAction.AG2, Parser.GetTokens(node.GetString("AG2", "")));
            actionMessages.Add(LastAction.AG3, Parser.GetTokens(node.GetString("AG3", "")));
            actionMessages.Add(LastAction.AG4, Parser.GetTokens(node.GetString("AG4", "")));
            actionMessages.Add(LastAction.AG5, Parser.GetTokens(node.GetString("AG5", "")));
            actionMessages.Add(LastAction.AG6, Parser.GetTokens(node.GetString("AG6", "")));
            actionMessages.Add(LastAction.AG7, Parser.GetTokens(node.GetString("AG7", "")));
            actionMessages.Add(LastAction.AG8, Parser.GetTokens(node.GetString("AG8", "")));
            actionMessages.Add(LastAction.AG9, Parser.GetTokens(node.GetString("AG9", "")));
            actionMessages.Add(LastAction.AG10, Parser.GetTokens(node.GetString("AG10", "")));
        }

        protected override void OnDraw(Rect bounds)
        {
            var action = Historian.Instance.LastAction;
            TokenizedText = actionMessages[action];
            base.OnDraw(bounds);
        }

    }
}
