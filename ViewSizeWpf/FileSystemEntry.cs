// <copyright file="FileSystemEntry.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CRLFLabs.ViewSizeWpf.Common;

namespace CRLFLabs.ViewSize.IO
{
    public partial class FileSystemEntry : INotifyPropertyChanged
    {
        private bool isExpanded;
        private bool isSelected;

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                if (this.isExpanded != value)
                {
                    this.isExpanded = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsExpanded"));
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }

        public ImageSource WpfIcon
        {
            get
            {
                return GetIcon(this.Path, true, this.IsDirectory);
            }
        }

        public static ImageSource GetIcon(string strPath, bool bSmall, bool folder)
        {
            Interop.SHFILEINFO info = new Interop.SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            Interop.SHGFI flags;
            if (bSmall)
            {
                flags = Interop.SHGFI.Icon | Interop.SHGFI.SmallIcon | Interop.SHGFI.UseFileAttributes;
            }
            else
            {
                flags = Interop.SHGFI.Icon | Interop.SHGFI.LargeIcon | Interop.SHGFI.UseFileAttributes;
            }

            Interop.SHGetFileInfo(strPath, folder ? Interop.FILE_ATTRIBUTE_DIRECTORY : Interop.FILE_ATTRIBUTE_NORMAL, out info, (uint)cbFileInfo, flags);

            IntPtr iconHandle = info.hIcon;

            ImageSource img = Imaging.CreateBitmapSourceFromHIcon(
                        iconHandle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
            Interop.DestroyIcon(iconHandle);
            return img;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
