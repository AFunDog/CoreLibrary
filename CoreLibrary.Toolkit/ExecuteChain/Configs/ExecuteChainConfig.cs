using CoreLibrary.Toolkit.ExecuteChain.EventArgs;
using CoreLibrary.Toolkit.ExecuteChain.Internals;

namespace CoreLibrary.Toolkit.ExecuteChain
{
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
}
