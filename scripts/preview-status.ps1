# Render the actual WinForms status window for visual review; no device access.
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$temp = Join-Path ([IO.Path]::GetTempPath()) ('mchose-status-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $temp | Out-Null
try {
    $source = @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MchoseTray;
class Preview {
 [STAThread] static void Main(string[] args) {
  Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
  using(var timer=new Timer {Interval=500}) {
   timer.Tick+=delegate {
    if(Application.OpenForms.Count==0) return;
    timer.Stop();var window=Application.OpenForms[0];
    using(var image=new Bitmap(window.Width,window.Height)) {window.DrawToBitmap(image,new Rectangle(0,0,image.Width,image.Height));image.Save(args[0],ImageFormat.Png);}
    window.Close();
   };
   timer.Start(); StatusWindow.Show("A7 V2 Pro: 电池供电 | 2.4G | 97%\n\nK7 V2 Pro: 正在充电 | USB | 62%\n\n本轮查询完成：13:30:00",false);
  }
 }
}
'@
    $file = Join-Path $temp 'Preview.cs'; $exe = Join-Path $temp 'Preview.exe'
    Set-Content $file $source -Encoding utf8
    $csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
    & $csc /nologo /codepage:65001 /reference:System.Drawing.dll /reference:System.Windows.Forms.dll "/out:$exe" (Join-Path $root 'src\StatusWindow.cs') $file
    if ($LASTEXITCODE -ne 0) { throw 'Status preview compilation failed.' }
    $outputDirectory = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [IO.Path]::GetTempPath() }
    $output = Join-Path $outputDirectory 'mchose-status-preview.png'
    & $exe $output
    if ($LASTEXITCODE -ne 0) { throw 'Status preview failed.' }
    Write-Host "Rendered $output"
} finally { Remove-Item $temp -Recurse -Force }
