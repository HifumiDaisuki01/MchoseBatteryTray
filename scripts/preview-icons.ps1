# Developer-only visual check; not shipped in the runtime executable.
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$temp = Join-Path ([IO.Path]::GetTempPath()) ('mchose-icons-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $temp | Out-Null
try {
    $source = @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using MchoseTray;
class Preview {
 static void Main(string[] args) {
  using(var sheet=new Bitmap(960,220)) using(var g=Graphics.FromImage(sheet)) using(var label=new Font("Segoe UI",12)) {
   string[] samples={"0","7","20","97","100","--"};
   for(int row=0;row<2;row++) {
    Color bg=row==0?Color.White:Color.FromArgb(32,32,32), fg=row==0?Color.FromArgb(0,100,32):Color.LightGreen;
    using(var fill=new SolidBrush(bg)) g.FillRectangle(fill,0,row*110,960,110);
    for(int col=0;col<samples.Length;col++) using(var icon=TrayIconRenderer.Render(samples[col],fg)) {
     int x=col*160,y=row*110;
     using(var brush=new SolidBrush(row==0?Color.Black:Color.White)) g.DrawString(samples[col]+" (16 / 32 px)",label,brush,x+6,y+6);
     g.DrawImage(icon,new Rectangle(x+24,y+53,16,16)); g.DrawImage(icon,new Rectangle(x+75,y+45,32,32));
    }
   }
   sheet.Save(args[0],ImageFormat.Png);
  }
 }
}
'@
    $file = Join-Path $temp 'Preview.cs'
    $exe = Join-Path $temp 'Preview.exe'
    Set-Content $file $source -Encoding utf8
    $csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
    & $csc /nologo /reference:System.Drawing.dll "/out:$exe" (Join-Path $root 'src\TrayIconRenderer.cs') $file
    if ($LASTEXITCODE -ne 0) { throw 'Icon preview compilation failed.' }
    $output = Join-Path ([IO.Path]::GetTempPath()) 'mchose-icon-preview.png'
    & $exe $output
    if ($LASTEXITCODE -ne 0) { throw 'Icon rendering failed.' }
    Write-Host "Rendered $output"
} finally { Remove-Item $temp -Recurse -Force }
