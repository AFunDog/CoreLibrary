using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreServices.Setting.Structs;

namespace CoreServicesWinUILibrary.Structs
{
    public sealed class SettingAttachedArgs(string iconGlyph) : AttachedArgs
    {
        public string IconGlyph { get; } = iconGlyph;
    }
}
