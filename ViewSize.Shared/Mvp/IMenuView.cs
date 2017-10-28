using System;
namespace CRLFLabs.ViewSize.Mvp
{
    public interface IMenuView : IView<IMainModel>
    {
        event EventHandler FileSizeTreeMapClick;
        event EventHandler FileCountTreeMapClick;
        event EventHandler FileOpenClick;

        bool IsFileSizeTreeMapChecked { get; set; }
        bool IsFileCountTreeMapChecked { get; set; }

        void ShowMainWindow();
    }
}
