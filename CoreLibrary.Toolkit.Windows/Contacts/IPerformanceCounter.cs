namespace Zeng.CoreLibrary.Toolkit.Windows.Contacts;

public interface IPerformanceCounter : IDisposable
{
    double NextValue();
}