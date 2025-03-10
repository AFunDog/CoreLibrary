namespace CoreLibrary.Toolkit.ExecuteChain.Internals;

internal sealed record ExecuteChainGroup(
    List<ExecuteChainStep> Steps,
    List<Action<Exception>> ErrorActions,
    string? GroupName = null
);
