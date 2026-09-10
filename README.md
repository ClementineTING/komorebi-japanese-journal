# 木漏れ日-日语学习唯美手帐

轻量、离线、米白与鼠尾草绿的日语学习手帐。木漏れ日（こもれび）是透过树叶洒落的阳光；每天一个词，慢慢积累。

## 下载与安装

在 [Releases](https://github.com/ClementineTING/komorebi-japanese-journal/releases/latest) 下载：

- **macOS 12+（Apple Silicon / Intel）**：`Komorebi-1.0.0-macOS-universal.pkg`，双击安装到 Applications；也可下载 ZIP，解压后将 Komorebi 拖入 Applications。
- **Windows / Linux / 其他设备**：下载 `komorebi.html`，用现代浏览器直接打开。当前尚未提供 Windows 或 Linux 原生安装包。

macOS 版本使用系统 WebKit，不捆绑浏览器。当前采用本地临时签名，**尚未取得 Apple Developer ID 签名与公证**，首次打开可能被 Gatekeeper 拦截。请核对本仓库来源和 SHA256；若信任此构建，可按 macOS 的“隐私与安全性”提示允许打开，或自行从源码构建。没有修改系统安全设置的安装脚本。

## 功能

- 内嵌 22,639 个 JMdict 常用日英词条（2026-09-07）。
- 20 张精选卡片附中文释义与原创例句；其他词条保留英文释义，没有声称全量中文或 JLPT 分级覆盖。
- 搜索、收藏、显示/隐藏释义、每日目标、简单间隔复习、系统日语语音。
- **手动添加词汇、CSV 批量导入、自定义词库学习、CSV 导出。**
- 无账号，无遥测，无云端同步。用户词库和学习记录仅保存在当前应用/浏览器的本地存储；清理存储会移除数据。HTML 与桌面版进度独立。
- 发音需要系统安装日语语音；是否支持离线朗读取决于语音包。

## 自定义词库

侧栏选择“自定义词库”。可下载 CSV 模板，也可以手动添加。

| 单词（必填） | 假名（必填） | 释义（必填） | 词性 | 例句 | 例句翻译 |
|---|---|---|---|---|---|
| 桜 | さくら | 樱花 | 名词 | 桜が咲いています。 | 樱花正在盛开。 |

Excel 请选择“CSV UTF-8（逗号分隔）”。首行列名使用上表名称，顺序可调整；后三列可省略。含逗号、换行或双引号的值使用标准 CSV 引号转义。

上传后会检查格式并显示预览，点击“导入并保存”写入本地。错误会给出行号，整批不写入。按“单词 + 假名”跳过重复自定义词，不修改内置词典。

单次最多 5,000 行、2 MB，累计最多 10,000 个自定义词；实际可保存量也受浏览器存储额度限制，存储失败会明确提示且不部分导入。导入成功后可在词汇手册筛选“我的词库”，也可点击“学习我的词库”。导出 CSV 可以在另一台设备重新导入词汇；学习进度 JSON 暂仅支持导出。

## 从源码运行和构建

HTML：直接打开 `web/index.html`。

macOS：安装 Apple Command Line Tools，然后执行：

```sh
bash scripts/build-macos.sh
```

输出为 `build/Komorebi-1.0.0-macOS-universal.pkg`。构建脚本不会下载第三方依赖，使用 Swift、AppKit、WKWebView 和系统打包工具。

CSV 解析与写入测试（Node.js）：

```sh
node scripts/test-import.cjs
```

词典更新（Python 3，需要网络）：

```sh
python3 scripts/update-dictionary.py
```

更新脚本从 jmdict-simplified 最新 Release 下载常用词库，重新生成内嵌 HTML。维护者应定期运行并在发版前检查数据日期、数量、许可说明和界面。界面源文件是 `web/template.html`；`scripts/custom-source.js` 是对应导入逻辑的测试副本，修改时应同步。

## 许可与致谢

应用代码采用 MIT。词典与派生词汇数据采用 CC BY-SA 4.0。

JMdict © James William BREEN and the Electronic Dictionary Research and Development Group (EDRDG)。通过 [jmdict-simplified](https://github.com/scriptin/jmdict-simplified) 获取 `jmdict-eng-common-3.6.2+20260907165411.json.tgz`；此应用提取主要词形、读音、英文释义、词性和搜索别名，并对 20 个词添加中文与原创例句。

- [EDRDG 许可说明](https://www.edrdg.org/edrdg/licence.html)，本地副本 `licenses/EDRDG-LICENCE.html`
- [JMdict 项目文档](https://www.edrdg.org/wiki/JMdict-EDICT_Dictionary_Project.html)，本地副本 `licenses/JMdict-documentation.html`
- [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/)

个人导入内容不会随源码或安装包发布。请在分享自行整理的词库时遵守其原始来源许可。
