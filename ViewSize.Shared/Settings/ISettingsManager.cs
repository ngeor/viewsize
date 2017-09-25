namespace CRLFLabs.ViewSize.Settings
{
    public interface ISettingsManager
    {
        Settings Settings { get; }
        void Save();
    }
}
