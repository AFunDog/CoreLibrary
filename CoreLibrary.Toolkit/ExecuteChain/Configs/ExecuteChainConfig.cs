using Zeng.CoreLibrary.Toolkit.ExecuteChain.EventArgs;
using Zeng.CoreLibrary.Toolkit.ExecuteChain.Internals;

namespace Zeng.CoreLibrary.Toolkit.ExecuteChain.Configs;

public sealed class ExecuteChainConfig
{
    internal List<ExecuteChainGroup> Groups { get; } = [];
    internal List<Action<IExecuteChain, ExecuteChainEventArgs>> Trackers { get; } = [];

    internal ExecuteChainConfig() { }

    public ExecuteChainGroupConfig Group => new(this);

    public ExecuteChainConfig Track(Action<IExecuteChain, ExecuteChainEventArgs> handler)
    {
        Trackers.Add(handler);
        return this;
    }

    public IExecuteChain Build()
    {
        var chain = new InternalExecuteChain(Groups);
        for (int i = 0; i < Trackers.Count; i++)
        {
            chain.ExecuteEvent += Trackers[i];
        }
        return chain;
    }
}
