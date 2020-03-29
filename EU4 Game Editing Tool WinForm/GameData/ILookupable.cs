using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    interface ILookupable
    {
        string LookUpId(int id);

        int LookUpValue(string value);
    }
}
