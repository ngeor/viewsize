using System;
using System.Collections.Generic;
using System.Text;

namespace CRLFLabs.ViewSize.Mvp
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PresenterAttribute : Attribute
    {
        public PresenterAttribute(Type presenterType)
        {
            PresenterType = presenterType;
        }

        public Type PresenterType { get; }
    }
}
