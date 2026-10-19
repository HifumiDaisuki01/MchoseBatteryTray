## MCHOSE Battery Tray v1.0.0

首个正式版。保留自然比例的黑白数字托盘、低电量提醒及开机启动，移除实验版标识与状态窗口中的首次测试提示。

### 更新

- 扩充 A7 普通版 / Pro / Ultra、L7、M7、K7 Ultra 等经典协议型号识别。
- 新增 0x4D 原生异步 HID 读取通道，适配 K7 V2、K5、A7 V3 及型号表中其他新协议鼠标；包含超时、报文身份、长度和校验检查。
- 重做状态窗口：底部显示 Keran Technology © 2026、http://tech.keran.cc、问题反馈邮箱 Keran@keran.cc 和 QQ: 3522952742；官网与邮箱可点击。
- 右键菜单标明版权，最底部“Powered by Keran”打开 www.keran.cc。
- 无新增运行依赖；继续使用系统 .NET Framework、WinForms 和 Windows HID API。

### 兼容性记录

A7 V2 Pro 的 2.4G 读取和托盘显示已有用户实机确认。其他新增型号按官方网页驱动的型号表和协议适配，尚无对应型号实机报告；详见 README。蓝牙、键盘、耳机及未列出的固件协议不在支持范围。

### 下载

下载 Windows ZIP 或单独 EXE，退出旧版后替换运行。设置自动保留。附 SHA-256 校验，旧版本 Release 保留。

First stable release with expanded classic mouse identification, a separate timeout-bounded 0x4D reader for newer families, and a branded status window with website and feedback links. No new runtime dependencies. A7 V2 Pro 2.4 GHz readings/tray status are user-confirmed; other listed models are protocol-adapted rather than physically certified. See README for exact support.
