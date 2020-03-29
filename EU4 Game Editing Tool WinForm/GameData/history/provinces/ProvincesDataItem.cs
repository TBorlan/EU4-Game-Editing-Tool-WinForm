using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    class ProvincesDataItem : DataItem, IComparer<ProvincesDataItem>
    {
        public DateTime? mDate;

        public int Compare(ProvincesDataItem x, ProvincesDataItem y)
        {
            if (x.mDate == null && y.mDate != null)
            {
                return -1;
            }
            else if(x.mDate != null && y.mDate == null)
            {
                return 1;
            }
            else if(x.mDate == null && y.mDate == null)
            {
                return 0;
            }
            else
            {
                return ((DateTime)x.mDate).CompareTo((DateTime)y.mDate);
            }
        }
    }
}
