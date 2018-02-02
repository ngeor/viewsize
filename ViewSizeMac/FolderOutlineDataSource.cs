// <copyright file="FolderOutlineDataSource.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using AppKit;
using Foundation;
using System.Linq;
using System.Collections.Generic;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.IO;

namespace ViewSizeMac
{
    public class FolderOutlineDataSource : NSOutlineViewDataSource
    {
        public FolderOutlineDataSource(IReadOnlyList<FileSystemEntry> topLevelFolders)
        {
            TopLevelFolders = topLevelFolders;
        }

        private IReadOnlyList<FileSystemEntry> TopLevelFolders { get; }

        public override System.nint GetChildrenCount(NSOutlineView outlineView, NSObject item)
        {
            return ListOf(item).Count;
        }

        public override NSObject GetChild(NSOutlineView outlineView, System.nint childIndex, NSObject item)
        {
            return ListOf(item)[(int)childIndex];
        }

        public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
        {
            return ListOf(item).Any();
        }

        private IReadOnlyList<FileSystemEntry> ListOf(NSObject item)
        {
            if (item == null)
            {
                return TopLevelFolders;
            }
            else
            {
                return ((FileSystemEntry)item).Children;
            }
        }
    }
}
