using System;

namespace CRLFLabs.ViewSize.Drawing
{
    /// <summary>
    /// Represents the origin of a rectangle.
    /// </summary>
    public struct OriginD
    {
        /// <summary>
        /// Creates a new origin.
        /// </summary>
        /// <param name="left">The left coordinate.</param>
        /// <param name="top">The top coordinate.</param>
        public OriginD(double left, double top)
        {
            if (left < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(left));
            }

            if (top < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(top));
            }

            Left = left;
            Top = top;
        }

        /// <summary>
        /// Gets the left coordinate.
        /// </summary>
        public double Left { get; }

        /// <summary>
        /// Gets the top coordinate.
        /// </summary>
        public double Top { get; }

        public override string ToString() => $"({Left}, {Top})";

        /// <summary>
        /// Creates a new origin which is moved relative to this origin by the given amounts.
        /// </summary>
        /// <param name="dleft">The amount by which to move the origin horizontally.</param>
        /// <param name="dtop">The amount by which to move the origin vertically.</param>
        /// <returns></returns>
        public OriginD Move(double dleft, double dtop) => new OriginD(Left + dleft, Top + dtop);
    }
}
