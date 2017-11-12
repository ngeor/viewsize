// <copyright file="ApplicationView.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.Mvp;

namespace ViewSizeWpf
{
    [Presenter(typeof(ApplicationPresenter))]
    internal class ApplicationView : IApplicationView
    {
        public ApplicationView(MainWindow mainWindow)
        {
            mainWindow.Closing += MainWindow_Closing;
            PresenterFactory.Create(this);
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
