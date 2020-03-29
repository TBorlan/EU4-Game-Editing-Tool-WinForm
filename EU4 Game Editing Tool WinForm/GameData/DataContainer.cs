using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EU4_Game_Editing_Tool_WinForm.FileParsing;

namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    abstract class DataContainer<T,X,Y> where Y:DataItem
    {
        static protected DataContainer<T, X, Y> mInstance;

        public static int[] mValidTypeList;

        protected static LUT<X> mLUT;

        protected static Dictionary<int, List<DataSet<Y>>> mData;

        public bool CheckAllowedDataType(string type)
        {
            int id = DataTypes.GetIndex(type);  //get data type id
            if (id == 0)  // if id not defined , it means it isn't a data type
            {
                return false;
            }
            else
            {
                if (mValidTypeList.Contains(id))  // see if container allows that data type
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        public static void Deserialize(TextNode node, int id)
        {
            mInstance.DeserializeNode(node,id);
        }

        protected abstract void DeserializeNode(TextNode node, int id);

    }
}
