namespace Zeng.CoreLibrary.Toolkit.ExecuteChain.EventArgs;

public enum ExecuteStepType
{
    Start,
    End,
    EndWithError
}

public sealed record ExecuteChainEventArgs(
    int GroupIndex,
    ExecuteStepType ExecuteStepType,
    string? GroupName = null,
    string? StepName = null
);
