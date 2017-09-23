using System.Collections.Generic;
using System.Linq;

namespace ViewSize.Tests
{
    // TODO : move to NGSoftware.Common?
    public static class Lists
    {
        public static IReadOnlyList<T> Empty<T>()
        {
            return new List<T>(0);
        }

        public static IReadOnlyList<T> Of<T>(params T[] elements)
        {
            return elements.ToList();
        }
    }

}
