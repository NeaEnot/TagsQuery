using System.Collections.Generic;

namespace TagsQuery
{
    internal class TokenExact : IToken
    {
        private string tokenString;

        internal TokenExact(string tokenString)
        {
            this.tokenString = tokenString;
        }

        public bool Validate(List<string> tokenStrings)
        {
            bool answer = tokenStrings.Contains(tokenString);
            return answer;
        }

        public static bool operator ==(TokenExact token1, TokenExact token2)
        {
            return token1.tokenString == token2.tokenString;
        }

        public static bool operator !=(TokenExact token1, TokenExact token2)
        {
            return token1 == token2;
        }

        public override bool Equals(object obj)
        {
            return tokenString == ((TokenExact)obj).tokenString;
        }
    }
}
