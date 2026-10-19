using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
namespace MchoseTray {
public static class StatusWindow {
 public static void OpenLink(string url) {
  try { Process.Start(new ProcessStartInfo(url) { UseShellExecute=true }); }
  catch(Exception ex) { MessageBox.Show(ex.Message,"无法打开链接"); }
 }
 static LinkLabel Link(string label,string url) {
  var link=new LinkLabel {Text=label,AutoSize=true,Margin=new Padding(0,4,16,4)};
  link.LinkClicked+=delegate { OpenLink(url); }; return link;
 }
 public static void Show(string details,bool demo) {
  using(var headingFont=new Font(SystemFonts.MessageBoxFont.FontFamily,11,FontStyle.Bold))
  using(var window=new Form {Text="设备状态 · MCHOSE 电量托盘",ClientSize=new Size(480,350),StartPosition=FormStartPosition.CenterScreen,FormBorderStyle=FormBorderStyle.FixedDialog,MaximizeBox=false,MinimizeBox=false,ShowInTaskbar=false,Font=SystemFonts.MessageBoxFont}) {
   var layout=new TableLayoutPanel {Dock=DockStyle.Fill,Padding=new Padding(20,16,20,12),ColumnCount=1,RowCount=6};
   layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
   layout.RowStyles.Add(new RowStyle(SizeType.Absolute,34));
   layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));
   layout.RowStyles.Add(new RowStyle(SizeType.Absolute,1));
   layout.RowStyles.Add(new RowStyle(SizeType.Absolute,27));
   layout.RowStyles.Add(new RowStyle(SizeType.Absolute,28));
   layout.RowStyles.Add(new RowStyle(SizeType.Absolute,36));
   layout.Controls.Add(new Label {Text="MCHOSE 电量托盘 1.0.0"+(demo?" · 演示":""),Font=headingFont,AutoSize=true,Margin=Padding.Empty},0,0);
   layout.Controls.Add(new TextBox {Text=details.Replace("\n","\r\n"),Multiline=true,ReadOnly=true,BorderStyle=BorderStyle.None,BackColor=window.BackColor,Dock=DockStyle.Fill,ScrollBars=ScrollBars.Vertical,TabStop=false},0,1);
   layout.Controls.Add(new Panel {BackColor=SystemColors.ControlDark,Dock=DockStyle.Fill,Margin=Padding.Empty},0,2);
   layout.Controls.Add(new Label {Text="Keran Technology © 2026",AutoSize=true,Margin=new Padding(0,8,0,0)},0,3);
   var contacts=new FlowLayoutPanel {Dock=DockStyle.Fill,WrapContents=false,Margin=Padding.Empty};
   contacts.Controls.Add(Link("http://tech.keran.cc","http://tech.keran.cc"));
   contacts.Controls.Add(new Label {Text="问题反馈",AutoSize=true,Margin=new Padding(0,4,8,4)});
   contacts.Controls.Add(Link("Keran@keran.cc","mailto:Keran@keran.cc")); layout.Controls.Add(contacts,0,4);
   var bottom=new TableLayoutPanel {Dock=DockStyle.Fill,ColumnCount=2,Margin=Padding.Empty};
   bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,80));
   bottom.Controls.Add(new Label {Text="QQ: 3522952742",AutoSize=true,Anchor=AnchorStyles.Left},0,0);
   var close=new Button {Text="关闭",DialogResult=DialogResult.OK,Dock=DockStyle.Fill,Margin=new Padding(0,6,0,0)};
   bottom.Controls.Add(close,1,0);window.AcceptButton=close;window.CancelButton=close;layout.Controls.Add(bottom,0,5);
   window.Controls.Add(layout);window.ShowDialog();
  }
 }
}
}
