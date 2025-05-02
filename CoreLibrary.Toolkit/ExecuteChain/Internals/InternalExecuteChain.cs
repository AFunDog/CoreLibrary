using System.Diagnostics;
using Microsoft.VisualStudio.Threading;
using Zeng.CoreLibrary.Core.Contacts;
using Zeng.CoreLibrary.Toolkit.ExecuteChain.EventArgs;

namespace Zeng.CoreLibrary.Toolkit.ExecuteChain.Internals;

internal class InternalExecuteChain : DisposableObject, IExecuteChain
{
    internal event Action<IExecuteChain, ExecuteChainEventArgs>? ExecuteEvent;
    internal List<ExecuteChainGroup> Groups { get; }
    internal AsyncAutoResetEvent ResetEvent { get; } = new();

    public int GroupCount => Groups.Count;

    internal InternalExecuteChain(List<ExecuteChainGroup> groups)
    {
        Groups = groups;
        ResetEvent.Set();
    }

    public void Run()
    {
        new JoinableTaskContext().Factory.Run(RunAsync);
    }

    public async Task RunAsync()
    {
        await ResetEvent.WaitAsync();
        for (int i = 0; i < Groups.Count; i++)
        {
            var group = Groups[i];
            try
            {
                await Task.WhenAll(
                    group.Steps.Select(s =>
                    {
                        var task = s.Func();
                        ExecuteEvent?.Invoke(
                            this,
                            new ExecuteChainEventArgs(i, ExecuteStepType.Start, group.GroupName, s.StepName)
                        );
                        return task;
                    })
                );
                group.Steps.ForEach(s =>
                    ExecuteEvent?.Invoke(
                        this,
                        new ExecuteChainEventArgs(i, ExecuteStepType.End, group.GroupName, s.StepName)
                    )
                );
            }
            catch (Exception e)
            {
                ExecuteEvent?.Invoke(this, new ExecuteChainEventArgs(i, ExecuteStepType.EndWithError, group.GroupName));
                if (group.ErrorActions.Count == 0)
                    throw;
                try
                {
                    group.ErrorActions.ForEach(a => a(e));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("错误 内部错误 {0} WithError错误 {1}", e, ex);
                }
                break;
            }
        }
        ResetEvent.Set();
    }

    protected override void DisposeManagedResource()
    {
        ExecuteEvent = null;
        Groups.Clear();
    }

    protected override void DisposeUnmanagedResource() { }

    protected override void OnDisposed() { }
}
