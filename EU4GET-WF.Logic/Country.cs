using System;
using System.Drawing;

namespace EU4GET_WF.Logic
{
    public class Country : IEquatable<Country>
    {
        private readonly string _mTag;
        private readonly Color _mColor;
        private readonly string _mName;


        public Country(string tag, Color color, string name)
        {
            this._mTag = tag;
            this._mColor = color;
            this._mName = name;
        }

        public string mTag
        {
            get { return this._mTag; }
        }

        public Color mColor
        {
            get
            {
                return this._mColor;
            }
        }

        public string mName
        {
            get
            {
                return this._mName;
            }
        }


        public bool Equals(Country other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return this._mTag == other._mTag;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.Equals((Country) obj);
        }

        public override int GetHashCode()
        {
            return (this._mTag != null ? this._mTag.GetHashCode() : 0);
        }

        public static bool operator ==(Country left, Country right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Country left, Country right)
        {
            return !Equals(left, right);
        }
    }
}