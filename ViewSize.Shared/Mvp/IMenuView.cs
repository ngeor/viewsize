using System;
namespace CRLFLabs.ViewSize.Mvp
{
    public interface IMenuView : IView<IMainModel>
    {
        bool IsFileSizeTreeMapChecked { get; set; }
        bool IsFileCountTreeMapChecked { get; set; }

        event EventHandler FileSizeTreeMapClick;
        event EventHandler FileCountTreeMapClick;
    }
}
