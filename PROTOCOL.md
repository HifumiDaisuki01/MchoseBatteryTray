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
