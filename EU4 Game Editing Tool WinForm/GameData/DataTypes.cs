using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    static class DataTypes
    {
        private static readonly Dictionary<int, string> DataTypeID = new Dictionary<int, string>()
        {
            [1] = "culture",
            [2] = "religion",
            [3] = "hre",
            [4] = "base_tax",
            [5] = "base_production",
            [6] = "base_manpower",
            [7] = "trade_goods",
            [8] = "capital",
            [9] = "native_hostileness",
            [10] = "native_size",
            [11] = "native_ferocity",
            [12] = "discovered_by",
            [13] = "owner",
            [14] = "controller",
            [15] = "add_core",
            [16] = "is_city",
            [17] = "remove_core",
            [18] = "type"
        };

        public static int GetIndex(string data)
        {
            if (DataTypeID.ContainsValue(data))
            {
                foreach (KeyValuePair<int, string> pair in DataTypeID)
                {
                    if (pair.Value.Equals(data))
                    {
                        return pair.Key;
                    }
                }
                return 0;
            }
            else
            {
                return 0;
            }
        }
    }
}
