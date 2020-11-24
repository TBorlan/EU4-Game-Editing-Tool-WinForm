using System;

namespace EU4GET_WF.SerDes.FileParsing
{
    public class TextElement : IEquatable<TextElement>
    {

        public string _mLeftValue;

        public string _mRightValue;

        bool IEquatable<TextElement>.Equals(TextElement other)
        {
            return !(other is null) && this._mLeftValue.Equals(other._mLeftValue) && this._mRightValue.Equals(other._mRightValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.Equals((TextElement)obj);
        }

        public static bool operator ==(TextElement left, TextElement right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TextElement left, TextElement right)
        {
            return !Equals(left, right);
        }
    }
}
