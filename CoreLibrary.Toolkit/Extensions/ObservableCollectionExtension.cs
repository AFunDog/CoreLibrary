using System.Collections.ObjectModel;

namespace Zeng.CoreLibrary.Toolkit.Extensions;

public static class ObservableCollectionExtension
{
    public static void ChangeFrom<T>(this ObservableCollection<T> target, IEnumerable<T> source)
    {
        // TDOD 同步两个集合的内容(保持集合顺序),并能触发事件以通知前台
        int i = 0,
            j = 0;
        for (; i < source.Count() && j < target.Count; i++, j++)
        {
            if (!target.Contains(source.ElementAt(i)))
            {
                target.Insert(j, source.ElementAt(i));
            }
            else if (!source.Contains(target.ElementAt(j)))
            {
                target.Remove(target.ElementAt(j));
                i--;
                j--;
            }
        }
        while (j < target.Count)
        {
            target.RemoveAt(j);
        }
        while (i < source.Count())
        {
            target.Add(source.ElementAt(i));
            i++;
        }
    }
}
