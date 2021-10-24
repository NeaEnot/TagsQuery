using System.Collections.Generic;
using System.Linq;

namespace TagsQuery
{
    internal class TokenLike : IToken
    {
        private string tokenString;

        internal TokenLike(string tokenString)
        {
            this.tokenString = tokenString.Replace("\'", "");
        }

        public bool Validate(List<string> tokenStrings)
        {
            bool answer = tokenStrings.Where(rec => rec.Contains(tokenString)).Count() > 0;
            return answer;
        }

        public static bool operator ==(TokenLike token1, TokenLike token2)
        {
            return token1.tokenString == token2.tokenString;
        }

        public static bool operator !=(TokenLike token1, TokenLike token2)
        {
            return token1 != token2;
        }

        public override bool Equals(object obj)
        {
            return tokenString == ((TokenLike)obj).tokenString;
        }
    }
}
