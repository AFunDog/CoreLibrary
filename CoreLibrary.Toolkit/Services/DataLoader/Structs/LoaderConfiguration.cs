using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Services.DataLoader.Structs
{
    internal sealed record LoaderConfiguration(Action Loader, bool DestoryAfterLoad);
}
