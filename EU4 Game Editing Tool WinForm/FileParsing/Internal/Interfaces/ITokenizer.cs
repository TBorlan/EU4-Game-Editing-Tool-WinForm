using System;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
#nullable enable
    internal interface ITokenizer
    {
        public String? GetNextToken();

        public void FeedLine(String[] line);
    }
#nullable restore
}
