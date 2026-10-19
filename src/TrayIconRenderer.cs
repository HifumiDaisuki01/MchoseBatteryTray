using System.Drawing;
using System.Drawing.Drawing2D;
namespace MchoseTray {
// Use the whole tray cell for digits; no background, border or decoration.
public static class TrayIconRenderer {
 public static Bitmap Render(string text, Color color) {
  var bitmap=new Bitmap(32,32);
  using(var g=Graphics.FromImage(bitmap)) using(var font=new FontFamily("Segoe UI")) using(var path=new GraphicsPath()) using(var brush=new SolidBrush(color)) {
   g.Clear(Color.Transparent); g.SmoothingMode=SmoothingMode.AntiAlias;
   path.AddString(text,font,(int)FontStyle.Bold,32,PointF.Empty,StringFormat.GenericTypographic);
   RectangleF bounds=path.GetBounds();
   if(bounds.Width>0 && bounds.Height>0) {
    float width=text.Length==1?20:30;
    float height=text=="--"?4:30;
    float left=(32-width)/2, top=(32-height)/2;
    using(var fit=new Matrix(bounds,new PointF[]{new PointF(left,top),new PointF(left+width,top),new PointF(left,top+height)})) path.Transform(fit);
    g.FillPath(brush,path);
   }
  }
  return bitmap;
 }
}
}
