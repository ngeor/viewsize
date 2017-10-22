using System;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MenuPresenter : PresenterBase<IMenuView, IMainModel>
    {
        public MenuPresenter(IMenuView view, IMainModel model)
            : base(view, model)
        {
        }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.FileSizeTreeMapClick += View_FileSizeTreeMapClick;
            View.FileCountTreeMapClick += View_FileCountTreeMapClick;
        }

        void View_FileSizeTreeMapClick(object sender, EventArgs e)
        {
            View.IsFileSizeTreeMapChecked = true;
            View.IsFileCountTreeMapChecked = false;
            Model.SortKey = SortKey.Size;
        }

        void View_FileCountTreeMapClick(object sender, EventArgs e)
        {
            View.IsFileSizeTreeMapChecked = false;
            View.IsFileCountTreeMapChecked = true;
            Model.SortKey = SortKey.Count;
        }
    }
}
