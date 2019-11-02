using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    class LUT<T> : IEnumerable<KeyValuePair<int,T>>, IEnumerable, ILookupable
    {
        public LUT(int count)
        {
            this.mTable = new Dictionary<int, T>(count);
            this.mKeys = new SortedSet<int>();
        }

        public LUT()
        {
            this.mTable = new Dictionary<int, T>();
            this.mKeys = new SortedSet<int>();
        }

        private Dictionary<int, T> mTable;

        private SortedSet<int> mKeys;

        public LUTStatus AddLutEntry(T data)
        {
            if (!mTable.ContainsValue(data))
            {
                int key;
                if (this.mKeys.Count < this.mKeys.Max)
                {
                    key = this.mKeys.Max - 1;
                    while (key >= 0)
                    {
                        if (!this.mKeys.Contains(key))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    key = this.mKeys.Max + 1;
                }
                this.mTable.Add(key, data);
                this.mKeys.Add(key);
                return LUTStatus.OK;
            }
            else
            {
                return LUTStatus.ErrorDuplicateEntry;
            }
        }

        public LUTStatus AddLutEntry(int index, T data)
        {
            if (!mTable.ContainsValue(data) || !mTable.ContainsKey(index))
            {
                mTable.Add(index, data);
                mKeys.Add(index);
                return LUTStatus.OK;
            }
            else
            {
                return LUTStatus.ErrorDuplicateEntry;
            }
        }

        public T GetData(int index)
        {
            if (mTable.ContainsKey(index))
            {
                return mTable[index];
            }
            else
            {
                return default(T);
            }
        }

        public int GetIndex(T data)
        {
            if (mTable.ContainsValue(data))
            {
                foreach(KeyValuePair<int,T> pair in mTable)
                {
                    if (pair.Value.Equals(data))
                    {
                        return pair.Key;
                    }
                }
                return default(int);
            }
            else
            {
                return default(int);
            }
        }

        public int Count()
        {
            return mTable.Count;
        }

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<int,T>>)mTable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public string LookUpId(int id)
        {
            if (this.mKeys.Contains(id))
            {
                return this.mTable[id].ToString();
            }
            else
            {
                return null;
            }
        }

        public int LookUpValue(string value)
        {
            foreach (KeyValuePair<int, T> keyValuePair in mTable)
            {
                if(keyValuePair.Value.ToString() == value)
                {
                    return keyValuePair.Key;
                }
            }
            return 0;
        }
    }
}
