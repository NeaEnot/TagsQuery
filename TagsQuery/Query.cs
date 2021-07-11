using System.Collections.Generic;

namespace TagsQuery
{
    public class Query
    {
        private IToken token;

        public Query(string queryString)
        {
            Tokenize(GetTokensStrings(queryString));
        }

        public bool Validate(string str)
        {
            bool answer = token.Validate(GetTokensStrings(str));
            return answer;
        }

        private List<string> GetTokensStrings(string str)
        {
            List<string> tokensStrings = new List<string>();

            bool isTokenStart = false;
            string currentToken = "";

            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '\"':
                        if (isTokenStart)
                        {
                            currentToken += str[i];
                            tokensStrings.Add(currentToken);
                            isTokenStart = false;
                            currentToken = "";
                        }
                        else
                        {
                            isTokenStart = true;
                            currentToken += str[i];
                        }
                        break;
                    case '-':
                        if (!isTokenStart)
                        {
                            tokensStrings.Add("-");
                        }
                        break;
                    case '&':
                        if (!isTokenStart)
                        {
                            tokensStrings.Add("&");
                        }
                        break;
                    default:
                        if (isTokenStart)
                        {
                            currentToken += str[i];
                        }
                        break;
                }
            }

            return tokensStrings;
        }

        private void Tokenize(List<string> tokenStrings)
        {
            Dictionary<int, IToken> dict = new Dictionary<int, IToken>();
            int[] ids = new int[tokenStrings.Count];
            int currentId = 1;
            int count;

            do
            {
                for (int i = 0; i < tokenStrings.Count; i++)
                {
                    if (tokenStrings[i] != "-" && tokenStrings[i] != "&" && ids[i] == 0)
                    {
                        dict.Add(currentId, new Token(tokenStrings[i], true));
                        ids[i] = currentId;
                        currentId++;
                    }
                }

                for (int i = 0; i < tokenStrings.Count; i++)
                {
                    if (tokenStrings[i] == "-" && ids[i + 1] > 0 && ids[i] == 0)
                    {
                        dict.Add(currentId, new Token(tokenStrings[i + 1], false));
                        ids[i] = currentId;
                        ids[i + 1] = currentId;
                        currentId++;
                    }
                }

                for (int i = 0; i < tokenStrings.Count; i++)
                {
                    if (tokenStrings[i] == "&" && ids[i - 1] > 0 && ids[i + 1] > 0 && ids[i] == 0)
                    {
                        dict.Add(currentId, new OperatorAnd(dict[ids[i - 1]], dict[ids[i + 1]], true));
                        ids[i] = currentId;
                        ids[i - 1] = currentId;
                        ids[i + 1] = currentId;
                        currentId++;
                    }
                }

                count = 0;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (ids[i] == 0)
                    {
                        count++;
                    }
                }
            } while (count > 0);

            token = dict[currentId - 1];
        }
    }
}
