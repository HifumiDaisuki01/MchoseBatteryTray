## v0.1.3 — 自然比例数字 / Natural tray digits

修复 v0.1.2 为填满托盘区域而分别缩放宽高，造成数字细长变形的问题。

- 保持 Segoe UI Bold 原始字形比例，统一缩放宽高、居中显示，并保留上下空白。
- 正常电量随任务栏主题显示黑色或白色；低电量红色、充电蓝色。
- 100 通过等比缩小完整显示；保持透明背景，无边框、圆形底板和横线。
- 不增加运行依赖或后台任务；低电量提示与设备查询逻辑保持可用。

Fixes the stretched digits introduced in v0.1.2. Text now uses uniform scaling, preserves Segoe UI Bold proportions, and is centered with vertical padding. Normal readings use black or white for the taskbar theme; low battery remains red and charging blue. Three-digit 100 scales down without distortion. No new runtime dependencies or background jobs.

Windows 编译、22 项协议及 26 项提醒逻辑检查，以及实际 Windows 字形预览均由关联 Actions 生成。请在自己的任务栏缩放设置下核对最终视觉效果。

下载 ZIP 或 EXE，退出旧版后替换即可；旧版本保留。
