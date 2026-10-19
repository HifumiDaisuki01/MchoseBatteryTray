## v0.1.1 — 可选低电量提醒 / Optional low-battery alerts

新增轻量低电量提示，使用 Windows 自带托盘通知，无新增依赖、后台定时器或额外 HID 查询。

- 默认关闭；右键“低电量提醒”可选 10%、20%、30%，设置自动保存。
- 每个设备每轮低电量提醒一次；充电或回升到阈值以上至少 5 个百分点后才重新允许提醒。
- 无效读数、断连、未知充电状态与模拟模式不提醒。重启或修改阈值会重新判断。
- Windows 的通知设置 / 勿扰模式可能阻止通知显示。

Built-in Windows tray notifications, off by default, with saved 10% / 20% / 30% thresholds. Reuses existing polling: no additional libraries, timers or device queries. Repeated polls, temporary disconnections and small fluctuations do not repeat an alert. Charging or recovery by at least five percentage points above the threshold rearms it. Restarting or changing the threshold starts a fresh decision. Windows may suppress notifications.

### 验证 / Validation

22 项合成协议检查 + 26 项提醒逻辑检查；Windows 编译及测试结果见关联 Actions。真实 A7 V2 Pro 和 Windows 通知显示仍待用户实机验证。

22 synthetic protocol checks plus 26 alert checks. See the associated Actions run for Windows build/test results. Physical mouse behavior and on-screen notification delivery remain unverified.

### 下载 / Downloads

推荐下载 `MchoseBatteryTray-Windows.zip` 并解压运行；也提供单独 EXE 与 SHA-256 校验。Windows 10 / 11，.NET Framework 4.5+，无需安装其他组件。

Bluetooth, keyboards and headsets remain unsupported. This is an experimental prerelease. Previous releases are retained.
