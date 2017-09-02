using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    public class FileEntry
    {
        public FileEntry(FolderScanner folderScanner, string path)
        {
            Path = path;
            FolderScanner = folderScanner;
            Owner = this;
        }

        private FileEntry(FolderScanner folderScanner, FileEntry owner, string path)
        {
            Path = path;
            FolderScanner = folderScanner;
            Owner = owner;
        }

        public string Path { get; }
        public long OwnSize { get; private set; }
        public long TotalSize { get; private set; }
        public List<FileEntry> Children { get; private set; }
        public double Percentage => (double)TotalSize / Owner.TotalSize;
        public bool IsRoot => Owner == this;

        private FileEntry Owner { get; }
        private FolderScanner FolderScanner { get; }

        public void Calculate()
        {
            if (FolderScanner.CancelRequested)
            {
                return;
            }

            Children = FileUtils.EnumerateDirectories(Path).Select(p => new FileEntry(FolderScanner, Owner, p)).ToList();

            OwnSize = FileUtils.EnumerateFiles(Path).Select(FileUtils.FileLength).Sum();
            foreach (var child in Children)
            {
                child.Calculate();
            }

            TotalSize = OwnSize + Children.Select(c => c.TotalSize).Sum();
        }
    }

    public delegate void NotifyProgress(int number);
}
