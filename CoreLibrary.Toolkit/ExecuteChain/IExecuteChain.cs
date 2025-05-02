namespace Zeng.CoreLibrary.Toolkit.ExecuteChain;

// TODO MCCN.Godot 有 ProgressChain 不知道能不能合在一起

public interface IExecuteChain : IDisposable
{
    int GroupCount { get; }

    void Run();
    Task RunAsync();
}
