using System;
namespace CRLFLabs.ViewSize.IO
{
    /// <summary>
    /// Represents the way a <see cref="FileSystemEntry"/> can be sorted.
    /// </summary>
    public enum SortKey
    {
        /// <summary>
        /// Sort by file size.
        /// </summary>
        Size,

        /// <summary>
        /// Sort by file count.
        /// </summary>
        Count
    }
}
