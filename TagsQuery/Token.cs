using System.Collections.Generic;

namespace TagsQuery
{
    internal class Token : IToken
    {
        private string tokenString;
        private bool boolean;

        internal Token(string tokenString, bool boolean)
        {
            this.tokenString = tokenString;
            this.boolean = boolean;
        }

        public bool Validate(List<string> tokenStrings)
        {
            bool answer = tokenStrings.Contains(tokenString);
            return answer == boolean;
        }

        public static bool operator ==(Token token1, Token token2)
        {
            return token1.tokenString == token2.tokenString && token1.boolean == token2.boolean;
        }

        public static bool operator !=(Token token1, Token token2)
        {
            return token1 == token2;
        }

        public override bool Equals(object obj)
        {
            return tokenString == ((Token)obj).tokenString && boolean == ((Token)obj).boolean;
        }
    }
}
