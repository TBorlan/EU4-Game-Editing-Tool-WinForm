﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    public interface IFileReader
    {
        string[] GetTokens(string filePath);
    }
}