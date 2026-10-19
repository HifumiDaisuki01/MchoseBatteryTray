using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace MchoseTray {
// Preserve the font aspect ratio; leave vertical breathing room in the tray cell.
public static class TrayIconRenderer {
 public static Bitmap Render(string text, Color color) {
  var bitmap=new Bitmap(32,32);
  using(var g=Graphics.FromImage(bitmap)) using(var font=new FontFamily("Segoe UI")) using(var path=new GraphicsPath()) using(var brush=new SolidBrush(color)) using(var format=StringFormat.GenericTypographic) {
   g.Clear(Color.Transparent); g.SmoothingMode=SmoothingMode.AntiAlias;
   path.AddString(text,font,(int)FontStyle.Bold,32,PointF.Empty,format);
   RectangleF bounds=path.GetBounds();
   if(bounds.Width>0 && bounds.Height>0) {
    float scale=Math.Min(30f/bounds.Width,24f/bounds.Height);
    // Dashes stay at their natural stroke height instead of filling the cell.
    if(text=="--") scale=Math.Min(scale,1f);
    float left=(32-bounds.Width*scale)/2, top=(32-bounds.Height*scale)/2;
    using(var fit=new Matrix(scale,0,0,scale,left-bounds.X*scale,top-bounds.Y*scale)) path.Transform(fit);
    g.FillPath(brush,path);
   }
  }
  return bitmap;
 }
}
}
