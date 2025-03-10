using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Contacts;

public interface IDataProvider
{
    void LoadData();

    Task LoadDataAsync();
}

public interface IDataProvider<T> : IDataProvider
    where T : notnull
{
    event Action<IDataProvider<T>>? DataChanged;

    IEnumerable<T> Datas { get; }

    public static IDataProvider<T> Empty { get; } = new EmptyProvider();

    private sealed class EmptyProvider : IDataProvider<T>
    {
        public IEnumerable<T> Datas => [];

        public event Action<IDataProvider<T>>? DataChanged;

        public void LoadData() { }

        public Task LoadDataAsync() => Task.CompletedTask;
    }
}
