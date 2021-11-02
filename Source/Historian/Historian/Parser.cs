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
using System.Text;

namespace KSEA.Historian
{
    public struct Token
    {
        public string Key;
        public string[] Args;
        public bool IsLiteral;
    }

    enum ParseMode
    {
        Undefined = 0,
        InToken = 1,
        InParams = 2
    }


    public static class Parser
    {
        public static List<Token> GetTokens(string text)
        {

            List<Token> tokens = new List<Token>();
            List<string> args = new List<string>();
            // scan template text string for parameter tokens
            int i = 0, ti = 0, pi = 0, tokenStart = 0, paramStart = 0;

            ParseMode mode = ParseMode.Undefined;

            while (i < text.Length)
            {
                char ch = text[i];
                switch (ch)
                {
                    case '<':
                        if (ti > 0)
                        {
                            // we don't allow nested tokens so flush buffer as literal
                            tokens.Add(NewLiteral(text, tokenStart, ti));
                        }
                        // start of new token
                        tokenStart = i + 1;
                        ti = 0;
                        mode = ParseMode.InToken;
                        args = new List<string>();

                        break;
                    case '>':
                        if (mode == ParseMode.InToken)
                        {
                            // found end of token
                            Token token = new Token {
                                IsLiteral = false,
                                Key = text.Substring(tokenStart, ti)
                            };
                            if (args.Count > 0)
                            {
                                token.Args = args.ToArray();
                            }

                            tokens.Add(token);
                            mode = ParseMode.Undefined;
                            tokenStart = i + 1;
                            ti = 0;
                        }
                        else
                            ti++; // keep parsing literal
                        break;
                    case ',':
                        if (mode == ParseMode.InParams)
                        {
                            // handle param separator
                            args.Add(text.Substring(paramStart, pi).Trim());
                            paramStart = i + 1;
                            pi = 0;
                        }
                        else
                        {
                            ti++;
                        }
                        break;
                    case '(':
                        if (mode == ParseMode.InToken)
                        {
                            mode = ParseMode.InParams;
                            paramStart = i + 1;
                            pi = 0;
                        }
                        else
                            ti++;
                        break;
                    case ')':
                        if (mode == ParseMode.InParams)
                        {
                            mode = ParseMode.InToken;
                            args.Add(text.Substring(paramStart, pi).Trim());
                        }
                        else
                                ti++; // keep parsing literal
                        break;
                    default:
                        if (mode == ParseMode.InParams)
                            pi++;
                        else
                            ti++;
                        break;
                }
                i++;
            }
            //tokens.Add(new Token { Key = text.Substring(tokenStart), IsLiteral = true, Args = null });
            if (tokenStart < text.Length - 1)
                tokens.Add(NewLiteral(text, tokenStart, ti));

            return tokens;
        }

        public static Token NewLiteral(string text, int start, int end)
        {
            Token token = new Token { IsLiteral = true, Args = null };
            token.Key = text.Substring(start, end);
            return token;
        }

        public static StringBuilder ExpandTokenizedText(
            this StringBuilder sb, 
            List<Token> tokens,
            CommonInfo info,
            Dictionary<string, Action<StringBuilder, CommonInfo, string[]>> parsers,
            bool allowCustomTag)
        {
            Token token;
            for (int i = 0; i < tokens.Count; i++)
            {
                token = tokens[i];
                if (token.IsLiteral)
                {
                    sb.Append(token.Key);
                }
                else
                {
                    try
                    {
                        if (parsers.ContainsKey(token.Key) && (token.Key != "Custom" || allowCustomTag))
                        {
                            parsers[token.Key](sb, info, token.Args);
                        }
                        else
                        {
                            sb.Append("<").Append(token.Key);
                            if (token.Args != null && token.Args.Length > 0)
                                sb.Append("(").Append(String.Join(",", token.Args)).Append(")");
                            sb.Append(">");
                        }
                    }
                    catch (Exception e)
                    {
                        sb.Append("Error expanding <").Append(token.Key).Append(">");
                        Historian.Print($"Exception: {e.Message} - {e.StackTrace}");
                    }
                }
            }

            return sb;
        }

        public static void AddTokenizedRange(this List<List<Token>> target, IEnumerable<string> rawTexts)
        {
            foreach (string text in rawTexts)
            {
                target.Add(GetTokens(text));
            }
        }
    }

    
}
