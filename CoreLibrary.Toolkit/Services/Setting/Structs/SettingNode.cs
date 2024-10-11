using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Services.Setting.Structs
{
    public abstract record SettingNode(string Key);

    public sealed record SettingCollectionNode(string Key, IReadOnlyDictionary<string, SettingNode> Values)
        : SettingNode(Key)
    {
        public void ForEach(Action<SettingNode> action, Func<SettingNode, bool>? filter = null)
        {
            foreach (var child in Values.Values)
            {
                if (filter?.Invoke(child) ?? true)
                {
                    action(child);
                    if (child is SettingCollectionNode c)
                        c.ForEach(action, filter);
                }
            }
        }
    }

    public sealed record SettingConfigNode(SettingConfig Config) : SettingNode(Config.SelfKey);
}
