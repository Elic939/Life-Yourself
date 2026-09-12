# 个人生活桌面 App 开发执行计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. 默认顺序执行，不自动派发子代理。

**Goal:** 根据根目录 PRD.md V1.1，交付可在当前 Windows 电脑上离线双击运行、数据自动保存为本机文件的个人生活桌面 App。

**Architecture:** 使用 WPF 构建独立桌面窗口，以 MVVM 分离界面与业务操作。业务数据由统一存储服务写入 SQLite 文件；首页及日志读取同一份数据的摘要。备份使用完整 JSON 文件，恢复通过数据库事务整份替换。

**Tech Stack:** 建议采用 C#、.NET 10、WPF、Microsoft.Data.Sqlite、System.Text.Json、MSTest。交付 Windows x64 自包含解压包。技术选择是本计划提出的实施方案，不应表述为用户之前已经指定。

## Global Constraints

- “日常使用不需要打开浏览器、输入网址或运行命令。”
- “用户数据留在当前电脑，不上传网络。”
- “关闭并重新打开 App、重启电脑后，已保存的数据必须保留。”
- “日常主数据必须由桌面 App 自动写入本机数据文件”。
- “不得使用浏览器存储作为主数据来源。”
- “数据目录与程序安装目录分开，更新或替换程序不得覆盖用户数据。”
- “第一版采用整份替换恢复，不做合并。”
- “首页的事项完成比例只统计日常事项”。
- 七个模块与 PRD 的中文命名、顺序保持一致；不得填入真实库演示数据。
- 不做账户、云服务、手机端、后台游戏监控或 PRD 排除的其他功能。
- 本次只编写计划，不安装 SDK、不创建应用项目、不开始编码。

---

## 1. 当前状态、实施选择与里程碑

### 1.1 已核实的起点

- 工作根目录：`C:\Users\WANGRUIPENG\Desktop\ppp+wzh`。
- 本次开始时仅有 `PRD.md`，没有现成源码或项目约定文件。
- 系统架构为 X64，可找到 dotnet 和 git 命令，但 `dotnet --list-sdks` 没有返回任何 SDK。
- 尚未取得页面预览的最终确认。任务 1 先完成布局确认；其余技术任务已有执行顺序。
- 不假定现有 Git 仓库；开始开发时再检查和初始化本地版本管理，不创建远程仓库。

### 1.2 本计划采用的具体选择

| 项目 | 选择 | 原因和边界 |
| --- | --- | --- |
| 桌面界面 | WPF 原生窗口，C# / XAML | 仅面向 Windows，不引入浏览器或本地网站服务器 |
| 主数据 | SQLite，单用户、单实例、单写入队列 | 有事务能力，便于可靠保存和失败恢复 |
| 数据目录 | Windows LocalApplicationData 下的 `PersonalLife` | 实际路径通过系统 API 获取，通常为 `%LOCALAPPDATA%\PersonalLife`；不硬编码用户名 |
| 主文件 | `life.db` | 设置页展示真实完整路径；允许 SQLite 自身的辅助日志文件 |
| 备份 | 用户选择位置的完整 JSON 文件 | 不要求用户理解数据库文件结构 |
| 交付 | `PersonalLife-win-x64.zip`，内含 `PersonalLife.exe` 及依赖 | 解压后双击，不要求用户安装 .NET；不要求所有程序依赖合成一个 EXE |
| 页面 | 沿用左导航、右内容、右侧编辑面板 | 细节在任务 1 定稿，避免先做完整业务后返工 |
| 日期和单位 | DateOnly；24 小时制；kg、ml、分钟 | 只在展示层格式化，内部不保存带单位的数字字符串 |
| 一周起始日 | 默认星期一，可切换星期日 | 周统计依据当前所选日期所在周计算 |

