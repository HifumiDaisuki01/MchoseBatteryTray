# MCHOSE Battery Tray

**See your MCHOSE mouse battery in the Windows system tray, without keeping M HUB or a web driver open.**

[简体中文](README.md) · [Download](https://github.com/HifumiDaisuki01/MchoseBatteryTray/releases) · [Report compatibility](https://github.com/HifumiDaisuki01/MchoseBatteryTray/issues/new/choose)

A small, offline C# / WinForms utility using native Windows HID feature reports. The initial target is the **MCHOSE A7 V2 Pro**.

> Experimental prerelease. The official web driver's protocol has been inspected, and synthetic parser tests pass. **The A7 V2 Pro and Windows HID behavior have not yet been tested on physical hardware.** Compatibility reports are welcome.

## Features

- Battery percentage rendered directly inside a notification-area icon.
- Green for normal battery operation, red at 20% or below, blue while charging, grey `--` when unavailable.
- Automatic polling and device rescanning every 5 / 10 / 30 / 60 seconds; 10 seconds by default.
- Hover for status, double-click for details, right-click for settings and diagnostics.
- Optional per-user startup without administrator privileges.
- One executable; no Python, Node.js, browser, M HUB, NuGet packages or network access required by the app.
- Status queries only: no DPI, button, polling-rate or firmware changes.

## Getting started

1. Download and extract the Windows ZIP from [Releases](https://github.com/HifumiDaisuki01/MchoseBatteryTray/releases).
2. Connect the 2.4 GHz receiver or a USB data cable. Bluetooth is not supported.
3. Close M HUB and the web driver during initial testing, then run `MchoseTray.exe`.
4. Drag the icon out of the hidden-icons area if necessary. Enable startup from its context menu after moving the EXE to a permanent location.

Windows 10 / 11 with .NET Framework 4.5+ is required. The executable is unsigned. The current interface is in Chinese.

## Compatibility

| Device / connection | Status |
| --- | --- |
| A7 V2 Pro, 2.4 GHz / USB | Primary target; official protocol inspected, hardware unverified |
| A7 V2 Pro+ / Ultra / Ultra+ | Model mappings included; hardware unverified |
| Other classic-protocol MCHOSE mice | Potentially compatible; unverified |
| Bluetooth | Not supported |
| Keyboards, headsets, other protocols | Not supported |

This is not an all-model driver. A cable and receiver may produce two icons. Receiver-side cached values and sleep behavior depend on firmware. Coexistence with official software has not been validated.

## Troubleshooting and feedback

Wake the mouse, check the receiver, close the official driver, and choose the second context-menu action to refresh. If readings remain unavailable or incorrect, [open an issue](https://github.com/HifumiDaisuki01/MchoseBatteryTray/issues/new/choose) with the exact model, firmware, Windows version, connection type, charging state, and a simultaneous M HUB reading.

The context-menu action `打开诊断文件夹` opens `%LOCALAPPDATA%\KeranMchoseTray`. Its `diagnostic.log` contains product names, VID/PID, HID usage pages, raw status replies and errors. Replies may contain firmware information; review before sharing publicly. Mouse movements, keystrokes and account data are not recorded. Logs are not uploaded.

## Building and testing

On Windows, double-click `build.cmd`, or run:

```powershell
./scripts/build.ps1 -Test
```

The build uses the system .NET Framework C# compiler. CI builds and tests on Windows, and tag pushes package a prerelease with SHA-256 checksums. The 22 synthetic protocol checks cover request framing, low percentages without incorrect scaling, identity and link parsing, charging, invalid replies and model mapping. These are not USB hardware tests.

`MchoseTray.exe --demo` shows a clearly labelled simulated device without querying HID. Normal launches never use simulated data.

## Protocol, contributing and license

See [PROTOCOL.md](PROTOCOL.md) for the official bundle inspected, field offsets, cross-references and validation limits. The native M HUB installer has not been independently analyzed.

Issues and pull requests are welcome, especially hardware reports, connection deduplication and evidence-backed support for additional protocols. If the tool helps, **please consider starring the repository** so other MCHOSE users can find it.

Maintained by Keran Technology. [MIT License](LICENSE). Unofficial; not affiliated with MCHOSE.
