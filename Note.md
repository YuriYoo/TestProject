# DI容器

** 单独服务 **
日志服务
上下文裁剪

** 智能体 **
Planner
Developer
Reviewer
Router

** 插件 **
命令行工具
文件系统工具
工作流控制工具
网络请求工具
子代理工具

** 服务 **
SettingsService
KernelService
BackgroundService
StreamingExecutionEngine
*MultiAgentOrchestrator
GPUStackClient
ChatUIService
*MainForm

** 新增 **
AgentFactory
AgentContextRepository

# 主流程

[必备对象]
AgentContext
MultiAgentOrchestrator

Planner
Developer
Reviewer
Router

[初始化]
创建 AgentContext