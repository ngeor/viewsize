// <copyright file="Settings.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

namespace CRLFLabs.ViewSize.Settings
{
    public class Settings
    {
        public string SelectedFolder { get; set; }

        /// <summary>
        /// Gets or sets the recent folders.
        /// </summary>
        /// <remarks>
        /// This is only used on Windows.
        /// Mac keeps track of recent folders on its own.
        /// </remarks>
        public string[] RecentFolders { get; set; }
    }
}
