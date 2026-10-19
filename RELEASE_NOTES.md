## MCHOSE Battery Tray v0.1.0 — 实验性预发布 / Experimental

Windows 托盘电量工具，优先面向 **迈从 MCHOSE A7 V2 Pro**。无需常驻 M HUB 或网页驱动。

### 下载 / Downloads

- `MchoseBatteryTray-Windows.zip`：推荐；EXE、中文/英文文档、协议说明及校验值。
- `MchoseTray.exe`：单文件程序，直接运行。
- `SHA256SUMS.txt`：下载校验。

Windows 10 / 11，.NET Framework 4.5+。本程序未签名。

### 功能 / Features

托盘数字电量、充电/低电量颜色、5–60 秒刷新、手动刷新、设备重扫、可选开机启动及诊断日志。离线运行，只查询状态。

Numeric tray battery icon, charging and low-battery colors, configurable polling, optional startup and local diagnostic logs. No browser or M HUB required.

### 验证范围 / Validation

协议依据官方网页源码；22 项合成协议测试已在本地通过。**没有 A7 V2 Pro 实机与 Windows HID 测试结果。** CI 构建/测试结果以关联 Actions 运行记录为准。

Official web protocol inspected; 22 synthetic parser checks passed locally. **Physical hardware and Windows HID behavior remain unverified.** See the associated Actions run for CI results.

暂不支持蓝牙、键盘、耳机和其他协议；接收器缓存及有线/接收器双图标属于已知限制。首次测试请退出官方驱动。

Bluetooth, keyboards, headsets and other protocols are not supported. Receiver caching and duplicate cable/receiver icons are known limitations.

欢迎通过 Issues 提交型号、固件、连接方式、实际读数和检查后的诊断日志。如果有帮助，欢迎 ⭐ Star 支持项目。
