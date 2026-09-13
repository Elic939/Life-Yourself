# Life-Yourself · 

Windows 本地生活管理 App。数据保存在本机文件，不需要登录或浏览器。

- 产品需求：[PRD.md](PRD.md)
- 开发计划：[开发执行计划](docs/superpowers/plans/2026-09-13-life-app-development.md)
- 使用方法：[使用说明](docs/user-guide.md)
- 数据说明：[数据格式](docs/data-format.md)
- 验收状态：[验收记录](docs/acceptance.md)

## 开发者

需要 global.json 指定的 .NET 10 SDK。执行 `powershell -File scripts/dev.ps1 test` 运行全部测试；`build` 构建，`publish` 生成自包含发布目录。脚本优先使用此前安装在 LocalApplicationData/PersonalLifeDev/dotnet 的 SDK。

普通使用者只需打开发布目录里的 `PersonalLife.exe`，不需要执行上述命令。
