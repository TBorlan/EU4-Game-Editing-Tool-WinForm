using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EU4GET_WF.ImageRendering.Properties;

namespace EU4GET_WF.ImageRendering.Border
{
    /// <summary>
    /// Represent an oriented line defined by two <see cref="BorderPoint"/> 
    /// </summary>
    public struct BorderLine : IEquatable<BorderLine>
    {
        #region Fields,Properties&Constructors

        private BorderPoint _mStart;

        private BorderPoint _mEnd;

        /// <summary>
        /// <see cref="BorderPoint"/> that defines the start of the line.
        /// </summary>
        /// <exception cref="ArgumentException"> If <see cref="mEnd"/> is smaller than
        /// <see cref="mStart"/>.</exception>
        public BorderPoint mStart
        {
            get { return this._mStart; }
            set
            {
                if (this.mEnd == BorderPoint.Empty)
                {
                    return;
                }

                if (!value.IsCollinear(this.mEnd) || (value >= this.mEnd))
                {
                    throw new ArgumentException(Resources.BorderLine_mStart_or_mEnd_Exception_Message, nameof(value));
                }
                else
                {
                    this._mStart = value;
                }
            }
        }

        /// <summary>
        /// <see cref="BorderPoint"/> that defines the end of the line.
        /// </summary>
        /// <exception cref="ArgumentException"> If <see cref="mStart"/> is bigger than
        /// <see cref="mEnd"/>.</exception>
        public BorderPoint mEnd
        {
            get { return this._mEnd; }
            set
            {
                if (this.mStart == BorderPoint.Empty) return;
                if (!value.IsCollinear(this.mStart) || (value <= this.mStart))
                {
                    throw new ArgumentException(Resources.BorderLine_mStart_or_mEnd_Exception_Message, nameof(value));
                }
                else
                {
                    this._mEnd = value;
                }
            }
        }

        /// <summary>
        /// Returns the orientation of the line
        /// </summary>
        /// <value>
        /// <see cref="BorderPlane"/> defining the orientation
        /// </value>
        public BorderPlane mOrientation
        {
            get
            {
                if (this.mStart.mX == this.mEnd.mX)
                {
                    return BorderPlane.Vertical;
                }
                else
                {
                    return BorderPlane.Horizontal;
                }
            }
        }

        /// <summary>
        /// Represent an empty <see cref="BorderLine"/> defined by two <see cref="BorderPoint.Empty"/> points.
        /// </summary>
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public static readonly BorderLine EmptyLine = new BorderLine(BorderPoint.Empty, BorderPoint.Empty);

        /// <summary>
        /// Initialize a new instance of <see cref="BorderLine"/> that has the specified coordinates. 
        /// </summary>
        /// <param name="start"> Start of the line.</param>
        /// <param name="end"> End of the line.</param>
        public BorderLine(BorderPoint start, BorderPoint end)
        {
            this._mStart = start;
            this._mEnd = end;
        }

        #endregion

        #region Operators

        public override bool Equals(object obj)
        {
            if (obj is BorderLine line)
            {
                return this.Equals(line);
            }

            return false;
        }

        public bool Equals(BorderLine other)
        {
            return (this.mStart == other.mStart) && (this.mEnd == other.mEnd);
        }

        #endregion

        #region Equals&Hash

        public override int GetHashCode()
        {
            return (this.mStart, this.mEnd).GetHashCode();
        }

        public static bool operator ==(BorderLine a, BorderLine b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BorderLine a, BorderLine b)
        {
            return !(a.Equals(b));
        }

        #endregion


        /// <summary>
        /// Checks if two line segments are part of the same line.
        /// </summary>
        /// <param name="line">The <see cref="BorderLine"/> to check with the current object.</param>
        /// <returns><see langword="true"/> if segments are collinear, <see langword="false"/> if not.</returns>
        public bool IsCollinear(BorderLine line)
        {
            if (this.mOrientation == line.mOrientation)
            {
                return this.mOrientation == BorderPlane.Horizontal
                    ? this.mStart.mY == line.mStart.mY
                    : this.mStart.mX == line.mStart.mX;
            }

            return false;
        }

