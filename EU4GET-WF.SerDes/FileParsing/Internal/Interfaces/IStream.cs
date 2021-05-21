using System;

namespace EU4GET_WF.SerDes.FileParsing.Internal.Interfaces
{
    public interface IStream
    {
        bool ReadLine(out String[] line, out int lineNumber);
    }
}
