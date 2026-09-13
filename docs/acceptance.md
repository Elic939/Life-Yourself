# 第一版验收记录

版本 1.0.0；2026-09-13；当前 Windows x64。

开发与自动验证已完成，最终人工验收尚未全部完成。下表自动测试证据不等同于完成 PRD 全部人工操作。没有删除、跳过测试或降低验收标准。完整原始结果位于 artifacts/test-results/all-tests.trx。

| PRD 编号 | 已执行结果和证据 | 待执行 |
| --- | --- | --- |
| A01 | 实际自包含 EXE 独立窗口启动；PublishedAppTests、DesktopUiTests | 实际断网后双击并录入 |
| A02 | 七页切换、选中导航、日期保持；DesktopViewModelTests、DesktopUiTests | 人工回到今天复核 |
| A03 | 空库无示例实体；Test1、DesktopUiTests | 人工逐页空状态复核 |
| A04 | 事项增改、完成、删除与摘要；TaskTests、SummaryTests | 人工完整录入 |
| A05 | 模块不增加日常事项分母；SummaryTests | 人工入口跳转 |
| A06 | 各训练组实际值独立；FitnessTests | 人工三组录入 |
| A07 | 有氧计划与实际分离，日志累计；FitnessTests、SummaryTests | 人工有氧录入 |
| A08 | 餐食计划实际分离，打开饮水面板不丢草稿；MealTests、DesktopUiTests | 人工首页状态 |
| A09 | 饮水增删、日期隔离；MealTests | 人工 250/300 ml 操作 |
| A10 | 区间、手填和周统计；GamingTests | 人工 45+30 分钟操作 |
| A11 | 跨午夜归开始日；GamingTests | 人工跨日输入 |
| A12 | 部分日志、同日唯一、保存；JournalTests | 人工心情与切日 |
| A13 | 全模块样本实际 EXE 退出重开一致；PublishedAppTests；关闭前保存 DesktopUiTests | 实际重启电脑 |
| A14 | 快速导航与退出保存；SaveCoordinatorTests、DesktopUiTests | 人工快速切换 |
| A15 | 注入写入失败，保留草稿并重试；SaveCoordinatorTests、DesktopUiTests | 已有故障注入证据 |
| A16 | 多日期全表逐字段备份往返；BackupTests | 人工文件对话框导出 |
| A17 | 完整恢复原记录设置；BackupTests、RecoveryTests | 人工恢复及实际重启电脑 |
| A18 | 无效格式、版本、字段、关系与时长拒绝；BackupTests | 人工错误提示 |
| A19 | 取消无变化、中途失败回滚；BackupTests、RecoveryTests | 人工取消确认 |
| A20 | 撤销保留其他修改，清空不删除备份；UndoTests、BackupTests | 人工清空取消及确认 |
| A21 | 空名称、负数、非法区间拒绝；TaskTests、MealTests、GamingTests | 人工错误提示 |
| A22 | 七页 WPF 渲染、1200×820 和 900×650 截图、可访问名称；DesktopUiTests | 完整键盘遍历、实际 150% 显示缩放 |
| A23 | 真实汇总、有氧与大数值累计；SummaryTests | 人工连续修改 |
| A24 | SQLite 文件重开恢复；StorageTests、PublishedAppTests | 人工打开数据文件夹 |
| A25 | 中文空格路径启动，覆盖程序后记录设置一致，程序目录无数据库；PublishedAppTests | 自动检查通过 |

## 最终真实机器验收

按 PRD 第 9 节在独立测试数据中完整操作一轮，断网后启动并录入，各模块与设置保存后导出、修改、恢复，再正常重启电脑核对。将显示缩放设为 150% 检查窗口、长文本和键盘，随后恢复原设置。

不会自动断网或重启当前电脑，以免打断用户其他工作。上述待执行项完成前，不标记全部验收通过。开发测试可用 --data-dir 指定独立目录，正式数据不得清空或覆盖。
