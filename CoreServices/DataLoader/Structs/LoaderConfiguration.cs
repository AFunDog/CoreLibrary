using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.DataLoader.Structs
{
    internal sealed record LoaderConfiguration(Action Loader, bool DestoryAfterLoad);
}
