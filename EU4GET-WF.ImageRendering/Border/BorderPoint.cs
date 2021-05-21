using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace EU4GET_WF.ImageRendering.Border
{
    /// <summary>
    /// Represent a pair of <see cref="int"/> 2D coordinates.
    /// </summary>
    public struct BorderPoint : IEquatable<BorderPoint>, IEquatable<Point>
    {
        #region Fields,Properties&Constructors 

        /// <summary>
        /// The X axis or horizontal coordinate.
        /// </summary>
        public int mX
        {
            get;
            set;
        }

        /// <summary>
        /// The Y axis or vertical coordinate.
        /// </summary>
        public int mY
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new <see cref="BorderPoint"/> with the given coordinates.
        /// </summary>
        /// <param name="x">The horizontal coordinate.</param>
        /// <param name="y">The vertical coordinate.</param>
        public BorderPoint(int x, int y)
        {
            this.mX = x;
            this.mY = y;
        }

        /// <summary>
        /// Represents a <see cref="BorderPoint"/> with coordinates representing the origin of the XY axis.
        /// </summary>
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public static readonly BorderPoint Empty = new BorderPoint(0, 0);

        #endregion

        #region Equals&Hash

        public bool Equals(Point other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(BorderPoint other)
        {
            return (this.mX == other.mX) && (this.mY == other.mY);
        }

        public override bool Equals(object obj)
        {
            if (obj is BorderPoint borderPoint)
            {
                return this.Equals(borderPoint);
            }
            else if (obj is Point point)
            {
                return point.Equals((Point)this);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return new Point(this.mX, this.mY).GetHashCode();
        }

        #endregion

        #region Operators

        public static implicit operator Point(BorderPoint v)
        {
            return new Point(v.mX, v.mY);
        }

        public static implicit operator BorderPoint(Point v)
        {
            return new BorderPoint(v.X, v.Y);
        }

        public static explicit operator PointF(BorderPoint v)
        {
            return (Point)v;
        }

        public static BorderPoint operator +(BorderPoint a, BorderPoint b)
        {
            return new BorderPoint(a.mX + b.mX, a.mY + b.mY);
        }

        public static BorderPoint operator -(BorderPoint a, BorderPoint b)
        {
            return new BorderPoint(a.mX - b.mX, a.mY - b.mY);
        }

        public static bool operator ==(BorderPoint a, BorderPoint b)
        {
            return (Point)a == (Point)b;
        }

        public static bool operator !=(BorderPoint a, BorderPoint b)
        {
            return (Point)a != (Point)b;
        }

        public static bool operator >(BorderPoint a, BorderPoint b)
        {
            BorderPlane plane = a.GetPlane(b);
            if(plane != BorderPlane.Invalid)
            {
                return (plane == BorderPlane.Horizontal) ? (a.mX > b.mX) : (a.mY > b.mY);
            }
            else
            {
                return false;
            }
        }

        public static bool operator <(BorderPoint a, BorderPoint b)
        {
            BorderPlane plane = a.GetPlane(b);
            if (plane != BorderPlane.Invalid)
            {
                return (plane == BorderPlane.Horizontal) ? (a.mX < b.mX) : (a.mY < b.mY);
            }
            else
            {
                return false;
            }
        }

        public static bool operator >=(BorderPoint a, BorderPoint b)
        {
            return (a == b) || (a > b);
        }

        public static bool operator <=(BorderPoint a, BorderPoint b)
        {
            return (a == b) || (a < b);
        }

        #endregion

        public bool IsCollinear(BorderPoint point)
        {
            return (this.mX == point.mX) || (this.mY == point.mY);
        }
        
        /// <summary>
        /// Checks if two <see cref="BorderPoint"/> are part of the same line and returns the orientation of that line.
        /// </summary>
        /// <param name="point">The <see cref="BorderPoint"/> to check with the current object.</param>
        /// <returns><see cref="BorderPlane"/> vale representing the orientation of the line containing the two <see cref="BorderPoint"/>, or <see cref="BorderPlane.Invalid"/>
        /// if the arguments are not on the same line.</returns>
        public BorderPlane GetPlane(BorderPoint point)
        {
            if (this.IsCollinear(point))
            {
                if(this.mX == point.mX)
                {
                    return BorderPlane.Vertical;
                }
                else
                {
                    return BorderPlane.Horizontal;
                }
            }
            else
            {
                return BorderPlane.Invalid;
            }
        }
    }
}
