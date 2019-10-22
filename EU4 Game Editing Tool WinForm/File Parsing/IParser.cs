using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.File_Parsing
{
    interface IParser
    {
        TextNode ParseFile(string filename);
    }
}
