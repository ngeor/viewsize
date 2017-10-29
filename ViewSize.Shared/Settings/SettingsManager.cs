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
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Settings));

        #region Settings Holder
        private readonly Lazy<Settings> settings = new Lazy<Settings>(() => Load());

        public Settings Settings => this.settings.Value;

        private static Settings Load()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                try
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Filename, FileMode.Open, storage))
                    {
                        return (Settings)serializer.Deserialize(stream);
                    }
                }
                catch (FileNotFoundException)
                {
                    return new Settings();
                }
            }
        }
        #endregion

        public void Save()
        {
            if (!this.settings.IsValueCreated)
            {
                // nothing changed apparently
                return;
            }

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Filename, FileMode.Create, storage))
                {
                    serializer.Serialize(stream, this.settings.Value);
                }
            }
        }
    }
}
