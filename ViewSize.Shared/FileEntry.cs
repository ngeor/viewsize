using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    class FileEntry
    {
        public FileEntry(string path)
        {
            Path = path;
            Owner = this;
        }

        private FileEntry(FileEntry root, string path)
        {
            Path = path;
            Owner = root;
        }

        public string Path { get; }
        public long OwnSize { get; private set; }
        public long TotalSize { get; private set; }
        public List<FileEntry> Children { get; private set; }
        public double Percentage => (double)TotalSize / Owner.TotalSize;

        private FileEntry Owner { get; }

        public void Calculate(NotifyProgress pending, NotifyProgress finished)
        {
            pending(1);
            Children = FileUtils.EnumerateDirectories(Path).Select(p => new FileEntry(Owner, p)).ToList();

            OwnSize = FileUtils.EnumerateFiles(Path).Select(FileUtils.FileLength).Sum();
            foreach (var child in Children)
            {
                child.Calculate(pending, finished);
            }

            TotalSize = OwnSize + Children.Select(c => c.TotalSize).Sum();

            finished(1);
        }
    }

    delegate void NotifyProgress(int number);
}
