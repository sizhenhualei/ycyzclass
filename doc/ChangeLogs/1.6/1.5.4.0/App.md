
# 1.5.4.0

1.6 - Himeko

🎉 本版本是 1.6 的最后一个 Beta 版，将在确认稳定并修复在这个版本收到的问题后，在下一个版本发布 1.6 的正式版。

> 1.6 - Himeko 测试版，可能包含未完善和不稳定的功能

## 🚀 新增功能与优化

- **【自动化】触发器：** 支持使用触发器启动自动化工作流
- **【自动化】行动时间点：** 从时间表运行行动 [#119](https://github.com/YcyzClass/YcyzClass/issues/119)
- **【档案】调课面板：** 支持提前预定换课课表和跨课表换课 [#321](https://github.com/YcyzClass/YcyzClass/issues/321) [#373](https://github.com/YcyzClass/YcyzClass/issues/373) [#617](https://github.com/YcyzClass/YcyzClass/issues/617)
- 【自动化】工作流允许不恢复和不考虑条件
- 【自动化/行动】添加提醒行动
- 【自动化/行动】通过行动显示天气提醒 [#494](https://github.com/YcyzClass/YcyzClass/issues/494)
- 【档案编辑器】优化档案编辑器行动编辑外观
- 【档案编辑器】分割线、行动时间点拖动功能 [#153](https://github.com/YcyzClass/YcyzClass/issues/153)
- 【档案编辑器】改进时间点添加体验
- 【档案】预定启用临时层课表 [#321](https://github.com/YcyzClass/YcyzClass/issues/321)
- 【档案】支持 CSES 课表格式转换 [#642](https://github.com/YcyzClass/YcyzClass/issues/642)
- 【档案/自动化】添加档案信任机制
- 【UI】修改崩溃窗口文字
- 【UI】开发中画面水印
- 【组件/课表】隐藏上过的课程 [#193](https://github.com/YcyzClass/YcyzClass/issues/193) ([#648](https://github.com/YcyzClass/YcyzClass/issues/648) by @itsHenry35)
- 【组件/课表】模糊倒计时 [#313](https://github.com/YcyzClass/YcyzClass/issues/313)
- 【组件/天气简报】气象预警图标增加预警类型表示 [#568](https://github.com/YcyzClass/YcyzClass/issues/568)
- 【组件/天气简报】显示降水提示 [#176](https://github.com/YcyzClass/YcyzClass/issues/176)
- 【提醒】下课提醒文字自定义 [#341](https://github.com/YcyzClass/YcyzClass/issues/341)
- 【提醒/天气】支持按小时显示天气预报 [#184](https://github.com/YcyzClass/YcyzClass/issues/184)
- 【日志】日志、插件搜索忽略大小写
- 【调试】不保存调试时间和时间流速对时间偏移的改动

## 🐛 Bug 修复

- 【应用设置】修复应用设置导航栏排序可能不正常的问题
- 【档案编辑器】修复删除时间点时卡顿的问题
- 【主界面】修复应用启动后不会应用反转指针移入隐藏的问题
- 【主界面】修复在加载课表组件时找不到资源“ClassPlanCollectionViewSource”的问题
