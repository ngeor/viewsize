using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSizeWpf.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CRLFLabs.ViewSize.IO
{
    partial class FileSystemEntry : INotifyPropertyChanged
    {
        private bool isExpanded;
        private bool isSelected;

        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsExpanded"));
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }

        public ImageSource WpfIcon
        {
            get
            {
                return GetIcon(Path, true, IsDirectory);
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
