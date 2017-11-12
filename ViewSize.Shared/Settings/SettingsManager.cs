// <copyright file="SettingsManager.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace CRLFLabs.ViewSize.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private const string Filename = "CRLFLabs.ViewSize.Settings.xml";
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Settings));

        private readonly Lazy<Settings> settings = new Lazy<Settings>(() => Load());

        public Settings Settings => settings.Value;

        public void Save()
        {
            if (!settings.IsValueCreated)
            {
                // nothing changed apparently
                return;
            }

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Filename, FileMode.Create, storage))
                {
                    Serializer.Serialize(stream, settings.Value);
                }
            }
        }

        private static Settings Load()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                try
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Filename, FileMode.Open, storage))
                    {
                        return (Settings)Serializer.Deserialize(stream);
                    }
                }
                catch (FileNotFoundException)
                {
                    return new Settings();
                }
            }
        }
    }
}
