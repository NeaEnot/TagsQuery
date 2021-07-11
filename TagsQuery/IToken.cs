using System.Collections.Generic;

namespace TagsQuery
{
    internal interface IToken
    {
        public bool Validate(List<string> tokenStrings);
    }
}
