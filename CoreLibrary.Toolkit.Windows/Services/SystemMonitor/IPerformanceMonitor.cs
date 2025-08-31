using System.ComponentModel;

namespace Zeng.CoreLibrary.Toolkit.Windows.Services.SystemMonitor;

/// <summary>
/// 性能监视器接口
/// 获取性能数据
/// </summary>
public interface IPerformanceMonitor : INotifyPropertyChanged
{
    /// <summary>
    /// CPU使用率 0-100
    /// </summary>
    double CpuUsage { get; }
    /// <summary>
    /// 内存使用率 0-100
    /// </summary>
    double MemoryUsage { get; }
    /// <summary>
    /// 总物理内存MB
    /// </summary>
    double TotalPhysicalMemoryMB { get; }
    /// <summary>
    /// 网络发送字节数
    /// </summary>
    double NetworkSentBytes { get; }
    /// <summary>
    /// 网络接收字节数
    /// </summary>
    double NetworkReceivedBytes { get; }
    /// <summary>
    /// 人类可读的网络发送字节数
    /// </summary>
    string NetworkSentBytesString { get; }
    /// <summary>
    /// 人类可读的网络接收字节数
    /// </summary>
    string NetworkReceivedBytesString { get; }

    /// <summary>
    /// 默认空实现
    /// </summary>
    public static IPerformanceMonitor Empty { get; } = new EmptyMonitor();

    private sealed class EmptyMonitor : IPerformanceMonitor
    {
        public double CpuUsage => 0;
        public double MemoryUsage => 0;
        public double TotalPhysicalMemoryMB => 0;
        public double NetworkSentBytes => 0;
        public double NetworkReceivedBytes => 0;

        public string NetworkSentBytesString { get; } = "0.00 KB/s";

        public string NetworkReceivedBytesString { get; } = "0.00 KB/s";

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}