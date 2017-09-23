using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface IView
    {
        void RunOnGuiThread(Action action);
        void ShowError(string message);
        void ShowError(Exception ex);
    }
}
