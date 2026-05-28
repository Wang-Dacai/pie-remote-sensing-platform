# PIE 遥感服务平台

这是一个基于 C# WinForms 和 PIE SDK 的遥感服务平台桌面应用，源自 2022 年 PIE 开发者大赛项目。

## 项目概览

| 项目 | 内容 |
|---|---|
| 仓库定位 | 遥感/GIS 桌面应用工程作品 |
| 开发语言 | C# |
| 应用类型 | Windows Forms |
| 目标框架 | .NET Framework 4.5 |
| 核心 SDK | PIE SDK 6.3 |
| 主要入口 | `PieRemoteSensingPlatform.sln` |
| 开源协议 | MIT License |

## 核心能力

| 模块 | 能力 |
|---|---|
| 数据管理 | 加载矢量、栅格、HDF、NC 等遥感/GIS 数据，并在地图控件中展示 |
| 地图浏览 | 图层树、右键菜单、放大、缩小、漫游、全图、图层属性、图层删除 |
| 在线底图 | 预留天地图矢量、影像与注记底图接入入口 |
| 预处理 | 辐射定标、大气校正、正射校正、影像裁剪、快速拼接 |
| 图像处理 | 滤波、锐化、波段合成、栅格矢量互转 |
| 分类分析 | ISODATA、K-Means、距离分类、最大似然分类、ROI 统计 |
| 指标计算 | 植被盖度、EVI、水体提取、黑臭水体、湿度、干度、土壤盐度、土壤强度、RSEI |
| 空间分析 | 克里金插值、热力图构建 |

## 代码结构

| 路径 | 说明 |
|---|---|
| `src/PieRemoteSensingPlatform/MainForm.cs` | 主窗体事件编排，负责连接 UI 与服务层 |
| `src/PieRemoteSensingPlatform/Models/` | 遥感指数定义，如表达式、波段组合和输出格式 |
| `src/PieRemoteSensingPlatform/Services/` | PIE 算法创建、执行和波段运算封装 |
| `src/PieRemoteSensingPlatform/Commands/` | 距离分类、最大似然分类等监督分类命令 |
| `src/PieRemoteSensingPlatform/Resources/` | 工具栏图标资源 |
| `src/PieRemoteSensingPlatform/Properties/` | WinForms 工程资源与程序集配置 |

## 运行依赖

| 依赖 | 说明 |
|---|---|
| Windows | WinForms 和 PIE SDK 运行环境依赖 Windows |
| .NET Framework 4.5 | 原项目目标框架 |
| PIE SDK 6.3 | 地图控件、图层管理、遥感算法和空间分析能力来自 PIE SDK |
| ArcGIS 10.2 组件 | 原工程引用了 ArcGIS COM/Interop 组件 |
| DevExpress 18.1 | 原工程保留了 DevExpress 相关组件引用 |

本仓库不提交第三方 SDK、构建产物和遥感数据。

## 开源协议

本项目使用 MIT License。详见 [LICENSE](LICENSE)。
