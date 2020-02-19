using System;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    public interface IStream
    {
        bool ReadLine(out String[] line, out int lineNumber);
    }
}
