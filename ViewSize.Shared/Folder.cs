﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    public class Folder
    {
        public Folder(FolderScanner folderScanner, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be empty", nameof(path));
            }

            if (folderScanner == null)
            {
                throw new ArgumentNullException(nameof(folderScanner));
            }

            Path = path;
            FolderScanner = folderScanner;
        }

        public string Path { get; }
        public long OwnSize { get; private set; }
        public long TotalSize { get; private set; }
        public List<Folder> Children { get; private set; }
        public double Percentage => (double)TotalSize / FolderScanner.TotalSize;
        public bool IsRoot => FolderScanner.IsRoot(this);

        public string DisplayText => IsRoot ? Path : System.IO.Path.GetFileName(Path);
        public string DisplaySize => FileUtils.FormatBytes(TotalSize) + $" ({Percentage:P2})";

        private FolderScanner FolderScanner { get; }

        public void Calculate()
        {
            if (FolderScanner.CancelRequested)
            {
                return;
            }

            FolderScanner.FireScanning(this);

            // my file size (should be zero for directories)
            OwnSize = FileUtils.FileLength(Path);

            // calculate children recursively
            Children = FileUtils.EnumerateFileSystemEntries(Path).Select(p => new Folder(FolderScanner, p)).ToList();
            foreach (var child in Children)
            {
                child.Calculate();
            }

            // now that children are done, we can calculate total size
            TotalSize = OwnSize + Children.Select(c => c.TotalSize).Sum();
            Children.Sort((x, y) =>
            {
                if (x.TotalSize > y.TotalSize)
                {
                    return -1;
                }
                else if (x.TotalSize < y.TotalSize)
                {
                    return 1;
                }
                else
                {
                    return x.Path.CompareTo(y.Path);
                }
            });
        }
    }
}
