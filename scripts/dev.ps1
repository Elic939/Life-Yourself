param([ValidateSet('test','build','publish')][string]$Action='test')
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
Set-Location -LiteralPath $projectRoot
$sdkExe=Join-Path $env:LOCALAPPDATA 'PersonalLifeDev\dotnet\dotnet.exe'
if(-not (Test-Path -LiteralPath $sdkExe)){$sdkExe='dotnet'}
$env:DOTNET_CLI_TELEMETRY_OPTOUT='1'
if($Action -eq 'test'){
 & $sdkExe publish src/PersonalLife.App/PersonalLife.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -p:PublishTrimmed=false -o artifacts/PersonalLife-win-x64
 if($LASTEXITCODE -ne 0){exit $LASTEXITCODE}
 $env:PERSONAL_LIFE_EVIDENCE_DIR=Join-Path $projectRoot 'artifacts\screenshots'
 & $sdkExe test PersonalLife.slnx -c Release --logger 'trx;LogFileName=all-tests.trx' --results-directory artifacts/test-results
}elseif($Action -eq 'build'){
 & $sdkExe build PersonalLife.slnx -c Release
}else{
 & $sdkExe publish src/PersonalLife.App/PersonalLife.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -p:PublishTrimmed=false -o artifacts/PersonalLife-win-x64
}
exit $LASTEXITCODE
