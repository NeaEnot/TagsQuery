using System.Collections.Generic;

namespace TagsQuery
{
    internal class OperatorOr : IToken
    {
        private IToken token1;
        private IToken token2;

        internal OperatorOr(IToken token1, IToken token2)
        {
            this.token1 = token1;
            this.token2 = token2;
        }

        public bool Validate(List<string> tokenStrings)
        {
            return token1.Validate(tokenStrings) || token2.Validate(tokenStrings);
        }
    }
}
