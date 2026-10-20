# 插件兼容转换工具（plugin-compat）

把按上游 **ClassIsland** ABI 编译的插件二进制，改写成 **YcyzClass** ABI，使其能在本项目中加载。

## 背景

YcyzClass 对上游做了全量改名（程序集名、命名空间 `ClassIsland.* → YcyzClass.*`），因此上游插件里
硬编码的 `[ClassIsland.Core]ClassIsland.Core.Abstractions.PluginBase` 之类的引用无法解析。
本工具用 [Mono.Cecil](https://github.com/jbevain/cecil) 直接改写插件元数据：

- 程序集引用 `ClassIsland.*` → `YcyzClass.*`
- 类型引用的命名空间 `ClassIsland.*` → `YcyzClass.*`
- 字符串字面量中的 `avares://ClassIsland` / `resm://ClassIsland` / `ClassIsland.xxx` / `classisland://` → 对应 `YcyzClass`

> 注意：**不是**所有上游插件都能这样转换。若插件使用了本分支已删除的 API（自动更新、遥测、插件市场、
> 在线 TTS 等），转换后会缺失类型/成员。用 `analyze` 模式可以先检查。

## 用法

```bash
# 1) 构建工具
dotnet build tools/plugin-compat/plugincompat.csproj -c Release -o tools/plugin-compat/bin

# 2) 先分析：检查插件是否只引用了本分支仍存在的 API
#    参数 2 = 已发布的 YcyzClass 程序集目录（含 YcyzClass.Core.dll 等）
tools/plugin-compat/bin/plugincompat.exe analyze <插件目录> <YcyzClass 程序集目录>
#    输出 missing types / missing members；均为 0 才建议转换

# 3) 转换到输出目录（保留目录结构，跳过 .pdb）
tools/plugin-compat/bin/plugincompat.exe convert <插件目录> <输出目录>

# 4) 把输出目录整体放到应用数据目录的 Plugins\<插件 id>\ 下，重启应用
```

插件目录 = 含 `manifest.yml` 的那一层（`entranceAssembly` 指向的 dll 也在其中）。

## 运行时行为（宿主侧配合）

`YcyzClass/PluginLoadContext.cs` 在解析程序集时，凡是以 `YcyzClass`（或 `YcyzClass.*`）开头的程序集，
一律返回宿主已加载的实例，而不再走默认绑定。原因是改写后的插件其程序集引用版本仍是上游的 1.x / 2.x，
与宿主的程序集版本不一致时默认绑定会抛 `FileNotFoundException`；复用宿主实例既绕过版本校验，
也保证 `PluginBase` 等类型的标识与宿主一致。

## 已验证可转换的插件

| 插件 | id | 上游 | 许可证 |
| --- | --- | --- | --- |
| 以管理员身份自启动 | `classisland.startUpAsAdmin` | [ClassIsland/StartUpAsAdmin](https://github.com/ClassIsland/StartUpAsAdmin) | GPL-3.0 |
| UIAccess 提权 | `grantUiAccess` | [HelloWRC/GrantUiAccess](https://github.com/HelloWRC/GrantUiAccess) | GPL-3.0 |
| ExtraIsland | `ink.lipoly.ext.extraisland` | [LiPolymer/ExtraIsland](https://gitlab.com/LiPolymer/ExtraIsland) | AGPL-3.0 |

以上三个插件已随本项目的便携版发行包作为默认插件分发（见 `NOTICE.md`）。
转换只做命名空间/程序集重定向，不改插件功能逻辑；如需上游原始二进制，请到上表仓库下载。