WPF 是 Windows 桌面界面框架；自包含发布可携带所需运行时，目标电脑无需预装 .NET。[WPF 官方文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)、[.NET 发布说明](https://learn.microsoft.com/en-us/dotnet/core/deploying/)

SQLite 事务用于保证批量修改整体提交或回滚，本计划用它保护恢复与清空操作。[Microsoft.Data.Sqlite 事务说明](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/transactions)

开发环境首次安装和依赖下载可以联网；交付的 App 运行不依赖网络。SDK 与依赖使用执行时 .NET 10 系列可用稳定补丁，记录确切版本到 `global.json` 和包锁定文件，不使用预览版或浮动版本发布。

### 1.3 里程碑与顺序

| 里程碑 | 任务 | 用户可检查的结果 |
| --- | --- | --- |
| M0 设计定稿 | 1 | 七页结构、编辑面板和主要状态得到确认 |
| M1 桌面与数据底座 | 2—4 | 独立窗口运行；能把数据写入文件并重开读取；保存失败可见 |
| M2 生活记录 | 5—9 | 日常事项、训练、饮食、游戏、日志可以真实记录和回看 |
| M3 完整联动与数据管理 | 10—11 | 首页真实汇总；备份、恢复、清空与撤销可用 |
| M4 本机交付 | 12—13 | 离线解压包、使用说明、25 项 PRD 验收结果 |

严格顺序：1 → 2 → 3 → 4 → 5 → 6 → 7 → 8 → 9 → 10 → 11 → 12 → 13。模块不拆成独立应用，不增加服务端。

## 2. 文件组织与职责

下列路径均相对工作根目录；是将要创建的文件，不表示已经存在。视图只负责展示和绑定，禁止在 XAML 的事件处理代码中写数据库操作。

```text
PersonalLife.slnx
global.json
Directory.Build.props
.gitignore
src/
  PersonalLife.Core/
    PersonalLife.Core.csproj
    Domain/Models.cs                 数据实体、枚举、备份对象
    Domain/Validation.cs             字段及跨字段校验
    Domain/DateRules.cs              归属日期、周范围、时长计算
    Storage/IDataStore.cs            存储契约
    Editing/SaveCoordinator.cs       草稿、排队、保存状态、切换屏障
    Editing/UndoService.cs           删除及撤销
    Tasks/TaskService.cs
    Fitness/FitnessService.cs
    Meals/MealService.cs
    Gaming/GamingService.cs
    Journal/JournalService.cs
    Summary/SummaryService.cs
    Backup/BackupService.cs
  PersonalLife.Storage/
    PersonalLife.Storage.csproj
    DataPaths.cs                     正式库与测试库路径
    SqliteDataStore.cs               连接、读写、事务与快照
    MigrationRunner.cs
    Migrations/001_initial.sql
    BackupFileWriter.cs              临时文件与目标文件替换
  PersonalLife.App/
    PersonalLife.App.csproj
    App.xaml
    App.xaml.cs                      组装服务、启动与退出
    MainWindow.xaml
    MainWindow.xaml.cs
    Infrastructure/ObservableObject.cs
    Infrastructure/AsyncCommand.cs
    Infrastructure/DesktopDialogs.cs
    Infrastructure/SingleInstance.cs
    Shell/ShellViewModel.cs
    Shared/DateBar.xaml
    Shared/EditorDrawer.xaml
    Shared/SaveStatusView.xaml
    Shared/UndoNotice.xaml
    Themes/Colors.xaml
    Themes/Controls.xaml
    Features/Home/HomeView.xaml
    Features/Home/HomeViewModel.cs
    Features/Tasks/TasksView.xaml
    Features/Tasks/TasksViewModel.cs
    Features/Fitness/FitnessView.xaml
    Features/Fitness/FitnessViewModel.cs
    Features/Meals/MealsView.xaml
    Features/Meals/MealsViewModel.cs
    Features/Gaming/GamingView.xaml
    Features/Gaming/GamingViewModel.cs
    Features/Journal/JournalView.xaml
    Features/Journal/JournalViewModel.cs
    Features/Settings/SettingsView.xaml
    Features/Settings/SettingsViewModel.cs
tests/
  PersonalLife.Tests/
    PersonalLife.Tests.csproj
    Support/TestStore.cs
    Support/FaultingStore.cs
    StorageTests.cs
    SaveCoordinatorTests.cs
    TaskTests.cs
    FitnessTests.cs
    MealTests.cs
    GamingTests.cs
    JournalTests.cs
    SummaryTests.cs
    BackupTests.cs
    UndoTests.cs
    DesktopViewModelTests.cs
docs/
  design/page-spec.md
  decisions/desktop-architecture.md
  data-format.md
  user-guide.md
  acceptance.md
  release-checklist.md
artifacts/                          构建输出，不提交 Git
```

项目依赖：App → Core 与 Storage；Storage → Core；Tests → Core 与 Storage。需要测试界面状态时将测试目标设为 `net10.0-windows`，引用 App。Core 不引用 WPF；所有项目启用 nullable。迁移 SQL 作为嵌入资源打包，不能依赖开发目录中的文件。

## 3. 数据契约与操作规则

### 3.1 统一数据类型

每条有独立身份的业务记录使用 Guid。实体使用不可变 record，界面草稿独立持有输入文本，避免无效输入污染已保存实体。数值可选值使用 null，不用 0 代表未填。

| 实体 / SQLite 表 | 必要字段与约束 |
| --- | --- |
| DailyTask / daily_tasks | Id、Date、Title、Time?、Priority(normal/important)、IsCompleted；Title 去首尾空格后非空 |
| Workout / workouts | Id、Date、Title、StartTime?、PlannedMinutes?、IsRestDay、IsCompleted；V1 每天一条训练总安排，多个动作归其下；Date 唯一 |
| Exercise / exercises | Id、WorkoutId、Name、Kind(strength/duration)、SortOrder、PlannedSets?、PlannedReps?、PlannedWeightKg?、PlannedMinutes?、ActualMinutes?、IsCompleted |
| ExerciseSet / exercise_sets | Id、ExerciseId、SetNumber、ActualReps、ActualWeightKg?；动作内组号唯一 |
| Meal / meals | Id、Date、Slot(breakfast/lunch/dinner/snack)、PlannedText、ActualText、IsRecorded；Date+Slot 唯一 |
| WaterEntry / water_entries | Id、Date、Milliliters；大于 0 的整数 |
| GamePlan / game_plans | Id、Date、GameName、StartTime、PlannedMinutes；时长大于 0，结束展示由开始+时长推导 |
| GameSession / game_sessions | Id、Date、GameName、Mode(range/manual)、StartedAt?、EndedAt?、DurationMinutes；起止时间含日期及当时偏移量，区间模式计算正分钟数，手动模式起止为 null |
| JournalEntry / journals | Id、Date、Mood?、Summary、Gains、Improvements、UpdatedAt；Date 唯一 |
| UserSettings / settings | 固定一行，WeekStartsOn(monday/sunday) |
| schema_meta | 数据库版本整数；不是业务记录，不用于日常统计 |

所有依赖关系使用外键；删除训练级联删除动作和实际组，撤销时整体恢复。真实库不允许创建孤立组记录。以参数绑定执行 SQL，不拼接用户文本。

### 3.2 跨模块接口

以下为契约骨架，实施时放入相应文件，完整实体字段按上表声明。每项业务服务都只能通过 IDataStore 访问已保存数据。

```csharp
public enum SaveState { Saved, Saving, Invalid, Failed }
public enum PageId { Home, Tasks, Fitness, Meals, Gaming, Journal, Settings }
public enum WeekStart { Monday, Sunday }
public sealed record DateRange(DateOnly From, DateOnly Through);
public sealed record WriteReceipt(long Revision);
public sealed record ValidationIssue(string Field, string Message);

public interface IDataStore : IDisposable
{
    AppSnapshot ReadAll();
    WriteReceipt Upsert<T>(T entity) where T : class;
    WriteReceipt Delete<T>(Guid id) where T : class;
    WriteReceipt ReplaceAll(AppSnapshot snapshot);
}
```

`AppSnapshot` 包含表中九类业务实体的只读列表和一份 UserSettings；SQLite 元数据不放入列表。存储只支持上表白名单类型，不进行任意反射表名映射。单写队列运行在后台线程，WPF 更新在 UI 线程；SQLite 工作不用 UI 线程等待。

业务服务接口统一为 `GetForDate(DateOnly date)`、`Save(实体 entity)`、`Delete(Guid id)`；Save 返回 WriteReceipt，验证失败返回字段错误，存储错误保留原异常交由保存协调器转换为用户提示。MealService 另有 `CopyPlanToActual(Guid id)`；GamingService 另有 `GetWeekMinutes(DateOnly selectedDate, WeekStart weekStart)`；FitnessService 保存组和动作使用各自实体类型。恢复和删除复合实体需要在同一事务内完成，不通过多次独立 Upsert 模拟。

`SummaryService.Build(AppSnapshot snapshot, DateOnly date, WeekStart weekStart)` 返回 `DaySummary`：日常事项总数和完成数、按时间排序的模块入口、训练摘要、四餐状态、饮水合计、当天和本周游戏分钟数、日志状态及心情。模块入口携带 PageId 和 DateOnly，不新建 DailyTask。

### 3.3 自动保存和一致性

1. 文本编辑后 500 ms 防抖；勾选和已有效的数字操作立即排队。
2. 每次编辑递增本地版本；同一记录旧保存回执不得把新草稿标为已保存。
3. SaveCoordinator 按顺序调用存储，在事务提交成功后发出数据变化事件；首页、今日计划、日志重新查询相关日期。
4. 无效草稿保留在页面并显示字段错误。新条目未满足必填项时不落库。
5. 切换日期、页面、关闭编辑面板、关闭主窗口前调用 `FlushAsync()`；失败或无效时保留界面并提示。关闭窗口可选择重试或明确放弃未保存修改，不静默退出。
6. 恢复、清空、导出前先等待有效编辑完成。恢复/清空锁住写队列；成功后丢弃旧草稿和撤销缓存、重新读库，防止旧草稿回写。
7. 单实例限制避免同时打开两个 App 修改同一数据目录；第二次启动聚焦已有窗口。
8. 启动数据库损坏时不新建空库覆盖，提示现有文件路径并提供从备份恢复的入口。

### 3.4 备份格式

```json
{
  "formatVersion": 1,
  "exportedAt": "2026-09-13T10:00:00+08:00",
  "data": {
    "tasks": [], "workouts": [], "exercises": [], "exerciseSets": [],
    "meals": [], "waterEntries": [], "gamePlans": [],
    "gameSessions": [], "journals": [],
    "settings": { "weekStartsOn": "monday" }
  }
}
```

导入必须校验字段类型、Guid 唯一、日期、枚举、数值、训练父子关系、餐次及日志日期唯一性；区间游戏的 DurationMinutes 必须与起止计算一致。缺少数组不能默认为空来通过校验。文件不包含外部数据路径。预览与确认使用同一份已校验内存快照，不在确认时重新读取可能已改变的文件。

## 4. 逐项执行任务

所有命令从工作根目录运行。测试命令预期退出码为 0 且至少发现并通过指定测试，不能以“没有找到测试”视为通过。每个任务实现前先增加下列关键行为测试并确认失败，实现后跑对应测试；布局与文档使用人工审阅，不写重复验证静态文字的测试。每个任务通过后做一次本地提交，只提交该任务文件；不推送远程。

### 任务 1：确认页面规格

**文件：** 新建 `docs/design/page-spec.md`、`docs/decisions/desktop-architecture.md`。

**输入：** PRD 和已有七页可点击预览。**输出：** 页面字段、布局和原生桌面技术决策。

- [ ] 列出七页的区块、主要按钮和字段，保留 PRD 的基础功能；首页任务概况与四类摘要采用双列布局，窄窗口改为单列。
- [ ] 写出共用状态：空数据、正在保存、保存失败、字段无效、删除可撤销、恢复预览、恢复失败。
- [ ] 明确默认窗口 1200×820、最小 900×650；左导航约 180、右编辑面板约 360；所有长内容允许纵向滚动，150% 缩放仍可操作。
- [ ] 展示首页、训练逐组记录、恢复确认三个关键状态供用户检查，收敛对现有七页框架的修改。
- [ ] 记录用户对布局的确认后进入任务 2；此处来自 PRD 第 10 节的设计确认顺序，不重复询问已经确认的功能范围。

**完成条件：** 页面布局可作为实现依据，未混入新功能。设计偏好未回复时可完善本文的数据与测试细节，但不代替用户确认视觉定稿。

### 任务 2：桌面外壳与可运行工程

**文件：** 三个 src 项目、测试项目、解决方案、global.json、Directory.Build.props、App 和 MainWindow、Shell、Infrastructure、Shared、Themes。

**接口：** ShellViewModel 提供 `CurrentPage`、`SelectedDate`；`NavigateAsync(PageId page, DateOnly? date)` 在任务 4 接入保存屏障。

- [ ] 安装 .NET 10 SDK（开发环境），重新运行 `dotnet --list-sdks`，确认存在 10.0 系列并记录准确版本。检查 Windows 系统版本符合所选 SDK 的 WPF 运行支持。
- [ ] 建立工程与引用，执行下列命令（仅在开始实施后执行）。

```powershell
dotnet new sln -n PersonalLife
dotnet new classlib -n PersonalLife.Core -o src/PersonalLife.Core -f net10.0
dotnet new classlib -n PersonalLife.Storage -o src/PersonalLife.Storage -f net10.0
dotnet new wpf -n PersonalLife.App -o src/PersonalLife.App -f net10.0
dotnet new mstest -n PersonalLife.Tests -o tests/PersonalLife.Tests -f net10.0
dotnet sln PersonalLife.slnx add src/PersonalLife.Core/PersonalLife.Core.csproj src/PersonalLife.Storage/PersonalLife.Storage.csproj src/PersonalLife.App/PersonalLife.App.csproj tests/PersonalLife.Tests/PersonalLife.Tests.csproj
dotnet add src/PersonalLife.Storage reference src/PersonalLife.Core
dotnet add src/PersonalLife.App reference src/PersonalLife.Core src/PersonalLife.Storage
dotnet add tests/PersonalLife.Tests reference src/PersonalLife.Core src/PersonalLife.Storage
dotnet add src/PersonalLife.Storage package Microsoft.Data.Sqlite
dotnet restore PersonalLife.slnx --use-lock-file
dotnet build PersonalLife.slnx
```

- [ ] 把安装后解析的稳定包版本固定在 csproj 中；生成锁文件后，后续恢复使用 `dotnet restore PersonalLife.slnx --locked-mode`。App AssemblyName 设为 PersonalLife。
- [ ] 实现七个入口和共享日期条；所有模块先显示各自空状态，不用虚构记录填页面。
- [ ] 加上窗口最小尺寸、键盘焦点、日期前后切换、回到今天，禁止页面导航重置日期。
- [ ] 启动 `dotnet run --project src/PersonalLife.App`，检查独立窗口、七页标题和日期状态。

**验证：** A02、A03、A22 的基础布局；Build 无错误。此时不能宣称业务模块已经完成。

### 任务 3：主数据文件与存储事务

**文件：** Core/Domain、Core/Storage；Storage 的 DataPaths、SqliteDataStore、MigrationRunner、001_initial.sql；StorageTests、Support/TestStore。

**输入/输出：** 实现第 3 节 IDataStore、AppSnapshot 和实体；生产路径固定来源于 LocalApplicationData；测试通过显式构造路径注入临时目录，绝不接触正式数据目录。

- [ ] 写 `StorageTests.PersistAndReopen`：保存一条中文事项，释放存储后重新打开同一测试文件，断言名称、日期和 Guid 一致。
- [ ] 写 `StorageTests.RollbackFailedReplacement`：旧快照有事项甲，在导入含无效外键的快照时触发事务失败，断言甲仍存在，导入内容完全不存在。
- [ ] 实现表、唯一约束、外键和字段 CHECK，打开连接启用 foreign_keys；使用明确事务和 FULL 同步级别；迁移在事务内递增版本。
- [ ] 实现存储白名单类型映射、完整快照读取和事务式 ReplaceAll；不使用单独删除再逐条提交的恢复流程。
- [ ] 实现数据目录初始化、可定位错误信息、启动损坏检测；禁止失败时偷偷换到新空文件。
- [ ] 写 `StorageTests.MigrationDoesNotEraseData`，验证重复初始化和迁移失败不覆盖原记录。

```powershell
dotnet test tests/PersonalLife.Tests --filter FullyQualifiedName~StorageTests
```

**完成条件：** A13 的文件重开部分、A24 的路径部分，以及恢复回滚底座通过；重启电脑的人工验收留到任务 13。

### 任务 4：自动保存、草稿与安全导航

**文件：** SaveCoordinator.cs、SaveStatusView.xaml、ShellViewModel.cs、App.xaml.cs、MainWindow.xaml.cs、SingleInstance.cs、SaveCoordinatorTests.cs、Support/FaultingStore.cs。

**接口：** `Queue(string key, Func<WriteReceipt> write)`；`Task<bool> FlushAsync()`；状态、最新编辑版本与错误消息可订阅。FaultingStore 包装 IDataStore，在第 N 次写入抛 IOException，仅注入测试。

- [ ] 测试 `LatestEditWins`：快速输入三次，只允许最后值最终保留；旧回执不能将最新草稿标为 Saved。
- [ ] 测试 `FailureKeepsDraft`：注入写失败，断言 Failed 和原草稿保留，恢复存储后重试成功。
- [ ] 实现 500 ms 防抖、串行后台写入和 UI Dispatcher 状态更新；成功事件只在提交后发出。
- [ ] 实现导航和窗口关闭的 FlushAsync 屏障；处理无效必填字段、重试、明确放弃草稿三种分支。
- [ ] 测试 `NavigateWaitsForSave`、`CloseFailureDoesNotDiscard`，运行时手动检查立即切页和点关闭。
- [ ] 实现按数据目录区分的单实例锁，防止正式 App 重复写库；测试实例使用独立目录。

```powershell
dotnet test tests/PersonalLife.Tests --filter FullyQualifiedName~SaveCoordinatorTests
```

**完成条件：** A14、A15；用户未看到“已保存”前，不宣称最新输入已落盘。

### 任务 5：今日计划与普通删除撤销

**文件：** TaskService、TasksView、TasksViewModel、UndoService、UndoNotice、TaskTests、UndoTests。

**接口：** TaskService 的日期查询、保存、删除；UndoService 对删除前的完整实体快照生成 10 秒有效的撤销项，恢复使用同一 Guid。

- [ ] 测试添加中文事项、重要级别、完成与取消、跨日期修改；空标题拒绝保存。
- [ ] 实现快速添加及右侧编辑面板；时间为空的事项单独分组，已完成事项可折叠。
- [ ] 实现保存成功才显示完成结果；失败状态可重试，不能出现勾选与数据库长期不一致。
- [ ] 实现删除与 10 秒撤销通知；撤销过期关闭入口；撤销失败保留错误消息，不声称恢复成功。
- [ ] `UndoTests.RestoresSameId` 验证删除后恢复同一记录；`TaskTests.MoveDate` 验证旧日期消失、新日期出现。
- [ ] 为模块安排留出单独只读入口区，数据在任务 10 接入。

```powershell
dotnet test tests/PersonalLife.Tests --filter 'FullyQualifiedName~TaskTests|FullyQualifiedName~UndoTests'
```

**完成条件：** 事项增删改查真实保存，A04 模块侧、A20 普通删除部分、A21 标题校验通过。

### 任务 6：健身安排与实际训练

**文件：** FitnessService、FitnessView、FitnessViewModel、FitnessTests；扩展 UndoService 的训练复合删除处理。

- [ ] 测试力量动作计划 3×10，实际两组分别 10 和 8 次、10 和 12 kg，断言计划不变、两组独立保存。
- [ ] 实现训练主题、时间、预计时长、动作排序和休息日；改为休息日但已有训练记录时提示先处理现有记录，不自动删除。
- [ ] 实现力量组表格与按时长运动两种不同表单；各自字段不会互相强制要求。
- [ ] 实现动作与训练完成状态；允许实际组数超过计划，不用预填值冒充实际值。
- [ ] 删除训练时快照包含动作和组，事务删除，撤销时事务恢复；写 `FitnessTests.UndoRestoresChildren`。
- [ ] 查看历史日期，修改实际次数后重开 App 检查保留。

```powershell
dotnet test tests/PersonalLife.Tests --filter FullyQualifiedName~FitnessTests
```

**完成条件：** A06、A07；负数和错误类型输入被阻止，重量为空允许保存。

### 任务 7：餐食计划、实际记录与饮水

**文件：** MealService、MealsView、MealsViewModel、MealTests。

- [ ] 测试晚餐复制计划为实际后修改实际，断言计划保持原值。
- [ ] 实现四个餐次区域，每区独立计划、实际和记录状态；只有点击照计划或填写有效实际内容才标为已记录。
- [ ] 清空实际时同步重置记录状态；删除计划不删除已有实际，清空餐次整体记录需走可撤销删除。
- [ ] 实现饮水新增、数字修正、删除与撤销，日总量实时由记录求和。
- [ ] 测试 250+300=550，删除 250 后为 300；负数和零饮水输入阻止保存。

```powershell
dotnet test tests/PersonalLife.Tests --filter FullyQualifiedName~MealTests
```

**完成条件：** A08、A09；日期查询隔离，三餐没有虚构的用餐时间。

### 任务 8：娱乐安排与时长计算

**文件：** GamingService、DateRules、GamingView、GamingViewModel、GamingTests。

- [ ] 先写以下关键测试，首次执行应因缺少实现失败。

```csharp
[TestMethod]
public void CrossMidnightUsesStartDate()
{
    var start = new DateTimeOffset(2026, 9, 12, 23, 30, 0, TimeSpan.FromHours(8));
    var end = start.AddHours(1);
    Assert.AreEqual(60, DateRules.GameMinutes(start, end));
    Assert.AreEqual(new DateOnly(2026, 9, 12), DateRules.GameDate(start));
}
```

- [ ] 定义 DateRules.GameMinutes(DateTimeOffset start, DateTimeOffset end)，只接受正整数分钟差；GameDate 取开始时间携带偏移下的日期，不转 UTC 日期。
- [ ] 实现计划时段表单与实际时长表单，后者显式切换“起止时间”和“直接填分钟”；切换时清除非选用字段。
- [ ] 保存区间记录时以时间重新计算分钟数；同一记录只进入合计一次。开始日期归属规则显示在区间录入区。
- [ ] 测试 45+30=75，删除 30 后为45；测试星期一和星期日起始的周范围、跨月及跨年。
- [ ] 实现编辑、删除、撤销以及日/周统计，修改设置后重算周范围。

```powershell
dotnet test tests/PersonalLife.Tests --filter FullyQualifiedName~GamingTests
```

**完成条件：** A10、A11、A21 时间校验；不需要识别系统进程。

### 任务 9：总结日志

**文件：** JournalService、JournalView、JournalViewModel、JournalTests。

- [ ] 写 `JournalTests.OneEntryPerDate` 和 `JournalTests.PartialEntryPersists`：同一日期多次编辑不产生多条，只有一段总结也能保存。
- [ ] 实现心情、今日总结、收获、待改进四个可选字段；新日志完全为空不创建记录，已存在日志清空所有字段时明确显示空内容，删除走独立操作。
- [ ] 将文字修改接入 SaveCoordinator，更新时间在成功保存时生成。
- [ ] 实现按日期和历史列表进入日志，保留未保存文本；为当天足迹摘要预留只读区域。
- [ ] 删除日志提供撤销，恢复原 Guid 和日期，不覆盖后续新建的冲突日志；冲突时提示撤销失败。

```powershell
dotnet test tests/PersonalLife.Tests --filter FullyQualifiedName~JournalTests
```

**完成条件：** A12；自动保存和历史回看有效，足迹联动在任务 10 完成。

### 任务 10：首页汇总和跨模块联动

**文件：** SummaryService、HomeView、HomeViewModel、SummaryTests；修改 TasksViewModel、JournalViewModel。

- [ ] `SummaryTests.TasksOnlyInCompletionRate`：2 条日常事项，其中1条完成，加1个训练、1餐、1个娱乐安排，断言比例仍为1/2。
- [ ] 实现第 3.2 节 DaySummary；饮水和游戏求和、训练实际进度、四餐实际状态、日志存在状态均从快照计算。
- [ ] 按 PRD 首页六区显示数据；零事项时显示“今天尚无事项”，不显示除零百分比。
- [ ] 实现首页勾选调用 TaskService，摘要入口携带日期进入对应模块；今日计划显示训练、餐次和娱乐入口。
- [ ] 将实际摘要接入日志足迹区；业务提交、删除、撤销、恢复后统一刷新，禁止复制成新记录。
- [ ] 测试编辑饮水、删除娱乐、日志新增后的摘要更新；切换历史日期不混入今天的数据。

```powershell
dotnet test tests/PersonalLife.Tests --filter FullyQualifiedName~SummaryTests
```

**完成条件：** A04 首页部分、A05、A23，全模块联动闭环完成。

### 任务 11：完整备份、事务恢复、设置与清空

**文件：** BackupService、BackupFileWriter、SettingsView、SettingsViewModel、DesktopDialogs、BackupTests；修改 UndoService 和 SaveCoordinator。

**接口：** `BackupService.Export(string path)` 返回成功文件路径；`Preview(string path)` 返回校验后的 BackupPreview（快照、时间、各表数量）；`Restore(BackupPreview preview)` 原子替换；`Clear()` 原子清空业务表并恢复默认设置。用户取消文件对话框时不调用这些操作。

- [ ] 写跨多个日期、覆盖全部表的 round-trip 测试，比较导出前后全部业务数据，不只比较条数。
- [ ] 导出前 Flush，事务读一致性快照；写目标同目录临时文件、刷新到磁盘后再改名/替换；失败清理临时文件但保留旧目标。
- [ ] 实现文件选择和严格导入校验；预览各模块数量及导出时间，显示替换提示和“先导出当前数据”。
- [ ] 确认恢复后独占写队列，在一个事务中按外键顺序删除和插入，提交成功才清理草稿并刷新界面；失败回滚旧库。
- [ ] 写损坏 JSON、未知版本、缺少数组、重复 Guid、孤立训练组、错误时长的拒绝测试。
- [ ] 在测试中于删除旧表数据后、插入中途注入异常，验证旧数据及设置全部保留；模拟导出失败，不能显示成功。
- [ ] 实现一周起始日保存和打开数据文件夹；只打开由 DataPaths 生成的目录。
- [ ] 清空对话框明确包含全部记录和偏好；取消不操作；确认后事务清空、清除撤销缓存，不扫描或删除外部备份文件。

```powershell
dotnet test tests/PersonalLife.Tests --filter 'FullyQualifiedName~BackupTests|FullyQualifiedName~UndoTests'
```

**完成条件：** A16—A20、A24 全部通过，导入和清空的用户确认是产品功能，不是开发执行中的额外授权流程。

### 任务 12：打包与离线使用说明

**文件：** App.csproj、docs/user-guide.md、docs/data-format.md、docs/release-checklist.md、.gitignore。

- [ ] 本地打包前执行完整测试与 Release 构建，修复新出现的问题后只重跑相关测试及最终发布检查。
- [ ] 使用独立 artifacts 输出目录发布自包含版本，不覆盖源码和用户数据。

```powershell
dotnet restore PersonalLife.slnx --locked-mode
dotnet test PersonalLife.slnx -c Release
dotnet publish src/PersonalLife.App/PersonalLife.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -p:PublishTrimmed=false -o artifacts/PersonalLife-win-x64
Compress-Archive -LiteralPath artifacts/PersonalLife-win-x64 -DestinationPath artifacts/PersonalLife-win-x64.zip -Force
```

- [ ] 把 ZIP 解压到新的测试程序目录，双击 PersonalLife.exe；验证中文路径和带空格路径可启动，断网后七模块可用。
- [ ] 检查发布目录包含 SQLite 原生依赖；测试不是从开发工程启动。不要将 life.db、测试备份或日志正文打进发布包。
- [ ] 使用说明写清解压、打开 EXE、数据目录、保存状态、备份、恢复、更新程序和故障处理；说明移动程序目录不会移动主数据。
- [ ] 使用独立测试数据目录完成发布包重开验证，正式数据目录验收前不得清空或覆盖已有记录。

**完成条件：** 发布包可独立运行；无需用户安装 SDK 或 .NET，不把安装 SDK 的开发步骤写成用户操作要求。

### 任务 13：逐条验收与交付记录

**文件：** docs/acceptance.md、docs/release-checklist.md。

- [ ] 按下一节映射逐条执行 A01—A25，每条记录实际版本、操作、预期、实际结果、证据路径及通过/失败/未执行。
- [ ] 用独立测试数据完整录入一天，跨日期修改，再备份、改数据、恢复，复查每个模块和首页。
- [ ] 在当前电脑调整窗口尺寸及显示缩放，检查焦点、文本溢出、面板遮挡、保存错误和恢复确认的可理解性。
- [ ] 正式执行重启电脑验证须由用户安排或明确授权重启；未实际重启时将 A13 的重启部分标为未执行，不以关闭进程代替。
- [ ] 替换测试发布目录的程序文件，保留测试数据目录，验证 A25；不得递归移动或删除未核实路径。
- [ ] 将未通过项修复并复验；任何数据丢失、错误恢复、不能离线启动问题都阻止宣布第一版完成。
- [ ] 交付 ZIP、使用说明和验收记录；说明代码目录与运行包的区别。未实际验证的系统版本不宣称支持。

**完成条件：** 25 项均有证据，所有必需项通过；若只差用户安排的机器重启，明确报告“等待重启验收”，不报告完全验收通过。

## 5. PRD 验收覆盖表

| PRD 项 | 实施任务 | 验证方式 |
| --- | --- | --- |
| A01 | 2、12、13 | 发布包断网启动 |
| A02 | 2、4、10 | 日期/导航状态测试和桌面操作 |
| A03 | 2、5—11 | 空库逐页检查 |
| A04 | 5、10 | TaskTests、SummaryTests 和首页操作 |
| A05 | 10 | SummaryTests，不产生额外待办 |
| A06—A07 | 6 | FitnessTests 和训练录入 |
| A08—A09 | 7 | MealTests 和餐食/饮水操作 |
| A10—A11 | 8 | GamingTests、跨日和周边界 |
| A12 | 9 | JournalTests、立即切页回看 |
| A13 | 3、12、13 | 重开文件、发布包、实际电脑重启 |
| A14—A15 | 4 | SaveCoordinatorTests、故障注入 |
| A16—A19 | 11 | BackupTests、有效/无效导入和中途失败 |
| A20 | 5、6、11 | UndoTests、复合删除及清空取消 |
| A21 | 3、5—9 | 各模块验证测试和错误输入 |
| A22 | 1、2、13 | 窗口缩放和键盘操作 |
| A23 | 10 | SummaryTests、修改后真实汇总 |
| A24 | 3、11 | 设置页打开真实数据目录 |
| A25 | 12、13 | 保留测试数据并替换程序验收 |

## 6. 执行纪律与边界

- 任务复核：确认 PRD 覆盖、接口字段一致、对应测试发现并通过、没有演示数据落入真实库，再进入下一任务。
- 不为每个模块创建不同保存体系；统一事务、保存状态和日期规则。
- 测试库使用唯一临时目录，生产代码不得支持从导入文件任意指定数据库路径。
- 出现计划未覆盖的设计冲突，先在决策文档写出影响；不借机加入联网或账户等功能。
- 每个任务完成后报告“完成内容、验证证据、剩余问题”，不是只列修改了哪些文件。
- 本计划不包含发布网站或发送外部消息；所有提交和交付默认留在本机。
- 执行起点是任务 1。用户只要求计划时止于交付本文，不自行进入开发。
