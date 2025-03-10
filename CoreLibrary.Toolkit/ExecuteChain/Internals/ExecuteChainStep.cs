using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.ExecuteChain.Internals;

internal sealed record ExecuteChainStep(Func<Task> Func, string? StepName = null);
