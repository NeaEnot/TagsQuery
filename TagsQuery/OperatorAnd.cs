using System.Collections.Generic;

namespace TagsQuery
{
    internal class OperatorAnd : IToken
    {
        private IToken token1;
        private IToken token2;

        internal OperatorAnd(IToken token1, IToken token2)
        {
            this.token1 = token1;
            this.token2 = token2;
        }

        public bool Validate(List<string> tokenStrings)
        {
            bool answer = token1.Validate(tokenStrings) && token2.Validate(tokenStrings);
            return answer;
        }
    }
}
