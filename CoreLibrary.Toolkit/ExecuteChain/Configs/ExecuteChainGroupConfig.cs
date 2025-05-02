using Zeng.CoreLibrary.Toolkit.ExecuteChain.Internals;

namespace Zeng.CoreLibrary.Toolkit.ExecuteChain.Configs;

public sealed class ExecuteChainGroupConfig
{
    public string? GroupName { get; private set; }
    internal List<ExecuteChainStep> Steps { get; } = [];
    internal List<Action<Exception>> ErrorActions { get; } = [];
    internal ExecuteChainConfig MainConfig { get; }

    public ExecuteChainGroupConfig Group
    {
        get
        {
            MainConfig.Groups.Add(new(Steps, ErrorActions));
            return MainConfig.Group;
        }
    }

    internal ExecuteChainGroupConfig(ExecuteChainConfig mainConfig)
    {
        MainConfig = mainConfig;
    }

    public ExecuteChainGroupConfig Config(string groupName)
    {
        GroupName = groupName;
        return this;
    }

    public ExecuteChainGroupConfig WithStep(Action action, string? stepName = null)
    {
        return WithStep(() => Task.Run(action), stepName);
    }

    public ExecuteChainGroupConfig WithStep(Func<Task> func, string? stepName = null)
    {
        Steps.Add(new(func, stepName));
        return this;
    }

    public ExecuteChainGroupConfig WithError(Action<Exception> errorAction)
    {
        ErrorActions.Add(errorAction);
        return this;
    }

    public IExecuteChain Build()
    {
        MainConfig.Groups.Add(new(Steps, ErrorActions));
        return MainConfig.Build();
    }
}
