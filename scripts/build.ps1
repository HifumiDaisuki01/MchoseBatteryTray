param([switch]$Test)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (!(Test-Path $csc)) { $csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
if (!(Test-Path $csc)) { throw '.NET Framework C# compiler not found.' }
$sources = @(Get-ChildItem (Join-Path $root 'src\*.cs') | ForEach-Object { $_.FullName })
& $csc /nologo /codepage:65001 /target:winexe /platform:anycpu /optimize+ "/out:$root\MchoseTray.exe" /reference:System.dll /reference:System.Core.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll $sources
if ($LASTEXITCODE -ne 0) { throw 'Application build failed.' }
if ($Test) {
    $testExe = Join-Path ([IO.Path]::GetTempPath()) ('MchoseTray-tests-' + [Guid]::NewGuid().ToString('N') + '.exe')
    try {
        & $csc /nologo /codepage:65001 /target:exe "/out:$testExe" (Join-Path $root 'src\Protocol.cs') (Join-Path $root 'tests\ProtocolTests.cs') (Join-Path $root 'src\LowBatteryAlerts.cs') (Join-Path $root 'tests\AlertTests.cs')
        if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
        & $testExe
        if ($LASTEXITCODE -ne 0) { throw 'Protocol tests failed.' }
    } finally { Remove-Item $testExe -ErrorAction SilentlyContinue }
}
Write-Host 'Built MchoseTray.exe'
