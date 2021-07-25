using System.Collections.Generic;

namespace TagsQuery
{
    public class Query
    {
        private IToken token;

        public Query(string queryString)
        {
            token = Tokenize(GetTokensStrings(queryString));
        }

        public bool Validate(string str)
        {
            bool answer = token != null ? token.Validate(GetTokensStrings(str)) : true;
            return answer;
        }

        public static List<string> GetTokensStrings(string str)
        {
            List<string> tokensStrings = new List<string>();

            int tokenStart = 0;
            string currentToken = "";

            for (int i = 0; i < str.Length; i++)
            {
                if (tokenStart == 1)
                {
                    if (str[i] == '\"')
                    {
                        currentToken += str[i];
                        tokensStrings.Add(currentToken);
                        tokenStart = 0;
                        currentToken = "";
                    }
                    else
                    {
                        currentToken += str[i];
                    }
                }
                else if (tokenStart == 2)
                {
                    if (str[i] == '\'')
                    {
                        currentToken += str[i];
                        tokensStrings.Add(currentToken);
                        tokenStart = 0;
                        currentToken = "";
                    }
                    else
                    {
                        currentToken += str[i];
                    }
                }
                else
                {
                    switch (str[i])
                    {
                        case '\"':
                            tokenStart = 1;
                            currentToken += str[i];
                            break;
                        case '\'':
                            tokenStart = 2;
                            currentToken += str[i];
                            break;
                        case '&':
                            tokensStrings.Add("&");
                            break;
                        case '|':
                            tokensStrings.Add("|");
                            break;
                        case '-':
                            tokensStrings.Add("-");
                            break;
                        case '(':
                            tokensStrings.Add("(");
                            break;
                        case ')':
                            tokensStrings.Add(")");
                            break;
                    }
                }
            }

            return tokensStrings;
        }

        private static IToken Tokenize(List<string> tokenStrings)
        {
            if (tokenStrings.Count == 0)
            {
                return null;
            }

            Dictionary<int, IToken> dict = new Dictionary<int, IToken>();
            int[] ids = new int[tokenStrings.Count];
            int currentId = 1;
            int count;

            do
            {
                currentId = FindExactTokens(tokenStrings, dict, ids, currentId);
                currentId = FindLikeTokens(tokenStrings, dict, ids, currentId);
                currentId = FindUnderTokens(tokenStrings, dict, ids, currentId);
                currentId = FindOperatorsNot(tokenStrings, dict, ids, currentId);
                currentId = FindOperatorsAnd(tokenStrings, dict, ids, currentId);
                currentId = FindOperatorsOr(tokenStrings, dict, ids, currentId);

                count = 0;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (ids[i] == 0)
                    {
                        count++;
                    }
                }
            } while (count > 0);

            return dict[currentId - 1];
        }

        private static int FindExactTokens(List<string> tokenStrings, Dictionary<int, IToken> dict, int[] ids, int currentId)
        {
            for (int i = 0; i < tokenStrings.Count; i++)
            {
                if (tokenStrings[i] != "-" &&
                    tokenStrings[i] != "&" &&
                    tokenStrings[i] != "|" &&
                    tokenStrings[i] != "(" &&
                    tokenStrings[i] != ")" &&
                    tokenStrings[i][0] == '\"' && tokenStrings[i][tokenStrings[i].Length - 1] == '\"' &&
                    ids[i] == 0)
                {
                    dict.Add(currentId, new TokenExact(tokenStrings[i]));
                    ids[i] = currentId;
                    currentId++;
                }
            }

            return currentId;
        }

        private static int FindLikeTokens(List<string> tokenStrings, Dictionary<int, IToken> dict, int[] ids, int currentId)
        {
            for (int i = 0; i < tokenStrings.Count; i++)
            {
                if (tokenStrings[i] != "-" &&
                    tokenStrings[i] != "&" &&
                    tokenStrings[i] != "|" &&
                    tokenStrings[i] != "(" &&
                    tokenStrings[i] != ")" &&
                    tokenStrings[i][0] == '\'' && tokenStrings[i][tokenStrings[i].Length - 1] == '\'' &&
                    ids[i] == 0)
                {
                    dict.Add(currentId, new TokenLike(tokenStrings[i]));
                    ids[i] = currentId;
                    currentId++;
                }
            }

            return currentId;
        }

