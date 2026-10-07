function EnvironmentCheckFailed {
    param (
        $item
    )
    exit
}

$ErrorActionPreference = "Stop"


Write-Host "欢迎开发 YcyzClass 插件" -ForegroundColor Blue


$scriptPath =  $MyInvocation.MyCommand.Definition

cd "$([System.IO.Path]::GetDirectoryName($scriptPath))\..\.."

# Check environment
Write-Host "🔧 正在检查环境…" -ForegroundColor Cyan

# TODO: Check env


Write-Host "您的开发环境可以开发 YcyzClass 插件。" -ForegroundColor Green


# Build YcyzClass

pwsh -ep Bypass -File ./tools/plugin/build.ps1