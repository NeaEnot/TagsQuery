using System.Collections.Generic;

namespace TagsQuery
{
    internal class Token : IToken
    {
        private string tokenString;

        internal Token(string tokenString)
        {
            this.tokenString = tokenString;
        }

        public bool Validate(List<string> tokenStrings)
        {
            bool answer = tokenStrings.Contains(tokenString);
            return answer;
        }

        public static bool operator ==(Token token1, Token token2)
        {
            return token1.tokenString == token2.tokenString;
        }

        public static bool operator !=(Token token1, Token token2)
        {
            return token1 == token2;
        }

        public override bool Equals(object obj)
        {
            return tokenString == ((Token)obj).tokenString;
        }
    }
}
