using System;
using System.Collections.Generic;

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

        public static bool CheckValueFormat(int id, string value)
        {
            switch (id)
            {
                case 1:
                    {
                        return CheckString(value);
                    }
                case 2:
                    {
                        return CheckBoolean(value);
                    }
                case 3:
                    {
                        return CheckInteger(value);
                    }
                case 4:
                    {
                        return CheckTAG(value);
                    }
                case 5:
                    {
                        return CheckDate(value);
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        private static bool CheckString(string value)
        {
            return true;
        }

        private static bool CheckBoolean(string value)
        {
            return true;
        }

        private static bool CheckInteger(string value)
        {
            return true;
        }

        private static bool CheckTAG(string value)
        {
            return true;
        }

        private static bool CheckDate(string value)
        {
            return true;
        }

        public static DateTime GetDate(string text)
        {
            string[] strings = text.Split('.');
            DateTime dateTime = new DateTime(int.Parse(strings[0]),
                                             int.Parse(strings[1]),
                                             int.Parse(strings[2]));
            return dateTime;
        }
    }
}
