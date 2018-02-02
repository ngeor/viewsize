// <copyright file="Main.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using AppKit;
using CRLFLabs.ViewSize.IoC;
using CRLFLabs.ViewSize.Mvp;

namespace ViewSizeMac
{
    /// <summary>
    /// Main entrypoint to the application.
    /// </summary>
    static class MainClass
    {
        public static void Main(string[] args)
        {
            ResolverContainer.Resolver.SetPostCreationAction<IMainModel>(r =>
            {
                r.Resolve<ModelSettingsFolderSync>();
            });

            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
