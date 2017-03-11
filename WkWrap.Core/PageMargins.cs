namespace WkWrap.Core
{
    /// <summary>
    /// Struct that represents PDF page margins.
    /// </summary>
    public struct PageMargins
    {
        /// <summary>
        /// Returns new instance of <see cref="PageMargins"/> with default margins.
        /// </summary>
        /// <returns></returns>
        public static PageMargins Default()
        {
            return new PageMargins();
        }

        /// <summary>
        /// Constructs new instance of <see cref="PageMargins"/>.
        /// </summary>
        /// <param name="left">PDF page left margin (in mm).</param>
        /// <param name="top">PDF page top margin (in mm).</param>
        /// <param name="right">PDF page right margin (in mm).</param>
        /// <param name="bottom">PDF page bottom margin (in mm).</param>
        public PageMargins(
            double? left = null,
            double? top = null,
            double? right = null,
            double? bottom = null)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Returns PDF page left margin (in mm).
        /// </summary>
        public double? Left { get; }

        /// <summary>
        /// Returns PDF page top margin (in mm).
        /// </summary>
        public double? Top { get; }

        /// <summary>
        /// Returns PDF page right margin (in mm).
        /// </summary>
        public double? Right { get; }

        /// <summary>
        /// Returns PDF page bottom margin (in mm).
        /// </summary>
        public double? Bottom { get; }
    }
}