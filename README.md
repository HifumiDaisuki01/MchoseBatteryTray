# MCHOSE Battery Tray · 迈从电量托盘

**在 Windows 右下角直接查看迈从鼠标电量，无需常驻 M HUB 或网页驱动。**

[English](README.en.md) · [下载试用版](https://github.com/HifumiDaisuki01/MchoseBatteryTray/releases) · [使用说明](USAGE.zh-CN.md) · [反馈兼容性](https://github.com/HifumiDaisuki01/MchoseBatteryTray/issues/new/choose)

![Windows](https://img.shields.io/badge/Windows-10%20%2F%2011-0078D4) ![License: MIT](https://img.shields.io/badge/License-MIT-green) ![Status](https://img.shields.io/badge/status-experimental-orange)

一个使用 C# / WinForms 编写的轻量托盘工具，通过 Windows 原生 HID 接口读取电池状态。优先面向 **MCHOSE A7 V2 Pro（迈从 A7 二代 Pro）**；源码、协议笔记和构建脚本均公开。

> **当前版本为实验性预发布，A7 V2 Pro 尚待实机验证。** 已依据官方网页驱动核对协议并通过合成解析测试；不能据此声称已通过 Windows HID 或真实硬件测试。欢迎提交实机兼容性报告。

## 功能

- **数字电量图标**：直接在通知区域显示百分比数值。
- **状态颜色**：绿色为正常供电，红色为低电量，蓝色为充电，灰色 `--` 为无法读取。
- **自动刷新与设备重扫**：默认 10 秒，可选 5 / 10 / 30 / 60 秒。
- **悬停看状态、双击看详情**，右键立即刷新、打开诊断文件夹。
- **可选开机启动**，无需管理员权限。
- **单 EXE、离线运行**：不依赖 Python、Node.js、浏览器或 M HUB；使用 Windows 自带 .NET Framework。
- 仅查询状态，不修改 DPI、按键、回报率或固件。

## 下载与使用

1. 从 [Releases](https://github.com/HifumiDaisuki01/MchoseBatteryTray/releases) 下载 Windows ZIP 并解压。
2. 插入 **2.4G 接收器**或用 **USB 数据线**连接鼠标。
3. 首次测试时退出 M HUB，关闭网页驱动，再双击 `MchoseTray.exe`。
4. 如图标藏在托盘折叠菜单，把它拖到任务栏可见区域；右键可设置刷新间隔与开机启动。

系统：Windows 10 / 11，.NET Framework 4.5+。详细说明见 [中文使用指南](USAGE.zh-CN.md)。发行文件尚未签名。

## 兼容性

| 型号 / 连接 | 当前状态 |
| --- | --- |
| A7 V2 Pro · 2.4G / USB | 优先适配；官方协议已核对，实机待验证 |
| A7 V2 Pro+ / Ultra / Ultra+ | 已有型号映射，实机待验证 |
| 其他经典协议迈从鼠标 | 可能兼容，尚未验证 |
| 蓝牙 | 暂不支持 |
| 键盘、耳机及其他协议设备 | 暂不支持 |

这不是全系列通用驱动。同一鼠标同时通过 USB 和接收器连接时可能显示两个图标；接收器也可能返回缓存电量。官方驱动共存、固件差异和睡眠状态仍需验证。

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

目前已通过 **22 项合成协议检查**，包括低电量数值、身份字段、连接位、充电状态和异常回包拒绝。合成测试不代表硬件验证。`MchoseTray.exe --demo` 使用明确标识的模拟数据。

## 工作原理

官网网页驱动通过 WebHID Feature Report 查询鼠标；本工具以 `hid.dll` 和 SetupAPI 实现同类查询。协议、来源与验证边界见 [PROTOCOL.md](PROTOCOL.md)。本次未对 M HUB 安装包做独立动态分析。

## 参与与支持

欢迎通过 Issues 报告兼容型号、纠正协议字段，或提交 Pull Request。路线方向包括实机验证、USB/接收器去重，以及有证据支持的更多设备协议。

**如果这个工具帮你省去了反复打开驱动的麻烦，欢迎点一个 ⭐ Star，让更多迈从用户找到它。**

由 Keran Technology 维护，使用 [MIT License](LICENSE)。非迈从官方项目，与 MCHOSE 无隶属关系。
