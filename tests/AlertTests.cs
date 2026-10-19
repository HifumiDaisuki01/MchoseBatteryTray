using System;
using MchoseTray;
static class AlertTests {
 static int count;
 static void Check(bool condition,string name) { if(!condition) throw new Exception("Alert: "+name); count++; }
 static Reading At(int level,int charge=0,bool linked=true) { return new Reading { Battery=level, Charge=charge, Linked=linked }; }
 public static int Run() {
  var a=new LowBatteryAlerts();
  Check(!a.ShouldNotify("mouse",At(5)),"off by default");
  a.Configure(20);
  Check(!a.ShouldNotify("mouse",At(21)),"above threshold");
  Check(a.ShouldNotify("mouse",At(20)),"threshold inclusive");
  Check(!a.ShouldNotify("mouse",At(19)),"no repeated polls");
  Check(!a.ShouldNotify("mouse",null),"failed read");
  Check(!a.ShouldNotify("mouse",At(80,0,false)),"disconnect is not recovery");
  Check(!a.ShouldNotify("mouse",At(15)),"reconnect remains latched");
  Check(!a.ShouldNotify("mouse",At(21)),"small rebound");
  Check(!a.ShouldNotify("mouse",At(20)),"jitter does not repeat");
  Check(!a.ShouldNotify("mouse",At(25)),"five-point recovery rearms");
  Check(a.ShouldNotify("mouse",At(18)),"new discharge reminder");
  Check(!a.ShouldNotify("mouse",At(10,1)),"charging rearms without warning");
  Check(a.ShouldNotify("mouse",At(10)),"unplugging low battery");
  Check(!a.ShouldNotify("mouse",At(10,2)),"charge complete rearms");
  Check(a.ShouldNotify("mouse",At(10)),"after charge complete");
  Check(!a.ShouldNotify("unknown",At(10,3)),"unknown charge code");
  Check(!a.ShouldNotify("bad",At(-1))&&!a.ShouldNotify("bad",At(101)),"invalid range");
  Check(!a.ShouldNotify(null,At(10)),"missing identity");
  Check(a.ShouldNotify("second",At(10)),"independent device");
  Check(!a.ShouldNotify("second",At(9)),"second device latched");
  a.Configure(0); Check(!a.ShouldNotify("mouse",At(1)),"disabled");
  a.Configure(10); Check(!a.ShouldNotify("mouse",At(11)),"ten percent threshold");
  Check(a.ShouldNotify("mouse",At(0)),"zero percent valid");
  a.Configure(10); Check(!a.ShouldNotify("mouse",At(0)),"same setting keeps latch");
  a.Configure(30); Check(a.ShouldNotify("mouse",At(30)),"threshold change applies");
  bool rejected=false; try { a.Configure(15); } catch(ArgumentOutOfRangeException) { rejected=true; }
  Check(rejected&&a.Threshold==30,"invalid setting rejected");
  return count;
 }
}
