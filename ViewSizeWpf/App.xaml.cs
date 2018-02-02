// <copyright file="App.xaml.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.Windows;
using CRLFLabs.ViewSize.IoC;
using CRLFLabs.ViewSize.Mvp;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ResolverContainer.Resolver.SetPostCreationAction<IMainModel>(r =>
            {
                r.Resolve<ModelSettingsFolderSync>();
                r.Resolve<ModelSettingsRecentFoldersSync>();
            });
        }
    }
}