        private static int FindUnderTokens(List<string> tokenStrings, Dictionary<int, IToken> dict, int[] ids, int currentId)
        {
            for (int i = 0; i < tokenStrings.Count; i++)
            {
                if (tokenStrings[i] == "(" && ids[i] == 0)
                {
                    int start = i, end = i + 1;
                    List<string> underToken = new List<string>();

                    int nest = 0;

                    for (int j = i + 1; j < tokenStrings.Count; j++)
                    {
                        if (tokenStrings[j] == "(")
                        {
                            nest++;
                        }
                        if (tokenStrings[j] == ")")
                        {
                            if (nest > 0)
                            {
                                nest--;
                            }
                            else
                            {
                                end = j;
                                break;
                            }
                        }
                        underToken.Add(tokenStrings[j]);
                    }

                    dict.Add(currentId, Tokenize(underToken));
                    for (int j = start; j <= end; j++)
                    {
                        ids[j] = currentId;
                    }
                    currentId++;
                }
            }

            return currentId;
        }

        private static int FindOperatorsNot(List<string> tokenStrings, Dictionary<int, IToken> dict, int[] ids, int currentId)
        {
            for (int i = 0; i < tokenStrings.Count; i++)
            {
                if (tokenStrings[i] == "-" && ids[i + 1] > 0 && ids[i] == 0)
                {
                    dict.Add(currentId, new OperatorNot(dict[ids[i + 1]]));
                    ids[i] = currentId;

                    int oldId = ids[i + 1];
                    for (int j = 0; j < ids.Length; j++)
                    {
                        if (ids[j] == oldId)
                        {
                            ids[j] = currentId;
                        }
                    }

                    currentId++;
                }
            }

            return currentId;
        }

        private static int FindOperatorsAnd(List<string> tokenStrings, Dictionary<int, IToken> dict, int[] ids, int currentId)
        {
            for (int i = 0; i < tokenStrings.Count; i++)
            {
                if (tokenStrings[i] == "&" && ids[i - 1] > 0 && ids[i + 1] > 0 && ids[i] == 0)
                {
                    dict.Add(currentId, new OperatorAnd(dict[ids[i - 1]], dict[ids[i + 1]]));
                    ids[i] = currentId;

                    int oldIdLeft = ids[i - 1];
                    int oldIdRight = ids[i + 1];
                    for (int j = 0; j < ids.Length; j++)
                    {
                        if (ids[j] == oldIdLeft)
                        {
                            ids[j] = currentId;
                        }
                        if (ids[j] == oldIdRight)
                        {
                            ids[j] = currentId;
                        }
                    }

                    currentId++;
                }
            }

            return currentId;
        }

        private static int FindOperatorsOr(List<string> tokenStrings, Dictionary<int, IToken> dict, int[] ids, int currentId)
        {
            for (int i = 0; i < tokenStrings.Count; i++)
            {
                if (tokenStrings[i] == "|" && ids[i - 1] > 0 && ids[i + 1] > 0 && ids[i] == 0)
                {
                    dict.Add(currentId, new OperatorOr(dict[ids[i - 1]], dict[ids[i + 1]]));
                    ids[i] = currentId;

                    int oldIdLeft = ids[i - 1];
                    int oldIdRight = ids[i + 1];
                    for (int j = 0; j < ids.Length; j++)
                    {
                        if (ids[j] == oldIdLeft)
                        {
                            ids[j] = currentId;
                        }
                        if (ids[j] == oldIdRight)
                        {
                            ids[j] = currentId;
                        }
                    }

                    currentId++;
                }
            }

            return currentId;
        }
    }
}
