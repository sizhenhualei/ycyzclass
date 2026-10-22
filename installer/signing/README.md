# 代码签名与「Windows 已保护你的电脑」提示

Windows 10/11 运行未签名的安装包时，SmartScreen 会弹「Windows 已保护你的电脑 / 未知发布者」。
这是基于「文件来源 + 数字签名 + 下载信誉」的判断，与程序本身是否安全无关。

本仓库使用一张**自签名代码签名证书**（`YcyzClass-CodeSigning.cer`）为发布产物签名：

| 项目 | 值 |
| --- | --- |
| 主题 | `CN=司振华蕾 x DeepSeekV4.1, O=YcyzClass, C=CN` |
| 指纹 (SHA-1) | `26C03DC4DD9B326FF64452DC4B5A243464C21D17` |
| 有效期 | 2026-10-23 — 2036-10-23 |
| 私钥 | **不在仓库中**（保存在构建机的 `CurrentUser\My` 证书库，另有 PFX 备份，仅维护者持有） |

## 三种处理方式

1. **信任本证书（内部使用，推荐）**
   右键「以管理员身份运行」`trust-certificate.cmd`，会把证书导入 `本地计算机\受信任的根证书颁发机构`
   与 `受信任的发布者`。之后本项目的安装包与主程序会显示发布者名称，不再提示「未知发布者」。
   > 注意：把自签名证书装入根证书库意味着「信任由该私钥签名的所有程序」，请仅在内部环境这样做。

2. **解除文件锁定（单次）**
   右键安装包 → 属性 → 底部勾选「解除锁定」→ 确定，然后再运行，可跳过 SmartScreen 检查。

3. **购买商业代码签名证书（彻底解决，需付费）**
   普通 OV 证书仍需累积下载信誉才会消除 SmartScreen；EV 证书可立即获得信誉，
   但需要企业/个人身份审核与年度费用。本项目不提供此类证书，故默认采用方案 1 + 2。

## 为发布产物签名

```powershell
# 用本机证书库中指纹对应的证书签名（默认指纹即上表）
pwsh tools/signing/sign-release.ps1 -Files "H:\ycyzclass-release\YcyzClass.Desktop.exe","H:\ycyzclass-release\YcyzClass-1.5.0-setup.exe"

# 或用 PFX（密码通过参数传入，PFX 不要提交到仓库）
pwsh tools/signing/sign-release.ps1 -Files <文件...> -PfxPath <pfx> -PfxPassword <密码>
```

签名后可用 `Get-AuthenticodeSignature <文件>` 检查，`Status` 为 `Valid` 表示签名与信任链均正常。
