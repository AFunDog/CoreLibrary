using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreLibrary.Core.Contacts;
using CoreLibrary.Toolkit.ExecuteChain.EventArgs;
using Microsoft.VisualStudio.Threading;

namespace CoreLibrary.Toolkit.ExecuteChain.Internals;

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
