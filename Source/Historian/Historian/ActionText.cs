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
