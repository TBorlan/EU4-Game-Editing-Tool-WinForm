using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    static class ValueTypes
    {
        private static readonly Dictionary<int, string> ValueTypeID = new Dictionary<int, string>()
        {
            [1] = "string",
            [2] = "boolean",
            [3] = "integer",
            [4] = "tag",
            [5] = "date"
        };

        public static string GetData(int index)
        {
            if (ValueTypeID.ContainsKey(index))
            {
                return ValueTypeID[index];
            }
            else
            {
                return default(string);
            }
        }
    }
}
