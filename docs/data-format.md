# 数据格式与可靠性

主数据文件为 SQLite，格式版本1。表分别为 daily_tasks、workouts、exercises、exercise_sets、meals、water_entries、game_plans、game_sessions、journals、settings、schema_meta。

每个实体具有独立 id 与结构化 JSON body；用于唯一性和父子关系的日期、餐次、组号和 parent 是 SQLite 生成列，数据库通过唯一约束、CHECK 和外键进行约束。JSON 属性在数据库内为 PascalCase，导出的备份使用 camelCase。所有 SQL 值使用参数绑定，表名为固定白名单。

训练删除级联删除动作和组，撤销保存已删除的完整子树并合入当前数据，不覆盖其他新编辑。撤销冲突时整体事务失败，原数据保留。

保存使用单写入队列；每条编辑带版本，旧保存回执不能覆盖更新的草稿状态。文本500ms防抖，切页及退出等待 Flush。数据库每次操作独立连接、关闭连接池，foreign_keys开启，synchronous=FULL。

备份包含 formatVersion、exportedAt、data。data 下包含九类业务数组与 settings。导出先写同目录临时文件、刷新磁盘，再原子替换目标文件；失败保留原备份。

恢复校验必需字段、数据类型、记录标识、关系、唯一性、枚举、数值和区间时长一致性。校验后的快照供用户预览；确认后在一个 SQLite 事务内整份替换。任何插入失败都回滚整个事务。

程序文件不包含用户数据库。测试目录通过显式构造参数或 `--data-dir` 指定；导入的备份不能指定主数据库路径。

实现相对计划的文件组织调整：页面采用分模块 WPF 视图构建文件（Features 下的 *View.cs），共用 XAML 主窗口。业务校验和数据操作仍在 Core/Storage，视图不执行 SQL。使用同一个数据源与保存协调器，未引入服务端。
