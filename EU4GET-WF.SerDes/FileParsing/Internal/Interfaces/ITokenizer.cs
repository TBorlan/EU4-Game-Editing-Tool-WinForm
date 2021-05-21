using System;

namespace EU4GET_WF.SerDes.FileParsing.Internal.Interfaces
{
#nullable enable
    internal interface ITokenizer
    {
        public String? GetNextToken();

        public void FeedLine(String[] line);
    }
#nullable restore
}
