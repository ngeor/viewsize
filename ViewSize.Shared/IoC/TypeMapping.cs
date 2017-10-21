using System;
using System.Collections.Generic;
using System.Text;

namespace CRLFLabs.ViewSize.IoC
{
    public class TypeMapping
    {
        public TypeMapping(Type actualType, bool singleton)
        {
            ActualType = actualType;
            Singleton = singleton;
        }

        public Type ActualType { get; }
        public bool Singleton { get; }
    }
}
