﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    class TextElement : IEquatable<TextElement>
    {
        public string mLeftValue;

        public string mRightValue;

        bool IEquatable<TextElement>.Equals(TextElement other)
        {
            return this.mLeftValue.Equals(other.mLeftValue) && this.mRightValue.Equals(other.mRightValue);
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }
            TextElement element = obj as TextElement;
            if(element == null)
            {
                return false;
            }
            else
            {
                return this.Equals(element);
            }
        }

    }
}