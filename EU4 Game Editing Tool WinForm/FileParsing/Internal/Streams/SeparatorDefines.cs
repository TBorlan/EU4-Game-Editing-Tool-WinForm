using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Streams
{
    enum NumberOfSeparators : int
    {
        UnokownNrOfSeparators = 0,
        UnlimitedNrOfSeparators = -1
    }

    static internal class Separators
    {
        static internal readonly char[] CSV_SEPARATOR = new char[] { ';' };
        static internal readonly char[] TEXT_SEPARATOR = new char[] { ' ', '\t' };
    }

}
