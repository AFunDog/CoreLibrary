using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Avalonia.Structs;

public record class LocalizationData(CultureInfo CultureInfo, string Key, string Value);
