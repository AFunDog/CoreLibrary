using System.Globalization;

namespace Zeng.CoreLibrary.Toolkit.Structs;

public record class LocalizationData(CultureInfo CultureInfo, string Key, string Value);
