# 木漏れ日-日语学习唯美手帐

轻量、离线、米白与鼠尾草绿的日语学习手帐。木漏れ日（こもれび）是透过树叶洒落的阳光；每天一个词，慢慢积累。

## 下载与安装

在 [Releases](https://github.com/ClementineTING/komorebi-japanese-journal/releases/latest) 下载 v1.1.0：

- **macOS 12+（Apple Silicon / Intel）**：`Komorebi-1.1.0-macOS-universal.dmg`，打开后将 Komorebi 拖入 Applications。
- **Windows 10 / 11 x64**：`Komorebi-1.1.0-Windows-x64-Setup.exe`，按向导安装。使用 .NET Framework 4.8 和 Microsoft WebView2；缺少 WebView2 时安装器会提示并联网安装微软运行时。
- **HTML 离线版**：`komorebi.html`，使用现代浏览器直接打开。

**签名说明：**macOS 构建暂为 ad-hoc 签名，未取得 Developer ID 签名和 Apple 公证；DMG 不会消除开发者验证提示。若确认来源可信，首次被阻止后请按 [Apple 官方说明](https://support.apple.com/102445)，在“系统设置 → 隐私与安全性”中为此应用选择“仍要打开”。公司管控设备可能不允许。Windows EXE 暂未代码签名，也可能出现 SmartScreen 提示。不提供关闭系统安全保护的脚本。

## 功能

- 内嵌 22,639 个 JMdict 常用日英词条（2026-09-07）。
- 20 张精选卡片附中文释义与原创例句；其他词条保留英文释义，不提供全量中文或例句覆盖。
- 搜索、收藏、显示/隐藏释义、每日目标、简单间隔复习、系统日语语音。
- **手动添加词汇、CSV 批量导入、自定义词库学习、CSV 导出。**
- 单词、列表和例句的汉字词显示平假名；新导入例句通过 Kuromoji / IPADIC 后台离线注音。未知读音保留原文；多音词与专名请结合语境核对。
- 搜索支持日语输入法确认与回车、平假名/片假名及全角/半角匹配。
- 学习、词汇手册与收藏支持 N5–N1 参考等级筛选，顶部显示当前等级总数和搜索匹配数。7,181 个内置词条匹配到参考等级，其余保留“未分级”；支持用户指定等级。
- HTML 约 28 MB，增加的主要部分是离线注音词典。分词在 Web Worker 中处理，初次加载和内存占用较 v1.0.0 增加。
- 无账号，无遥测，无云端同步。用户词库和学习记录仅保存在当前应用/浏览器的本地存储；清理存储会移除数据。HTML 与桌面版进度独立。
- 发音需要系统安装日语语音；是否支持离线朗读取决于语音包。

## 自定义词库

侧栏选择“自定义词库”。可下载 CSV 模板，也可以手动添加。

| 单词（必填） | 假名（必填） | 释义（必填） | 词性 | 例句 | 例句翻译 |
|---|---|---|---|---|---|
| 桜 | さくら | 樱花 | 名词 | 桜が咲いています。 | 樱花正在盛开。 |

Excel 请选择“CSV UTF-8（逗号分隔）”。首行列名使用上表名称，顺序可调整；选填列可省略，可另加“等级”列（N5–N1 或留空），用户指定等级会显示“自设”。含逗号、换行或双引号的值使用标准 CSV 引号转义。

上传后会检查格式并显示预览，点击“导入并保存”写入本地。错误会给出行号，整批不写入。按“单词 + 假名”跳过重复自定义词，不修改内置词典。

单次最多 5,000 行、2 MB，累计最多 10,000 个自定义词；实际可保存量也受浏览器存储额度限制，存储失败会明确提示且不部分导入。导入成功后可在词汇手册筛选“我的词库”，也可点击“学习我的词库”。导出 CSV 可以在另一台设备重新导入词汇；学习进度 JSON 暂仅支持导出。

## 从源码运行和构建

HTML：直接打开 `web/index.html`。

macOS：安装 Apple Command Line Tools，然后执行：

```sh
bash scripts/build-macos.sh
```

输出为 `build/Komorebi-1.1.0-macOS-universal.dmg`。构建脚本不会下载第三方依赖，使用 Swift、AppKit、WKWebView 和系统打包工具。

CSV 解析与写入测试（Node.js）：

```sh
npm ci
npm run build
npm test
```

词典更新（Python 3，需要网络）：

```sh
python3 scripts/update-dictionary.py
```

更新脚本从 jmdict-simplified 最新 Release 下载常用词库，重新生成内嵌 HTML。维护者应定期运行并在发版前检查数据日期、数量、许可说明和界面。界面源文件是 `web/template.html`，词条位于 `data/vocabulary.json`，后台注音入口为 `web/furigana-worker.js`。`npm run build` 生成可独立打开的 `web/index.html`。

## 许可与致谢

应用代码采用 MIT。词典与派生词汇数据采用 CC BY-SA 4.0。

JMdict © James William BREEN and the Electronic Dictionary Research and Development Group (EDRDG)。通过 [jmdict-simplified](https://github.com/scriptin/jmdict-simplified) 获取 `jmdict-eng-common-3.6.2+20260907165411.json.tgz`；此应用提取主要词形、读音、英文释义、词性和搜索别名，并对 20 个词添加中文与原创例句。

- [EDRDG 许可说明](https://www.edrdg.org/edrdg/licence.html)，本地副本 `licenses/EDRDG-LICENCE.html`
- [JMdict 项目文档](https://www.edrdg.org/wiki/JMdict-EDICT_Dictionary_Project.html)，本地副本 `licenses/JMdict-documentation.html`
- [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/)

个人导入内容不会随源码或安装包发布。请在分享自行整理的词库时遵守其原始来源许可。

## Windows 构建

需要 Windows、.NET 8 SDK（用于编译 net48）、Inno Setup 6 和网络。运行 `powershell -File scripts/build-windows.ps1`，输出 `build/Komorebi-1.1.0-Windows-x64-Setup.exe`。微软运行时引导程序下载后会校验 Authenticode 签名。安装和应用数据使用当前用户目录，卸载保留用户学习数据。

GitHub Actions 的 `Build desktop installers` 工作流构建两平台安装包，并验证 DMG 挂载/应用启动，以及 Windows 安装、内嵌词典、离线注音引擎初始化、搜索、自定义导入/去重/本地保存和卸载。

## 注音与分级来源

- Kuromoji 0.1.2：Apache-2.0；IPADIC：NAIST/ICOT 数据许可。
- JLPT 等级：OpenJLPT，固定版本 `c42fd9fa3777bfc1775446f7c418d549dfd6e4cf`，上游 Jonathan Waller / Tanos。仅使用词形、读音、等级；未引入其例句。
- N5–N1 是第三方参考分级，不是官方完整考纲。内置词条数量：N5 676、N4 612、N3 1,648、N2 1,648、N1 2,597，未分级 15,458。
- 详细声明见 `licenses/FURIGANA-JLPT-NOTICES.txt`，也内嵌于学习设置页面。
