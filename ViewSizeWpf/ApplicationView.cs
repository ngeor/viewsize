using CRLFLabs.ViewSize.Mvp;
using System;
using System.ComponentModel;

namespace ViewSizeWpf
{
    [Presenter(typeof(ApplicationPresenter))]
    class ApplicationView : IApplicationView
    {
        public ApplicationView(MainWindow mainWindow)
        {
            mainWindow.Closing += MainWindow_Closing;
            PresenterFactory.Create(ResolverContainer.Resolver, this);
            Load?.Invoke(this, EventArgs.Empty);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Closing?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Closing;
        public event EventHandler Load;
    }
}
