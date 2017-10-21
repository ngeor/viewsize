using System;
namespace CRLFLabs.ViewSize.Mvp
{
    public interface IApplicationView : IView
    {
        event EventHandler Closing;
    }
}
