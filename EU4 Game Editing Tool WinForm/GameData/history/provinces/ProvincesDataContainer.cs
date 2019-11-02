using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using EU4_Game_Editing_Tool_WinForm.FileParsing;

namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    class ProvincesDataContainer : DataContainer<ProvincesDataContainer,Color,ProvincesDataItem>
    {
        static ProvincesDataContainer()
        {
            mValidTypeList = Enumerable.Range(1, 17).ToArray();
            mData = new Dictionary<int, List<DataSet<ProvincesDataItem>>>();
            mLUT = new LUT<Color>();
            mInstance = new ProvincesDataContainer();
        }

        protected override void DeserializeNode(TextNode node, int id)
        {

            this.DeserializeElement(node, id);
            foreach(TextNode childNode in node.mChildNodes)
            {
                if(childNode.mValue != "revolt")
                this.DeserializeNode(childNode, id);
            }
        }

        private void DeserializeElement(TextNode node,int id)
        {
            foreach(TextElement element in node.mChildElements)
            {
                if (this.CheckAllowedDataType(element.mLeftValue))
                {
                    int dataID = DataTypes.GetIndex(element.mLeftValue);
                    ProvincesDataItem item = new ProvincesDataItem();
                    DataSet<ProvincesDataItem> set = (from queryResult in mData[id]
                                                      where queryResult.mDataTypeID == dataID
                                                      select queryResult).FirstOrDefault();
                    if(set == default(DataSet<ProvincesDataItem>))
                    {
                        set = new DataSet<ProvincesDataItem>();
                    }
                    if(node.mValue != null)
                    {
                        item.mDate = ValueTypes.GetDate(node.mValue);
                    }
                    else
                    {
                        item.mDate = null;
                    }
                    ILookupable lut = DataTypes.GetDataTypeLUT(dataID);
                    if(lut != null)
                    {
                        int valueId = lut.LookUpValue(element.mRightValue);
                        if (valueId > 0)
                        {
                            item.mValueID = valueId;
                            item.mValueHasID = true;
                            set.Add(item);
                            return;
                        }
                        item.mValue = element.mRightValue;
                        item.mValueHasID = false;
                        set.Add(item);
                        return;
                    }
                }
            }
        }
    }
}
