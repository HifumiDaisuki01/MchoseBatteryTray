using System;
using System.Collections.Generic;
namespace MchoseTray {
// One latch per HID path. Disconnects and failed reads do not rearm an alert.
public sealed class LowBatteryAlerts {
 readonly HashSet<string> notified = new HashSet<string>();
 public int Threshold { get; private set; }
 public void Configure(int threshold) {
  if(threshold!=0 && threshold!=10 && threshold!=20 && threshold!=30) throw new ArgumentOutOfRangeException("threshold");
  if(Threshold!=threshold) { Threshold=threshold; notified.Clear(); }
 }
 public bool ShouldNotify(string key, Reading reading) {
  if(Threshold==0 || String.IsNullOrEmpty(key) || reading==null || !reading.Linked || reading.Battery<0 || reading.Battery>100) return false;
  if(reading.Charge==1 || reading.Charge==2 || reading.Battery>=Threshold+5) { notified.Remove(key); return false; }
  // Unknown charging codes are not reliable evidence of battery operation.
  if(reading.Charge!=0 || reading.Battery>Threshold) return false;
  return notified.Add(key);
 }
}
}
