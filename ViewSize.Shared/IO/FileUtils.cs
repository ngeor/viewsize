// <copyright file="FileUtils.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;

namespace CRLFLabs.ViewSize.IO
{
    /// <summary>
    /// File utilities.
    /// </summary>
    public class FileUtils : IFileUtils
    {
        /// <summary>
        /// Checks if the given path is a directory.
        /// </summary>
        /// <returns><c>true</c>, if the path is a directory, <c>false</c> otherwise.</returns>
        /// <param name="path">The path to check.</param>
        /// <remarks>
        /// This should ignore symlinks, reparse points, paths that have too long names,
        /// and paths that the user cannot access.
        /// </remarks>
        public bool IsDirectory(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (!directoryInfo.Exists)
                {
                    return false;
                }

                if (directoryInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    // ignore symlinks
                    return false;
                }

                return true;
            }
            catch (PathTooLongException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        /// <summary>
        /// Enumerates file system entries in the given path.
        /// </summary>
        /// <param name="path">The path to look into.</param>
        /// <returns>
        /// If the path represents a directory, the contents are returned.
        /// If the path is not a directory, <c>null</c> will be returned.
        /// This is bad practice but it serves to differentiate between a directory and non-directory.
        /// </returns>
        public IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            try
            {
                if (!this.IsDirectory(path))
                {
                    return null;
                }
                else
                {
                    return Directory.EnumerateFileSystemEntries(path);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        public long FileLength(string path)
        {
            try
            {
                var fileInfo = new FileInfo(path);
                return fileInfo.Exists ? fileInfo.Length : 0;
            }
            catch (PathTooLongException)
            {
                return 0;
            }
        }

        public string FormatBytes(double size)
        {
            string[] suffixes = new string[]
            {
                "bytes",
                "KB",
                "MB",
                "GB",
                "TB",
                "PB"
            };

            int suffixIndex = 0;

            while (size >= 1024 && suffixIndex < suffixes.Length)
            {
                size = size / 1024;
                suffixIndex++;
            }

            return $"{size:N2} {suffixes[suffixIndex]}";
        }
    }
}
