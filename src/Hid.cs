using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
namespace MchoseTray {
public sealed class Device { public string Path, Name; public int Vid,Pid,InputLength,OutputLength,FeatureLength; public bool Modern; }
public static class Hid {
 [StructLayout(LayoutKind.Sequential)] struct InterfaceData { public int Size; public Guid ClassGuid; public int Flags; public IntPtr Reserved; }
 [StructLayout(LayoutKind.Sequential)] struct Attributes { public int Size; public ushort Vid,Pid,Version; }
 [StructLayout(LayoutKind.Explicit, Size=64)] struct Caps { [FieldOffset(0)] public ushort Usage; [FieldOffset(2)] public ushort Page; [FieldOffset(4)] public ushort InputLength; [FieldOffset(6)] public ushort OutputLength; [FieldOffset(8)] public ushort FeatureLength; }
 [DllImport("hid.dll")] static extern void HidD_GetHidGuid(out Guid guid);
 [DllImport("setupapi.dll",SetLastError=true)] static extern IntPtr SetupDiGetClassDevs(ref Guid guid,IntPtr enumerator,IntPtr hwnd,uint flags);
 [DllImport("setupapi.dll",SetLastError=true)] static extern bool SetupDiEnumDeviceInterfaces(IntPtr set,IntPtr data,ref Guid guid,uint index,ref InterfaceData detail);
 [DllImport("setupapi.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr set,ref InterfaceData data,IntPtr detail,int size,out int required,IntPtr info);
 [DllImport("setupapi.dll")] static extern bool SetupDiDestroyDeviceInfoList(IntPtr set);
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern SafeFileHandle CreateFile(string path,uint access,uint share,IntPtr security,uint disposition,uint flags,IntPtr template);
 [DllImport("hid.dll")] [return: MarshalAs(UnmanagedType.U1)] static extern bool HidD_GetAttributes(SafeFileHandle file,ref Attributes a);
 [DllImport("hid.dll")] [return: MarshalAs(UnmanagedType.U1)] static extern bool HidD_GetPreparsedData(SafeFileHandle file,out IntPtr p);
 [DllImport("hid.dll")] [return: MarshalAs(UnmanagedType.U1)] static extern bool HidD_FreePreparsedData(IntPtr p);
 [DllImport("hid.dll")] static extern int HidP_GetCaps(IntPtr p,out Caps caps);
 [DllImport("hid.dll")] [return: MarshalAs(UnmanagedType.U1)] static extern bool HidD_GetProductString(SafeFileHandle f,byte[] buffer,int length);
 [DllImport("hid.dll",SetLastError=true)] [return: MarshalAs(UnmanagedType.U1)] static extern bool HidD_SetFeature(SafeFileHandle f,byte[] buffer,int length);
 [DllImport("hid.dll",SetLastError=true)] [return: MarshalAs(UnmanagedType.U1)] static extern bool HidD_GetFeature(SafeFileHandle f,byte[] buffer,int length);
 public static List<Device> Enumerate(Action<string> log) {
  var result=new List<Device>(); Guid guid; HidD_GetHidGuid(out guid); IntPtr set=SetupDiGetClassDevs(ref guid,IntPtr.Zero,IntPtr.Zero,18);
  if(set==new IntPtr(-1)) throw new InvalidOperationException("无法枚举 HID: "+Marshal.GetLastWin32Error());
  try { for(uint i=0;;i++) {
   InterfaceData data=new InterfaceData(); data.Size=Marshal.SizeOf(typeof(InterfaceData));
   if(!SetupDiEnumDeviceInterfaces(set,IntPtr.Zero,ref guid,i,ref data)) { if(Marshal.GetLastWin32Error()!=259) log("枚举结束: "+Marshal.GetLastWin32Error()); break; }
   int needed; SetupDiGetDeviceInterfaceDetail(set,ref data,IntPtr.Zero,0,out needed,IntPtr.Zero); if(needed<8) continue;
   IntPtr detail=Marshal.AllocHGlobal(needed);
   try {
    Marshal.WriteInt32(detail,IntPtr.Size==8?8:6);
    if(!SetupDiGetDeviceInterfaceDetail(set,ref data,detail,needed,out needed,IntPtr.Zero)) continue;
    string path=Marshal.PtrToStringUni(IntPtr.Add(detail,4));
    using(var file=CreateFile(path,0,3,IntPtr.Zero,3,0,IntPtr.Zero)) {
     if(file.IsInvalid) continue;
     Attributes a=new Attributes(); a.Size=Marshal.SizeOf(typeof(Attributes)); if(!HidD_GetAttributes(file,ref a) || (a.Vid!=0x3837 && a.Vid!=0x5253)) continue;
     IntPtr pp; if(!HidD_GetPreparsedData(file,out pp)) continue;
     Caps caps; try { if(HidP_GetCaps(pp,out caps)!=0x110000) continue; } finally { HidD_FreePreparsedData(pp); }
     byte[] nameBuf=new byte[256]; string name=HidD_GetProductString(file,nameBuf,nameBuf.Length)?Encoding.Unicode.GetString(nameBuf).TrimEnd('\0'):"MCHOSE";
     log(String.Format("HID {0:X4}:{1:X4} page={2:X4} feature={3} product={4}",a.Vid,a.Pid,caps.Page,caps.FeatureLength,name));
     // Only the classic mouse configuration collections. Never send mouse requests to keyboard/headset collections.
     if(caps.Page!=0xFF01 && caps.Page!=0xFF0B) continue;
     bool modern=MouseCatalog.IsModern(a.Vid,a.Pid);
     if(modern ? (caps.InputLength<23||caps.InputLength>512||caps.OutputLength<64||caps.OutputLength>512) : (!MouseCatalog.IsClassic(a.Vid,a.Pid)||caps.FeatureLength<21)) continue;
     result.Add(new Device {Path=path,Name=name,Vid=a.Vid,Pid=a.Pid,Modern=modern,InputLength=caps.InputLength,OutputLength=caps.OutputLength,FeatureLength=caps.FeatureLength});
    }
   } finally { Marshal.FreeHGlobal(detail); }
  } } finally { SetupDiDestroyDeviceInfoList(set); }
  return result;
 }
 static Reading QueryModern(Device device,Action<string> log) {
  using(var file=CreateFile(device.Path,0xC0000000,3,IntPtr.Zero,3,0x40000000,IntPtr.Zero)) {
   if(file.IsInvalid) throw new InvalidOperationException("打开设备失败: "+Marshal.GetLastWin32Error());
   // Framework async I/O owns pinned buffers and cancels pending operations when disposed.
   using(var stream=new FileStream(file,FileAccess.ReadWrite,1,true)) {
    byte[] response=new byte[device.InputLength], request=Protocol.ModernRequest(device.OutputLength);
    Task<int> read=stream.ReadAsync(response,0,response.Length);
    var clock=Stopwatch.StartNew();
    Task write=stream.WriteAsync(request,0,request.Length);
    if(!write.Wait(600)) throw new TimeoutException("状态查询发送超时");
    while(clock.ElapsedMilliseconds<1500) {
     if(!read.Wait(Math.Max(1,1500-(int)clock.ElapsedMilliseconds))) throw new TimeoutException("状态读取超时");
     int received=read.Result; if(received==0) throw new IOException("设备连接已关闭");
     byte[] frame=new byte[received]; Array.Copy(response,frame,received);
     // Input events are not battery replies. Only the requested command is parsed.
     if(received>8 && frame[0]==0x4D && frame[4]==0 && frame[5]==9) {
      log("RX-V2 "+BitConverter.ToString(frame));
      if(frame[8]==0xFF) {
       Thread.Sleep(30); read=stream.ReadAsync(response,0,response.Length);
       if(!stream.WriteAsync(request,0,request.Length).Wait(Math.Max(1,Math.Min(400,1500-(int)clock.ElapsedMilliseconds)))) throw new TimeoutException("设备忙碌");
       continue;
      }
      return Protocol.ParseModern(frame,device.Pid);
     }
     read=stream.ReadAsync(response,0,response.Length);
    }
    throw new TimeoutException("未收到状态应答");
   }
  }
 }
 public static Reading Query(Device device,Action<string> log) {
  if(device.Modern) return QueryModern(device,log);
  // Access zero permits HID feature queries without claiming the normal mouse input interface.
  using(var f=CreateFile(device.Path,0,3,IntPtr.Zero,3,0,IntPtr.Zero)) {
   if(f.IsInvalid) throw new InvalidOperationException("打开设备失败: "+Marshal.GetLastWin32Error());
   string error="未收到有效应答";
   for(int attempt=0;attempt<3;attempt++) {
    byte[] request=Protocol.Request();
    if(!HidD_SetFeature(f,request,request.Length)) { error="查询发送失败: "+Marshal.GetLastWin32Error(); Thread.Sleep(100); continue; }
    Thread.Sleep(100);
    byte[] response=new byte[21]; response[0]=0x11;
    if(!HidD_GetFeature(f,response,response.Length)) { error="读取失败: "+Marshal.GetLastWin32Error(); continue; }
    log("RX "+BitConverter.ToString(response));
    try { return Protocol.Parse(response); } catch(FormatException ex) { error=ex.Message; Thread.Sleep(120); }
   }
   throw new InvalidOperationException(error);
  }
 }
}
}
