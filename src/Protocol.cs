using System;
namespace MchoseTray {
public sealed class Reading {
 public int Vid, Pid, Battery, Charge, Mode; public bool Linked;
 public string State { get { return !Linked ? "未连接 / 休眠" : Charge==1 ? "正在充电" : Charge==2 ? "充电完成" : Charge==0 ? "电池供电" : "充电状态未知"; } }
 public string Connection { get { return Mode==0 ? "USB" : Mode==1 ? "2.4G" : Mode==2 ? "蓝牙" : "连接模式 " + Mode; } }
}
public static class Protocol {
 public static byte[] Request() { byte[] b=new byte[21]; b[0]=0x11; for(int i=1;i<b.Length;i++) b[i]=255; b[1]=0xF9; return b; }
 public static Reading Parse(byte[] raw) {
  if(raw==null || raw.Length<13 || raw[0]!=0x11 || raw[1]!=0xF9) throw new FormatException("不是 0x11/0x06 应答");
  byte[] d=new byte[raw.Length-2]; for(int i=0;i<d.Length;i++) d[i]=(byte)(raw[i+2]^255);
  int vid=d[0]|d[1]<<8, pid=d[2]|d[3]<<8;
  if((vid!=0x3837 && vid!=0x5253) || pid==0 || pid==65535) throw new FormatException("身份字段无效，拒绝当作电量");
  if(d[9]>100) throw new FormatException("电量超出 0–100，拒绝显示");
  return new Reading { Vid=vid, Pid=pid, Mode=d[8]&7, Linked=(d[8]&8)!=0, Battery=d[9], Charge=d[10] };
 }
 public static byte[] ModernRequest(int length) {
  if(length<64 || length>512) throw new ArgumentOutOfRangeException("length");
  byte[] b=new byte[length]; b[0]=0x4D; b[1]=1; b[2]=1; b[5]=9; b[8]=8; return b;
 }
 public static Reading ParseModern(byte[] raw,int usbPid) {
  if(raw==null || raw.Length<22 || raw[0]!=0x4D || raw[1]!=1 || raw[4]!=0 || raw[5]!=9 || raw[6]!=0 || raw[7]!=0) throw new FormatException("不是有效的 0x4D/0x0900 状态应答");
  int length=raw[3]; if(length<14 || length>raw.Length-8) throw new FormatException("状态应答长度无效");
  if((raw[2]&1)!=0) {
   if(raw.Length<=8+length) throw new FormatException("状态应答缺少校验位");
   byte sum=0; for(int i=2;i<8+length;i++) sum^=raw[i];
   if(sum!=raw[8+length]) throw new FormatException("状态应答校验失败");
  }
  int vid=raw[8]|raw[9]<<8, pid=raw[10]|raw[11]<<8;
  if((vid!=0x3837 && vid!=0x5253)||!MouseCatalog.Modern.ContainsKey(pid)||raw[20]>100) throw new FormatException("状态应答身份或电量无效");
  return new Reading {Vid=vid,Pid=pid,Battery=raw[20],Charge=raw[19],Mode=MouseCatalog.Modern.ContainsKey(usbPid)?0:1,Linked=raw[18]!=0||raw[20]>0||raw[19]==1||raw[19]==2};
 }
 public static string Model(int pid, string fallback) {
  return MouseCatalog.Name(pid,fallback);
 }
}
}
