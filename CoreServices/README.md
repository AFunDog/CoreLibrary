# CoreService

## ⚙️设置服务

示例：
```C#
SettingService settingService = new();
settingService.Init(
    (builder) =>
    {
        builder.AddSetting("Theme", new SettingValueEnum(
            0,
            [
                new("System",0),
                new("Dark",1),
                new("Light",2)
                ],
            new SettingValueCommand(
                (s, e) => { Console.WriteLine($"ValueChanging{e.OldValue}"); },
                (s, e) => { Console.WriteLine($"ValueChanged{e.NewValue}"); }
                )
            ));
    });
```

## 🌐本地化服务