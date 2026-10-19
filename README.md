# MCHOSE Battery Tray · 迈从电量托盘

**在 Windows 右下角直接查看迈从鼠标电量，无需常驻 M HUB 或网页驱动。**

[English](README.en.md) · [下载正式版](https://github.com/HifumiDaisuki01/MchoseBatteryTray/releases) · [使用说明](USAGE.zh-CN.md) · [反馈兼容性](https://github.com/HifumiDaisuki01/MchoseBatteryTray/issues/new/choose)

![Windows](https://img.shields.io/badge/Windows-10%20%2F%2011-0078D4) ![License: MIT](https://img.shields.io/badge/License-MIT-green) ![Status](https://img.shields.io/badge/status-stable-brightgreen)

一个使用 C# / WinForms 编写的轻量托盘工具，通过 Windows 原生 HID 接口读取电池状态。支持 **A7、L7、M7、K7 Ultra 以及 K7 V2 / K5 等新协议系列**；源码、协议笔记和构建脚本均公开。

## 功能

- **大号纯数字图标**：透明背景，无圆形底板、边框或横线；按字体原始比例等比缩放并居中，避免细长变形；正常电量随任务栏主题显示黑色或白色。
- **状态颜色**：黑色／白色为正常供电，红色为低电量，蓝色为充电，灰色 `--` 为无法读取。
- **自动刷新与设备重扫**：默认 10 秒，可选 5 / 10 / 30 / 60 秒。
- **悬停看状态、双击看详情**：状态窗口带版权、官网及反馈联系方式；右键菜单底部可访问 Keran 官网。
- **可选低电量通知**：默认关闭，右键选择 10% / 20% / 30%；使用 Windows 原生托盘通知，不增加依赖。
- **可选开机启动**，无需管理员权限。
- **单 EXE、离线运行**：不依赖 Python、Node.js、浏览器或 M HUB；使用 Windows 自带 .NET Framework。
- 仅查询状态，不修改 DPI、按键、回报率或固件。

## 下载与使用

1. 从 [Releases](https://github.com/HifumiDaisuki01/MchoseBatteryTray/releases) 下载 Windows ZIP 并解压。
2. 插入 **2.4G 接收器**或用 **USB 数据线**连接鼠标。
3. 首次测试时退出 M HUB，关闭网页驱动，再双击 `MchoseTray.exe`。
4. 如图标藏在托盘折叠菜单，把它拖到任务栏可见区域；右键可设置刷新间隔与开机启动。

系统：Windows 10 / 11，.NET Framework 4.5+。详细说明见 [中文使用指南](USAGE.zh-CN.md)。发行文件尚未签名。

## 低电量提醒

右键托盘 → **低电量提醒** → 选择阈值。设置自动保存；选择“关闭（默认）”即可停用。

- 电池供电且读数有效、已连接、电量达到阈值时提示一次；开启时已低电量也会提示。
- 同一运行会话内不重复提醒；失败读取、短暂断连或小幅波动不会重置提醒。
- 开始充电、充电完成，或电量回升到阈值以上至少 **5 个百分点**后，再次进入低电量才提醒。切换阈值或重启程序会重新判断。
- 演示模式不发送通知。Windows 勿扰模式和系统通知设置可能抑制显示。
- 复用原有刷新结果，没有新增查询、定时器、通知组件或安装步骤。

## 兼容性

| 系列 | 型号 / 读取方式 | 验证记录 |
| --- | --- | --- |
| A7 | A7、Pro、Ultra、Ultra(RE)、A7X Ultra；经典 HID 协议 | A7 V2 Pro 的 2.4G 读取和托盘显示已有用户实机确认 |
| A7 V2 | Pro、Pro+、Ultra、Ultra+；经典 HID 协议 | 其余型号按官网协议适配 |
| L7 / M7 | 普通版、Pro、Ultra，以及 L7 Pro+ / Ultra+；经典 HID 协议 | 官网型号表和协议适配 |
| K7 Ultra | 经典 HID 协议 | 官网型号表和协议适配 |
| K7 V2 | Pro、Pro+、Ultra、Ultra+；0x4D 新协议 | 新读取通道和解析检查已实现，等待型号实机反馈 |
| K5 | Pro、Ultra、V2；0x4D 新协议 | 官网型号表和新协议适配 |
| 更多新协议鼠标 | A7 V3 系列、A5 V3 Pro / Ultra+、R7 Ultra、V7 / Pro、G3 V3 | 官网型号表和新协议适配 |
| A5 V2 Ultra / AX5 V2 | 经典 HID 协议 | 官网型号表和协议适配 |

支持上述协议的 USB / 2.4G 模式。蓝牙、键盘、耳机、旧 G3 协议及未列出的 K7 普通版固件不在本版支持表内。协议适配与实机验证分别记录，不代表所有固件都已测试。同一鼠标的 USB 和接收器可能各显示一个图标，接收器缓存行为取决于固件。

## 电量不显示或不准确？

先移动鼠标唤醒，检查接收器，退出官方驱动，然后右键“立即刷新”。仍有问题时，请提交 [问题报告](https://github.com/HifumiDaisuki01/MchoseBatteryTray/issues/new/choose)，附上：

- 完整型号、固件版本、Windows 版本和连接方式。
- 工具显示值与 M HUB 同时显示值，是否正在充电。
- 右键“打开诊断文件夹”中的 `diagnostic.log`。

日志包含设备产品名、VID/PID、HID 使用页、状态回包及错误，可能包含设备固件等字段；公开前请自行检查。程序不记录鼠标移动、按键或账户信息，也不上传日志。

## 从源码构建

Windows 双击 `build.cmd`，或在 PowerShell 中执行：

```powershell
./scripts/build.ps1 -Test
```

构建及测试均只使用系统 .NET Framework 编译器，无 NuGet 依赖。CI 在 Windows runner 上构建并运行协议测试；标签推送会生成 Release 附件及 SHA-256 校验。

目前已通过 **原有协议、提醒逻辑以及新增型号和报文兼容性检查**，包括低电量数值、身份字段、连接位、充电状态和异常回包拒绝。合成测试不代表硬件验证。`MchoseTray.exe --demo` 使用明确标识的模拟数据。

## 工作原理

官网网页驱动通过 WebHID Feature Report 查询鼠标；本工具以 `hid.dll` 和 SetupAPI 实现同类查询。协议、来源与验证边界见 [PROTOCOL.md](PROTOCOL.md)。本次未对 M HUB 安装包做独立动态分析。

## 参与与支持

欢迎通过 Issues 报告兼容型号、纠正协议字段，或提交 Pull Request。路线方向包括实机验证、USB/接收器去重，以及有证据支持的更多设备协议。

**如果这个工具帮你省去了反复打开驱动的麻烦，欢迎点一个 ⭐ Star，让更多迈从用户找到它。**

Keran Technology © 2026 · [tech.keran.cc](http://tech.keran.cc)

问题反馈：[Keran@keran.cc](mailto:Keran@keran.cc) · QQ：3522952742

[Powered by Keran](https://www.keran.cc) · [MIT License](LICENSE) · 非迈从官方项目。
