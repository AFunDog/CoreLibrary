using Bogus;
using Zeng.CoreLibrary.Toolkit.Contacts;
using Zeng.CoreLibrary.Toolkit.Structs;

namespace CoreLibrary.Toolkit.Test;

public class DataProviderTest
{
    class FirstNameDataProvider : IDataProvider<string>
    {
        public event Action<IDataProvider<string>, DataProviderDataChangedEventArgs<string>>?
            DataChanged;

        public string? Data { get; private set; }

        public void LoadData()
        {
            var faker = new Faker();
            Data = faker.Name.FirstName();
        }
    }

    [Fact]
    public void DataProviderTest1()
    {
        var firstNameDataProvider = new FirstNameDataProvider();
        Assert.Null(firstNameDataProvider.Data);
        firstNameDataProvider.LoadData();
        var name = firstNameDataProvider.Data;
        var name2 = firstNameDataProvider.Data;
        Assert.NotNull(name);
        Assert.NotNull(name2);
        Assert.Equal(name, name2);
    }
}