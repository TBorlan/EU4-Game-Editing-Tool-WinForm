using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    class DataSet<T> where T:DataItem
    {
        public DataSet()
        {
            this.mData = new SortedSet<T>(mComparer);
        }

        public int mDataTypeID;

        protected SortedSet<T> mData;

        protected class Comparer<T> : IComparer<T>
        {
            public virtual int Compare(T x, T y)
            {
                throw new NotImplementedException();
            }
        }

        protected Comparer<T> mComparer;

        public void Add(T item)
        {
            if (!this.mData.Contains(item))
            {
                this.mData.Add(item);
            }
        }
    }
}
