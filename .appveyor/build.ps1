$ErrorActionPreference = 'Stop';
Write-Host Starting build

docker build  -t tgiachi/neon:dev .