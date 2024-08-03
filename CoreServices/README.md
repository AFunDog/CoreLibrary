# CoreService

## 🔄️数据绑定服务

可以将一个`INotifyPropertyChanged`对象的某个属性绑定到另一对象的相同类型的属性

如果绑定到不同类型的属性上需要设置自定义的`IValueConverter`

```C#
using DataBindingService service = new();
ViewModel viewModel = new();
View view = new();
service.Bind(
    viewModel, nameof(ViewModel.Name),
    view, nameof(View.Name));
viewModel.Name = "123";
viewModel.Name = "345";
service.UnBind(
    viewModel, nameof(ViewModel.Name),
    view, nameof(View.Name));
viewModel.Name = "123";
```
## ⚙️设置服务

提供了一套用于配置设置的服务。

示例：

```C#
// 创建设置服务
SettingService settingService = new();
// 初始化设置服务
settingService.BuildSettings(builder =>
            {
                builder
                    // 配置设置
                    .ConfigureSetting(
                        new(
                            "Number",				// 设置键
                            new SettingValue(		// 设置的值
                                100,
                                new SettingValueCommand(		// 注入设置命令
                                    (_, e) =>
                                    {
                                        Number = (int)e.NewValue;
                                    }
                                )
                            )
                        )
                    );
            }
```

这个示例创建了一个设置服务，并用`Init`对其进行初始化。

通过调用`builder`对象添加主题设置初始值，并设置选项发生改变时的的回调函数。

## 🌐本地化服务

```C#
using LocalizeService service = new();
service.SetLocalization(new CultureInfo("zh-cn"), "SubTitleUid", "副标题");
service.SetLocalization(new CultureInfo("en-us"), "SubTitleUid", "SubTitle");
SubTitle = service.Localize("SubTitleUid");
service.CurrentCultureChanged += (s,c) =>
{
    SubTitle = s.Localize("SubTitleUid");
};
service.CurrentCultrue = new("en-us");
```