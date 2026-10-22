param(
    [Parameter(Mandatory = $true)][string[]]$Files,
    [string]$Thumbprint = '26C03DC4DD9B326FF64452DC4B5A243464C21D17',
    [string]$PfxPath,
    [string]$PfxPassword,
    [string]$TimestampServer = ''
)

$ErrorActionPreference = 'Stop'

# 取签名证书：优先从 PFX 导入到当前用户证书库，否则按指纹在证书库里找。
if ($PfxPath) {
    if (-not (Test-Path $PfxPath)) { throw "找不到 PFX：$PfxPath" }
    $sec = ConvertTo-SecureString $PfxPassword -AsPlainText -Force
    $imported = Import-PfxCertificate -FilePath $PfxPath -CertStoreLocation Cert:\CurrentUser\My -Password $sec
    $cert = Get-Item "Cert:\CurrentUser\My\$($imported.Thumbprint)"
} else {
    $cert = Get-Item "Cert:\CurrentUser\My\$Thumbprint" -ErrorAction SilentlyContinue
    if (-not $cert) { throw "证书库里找不到指纹 $Thumbprint 的证书（可用 -PfxPath 指定 PFX）" }
}
if (-not $cert.HasPrivateKey) { throw "证书 $($cert.Thumbprint) 没有私钥，无法签名" }

Write-Host "签名证书：$($cert.Subject) ($($cert.Thumbprint))"
foreach ($file in $Files) {
    if (-not (Test-Path $file)) { throw "找不到文件：$file" }
    if ($TimestampServer) {
        $result = Set-AuthenticodeSignature -FilePath $file -Certificate $cert -HashAlgorithm SHA256 -TimestampServer $TimestampServer
    } else {
        $result = Set-AuthenticodeSignature -FilePath $file -Certificate $cert -HashAlgorithm SHA256
    }
    $check = Get-AuthenticodeSignature -FilePath $file
    if ($check.Status -eq 'NotSigned') { throw "签名失败：$file（$($result.StatusMessage)）" }
    # 未在本机信任根证书时 Status 会是 UnknownError（链不受信任），但签名本身已写入，属预期。
    Write-Host ("  {0}  ->  {1}" -f (Split-Path $file -Leaf), $check.Status)
}
