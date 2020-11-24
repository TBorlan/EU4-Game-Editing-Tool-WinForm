using System;
using System.Collections.Generic;

namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    class DataItemComparer<T> : IComparer<T> where T : DataItem
    {
        public int Compare(T x, T y)
        {
            throw new NotImplementedException();
        }
    }
}
