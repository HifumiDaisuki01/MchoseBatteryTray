using System.Collections.Generic;
namespace MchoseTray {
public static class MouseCatalog {
 public static readonly Dictionary<int,string> Classic = new Dictionary<int,string> {
  { 16, "A7 Pro" },
  { 17, "A7 Ultra" },
  { 32, "M7" },
  { 48, "M7 Pro" },
  { 49, "M7 Ultra" },
  { 112, "A7" },
  { 161, "AX5 V2" },
  { 176, "L7 Pro" },
  { 177, "L7 Ultra" },
  { 192, "L7" },
  { 4353, "A5 V2 Ultra" },
  { 16400, "AX5 V2" },
  { 16401, "A7X Ultra" },
  { 16405, "L7 Pro+" },
  { 16406, "L7 Ultra+" },
  { 16408, "A7 V2 Pro" },
  { 16409, "A7 V2 Ultra" },
  { 16417, "A7 V2 Ultra+" },
  { 16419, "A7 V2 Pro+" },
  { 16656, "A7 Ultra(RE)" },
  { 16720, "K7 Ultra" },
 };
 public static readonly Dictionary<int,string> Modern = new Dictionary<int,string> {
  { 16422, "A5 V3 Ultra+" },
  { 16423, "K7 V2 Pro+" },
  { 16424, "K7 V2 Ultra+" },
  { 16425, "G3 V3" },
  { 16426, "V7" },
  { 16432, "A7 V3" },
  { 16433, "A7 V3 Pro" },
  { 16434, "A7 V3 Pro+" },
  { 16435, "A7 V3 Ultra+" },
  { 16436, "A5 V3 Ultra+(3955)" },
  { 16437, "A5 V3 Pro" },
  { 16438, "R7 Ultra" },
  { 16439, "K5 Pro" },
  { 16440, "K5 Ultra" },
  { 16442, "A7 V3-X" },
  { 16443, "K7 V2 Pro" },
  { 16444, "K7 V2 Ultra" },
  { 16445, "A7 V3 Ultra" },
  { 16446, "V7 Pro" },
  { 16447, "A7 V3 Magnetic" },
  { 16448, "K5 V2" },
 };
 public static bool IsModern(int vid,int pid) { return vid==0x3837 && (Modern.ContainsKey(pid)||pid==4116||pid==4120||pid==4118||pid==4124||pid==4125); }
 public static bool IsClassic(int vid,int pid) { return (vid==0x3837||vid==0x5253) && (Classic.ContainsKey(pid)||pid==4096||pid==4097||pid==4105||pid==4106||pid==4107||pid==4108||pid==4128||pid==4129); }
 public static string Name(int pid,string fallback) { string name; if(Classic.TryGetValue(pid,out name)||Modern.TryGetValue(pid,out name)) return name; return System.String.IsNullOrWhiteSpace(fallback)?"MCHOSE "+pid.ToString("X4"):fallback; }
}
}
