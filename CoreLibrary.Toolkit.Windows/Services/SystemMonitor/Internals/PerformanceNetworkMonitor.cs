using System.Diagnostics;
using Serilog;
using Zeng.CoreLibrary.Toolkit.Extensions;

namespace Zeng.CoreLibrary.Toolkit.Windows.Services.SystemMonitor.Internals;

internal sealed class PerformanceNetworkMonitor : IDisposable
{
    private const string CategoryName = "Network Interface";
    private const string ReceivedCounterName = "Bytes Received/sec";
    private const string SentCounterName = "Bytes Sent/sec";

    private static string[] InstanceNames => new PerformanceCounterCategory(CategoryName).GetInstanceNames();
    private ILogger Logger { get; set; }

    private List<PerformanceCounter> NetworkSentCounter { get; set; } = [];
    private List<PerformanceCounter> NetworkReceivedCounter { get; set; } = [];

    public float TotalNetworkSent
    {
        get
        {
            float value = 0;
            try
            {
                foreach (var counter in NetworkSentCounter)
                    value += counter.NextValue();
            }
            catch (Exception e)
            {
                Logger.Trace().Verbose(e, "网络监控器异常 {Instance}", InstanceNames);
                ResetCounter();
            }

            return value;
        }
    }

    public float TotalNetworkReceived
    {
        get
        {
            float value = 0;
            try
            {
                foreach (var counter in NetworkReceivedCounter)
                    value += counter.NextValue();
            }
            catch (Exception e)
            {
                Logger.Trace().Verbose(e, "网络监控器异常 {Instance}", InstanceNames);
                ResetCounter();
            }

            return value;
        }
    }

    public PerformanceNetworkMonitor() : this(Log.Logger)
    {
    }

    public PerformanceNetworkMonitor(ILogger logger)
    {
        Logger = logger.ForContext<PerformanceNetworkMonitor>();
        ResetCounter();
    }

    public void ResetCounter()
    {
        Logger.Trace().Verbose("尝试重置网络性能计数器");
        
        
        var instanceNames = InstanceNames;
        var newInstances = instanceNames.Except(NetworkSentCounter.Select(counter => counter.InstanceName)).ToArray();
        var removeInstances
            = NetworkSentCounter.Select(counter => counter.InstanceName).Except(instanceNames).ToArray();
        try
        {
            if (removeInstances.Length != 0)
            {
                NetworkSentCounter.RemoveAll(counter =>
                    {
                        if (removeInstances.Contains(counter.InstanceName))
                        {
                            counter.Dispose();
                            return true;
                        }

                        return false;
                    }
                );
                NetworkReceivedCounter.RemoveAll(counter =>
                    {
                        if (removeInstances.Contains(counter.InstanceName))
                        {
                            counter.Dispose();
                            return true;
                        }

                        return false;
                    }
                );
            }

            if (newInstances.Length != 0)
            {
                NetworkSentCounter.AddRange(
                    newInstances.Select(ins => new PerformanceCounter(CategoryName, SentCounterName, ins))
                );
                NetworkReceivedCounter.AddRange(
                    newInstances.Select(ins => new PerformanceCounter(CategoryName, ReceivedCounterName, ins))
                );
            }

            //NetworkSentCounter = instanceNames.Select(ins => new PerformanceCounter("Network Interface", "Bytes Sent/sec", ins)).ToList();
            //NetworkReceivedCounter = instanceNames
            //    .Select(ins => new PerformanceCounter("Network Interface", "Bytes Received/sec", ins))
            //    .ToList();
        }
        catch (Exception e)
        {
            Logger.Trace().Error(e, "初始化或重置网络性能计数器失败");
        }
    }

    public void Dispose()
    {
        foreach (var counter in NetworkSentCounter)
            counter.Dispose();
        NetworkSentCounter.Clear();

        foreach (var counter in NetworkReceivedCounter)
            counter.Dispose();
        NetworkReceivedCounter.Clear();
    }
}