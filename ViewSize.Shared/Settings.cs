using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace CRLFLabs.ViewSize
{
    public class Settings
    {
        public string SelectedFolder { get; set; }
    }

    public class SettingsManager
    {
        private const string Filename = "settings.xml";
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(Settings));

        public Settings Load()
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

        public void Save(Settings settings)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Filename, FileMode.Create, storage))
                {
                    serializer.Serialize(stream, settings);
                }
            }
        }
    }
}
