// <copyright file="ResolverContainer.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.IoC
{
    /// <summary>
    /// Contains the resolver of the application.
    /// </summary>
    public static class ResolverContainer
    {
        internal static readonly Resolver Resolver = ConfigureResolver();

        private static Resolver ConfigureResolver()
        {
            var resolver = new Resolver();
            resolver.Map<IFileUtils, FileUtils>();
            resolver.Map<ISettingsManager, SettingsManager>(singleton: true);
            resolver.Map<IFolderScanner, FolderScanner>();
            resolver.Map<IMainModel, MainModel>(singleton: true);
            return resolver;
        }
    }
}
