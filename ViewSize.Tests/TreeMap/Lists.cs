using System.Collections.Generic;
using System.Linq;

namespace ViewSize.Tests.TreeMap
{
    // TODO : move to NGSoftware.Common?
    public static class Lists
    {
        public static IList<T> Empty<T>()
        {
            return new List<T>(0);
        }

        public static IList<T> Of<T>(params T[] elements)
        {
            return elements.ToList();
        }
    }

}
