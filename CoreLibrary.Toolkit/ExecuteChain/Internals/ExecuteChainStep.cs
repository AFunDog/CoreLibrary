namespace Zeng.CoreLibrary.Toolkit.ExecuteChain.Internals;

internal sealed record ExecuteChainStep(Func<Task> Func, string? StepName = null);
