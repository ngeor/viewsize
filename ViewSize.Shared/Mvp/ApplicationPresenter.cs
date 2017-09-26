using System;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    public class ApplicationPresenter : PresenterBase<IApplicationView>
    {
        public ApplicationPresenter(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        private ISettingsManager SettingsManager { get; }

        protected override void Detach(IApplicationView view)
        {
            view.Closing -= View_Closing;
        }

        protected override void Attach(IApplicationView view)
        {
            view.Closing += View_Closing;
        }

        private void View_Closing(object sender, EventArgs e)
        {
            SettingsManager.Save();
        }
    }
}
