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
            MainWindow = mainWindow;
            MainWindow.btnSelectFolder.Click += BtnSelectFolder_Click;
            MainWindow.txtFolder.TextChanged += TxtFolder_TextChanged;

            // start MVP
            PresenterFactory.Create(this);
            Load?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public event EventHandler SelectFolderClick;

        public event EventHandler Load;

        public IMainModel Model { get; set; }

        public string Folder
        {
            get => MainWindow.txtFolder.Text;
            set => MainWindow.txtFolder.Text = value;
        }

        public bool HasNativeRecentFolders => false;

        private MainWindow MainWindow { get; }

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
            SelectFolderClick?.Invoke(this, EventArgs.Empty);
        }

        private void TxtFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Model.Folder = MainWindow.txtFolder.Text;
        }
    }
}
