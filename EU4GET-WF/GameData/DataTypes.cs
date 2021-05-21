using System.Collections.Generic;

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
        };

        private static readonly Dictionary<int, int[]> AllowedValueTypes = new Dictionary<int, int[]>()
        {
            [1] = new int[] { 1 },
            [2] = new int[] { 1 },
            [3] = new int[] { 2 },
            [4] = new int[] { 3 },
            [5] = new int[] { 3 },
            [6] = new int[] { 3 },
            [7] = new int[] { 1 },
            [8] = new int[] { 1 },
            [9] = new int[] { 3 },
            [10] = new int[] { 3 },
            [11] = new int[] { 3 },
            [12] = new int[] { 4 },
            [13] = new int[] { 4 },
            [14] = new int[] { 4 },
            [15] = new int[] { 4 },
            [16] = new int[] { 2 },
            [17] = new int[] { 4 },
        };

        private static readonly Dictionary<int, ILookupable> DataToLUTCorespondence = new Dictionary<int, ILookupable>
        {

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

        public static ILookupable GetDataTypeLUT(int id)
        {
            if (DataToLUTCorespondence.ContainsKey(id))
            {
                return DataToLUTCorespondence[id];
            }
            else
            {
                return null;
            }
        }
    }
}
