using System;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MenuPresenter : PresenterBase<IMenuView, IMainModel>
    {
        public MenuPresenter(IMenuView view, IMainModel model, ICommandBus commandBus)
            : base(view, model)
        {
            CommandBus = commandBus;
        }

        private ICommandBus CommandBus { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.FileSizeTreeMapClick += View_FileSizeTreeMapClick;
            View.FileCountTreeMapClick += View_FileCountTreeMapClick;
            View.FileOpenClick += View_FileOpenClick;
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

        void View_FileOpenClick(object sender, EventArgs e)
        {
            CommandBus.Publish("SelectFolder");
            View.ShowMainWindow();
        }
    }
}
