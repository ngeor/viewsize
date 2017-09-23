namespace CRLFLabs.ViewSize
{
    public static class FileSystemEntryContainerExtensions
    { 
        public static IFileSystemEntryContainer RootContainer(this IFileSystemEntryContainer container)
        {
            if (container.Parent == null)
            {
                return container;
            }

            return container.Parent.RootContainer();
        }
    }
}
