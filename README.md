# CoreService

## ⚙️设置服务

示例：

```C#
// 创建设置服务
SettingService settingService = new();
// 初始化设置服务
settingService.Init(
    // 使用回调函数进行初始化配置
    (builder) =>
    {
        // 添加一个主题设置
        builder.AddSetting("Theme", new SettingValueEnum(
            0,                              // 默认选择
            [
                new("System",0),            // 设置选项的名称和携带数据
                new("Dark",1),
                new("Light",2)
                ],
            // 设置值更改命令
            new SettingValueCommand(
                // 值更改时触发
                (s, e) => { Console.WriteLine($"ValueChanging{e.OldValue}"); },
                // 值更改后触发
                (s, e) => { Console.WriteLine($"ValueChanged{e.NewValue}"); }
                )
            ));
    });
```

这个示例创建了一个设置服务，并用`Init`对其进行初始化。

通过调用`builder`对象添加主题设置初始值，并设置选项发生改变时的的回调函数。

## 🌐本地化服务