using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    public interface IStream
    {
        bool ReadLine(out String[] line, out int lineNumber);
    }
}
