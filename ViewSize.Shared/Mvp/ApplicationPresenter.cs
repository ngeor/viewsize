using System;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    public class ApplicationPresenter : PresenterBase<IApplicationView>
    {
        public ApplicationPresenter(IApplicationView view, ISettingsManager settingsManager)
            : base(view)
        {
            SettingsManager = settingsManager;
        }

        private ISettingsManager SettingsManager { get; }

        protected override void AttachToView()
        {
            View.Closing += View_Closing;
        }

        private void View_Closing(object sender, EventArgs e)
        {
            SettingsManager.Save();
        }
    }
}
