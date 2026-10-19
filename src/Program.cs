using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
namespace MchoseTray {
static class Program {
 [STAThread] static void Main(string[] args) {
  bool created; using(var mutex=new Mutex(true,"Local\\KeranMchoseTray-1",out created)) {
   if(!created) return;
   Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
   try { Application.Run(new TrayContext(Array.IndexOf(args,"--demo")>=0)); }
   catch(Exception ex) { MessageBox.Show(ex.ToString(),"MCHOSE 电量托盘启动失败"); }
  }
 }
}
sealed class Snapshot { public string Key,Name,Error; public Reading Reading; }
sealed class TrayContext : ApplicationContext {
 readonly Dictionary<string,NotifyIcon> icons=new Dictionary<string,NotifyIcon>();
 readonly Control dispatch=new Control(); readonly System.Windows.Forms.Timer timer=new System.Windows.Forms.Timer();
 readonly string dir=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"KeranMchoseTray");
 readonly bool demo; bool busy,closing; DateTime started; int seconds=10; string details="正在查找设备…"; NotifyIcon main;
 readonly object logLock=new object();
 readonly LowBatteryAlerts alerts=new LowBatteryAlerts();
 const string RunKey="Software\\Microsoft\\Windows\\CurrentVersion\\Run", RunName="KeranMchoseTray";
 [DllImport("user32.dll")] static extern bool DestroyIcon(IntPtr icon);
 public TrayContext(bool demoMode) {
  demo=demoMode; Directory.CreateDirectory(dir); dispatch.CreateControl(); IntPtr dispatchHandle=dispatch.Handle;
  int saved; string config=Path.Combine(dir,"interval.txt"); if(File.Exists(config)&&Int32.TryParse(File.ReadAllText(config),out saved)&&(saved==5||saved==10||saved==30||saved==60)) seconds=saved;
  int savedAlert; string alertConfig=Path.Combine(dir,"low-battery.txt"); if(File.Exists(alertConfig)&&Int32.TryParse(File.ReadAllText(alertConfig),out savedAlert)&&(savedAlert==0||savedAlert==10||savedAlert==20||savedAlert==30)) alerts.Configure(savedAlert);
  main=NewIcon(); SetIcon(main,"--",Color.Gray,"MCHOSE：正在查找设备"); main.Visible=true;
  timer.Interval=seconds*1000; timer.Tick+=delegate { Poll(); }; timer.Start(); Poll();
 }
 void Log(string line) {
  try { lock(logLock) { string path=Path.Combine(dir,"diagnostic.log"); if(File.Exists(path)&&new FileInfo(path).Length>262144) File.WriteAllText(path,""); File.AppendAllText(path,DateTime.Now.ToString("s")+" "+line+Environment.NewLine); } } catch { }
 }
 NotifyIcon NewIcon() { var n=new NotifyIcon(); n.ContextMenuStrip=Menu(); n.DoubleClick+=delegate { ShowDetails(); }; return n; }
 bool Startup() { using(var k=Registry.CurrentUser.OpenSubKey(RunKey)) return k!=null&&k.GetValue(RunName)!=null; }
 ContextMenuStrip Menu() {
  var menu=new ContextMenuStrip();
  menu.Items.Add("MCHOSE 电量托盘 0.1.1").Enabled=false;
  menu.Items.Add("查看状态",null,delegate { ShowDetails(); });
  menu.Items.Add("立即刷新",null,delegate { Poll(); });
  var rates=new ToolStripMenuItem("刷新间隔");
  foreach(int value in new int[]{5,10,30,60}) { int selected=value; var item=new ToolStripMenuItem(value+" 秒"); item.Checked=seconds==value; item.Click+=delegate { seconds=selected; timer.Interval=seconds*1000; File.WriteAllText(Path.Combine(dir,"interval.txt"),seconds.ToString()); RebuildMenus(); }; rates.DropDownItems.Add(item); }
  menu.Items.Add(rates);
  var alertMenu=new ToolStripMenuItem("低电量提醒");
  foreach(int value in new int[]{0,10,20,30}) {
   int selected=value; var item=new ToolStripMenuItem(value==0?"关闭（默认）":value+"% 及以下"); item.Checked=alerts.Threshold==value;
   item.Click+=delegate {
    try { File.WriteAllText(Path.Combine(dir,"low-battery.txt"),selected.ToString()); alerts.Configure(selected); RebuildMenus(); Poll(); }
    catch(Exception ex) { MessageBox.Show(ex.Message,"无法保存低电量提醒设置"); }
   }; alertMenu.DropDownItems.Add(item);
  }
  menu.Items.Add(alertMenu);
  var startup=new ToolStripMenuItem("开机启动"); startup.Checked=Startup(); startup.Click+=delegate {
   try { using(var k=Registry.CurrentUser.CreateSubKey(RunKey)) { if(Startup()) k.DeleteValue(RunName,false); else k.SetValue(RunName,"\""+Application.ExecutablePath+"\""); } RebuildMenus(); }
   catch(Exception ex) { MessageBox.Show(ex.Message,"无法设置开机启动"); }
  }; menu.Items.Add(startup);
  menu.Items.Add("打开诊断文件夹",null,delegate { Process.Start("explorer.exe",dir); });
  menu.Items.Add(new ToolStripSeparator()); menu.Items.Add("退出",null,delegate { ExitThread(); }); return menu;
 }
 void RebuildMenus() { ReplaceMenu(main); foreach(var n in icons.Values) ReplaceMenu(n); }
 void ReplaceMenu(NotifyIcon n) { var old=n.ContextMenuStrip; n.ContextMenuStrip=Menu(); if(old!=null) old.Dispose(); }
 void ShowDetails() { MessageBox.Show(details+"\n\n默认每 10 秒查询；灰色 -- 表示当前无法读取。\n首次测试建议退出 M HUB，并关闭网页驱动。\n双击图标查看状态，右键调整设置。"+(demo?"\n【演示模式：电量为模拟数据】":""),"MCHOSE 电量托盘"); }
 void SetIcon(NotifyIcon n,string text,Color color,string tooltip) {
  using(var bmp=new Bitmap(32,32)) using(var g=Graphics.FromImage(bmp)) using(var brush=new SolidBrush(color)) using(var font=new Font("Segoe UI",text.Length>2?13:17,FontStyle.Bold,GraphicsUnit.Pixel)) {
   g.Clear(Color.Transparent); g.SmoothingMode=SmoothingMode.AntiAlias;
   using(var bg=new SolidBrush(Color.FromArgb(225,24,27,34))) g.FillEllipse(bg,0,0,32,32);
   using(var fmt=new StringFormat {Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center}) g.DrawString(text,font,brush,new RectangleF(0,0,32,29),fmt);
   using(var pen=new Pen(color,2)) g.DrawLine(pen,8,29,24,29);
   IntPtr h=bmp.GetHicon(); try { using(var temp=Icon.FromHandle(h)) { Icon next=(Icon)temp.Clone(); var previous=n.Icon; n.Icon=next; if(previous!=null) previous.Dispose(); } } finally { DestroyIcon(h); }
  }
  n.Text=tooltip.Length>63?tooltip.Substring(0,63):tooltip;
 }
 void Poll() {
  if(closing) return;
  if(busy) {
   if((DateTime.UtcNow-started).TotalSeconds>30) { foreach(var icon in icons.Values) SetIcon(icon,"--",Color.Gray,"查询超时：当前电量未知"); SetIcon(main,"--",Color.Gray,"查询超时：当前电量未知"); details="设备查询超过 30 秒。当前电量未知；可退出重启工具，并检查接收器。"; }
   return;
  }
  busy=true; started=DateTime.UtcNow;
  Task.Run(delegate {
   var snapshots=new List<Snapshot>();
   try {
    if(demo) snapshots.Add(new Snapshot {Key="demo",Name="A7 V2 Pro [演示]",Reading=new Reading {Battery=76,Linked=true,Mode=1}});
    else foreach(var d in Hid.Enumerate(Log)) {
     var s=new Snapshot {Key=d.Path,Name=d.Name};
     try { s.Reading=Hid.Query(d,Log); s.Name=Protocol.Model(s.Reading.Pid,d.Name); }
     catch(Exception ex) { s.Error=ex.Message; Log(d.Name+": "+ex.Message); }
     snapshots.Add(s);
    }
   } catch(Exception ex) { snapshots.Add(new Snapshot {Key="error",Name="扫描失败",Error=ex.Message}); Log(ex.ToString()); }
   if(!closing) { try { dispatch.BeginInvoke((Action)delegate { if(!closing) Apply(snapshots); busy=false; }); } catch(InvalidOperationException) { } }
  });
 }
 void Apply(List<Snapshot> snapshots) {
  var seen=new HashSet<string>(); var lines=new List<string>();
  foreach(var s in snapshots) {
   seen.Add(s.Key); NotifyIcon n; if(!icons.TryGetValue(s.Key,out n)) { n=NewIcon(); icons.Add(s.Key,n); }
   Reading r=s.Reading; string status=s.Name+": "; Color color=Color.Gray; string number="--";
   if(r!=null) { status+=r.State+" | "+r.Connection+" | "+r.Battery+"%"; if(r.Linked) { number=r.Battery.ToString(); color=r.Charge==1?Color.DeepSkyBlue:r.Battery<=20?Color.OrangeRed:Color.LightGreen; }
    status+=String.Format("\n设备身份 {0:X4}:{1:X4}；连接标志 {2}；充电码 {3}",r.Vid,r.Pid,r.Linked,r.Charge);
   } else status+=s.Error;
   SetIcon(n,number,color,s.Name+" "+(r==null?s.Error:r.State+" "+(r.Linked?r.Battery+"%":""))); n.Visible=true; lines.Add(status);
   if(!demo && alerts.ShouldNotify(s.Key,r)) {
    try { n.ShowBalloonTip(5000,"鼠标低电量",s.Name+" 当前电量 "+r.Battery+"%，请及时充电。",ToolTipIcon.Warning); }
    catch(Exception ex) { Log("低电量通知失败: "+ex.Message); }
   }
  }
  var remove=new List<string>(); foreach(var key in icons.Keys) if(!seen.Contains(key)) remove.Add(key);
  foreach(var key in remove) { DisposeIcon(icons[key]); icons.Remove(key); }
  main.Visible=snapshots.Count==0;
  if(main.Visible) { SetIcon(main,"--",Color.Gray,"MCHOSE：未找到受支持的设备"); lines.Add("未找到受支持的经典协议鼠标。\n请接入 2.4G 接收器或 USB 数据线；蓝牙暂未支持。\n键盘、耳机及其他协议型号暂未支持。"); }
  details=(demo?"【模拟数据】\n":"")+String.Join("\n\n",lines.ToArray())+"\n\n本轮查询完成："+DateTime.Now.ToString("HH:mm:ss");
 }
 void DisposeIcon(NotifyIcon n) { n.Visible=false; if(n.Icon!=null) n.Icon.Dispose(); if(n.ContextMenuStrip!=null) n.ContextMenuStrip.Dispose(); n.Dispose(); }
 protected override void ExitThreadCore() { closing=true; timer.Stop(); timer.Dispose(); DisposeIcon(main); foreach(var n in icons.Values) DisposeIcon(n); dispatch.Dispose(); base.ExitThreadCore(); }
}
}
