## v0.1.2 — 更清晰的纯数字托盘 / Clear borderless tray digits

- 移除圆形背景和底部横线，只显示透明背景的大号粗体数字。
- 按字形实际边界填满托盘空间，包含 0、两位电量和 100；未知状态仍显示 `--`。
- 浅色任务栏使用更深的状态色，提升对比度；保留充电蓝色和低电量红色。
- 无新增依赖、轮询或后台定时器；低电量提醒与设置保持可用。

Transparent, borderless, larger bold digits replace the circular background and underline. Glyph bounds fill the tray area, including three-digit 100. Darker status colors improve contrast on light taskbars. No new dependencies or polling.

用户已确认 A7 V2 Pro 的 2.4G 读取和托盘状态显示正常。新图标在不同 DPI、Windows 主题下的最终效果仍请实机核对；USB、充电及提醒显示未据此宣称通过验证。

A user confirmed A7 V2 Pro readings and tray status over 2.4 GHz. Please check the new icon on your Windows theme/DPI. USB, charging and notification delivery remain unverified.

Windows 构建及 22 项协议 + 26 项提醒逻辑检查由关联 Actions 验证。下载 ZIP 或单独 EXE，退出旧版后替换即可。旧版 Release 保留。
