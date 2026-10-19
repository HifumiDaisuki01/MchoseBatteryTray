# MCHOSE Battery Tray

**See your MCHOSE mouse battery in the Windows system tray, without keeping M HUB or a web driver open.**

[简体中文](README.md) · [Download](https://github.com/HifumiDaisuki01/MchoseBatteryTray/releases) · [Report compatibility](https://github.com/HifumiDaisuki01/MchoseBatteryTray/issues/new/choose)

A small, offline C# / WinForms utility using native Windows HID feature reports. Supports the A7, L7, M7 and K7 Ultra families, with a separate newer-protocol reader for K7 V2, K5, A7 V3 and other listed models.

## Features

- Borderless digits on a transparent tray icon: natural font proportions, centered with vertical padding, without stretching or decoration.
- Black or white to match the taskbar for normal battery operation, red at 20% or below, blue while charging, grey `--` when unavailable.
- Automatic polling and device rescanning every 5 / 10 / 30 / 60 seconds; 10 seconds by default.
- Hover for status, double-click for details, right-click for settings and diagnostics.
- Optional low-battery notifications, disabled by default, with 10% / 20% / 30% thresholds.
- Optional per-user startup without administrator privileges.
- One executable; no Python, Node.js, browser, M HUB, NuGet packages or network access required by the app.
- Status queries only: no DPI, button, polling-rate or firmware changes.

## Getting started

1. Download and extract the Windows ZIP from [Releases](https://github.com/HifumiDaisuki01/MchoseBatteryTray/releases).
2. Connect the 2.4 GHz receiver or a USB data cable. Bluetooth is not supported.
3. Close M HUB and the web driver during initial testing, then run `MchoseTray.exe`.
4. Drag the icon out of the hidden-icons area if necessary. Enable startup from its context menu after moving the EXE to a permanent location.

Windows 10 / 11 with .NET Framework 4.5+ is required. The executable is unsigned. The current interface is in Chinese.

## Low-battery notifications

Right-click the tray icon and open `低电量提醒`. Choose 10%, 20% or 30%; `关闭（默认）` disables alerts. The choice persists across restarts.

Notifications use the built-in Windows tray balloon API and the existing polling results: no new dependencies, timers or extra device queries. Only valid, connected readings on battery power can trigger an alert. Each device alerts once per low-battery episode within the current session. Failed reads, disconnections and small fluctuations do not rearm it; charging, charge completion or a recovery at least five percentage points above the threshold does. Changing the threshold or restarting starts a fresh decision. Demo mode never sends alerts. Windows notification settings or Do Not Disturb may suppress delivery.

## Compatibility

| Protocol | Model families | Validation |
| --- | --- | --- |
| Classic feature reports | A7 / Pro / Ultra / Ultra(RE), A7 V2 series, A7X Ultra, L7 and M7 variants, K7 Ultra, A5 V2 Ultra, AX5 V2 | A7 V2 Pro readings/tray status over 2.4 GHz confirmed by a user; other models adapted from official tables |
| New 0x4D output/input reports | K7 V2 Pro / Pro+ / Ultra / Ultra+, K5 Pro / Ultra / V2, A7 V3 variants, A5 V3 Pro / Ultra+, R7 Ultra, V7 / Pro, G3 V3 | Separate reader and synthetic compatibility checks implemented from official protocol; awaiting model-specific hardware reports |

USB and 2.4 GHz only. Bluetooth, keyboards, headsets, older G3 protocols and unlisted ordinary-K7 firmware are not claimed. Protocol implementation and physical-device confirmation are tracked separately. A cable and receiver may produce two icons; receiver-side caching depends on firmware.

## Troubleshooting and feedback

Wake the mouse, check the receiver, close the official driver, and choose the second context-menu action to refresh. If readings remain unavailable or incorrect, [open an issue](https://github.com/HifumiDaisuki01/MchoseBatteryTray/issues/new/choose) with the exact model, firmware, Windows version, connection type, charging state, and a simultaneous M HUB reading.

The context-menu action `打开诊断文件夹` opens `%LOCALAPPDATA%\KeranMchoseTray`. Its `diagnostic.log` contains product names, VID/PID, HID usage pages, raw status replies and errors. Replies may contain firmware information; review before sharing publicly. Mouse movements, keystrokes and account data are not recorded. Logs are not uploaded.

## Building and testing

On Windows, double-click `build.cmd`, or run:

```powershell
./scripts/build.ps1 -Test
```

The build uses the system .NET Framework C# compiler. CI builds and tests on Windows, and tag pushes package a stable release with SHA-256 checksums. Protocol, alert and model compatibility checks cover request framing, low percentages without incorrect scaling, identity and link parsing, charging, invalid replies and model mapping. These are not USB hardware tests.

`MchoseTray.exe --demo` shows a clearly labelled simulated device without querying HID. Normal launches never use simulated data.

## Protocol, contributing and license

See [PROTOCOL.md](PROTOCOL.md) for the official bundle inspected, field offsets, cross-references and validation limits. The native M HUB installer has not been independently analyzed.

Issues and pull requests are welcome, especially hardware reports, connection deduplication and evidence-backed support for additional protocols. If the tool helps, **please consider starring the repository** so other MCHOSE users can find it.

Keran Technology © 2026 · [tech.keran.cc](http://tech.keran.cc)

Feedback: [Keran@keran.cc](mailto:Keran@keran.cc) · QQ: 3522952742

[Powered by Keran](https://www.keran.cc) · [MIT License](LICENSE) · Not affiliated with MCHOSE.