        /// <summary>
        /// Checks if two line segments share a common segment.
        /// </summary>
        /// <param name="line">The <see cref="BorderLine"/> to check with the current object.</param>
        /// <returns><see langword="true"/> if segments have a common segment, <see langword="false"/> if not.</returns>
        public bool IsOverlapping(BorderLine line)
        {
            if (this.IsCollinear(line))
            {
                if (this.mStart >= line.mEnd || this.mEnd <= line.mStart)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if two line segments are continuous.
        /// </summary>
        /// <param name="line">The <see cref="BorderLine"/> to check with the current object.</param>
        /// <returns><see langword="true"/> if segments are continuous, <see langword="false"/> if not.</returns>
        public bool IsContinuous(BorderLine line)
        {
            if (this.IsCollinear(line))
            {
                if ((line.mStart == this.mEnd) || (line.mEnd == this.mStart))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns an array containing segments that are part of the current <see cref="BorderLine"/> or <paramref name="line"/> but not both.
        /// </summary>
        /// <param name="line">The <see cref="BorderLine"/> to check with the current object.</param>
        /// <returns><list type="bullet">
        /// <item>An array containing the segments resulted from exclusion</item>
        /// <item>An empty array if the two segments are identical</item>
        /// <item><see langword="null"/> if there isn't any common segment between the two lines</item></list></returns>
        public BorderLine[] Exclude(BorderLine line)
        {
            BorderLine[] result = null;
            if (this.IsOverlapping(line))
            {
                if (this.mStart == line.mStart) // 3 and 9 and 11
                {
                    if (this.mEnd > line.mEnd) //3
                    {
                        result = new[] {new BorderLine(line.mEnd, this.mEnd)};
                    }
                    else if (this.mEnd < line.mEnd) // 9
                    {
                        result = new[] {new BorderLine(this.mEnd, line.mEnd)};
                    }
                    else // 11
                    {
                        result = Array.Empty<BorderLine>();
                    }
                }
                else if (this.mEnd == line.mEnd) // 4 and 10
                {
                    if (this.mStart > line.mStart) // 10
                    {
                        result = new[] {new BorderLine(line.mStart, this.mStart)};
                    }
                    else // 4
                    {
                        result = new[] {new BorderLine(this.mStart, line.mStart)};
                    }
                }
                else if (this.mStart > line.mStart) // 6 and 8
                {
                    if (this.mEnd > line.mEnd) // 6
                    {
                        result = new[] {new BorderLine(line.mStart, this.mStart), new BorderLine(line.mEnd, this.mEnd)};
                    }
                    else // 8
                    {
                        result = new[] {new BorderLine(line.mStart, this.mStart), new BorderLine(this.mEnd, line.mEnd)};
                    }
                }
                else // 5 and 7 
                {
                    if (this.mEnd > line.mEnd) // 7
                    {
                        result = new[] {new BorderLine(this.mStart, line.mStart), new BorderLine(line.mEnd, this.mEnd)};
                    }
                    else // 5
                    {
                        result = new[] {new BorderLine(this.mStart, line.mStart), new BorderLine(this.mEnd, line.mEnd)};
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="BorderLine"/> representing the concatenation of the current object with <paramref name="line"/>.
        /// </summary>
        /// <param name="line">The <see cref="BorderLine"/> to concatenate with the current object.</param>
        /// <returns><see cref="BorderLine"/> representing the concatenation of the two lines, or <see cref="EmptyLine"/> if the two lines can't be concatenated.</returns>
        public BorderLine Concatenate(BorderLine line)
        {
            BorderLine result = BorderLine.EmptyLine;
            if (this.IsCollinear(line))
            {
                BorderPoint start, end;
                if (this.mStart == line.mEnd)
                {
                    start = line.mStart;
                    end = this.mEnd;
                }
                else
                {
                    start = this.mStart;
                    end = line.mEnd;
                }

                result = new BorderLine(start, end);
            }

            return result;
        }
    }

    public enum BorderPlane
    {
        Vertical = -2,
        Horizontal = -1,
        Invalid = 0
    }
}
