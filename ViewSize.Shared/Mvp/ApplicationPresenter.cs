using System;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    public class ApplicationPresenter
    {
        private IApplicationView _view;

        public ApplicationPresenter(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        private ISettingsManager SettingsManager { get; }

        public IApplicationView View
        {
            get
            {
                return _view;    
            }
            set
            {
                Detach();
                _view = value;
                Attach();
            }
        }

        private void Detach()
        {
            if (_view != null)
            {
                _view.Closing -= View_Closing;
            }
        }

        private void Attach()
        {
            if (_view != null)
            {
                _view.Closing += View_Closing;
            }
        }

        private void View_Closing(object sender, EventArgs e)
        {
            SettingsManager.Save();
        }
    }
}
