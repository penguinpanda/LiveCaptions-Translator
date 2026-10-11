# 设置项说明

本文档逐项说明 LiveCaptions Translator 的所有可配置项：它在哪里、是什么意思、默认值是多少、取值范围如何。

> 文档中的名称均对应界面上的英文文案。配置文件为程序工作目录下的 `setting.json`，**建议通过界面修改**，程序会在每次改动后自动保存。

## 目录

- [1. 设置在哪里](#1-设置在哪里)
- [2. 主窗口任务栏按钮](#2-主窗口任务栏按钮)
- [3. Setting 页（主设置页）](#3-setting-页主设置页)
- [4. 字幕页（Caption）交互](#4-字幕页caption交互)
- [5. 悬浮窗（Overlay Window）控制面板](#5-悬浮窗overlay-window控制面板)
- [6. API Setting 窗口](#6-api-setting-窗口)
- [7. setting.json 字段总览](#7-settingjson-字段总览)
- [8. 注意事项与已知限制](#8-注意事项与已知限制)

---

## 1. 设置在哪里

| 入口 | 位置 | 说明 |
|---|---|---|
| **Setting 页** | 主窗口左侧导航 `Setting` | 日常使用的核心设置 |
| **API Setting 窗口** | Setting 页 → `API Setting` → `Open` | 提示词与各翻译服务的密钥、地址等 |
| **Overlay Window 控制面板** | 悬浮窗左下角一排图标按钮 | 悬浮字幕的外观与行为 |
| **主窗口标题栏按钮** | 主窗口右上角四个图标 | 日志卡片、暂停、悬浮窗、置顶 |
| **字幕页交互** | `Caption` 页文本区域 | 点击复制、`Ctrl+滚轮` 缩放字号 |

---

## 2. 主窗口任务栏按钮

位于主窗口标题栏右侧，从左到右：

| 按钮 | 作用 | 对应配置字段 | 默认值 |
|---|---|---|---|
| **Log Cards**（历史图标） | 在字幕页下方显示最近几条「原文 + 译文」卡片，帮助把握上下文。**该按钮仅在 `Caption` 页显示**，切到其他页时会隐藏 | `MainWindow.CaptionLogEnabled` | `false`（关） |
| **Pause (Log Only)**（暂停图标） | 暂停翻译：实时字幕继续记录到历史，但不再调用翻译 API | 运行时状态，不持久化 | — |
| **Overlay Window**（字幕图标） | 打开/关闭悬浮字幕窗口 | 运行时状态，不持久化 | — |
| **Always on Top**（图钉图标） | 主窗口是否置顶 | `MainWindow.Topmost` | `true`（开） |

---

## 3. Setting 页（主设置页）

主窗口导航到 `Setting`，界面从左到右共五列。

### 3.1 LiveCaptions（显示/隐藏实时字幕）

| 项 | 说明 |
|---|---|
| 控件 | `Show` / `Hide` 按钮 |
| 作用 | 显示或隐藏 Windows 实时字幕窗口本身 |

> ⚠️ **注意**：Windows 11 24H2 之后，实时字幕的**源语言**只能在实时字幕自己的界面里改。
> 另外请务必点击 `Hide` 隐藏实时字幕，**不要直接用 X 关闭它**。

### 3.2 API Interval（翻译调用频率）

| 项 | 值 |
|---|---|
| 字段 | `MaxSyncInterval` |
| 默认值 | `3` |
| 范围 | `0 ~ 10`（滑块） |
| 含义 | 原文每变化 N 次，才调用一次翻译 API。**值越小调用越频繁**，越接近实时，但 API 消耗越大 |

内部还有一个不可调的空闲间隔 `MaxIdleInterval = 50`：当原文长时间不变时，累计 50 次轮询后强制同步一次。

### 3.3 Translate API（选择翻译服务）

| 项 | 值 |
|---|---|
| 字段 | `ApiName` |
| 默认值 | `Google` |
| 可选值 | `Google`、`Google2`、`Ollama`、`OpenAI`、`OpenRouter`、`LMStudio`、`DeepL`、`Youdao`、`Baidu`、`MTranServer`、`LibreTranslate` |

除 `Google` / `Google2` 外，**其他服务都需要先在 API Setting 窗口配置**才能使用。

按类型可分为：

- **LLM 类**（支持上下文、更擅长不完整句子）：`Ollama`、`OpenAI`、`OpenRouter`、`LMStudio`
- **传统机器翻译类**：`Google`、`Google2`、`DeepL`、`Youdao`、`Baidu`、`MTranServer`、`LibreTranslate`

### 3.4 Target Language（目标语言）

| 项 | 值 |
|---|---|
| 字段 | `TargetLanguage` |
| 默认值 | `zh-CN` |
| 输入方式 | 下拉选择，且**可直接编辑**自定义 |

内置语言标签遵循 **BCP 47**（如 `zh-CN`、`en-US`、`ja-JP`）。内置语言已做好各服务商的标签映射，无需担心；若你填了列表之外的语言，则需要自己按服务商要求填写（例如 DeepL 用 `ZH-HANS`、有道用 `zh-CHS`、百度用 `zh`）。详见 [6.9 各服务的目标语言映射](#69-各服务的目标语言映射)。

### 3.5 API Setting（打开设置窗口）

`Open` 按钮，打开 [第 6 节](#6-api-setting-窗口) 所述的 API Setting 窗口。

### 3.6 Show Latency（显示延迟）

| 项 | 值 |
|---|---|
| 字段 | `MainWindow.LatencyShow` |
| 默认值 | `false` |
| 含义 | 在译文旁显示本次翻译的耗时，便于判断 API 快慢 |

### 3.7 Contexts（上下文句数）

| 项 | 值 |
|---|---|
| 字段 | `NumContexts` |
| 默认值 | `2` |
| 范围 | `1 ~ 10` |
| 含义 | 开启 `Context Aware` 后，作为上下文一并送入 LLM 的历史句数 |

### 3.8 Display Sentences（显示句数）

| 项 | 值 |
|---|---|
| 字段 | `DisplaySentences` |
| 默认值 | `1` |
| 范围 | `0 ~ 6` |
| 含义 | 开启 `Log Cards` 时显示的卡片数量，同时决定悬浮窗最多显示多少句 |

> ⚠️ **约束**：`Contexts` 必须 **≥** `Display Sentences`。若不满足，程序会自动调整。

### 3.9 Context Aware（上下文感知）

| 项 | 值 |
|---|---|
| 字段 | `ContextAware` |
| 默认值 | `false` |
| 含义 | 把最近若干句「原文 + 译文」作为对话历史发给 LLM，提高翻译准确度和连贯性 |
| 代价 | 消耗更多 token、单次请求更慢 |

> 仅对 **LLM 类**服务有效（`Ollama` / `OpenAI` / `OpenRouter` / `LMStudio`）。

---

## 4. 字幕页（Caption）交互

| 操作 | 效果 |
|---|---|
| **单击**原文或译文 | 复制该段文本 |
| **Ctrl + 滚轮** | 调整字号，范围 `8 ~ 40`，步长 `1` |

字号分别保存为：

| 字段 | 含义 | 默认值 |
|---|---|---|
| `MainWindow.OriginalFontSize` | 原文（字幕）字号 | `15` |
| `MainWindow.TranslatedFontSize` | 译文（翻译）字号 | `18` |

---

## 5. 悬浮窗（Overlay Window）控制面板

鼠标移入悬浮窗时，右下角会出现一排图标按钮。它们修改 `Setting.OverlayWindow`。

| 按钮 | 作用 | 字段 | 默认值 | 取值范围 / 步长 |
|---|---|---|---|---|
| **Font Size Increase / Decrease** | 悬浮窗字号加减 | `OverlayWindow.FontSize` | `15` | `8 ~ 40`，步长 `1` |
| **Font Stroke Increase / Decrease** | 文字描边粗细（提升复杂背景下的可读性） | `OverlayWindow.FontStroke` | `0.0` | `0 ~ 7.5`，步长 `1.5` |
| **Font Bold** | 加粗模式，循环切换：`None` → `TranslationOnly` → `SubtitleOnly` → `Both` | `OverlayWindow.FontBold` | `None` | 枚举，见下 |
| **Font Color** | 文字颜色，循环切换 | `OverlayWindow.FontColor` | `White` | 8 色循环 |
| **Background Opacity Increase / Decrease** | 背景不透明度 | `OverlayWindow.Opacity` | `150` | `1 ~ 251`，步长 `25` |
| **Background Color** | 背景颜色，循环切换 | `OverlayWindow.BackgroundColor` | `Black` | 8 色循环 |
| **Only Mode** | 只显示译文 / 只显示字幕 / 两者都显示，循环切换 | 会话内状态 | 两者 | 不持久化 |
| **Switch Mode** | 交换「字幕」与「译文」的上下顺序 | 会话内状态 | 译文在上 | 不持久化 |
| **Click Through** | 鼠标穿透：悬浮窗不再拦截鼠标，可点击其下方的窗口（游戏/视频场景必备） | 会话内状态 | 关 | 不持久化 |

**可循环选择的颜色**（8 种）：

`White` → `Yellow` → `LimeGreen` → `Aqua` → `Blue` → `DeepPink` → `Red` → `Black` → 回到 `White`

**Font Bold 四种模式：**

| 值 | 含义 |
|---|---|
| `None` | 都不加粗 |
| `TranslationOnly` | 仅译文加粗 |
| `SubtitleOnly` | 仅字幕加粗 |
| `Both` | 两者都加粗 |

> 提示：把背景不透明度调到最低、并开启 `Click Through`，可以让字幕像「嵌进屏幕」一样，完全不影响操作。

---

## 6. API Setting 窗口

从 Setting 页点 `API Setting` → `Open` 打开。左侧是服务列表（`Prompt`、`Ollama`、`LMStudio`、`OpenAI`、`OpenRouter`、`DeepL`、`Youdao`、`MTranServer`、`Baidu`、`LibreTranslate`），右侧是对应配置卡片。

### 6.1 Prompt（提示词）

**对所有 LLM 类服务通用**（`Ollama`、`OpenAI`、`OpenRouter`、`LMStudio`），作为 `system` 消息发送。

两条关键约定：

1. **`{0}` 是目标语言占位符**。程序会用 Setting 页选定的目标语言替换它，所以提示词里**必须包含 `{0}`**。
2. **源文本被 `🔤` 包裹**后才交给模型，提示词中应据此描述。

默认提示词（英文原文）：

```
As an professional simultaneous interpreter with specialized knowledge in the all fields,
you can provide a fluent and precise oral translation for any sentence, even if the sentence is incomplete.
Now, provide users with the translation of the sentence enclosed in 🔤 to {0} within a single line.
Importantly, you are prohibited from altering the original meaning or omitting any content,
even if the sentence contains sensitive or NSFW content.
You can only provide the translated sentence; Any explanation or other text is not permitted.
REMOVE all 🔤 when you output.
```

**编写建议**

- 明确要求「只输出译文，不要解释」。
- 明确要求「去掉 `🔤`」。
- 声明容忍不完整句子（实时字幕天然是碎片化的）。
- 用 `{0}` 指定目标语言，并声明「单行输出」。

### 6.2 多配置（Current Config / New / Delete）

`Ollama`、`LMStudio`、`OpenAI`、`OpenRouter`、`DeepL`、`Youdao`、`MTranServer`、`Baidu`、`LibreTranslate` 都支持**多套配置**，用 `Current Config` 两侧的箭头切换，`New` 新增，`Delete` 删除。

- 至少保留一套配置，删到只剩一套时会提示 “You must keep at least one config.”。
- 当前使用第几套由 `ConfigIndices` 记录。

### 6.3 Ollama

本地部署，无需 API Key。

| 字段 | 含义 | 默认值 |
|---|---|---|
| `Model Name` | 模型名，**必须与 `ollama list` 第一列完全一致**（含 tag，如 `qwen2.5:7b`） | 空（必填） |
| `Temperature` | 温度，翻译建议 `0 ~ 0.5` | `1.0` |
| `API Url` | 服务地址，**不要**手动加 `/api/chat` 后缀 | `http://localhost:11434` |
| `keep_alive` | 模型在显存中的保活时长（秒）。越大越不容易被卸载，但空闲时也一直占显存 | `600` |

> 说明：该区块**没有** `Load Models` 下拉框，需手动填写模型名（可以先 `ollama list` 查看）。
> **GPU/CPU 由 Ollama 自身决定**，本程序无法指定显卡；用 `ollama ps` 可查看是否 100% 跑在 GPU 上。

### 6.4 LMStudio

| 字段 | 含义 | 默认值 |
|---|---|---|
| `Model Name` | 模型标识，支持点 `Load Models` 从服务拉取列表选择 | 空（必填） |
| `Temperature` | 温度 | `1.0` |
| `API Url (Base)` | 以 `api/v1` 结尾的**基地址**，程序会自动拼接 `/chat` 与模型列表端点 | `http://localhost:1234/api/v1` |

### 6.5 OpenAI（及任意 OpenAI 兼容接口）

| 字段 | 含义 | 默认值 |
|---|---|---|
| `Model Name` | 模型名，如 `gpt-4o-mini` | 空（必填） |
| `Temperature` | 温度 | `1.0` |
| `API Url` | **完整地址**，一般以 `v1/chat/completions` 结尾 | 空（必填） |
| `API Key` | 密钥 | 空（必填） |

> 注意这里是**完整 URL**，不是 Base URL。第三方兼容接口（如 DeepSeek、硅基流动、自建网关）填各自完整的 chat completions 地址即可。

### 6.6 OpenRouter

| 字段 | 含义 | 默认值 |
|---|---|---|
| `Model Name` | 模型名，如 `anthropic/claude-3.5-sonnet` | 空（必填） |
| `Temperature` | 温度 | `1.0` |
| `API Key` | OpenRouter 密钥 | 空（必填） |

地址内置为 OpenRouter 官方端点，界面不提供修改。

### 6.7 DeepL

| 字段 | 含义 | 默认值 |
|---|---|---|
| `API Url` | 接口地址 | `https://api.deepl.com/v2/translate` |
| `API Key` | DeepL 密钥 | 空（必填） |

> 免费版密钥需把地址改为 `https://api-free.deepl.com/v2/translate`。

### 6.8 其余服务的一览

| 服务 | 字段 | 默认值 |
|---|---|---|
| **Youdao** | `API Url` | `https://openapi.youdao.com/api` |
| | `APP Key` | 空（必填） |
| | `APP Secret` | 空（必填） |
| **Baidu** | `API Url` | `https://fanyi-api.baidu.com/api/trans/vip/translate` |
| | `APP ID`（字段 `AppId`） | 空（必填） |
| | `APP Secret`（字段 `AppSecret`） | 空（必填） |
| **MTranServer** | `Source Language` | `en`（可选 `en`/`ja`/`fr`/`ko`/`de`，可编辑） |
| | `API Url` | `http://localhost:8989/translate` |
| | `API Key` | 空（可选） |
| **LibreTranslate** | `API Url` | `http://localhost:5000/translate` |
| | `API Key` | 空（可选） |
| **Google / Google2** | 无 | 开箱即用，无需配置 |

> `MTranServer` 的 `Source Language` 是**源语言**（非目标语言）；目标语言仍在 Setting 页设置。

### 6.9 各服务的目标语言映射

Setting 页选了 BCP 47 标签后，程序按各服务要求自动转换：

| 目标语言 | DeepL | Youdao | Baidu | MTranServer / LibreTranslate |
|---|---|---|---|---|
| `zh-CN` | `ZH-HANS` | `zh-CHS` | `zh` | `zh` |
| `zh-TW` | `ZH-HANT` | `zh-CHT` | `cht` | `zh` |
| `en-US` / `en-GB` | `EN-US` / `EN-GB` | `en` | `en` | `en` |
| `ja-JP` | `JA` | `ja` | `jp` | `ja` |
| `ko-KR` | `KO` | `ko` | `kor` | `ko` |
| `fr-FR` | `FR` | `fr` | `fra` | `fr` |
| `th-TH` | `TH` | `th` | `th` | `th` |
| `ru-RU` | `RU` | `ru` | `ru` | `ru` |
| `es-ES` | `ES` | `es` | `spa` | `es` |
| `pt-BR` | `PT-BR` | `pt` | `pt` | `pt` |
| `tr-TR` | `TR` | `tr` | `tr` | `tr` |
| `ar-SA` | `AR` | `ar` | `ara` | `ar` |

LLM 类服务（Ollama / OpenAI / OpenRouter / LMStudio）不映射，直接把标签原样传给模型。

内置语言列表：`zh-CN`、`zh-TW`、`en-US`、`en-GB`、`ja-JP`、`ko-KR`、`fr-FR`、`th-TH`、`ru-RU`、`es-ES`、`pt-BR`、`tr-TR`、`ar-SA`。

---

## 7. setting.json 字段总览

配置文件：程序工作目录下的 `setting.json`（保存时缩进格式化）。若解析失败，会自动改名成 `setting.json.bak` 并重建默认配置。

### 顶层字段

| 字段 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ApiName` | string | `"Google"` | 当前翻译服务 |
| `TargetLanguage` | string | `"zh-CN"` | 目标语言 |
| `Prompt` | string | 见 [6.1](#61-prompt提示词) | LLM 提示词模板 |
| `MaxSyncInterval` | int | `3` | API Interval，`0 ~ 10` |
| `MaxIdleInterval` | int | `50` | 空闲强制同步阈值（只读，无界面） |
| `NumContexts` | int | `2` | Contexts，`1 ~ 10` |
| `DisplaySentences` | int | `1` | Display Sentences，`0 ~ 6` |
| `ContextAware` | bool | `false` | 上下文感知 |
| `IgnoredUpdateVersion` | string? | `null` | 已忽略的更新版本号 |
| `WindowBounds` | map | 屏幕居中 | 各窗口位置尺寸，格式 `"X, Y, W, H"` |
| `Configs` | map | — | 各服务的配置集合（每项是一个数组，支持多套） |
| `ConfigIndices` | map | 全 `0` | 各服务当前使用第几套配置 |

### MainWindow

| 字段 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Topmost` | bool | `true` | 主窗口置顶 |
| `CaptionLogEnabled` | bool | `false` | 显示日志卡片 |
| `LatencyShow` | bool | `false` | 显示翻译延迟 |
| `OriginalFontSize` | int | `15` | 原文字号 |
| `TranslatedFontSize` | int | `18` | 译文字号 |

### OverlayWindow

| 字段 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `FontSize` | int | `15` | 悬浮窗字号 |
| `FontColor` | enum | `White` | 文字颜色 |
| `FontBold` | enum | `None` | 加粗模式 |
| `FontStroke` | double | `0.0` | 描边粗细 |
| `BackgroundColor` | enum | `Black` | 背景色 |
| `Opacity` | int | `150` | 背景不透明度 |

### 各服务配置字段

| 服务 | 字段（默认值） |
|---|---|
| `Ollama` | `ModelName`(`""`)、`Temperature`(`1.0`)、`ApiUrl`(`http://localhost:11434`)、`keep_alive`(`600`) |
| `LMStudio` | `ModelName`(`""`)、`Temperature`(`1.0`)、`ApiUrl`(`http://localhost:1234/api/v1`) |
| `OpenAI` | `ModelName`(`""`)、`Temperature`(`1.0`)、`ApiUrl`(`""`)、`ApiKey`(`""`) |
| `OpenRouter` | `ModelName`(`""`)、`Temperature`(`1.0`)、`ApiKey`(`""`) |
| `DeepL` | `ApiUrl`(`https://api.deepl.com/v2/translate`)、`ApiKey`(`""`) |
| `Youdao` | `ApiUrl`(`https://openapi.youdao.com/api`)、`AppKey`(`""`)、`AppSecret`(`""`) |
| `Baidu` | `ApiUrl`(`https://fanyi-api.baidu.com/api/trans/vip/translate`)、`AppId`(`""`)、`AppSecret`(`""`) |
| `MTranServer` | `ApiUrl`(`http://localhost:8989/translate`)、`ApiKey`(`""`)、`SourceLanguage`(`en`) |
| `LibreTranslate` | `ApiUrl`(`http://localhost:5000/translate`)、`ApiKey`(`""`) |
| `Google` / `Google2` | 无配置项 |

---

## 8. 注意事项与已知限制

1. **翻译请求超时为 8 秒**（在代码中硬编码）。本地大模型（如 Ollama 的 12B 模型）冷启动或生成较慢时会直接报错：
   `The request was canceled due to timeout (> 8 seconds), please use a faster API or check network connection.`
   缓解办法：
   - 先用 `ollama run <model> "hi"` 预热，把模型加载进显存；
   - `keep_alive` 保持较大值，避免模型被卸载；
   - 选用更小/更快的模型（3B 级别或更小量化）；
   - `ollama ps` 确认是否 100% 跑在 GPU 上，显存不足会退化为 CPU 推理。

2. **上下文感知会显著变慢**。`ContextAware` 打开后，每次请求都会带上历史对话，token 与耗时同步上升。

3. **`Contexts` ≥ `Display Sentences`**，否则程序会自动纠正。

4. **悬浮窗的 `Only Mode` / `Switch Mode` / `Click Through` 不做持久化**，重启程序后恢复默认。

5. **实时字幕设置不可跳过**：首次使用前必须在实时字幕里把「位置」设为「覆盖在屏幕上」，否则隐藏字幕后屏幕会出现显示残留。

6. **Windows 11 24H2 及以后**，实时字幕的源语言只能在实时字幕自己的界面修改，本程序无法代改。
