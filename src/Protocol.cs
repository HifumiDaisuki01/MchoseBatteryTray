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
 public static string Model(int pid, string fallback) {
  switch(pid) { case 16408:return "A7 V2 Pro"; case 16419:return "A7 V2 Pro+"; case 16409:return "A7 V2 Ultra"; case 16417:return "A7 V2 Ultra+"; }
  return String.IsNullOrWhiteSpace(fallback)?"MCHOSE " + pid.ToString("X4"):fallback;
 }
}
}
