using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.IoC;
using CRLFLabs.ViewSize.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRLFLabs.ViewSize.IoC
{
    public static class ResolverContainer
    {
        internal static readonly Resolver Resolver = ConfigureResolver();

        private static Resolver ConfigureResolver()
        {
            var resolver = new Resolver();

            // TODO move this common part to Shared
            resolver.Map<IFileUtils, FileUtils>();
            resolver.Map<ISettingsManager, SettingsManager>(true);
            resolver.Map<IFolderScanner, FolderScanner>();
            return resolver;
        }
    }
}
