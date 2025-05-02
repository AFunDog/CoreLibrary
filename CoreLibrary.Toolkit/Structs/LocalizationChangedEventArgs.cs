namespace Zeng.CoreLibrary.Toolkit.Structs;

public sealed class LocalizationChangedEventArgs : EventArgs
{
    public string Key { get; }

    public LocalizationChangedEventArgs(string key)
    {
        Key = key;
    }
}
