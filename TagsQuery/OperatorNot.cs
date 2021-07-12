using System.Collections.Generic;

namespace TagsQuery
{
    internal class OperatorNot : IToken
    {
        private IToken token;

        internal OperatorNot(IToken token)
        {
            this.token = token;
        }

        public bool Validate(List<string> tokenStrings)
        {
            return !token.Validate(tokenStrings);
        }
    }
}
