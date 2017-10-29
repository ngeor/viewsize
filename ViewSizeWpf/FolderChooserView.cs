// <copyright file="FolderChooserView.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Windows;
using CRLFLabs.ViewSize.Mvp;

namespace ViewSizeWpf
{
    [Presenter(typeof(FolderChooserPresenter))]
    internal class FolderChooserView : IFolderChooserView
    {
        public FolderChooserView(MainWindow mainWindow)
        {
            // assign references to main window
            this.MainWindow = mainWindow;
            this.MainWindow.btnSelectFolder.Click += this.BtnSelectFolder_Click;
            this.MainWindow.txtFolder.TextChanged += this.TxtFolder_TextChanged;

            // start MVP
            PresenterFactory.Create(this);
            this.Load?.Invoke(this, EventArgs.Empty);
        }

        public IMainModel Model { get; set; }

        private MainWindow MainWindow { get; }

        /// <inheritdoc/>
        public event EventHandler OnSelectFolderClick;

        public event EventHandler Load;

        public string Folder
        {
            get => this.MainWindow.txtFolder.Text;
            set => this.MainWindow.txtFolder.Text = value;
        }

        public string SelectFolder()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }

            return null;
        }

        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            this.OnSelectFolderClick?.Invoke(this, EventArgs.Empty);
        }

        private void TxtFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.Model.Folder = this.MainWindow.txtFolder.Text;
        }
    }
}
