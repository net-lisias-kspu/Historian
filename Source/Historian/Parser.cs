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

            var tokens = new List<Token>();
            var args = new List<string>();
            // scan template text string for parameter tokens
            int i = 0, ti = 0, pi = 0, tokenStart = 0, paramStart = 0;

            var mode = ParseMode.Undefined;

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
                            var token = new Token {
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
            var token = new Token { IsLiteral = true, Args = null };
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
            }

            return sb;
        }

        public static void AddTokenizedRange(this List<List<Token>> target, IEnumerable<string> rawTexts)
        {
            foreach (var text in rawTexts)
            {
                target.Add(GetTokens(text));
            }
        }
    }

    
}
