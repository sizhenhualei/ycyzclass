# 声明与致谢 / Notice and Acknowledgements

## 本作品

- 名称：YcyzClass
- 作者：司振华蕾 x DeepSeekV4.1
- 仓库：<https://github.com/sizhenhualei/ycyzclass>

## 修改自 ClassIsland

本作品是 [ClassIsland](https://github.com/ClassIsland/ClassIsland) 的修改版本（fork），**不是** ClassIsland 官方项目，与 ClassIsland 项目组无隶属关系。

- 原作者：HelloWRC 及 ClassIsland 全体贡献者
- 上游仓库：<https://github.com/ClassIsland/ClassIsland>
- 上游版本：`master` 分支，提交 `235914a2fac1733cc2a169457c7bad4edb2ca2f6`
- 上游许可证：GNU General Public License v3.0（见 [LICENSE.txt](LICENSE.txt)）

本作品保留上游项目的全部版权声明与许可证文本。相对于上游，本作品所做的改动包括：重命名（ClassIsland → YcyzClass）、更新作者与品牌信息，移除了自动更新、遥测（Sentry）、插件市场与在线主题市场、上游公告服务、在线语音合成（Edge TTS / GPT-SoVITS）等依赖上游服务的模块（仅保留系统 TTS 与本地插件），并新增了「上游插件二进制兼容」支持（把按上游 ABI 编译的插件重定向到本项目的程序集/命名空间，见 [tools/plugin-compat](tools/plugin-compat)）。除上述改动外，上游的其余功能与设计均归原作者及 ClassIsland 贡献者所有。

## 随发行包分发的第三方插件

便携版发行包（`Plugins/` 目录）内置以下三个插件作为默认插件。它们由各自作者独立开发，版权归其作者所有；本项目仅对二进制做了**机械性的程序集/命名空间重定向**（`ClassIsland.*` → `YcyzClass.*`，工具见 [tools/plugin-compat](tools/plugin-compat)），未修改插件的功能逻辑。各插件的对应源码请访问其上游仓库。

| 插件 | 插件 id | 作者 | 上游仓库 | 许可证 |
| --- | --- | --- | --- | --- |
| 以管理员身份自启动 | `classisland.startUpAsAdmin` | ClassIsland | <https://github.com/ClassIsland/StartUpAsAdmin> | GPL-3.0 |
| UIAccess 提权 | `grantUiAccess` | HelloWRC、Doubx690i | <https://github.com/HelloWRC/GrantUiAccess> | GPL-3.0 |
| ExtraIsland | `ink.lipoly.ext.extraisland` | LiPolymer | <https://gitlab.com/LiPolymer/ExtraIsland>（镜像 <https://github.com/LiPolymer/ExtraIsland>） | AGPL-3.0 |

## 许可证

根据 GPL-3.0 的要求，本作品整体同样以 [GNU General Public License v3.0](LICENSE.txt) 发布。

以下子项目沿用上游的 GNU Lesser General Public License v3.0 许可：

- [YcyzClass.PluginSdk](YcyzClass.PluginSdk)
- [YcyzClass.Core](YcyzClass.Core)
- [YcyzClass.Shared.IPC](YcyzClass.Shared.IPC)
- [YcyzClass.Shared](YcyzClass.Shared)

## 第三方组件

本作品包含的第三方依赖（如 [ClassIsland.PluginSdk](https://www.nuget.org/packages/ClassIsland.PluginSdk)、[ClassIsland.SimpleGitInfoGenerator](https://www.nuget.org/packages/ClassIsland.SimpleGitInfoGenerator) 等）版权归其各自作者所有，详见各依赖项的许可证。

本作品的 Windows 发布包中还包含以 app-local 方式分发的 MSVC 运行时 `vcruntime140.dll`（Microsoft Visual C++ 可再发行组件，版权归 Microsoft 所有），用于支持音频后端所依赖的原生库 `miniaudio.dll`（来自 [SoundFlow](https://www.nuget.org/packages/SoundFlow)）。
