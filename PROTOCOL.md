# 协议依据与实现边界

研究日期：2026-10-19。

## 官方第一手来源

- https://www.mchose.com.cn/
- 当次入口脚本：`/assets/main-DBiLSPI3.js`
- 当次核心包：`/assets/purify.es-CjCZz1i7.js`
- 核心包 SHA-256：`844150437016c6e96f01816938062adb25717503b49efdfec015c451b9a02818`

从核心包直接核对：

- A7 V2 Pro 型号表 pids 为十进制 16408、4106、4107、4128；其中实际回复内的 16408（0x4018）用于识别型号，USB 接收器 PID 与鼠标内部 PID 不能混为一谈。
- 查询常量 Oat：report 0x11，命令 0x06，20 字节 payload。
- LG 查询实现：payload 全部 XOR 0xFF；通过 sendFeatureReport 发送，receiveFeatureReport 接收；跳过 report ID 和命令回显后再解析；命令不匹配时重试。
- jat 解析器：little endian 的 vid:u16、pid:u16、fwVersion:u32、connectMode:bit3、connectStatus:bit1、reserved:bit4、batteryLevel:u8、chargeStatus:u8。
- 包内 binary-parser generateBit 实现确认 little endian 位字段从低位读取，故 mode = flags & 7，linked = flags & 8。

Windows 完整应答（含 report ID）的偏移：

| 偏移 | 含义 |
| --- | --- |
| 0 | 0x11，不反转 |
| 1 | 0xF9，反转后为命令 0x06 |
| 2–3 | 反转后 VID，小端 |
| 4–5 | 反转后内部 PID，小端 |
| 6–9 | 反转后固件版本 |
| 10 | 反转后连接位字段 |
| 11 | 反转后百分比，直接使用 0–100 |
| 12 | 反转后充电状态 |

不使用部分第三方文档声称的 data[8]/data[9] 电量偏移，也不把 1–10 的数值自动乘十，以免把真实 7% 显示为 70%。

只筛选 VID 0x3837/0x5253、usage page 0xFF01/0xFF0B、feature 长度 >=21 的经典鼠标配置集合。应答需回显正确命令、有效身份且电量 <=100。仅有这些筛选不意味着每个被枚举型号均已验证。

## 交叉参考

- https://github.com/alexfrih/mchose-linux ：`src/proto.rs` 和 `PROTOCOL.md`，经典协议结构、连接模式 0 USB / 1 2.4G / 2 Bluetooth。
- https://github.com/Rapou7/offline-mchose ：确认存在 Windows 原生 HID 路径；其 A7 V2 Pro 标记为未测试，且部分电量偏移描述与官方源码不一致，因此未照搬解析。

本工具的 C# 实现为本次独立编写，未复制第三方驱动源码；未打包官方 JS、图标、安装器或固件。

## 尚需实机确认

Windows HID 查询权限、各固件应答长度、A7 V2 Pro 连接及充电状态的实际取值、充电码 2 的含义、休眠时缓存行为、官方驱动共存及多设备去重。当前充电码 0/1/2 分别解释为电池供电/充电中/充电完成，其他值保留为未知并在详情显示原值。合成单元测试仅检验代码按此定义工作，不能代替实机验证。


## v1.0.0: extended families and new output/input protocol

Official source rechecked 2026-10-19: same `purify.es-CjCZz1i7.js` bundle.

`MouseCatalog.cs` contains the official model-table identity IDs, separate from shared receiver IDs. Classic A7 / Pro / Ultra, L7 / M7 variants and K7 Ultra use the existing inverted `0x11 / 0x06` status read. A7 V2 Pro 2.4 GHz readings and tray display have been confirmed by the user.

For newer K7 V2, K5, A7 V3 and other explicitly listed models, official functions `a1e`, `mu`, `u1e` and `zK` establish:

- Full request is 64 bytes: report `0x4D`, version 1, flags 1, payload length 0, command `0x0900` little-endian, bizCode 0, sequence 0, XOR checksum at index 8 (`0x08`).
- WebHID input data excludes the report ID. Its command is data[3..4], and payload starts data[7]; Windows includes report ID, so these become full-frame indices [4..5] and [8].
- Identity payload: VID u16 at 0, PID u16 at 2, capability bytes at 4..9, connection code at 10, charging at 11, percentage at 12, game mode at 13.
- Thus the full Windows frame has connection code at 18, charging at 19 and battery at 20. All values here are direct, not XOR-inverted.
- Frames that declare checksum flag bit 0 must have the correct XOR over full-frame indices 2 through 7+payloadLength. Command, version, sequence, bizCode, size, model identity and range are checked before displaying a value.
- New protocol selection is limited to the official vendor 0x3837, known direct model IDs / receiver IDs, vendor usage pages 0xFF01 or 0xFF0B and adequate input/output report sizes. Classic traffic is never used as a fallback on these new devices.
- Windows overlapped I/O uses the Framework FileStream async implementation with buffering disabled (buffer size 1), explicit write/read waits and a 1.5 second transaction budget. Unrelated input events are skipped; the official busy marker 0xFF is retried within the same budget. Disposing the stream closes the handle and cancels outstanding native operations.
- The USB-versus-receiver label is inferred from the physical PID, not from an assumed bit layout of the connection code. Empty identity frames are rejected. A valid reading with zero connection code and zero battery/charge is shown as unlinked rather than triggering a false empty-battery alert.

New-family support is based on official protocol/code, not claimed physical validation. The ambiguous base A5 V3 identity 16418 appears across protocol generations and is deliberately not selected as a new-protocol direct device. Older G3 / G3 V2 and unlisted ordinary K7 firmware are not claimed; they require separate protocol evidence. Bluetooth is outside this USB reader.
