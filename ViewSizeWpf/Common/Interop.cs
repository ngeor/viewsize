// <copyright file="Interop.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace CRLFLabs.ViewSizeWpf.Common
{
#pragma warning disable SA1310 // Field names must not contain underscore
#pragma warning disable SA1307 // Field naming
    public static class Interop
    {
        public const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
        public const int FILE_ATTRIBUTE_NORMAL = 0x80;

        /// <summary>Maximal Length of unmanaged Windows-Path-strings</summary>
        private const int MAX_PATH = 260;

        /// <summary>Maximal Length of unmanaged Typename</summary>
        private const int MAX_TYPE = 80;

        [Flags]
        public enum SHGFI : int
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,

            /// <summary>get display name</summary>
            DisplayName = 0x000000200,

            /// <summary>get type name</summary>
            TypeName = 0x000000400,

            /// <summary>get attributes</summary>
            Attributes = 0x000000800,

            /// <summary>get icon location</summary>
            IconLocation = 0x000001000,

            /// <summary>return exe type</summary>
            ExeType = 0x000002000,

            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,

            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,

            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,

            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,

            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,

            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,

            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,

            /// <summary>get shell size icon</summary>
            ShellIconSize = 0x000000004,

            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,

            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,

            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,

            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int SHGetFileInfo(
            string pszPath,
            int dwFileAttributes,
            out SHFILEINFO psfi,
            uint cbfileInfo,
            SHGFI uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_TYPE)]
            public string szTypeName;

            public SHFILEINFO(bool b)
            {
                this.hIcon = IntPtr.Zero;
                this.iIcon = 0;
                this.dwAttributes = 0;
                this.szDisplayName = string.Empty;
                this.szTypeName = string.Empty;
            }
        }
    }
#pragma warning restore SA1310 // Field names must not contain underscore
#pragma warning restore SA1307 // Field naming
}
