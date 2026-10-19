using System;
using MchoseTray;
class Tests {
 static int count;
 static void Check(bool ok,string label) { if(!ok) throw new Exception(label); count++; }
 static byte[] Frame(int battery,int flags,int charge) {
  byte[] b=Protocol.Request(); byte[] data=new byte[19]; data[0]=0x37;data[1]=0x38;data[2]=0x18;data[3]=0x40;data[8]=(byte)flags;data[9]=(byte)battery;data[10]=(byte)charge;
  for(int i=0;i<data.Length;i++) b[i+2]=(byte)(data[i]^255); return b;
 }
 static void Reject(byte[] b) { bool rejected=false; try { Protocol.Parse(b); } catch(FormatException) { rejected=true; } Check(rejected,"reject invalid response"); }
 static void Main() {
  var q=Protocol.Request(); Check(q.Length==21&&q[0]==17&&q[1]==249&&q[20]==255,"request frame");
  foreach(int battery in new int[]{0,1,7,10,76,100}) { var r=Protocol.Parse(Frame(battery,9,0)); Check(r.Battery==battery,"direct percent, including low level"); Check(r.Linked&&r.Mode==1&&r.Pid==16408,"identity / link flags"); }
  Check(Protocol.Parse(Frame(76,8,1)).State=="正在充电","charging"); Check(!Protocol.Parse(Frame(76,1,0)).Linked,"receiver without mouse");
  Reject(null); Reject(new byte[4]); Reject(new byte[21]); Reject(Frame(101,9,0));
  var bad=Frame(50,9,0);bad[1]=0;Reject(bad);bad=Frame(50,9,0);bad[2]=255;bad[3]=255;Reject(bad);
  Check(Protocol.Model(16408,"receiver")=="A7 V2 Pro","model id");
  Console.WriteLine("PASS: "+count+" protocol checks + "+AlertTests.Run()+" alert checks + "+CompatibilityTests.Run()+" compatibility checks (synthetic fixtures, not hardware tests)");
 }
}
