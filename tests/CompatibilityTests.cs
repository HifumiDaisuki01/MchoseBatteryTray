using System;
using MchoseTray;
static class CompatibilityTests {
 static int count;
 static void Check(bool ok,string name) { if(!ok) throw new Exception("Compatibility: "+name);count++; }
 static byte[] Reply(int pid,int percent,int charge=0,int link=2) {
  var raw=new byte[64];raw[0]=0x4D;raw[1]=1;raw[2]=1;raw[3]=14;raw[5]=9;
  raw[8]=0x37;raw[9]=0x38;raw[10]=(byte)pid;raw[11]=(byte)(pid>>8);raw[18]=(byte)link;raw[19]=(byte)charge;raw[20]=(byte)percent;
  FixChecksum(raw);return raw;
 }
 static void FixChecksum(byte[] raw) { raw[22]=0;for(int i=2;i<22;i++) raw[22]^=raw[i]; }
 static void Reject(byte[] raw) { bool rejected=false;try { Protocol.ParseModern(raw,4116); }catch(FormatException) {rejected=true;}Check(rejected,"invalid V2 frame rejected"); }
 public static int Run() {
  var request=Protocol.ModernRequest(64);Check(request.Length==64&&request[0]==77&&request[5]==9&&request[8]==8,"official request and checksum");
  Check(Protocol.ModernRequest(65).Length==65,"descriptor-sized padded report");
  bool lengthRejected=false;try {Protocol.ModernRequest(63);}catch(ArgumentOutOfRangeException) {lengthRejected=true;}Check(lengthRejected,"short report rejected");
  foreach(var model in MouseCatalog.Modern) {
   var reading=Protocol.ParseModern(Reply(model.Key,7),4116);
   Check(reading.Pid==model.Key&&reading.Battery==7&&reading.Linked&&reading.Mode==1,"new model receiver: "+model.Value);
   Check(Protocol.ParseModern(Reply(model.Key,100,1,1),model.Key).Mode==0,"new model USB: "+model.Value);
   Check(Protocol.Model(model.Key,"receiver")==model.Value,"new model name");
  }
  foreach(var model in MouseCatalog.Classic) Check(Protocol.Model(model.Key,"receiver")==model.Value&&MouseCatalog.IsClassic(0x5253,model.Key),"classic model name");
  Check(Protocol.Model(112,"receiver")=="A7","ordinary A7");
  Check(Protocol.Model(16720,"receiver")=="K7 Ultra","K7 classic");
  Check(MouseCatalog.IsModern(0x3837,4116)&&!MouseCatalog.IsClassic(0x3837,4116),"modern receiver isolated");
  Check(!MouseCatalog.IsModern(0x1234,4116)&&!MouseCatalog.IsClassic(0x1234,112),"other vendors excluded");
  Check(!MouseCatalog.IsClassic(0x3837,0x4245),"different G3 protocol excluded");
  Check(Protocol.ParseModern(Reply(16443,0,0,2),4116).Battery==0,"zero percent preserved");
  Check(!Protocol.ParseModern(Reply(16443,0,0,0),4116).Linked,"empty link stays offline");
  Reject(null);Reject(new byte[10]);Reject(Reply(16443,101));Reject(Reply(999,50));
  var b=Reply(16443,50);b[20]=60;Reject(b);
  b=Reply(16443,50);b[4]=1;Reject(b);
  b=Reply(16443,50);b[6]=1;Reject(b);
  b=Reply(16443,50);b[7]=1;Reject(b);
  b=Reply(16443,50);b[3]=255;Reject(b);
  b=Reply(16443,50);b[3]=1;Reject(b);
  b=Reply(16443,50);b[8]=0;FixChecksum(b);Reject(b);
  return count;
 }
}
