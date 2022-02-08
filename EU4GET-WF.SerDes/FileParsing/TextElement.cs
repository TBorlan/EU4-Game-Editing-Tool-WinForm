using System;

namespace EU4GET_WF.SerDes.FileParsing
{
    /// <summary>
    /// Represents a key and value pair.
    /// </summary>
    public class TextElement : IEquatable<TextElement>
    {
        /// <summary>
        /// The key of the <see cref="TextElement"/>.
        /// </summary>
        public string _mLeftValue;

        /// <summary>
        /// The value of the <see cref="TextElement"/>.
        /// </summary>
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
            TextElement otherElement = obj as TextElement;
            return obj.GetType() == this.GetType() && this.Equals(otherElement);
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
