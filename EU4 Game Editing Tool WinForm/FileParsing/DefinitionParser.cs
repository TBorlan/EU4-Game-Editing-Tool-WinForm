using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace EU4_Game_Editing_Tool_WinForm
{
    static class DefinitionParser
    {
        static private readonly char[] Separator = new char[1] { ';' };

        public static Province? ReadElement(StreamReader reader)
        {
            String line = reader.ReadLine();
            String[] values = line.Split(Separator, 5);
            Province province = new Province();
            try
            {
                province.id = int.Parse(values[0]);
                province.color = Color.FromArgb(int.Parse(values[1]),
                                                int.Parse(values[2]),
                                                int.Parse(values[3]));
                return province;
            }
            catch (FormatException)
            {
                return null;
            }
        }
        public static Province[] ReadAllElements(StreamReader reader)
        {
            List<Province> tempList = new List<Province>();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                Province? province = ReadElement(reader);
                if(province!= null)
                {
                    tempList.Add((Province)province);
                }
            }
            return tempList.ToArray();
        }
        public static StreamReader GetReader(String file)
        {
            FileStream stream = File.OpenRead(file);
            StreamReader reader = new StreamReader(stream);
            return reader;
        }
        public static void CloseReader(StreamReader stream)
        {
            stream.Close();
        }

    }
}
