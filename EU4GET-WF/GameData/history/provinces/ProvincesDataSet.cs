using System;

namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    class ProvincesDataSet : DataSet<ProvincesDataItem>
    {
        public ProvincesDataSet() : base()
        {
            this.mComparer = new ProvinceDataItemComparer();
        }

        private class ProvinceDataItemComparer : ProvincesDataSet.Comparer<ProvincesDataItem>
        {
            public override int Compare(ProvincesDataItem x, ProvincesDataItem y)
            {
                if (x.mDate == null && y.mDate != null)
                {
                    return -1;
                }
                else if (x.mDate != null && y.mDate == null)
                {
                    return 1;
                }
                else if (x.mDate == null && y.mDate == null)
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
}
