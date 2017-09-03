using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    static class FileUtils
    {
        public static bool IsDirectory(string path)
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

        public static IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            try
            {
                if (!IsDirectory(path))
                {
                    return Enumerable.Empty<string>();
                }
                else
                {
                    return Directory.EnumerateFileSystemEntries(path);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<string>();
            }
        }

        public static long FileLength(string path)
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

        public static string FormatBytes(double size)
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
