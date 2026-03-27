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

# 对象说明

** 智能体 **
BaseAgent - 基类
RouterAgent - 依赖注入(瞬时) - MainForm创建会话时通过工厂创建 - 需要AgentContext
PlannerAgent - 依赖注入(瞬时) - MultiAgentOrchestrator构造函数通过工厂创建 - 需要AgentContext
DeveloperAgent - 依赖注入(瞬时) - MultiAgentOrchestrator构造函数通过工厂创建 - 需要AgentContext
ReviewerAgent - 依赖注入(瞬时) - MultiAgentOrchestrator构造函数通过工厂创建 - 需要AgentContext
SubDeveloperAgent - 依赖注入(瞬时) - SubAgentPlugin中创建子代理工具被调用时通过工厂创建 - 需要AgentContext

** 插件/工具 **
FileSystemPlugin - 依赖注入(瞬时) - KernelService在BuildKernel时创建 - 需要AgentContext
TerminalPlugin - 依赖注入(瞬时) - KernelService在BuildKernel时创建 - 需要AgentContext
HttpTestPlugin - 依赖注入(瞬时) - KernelService在BuildKernel时创建
WorkflowPlugin - 依赖注入(瞬时) - KernelService在BuildKernel时创建 - 需要AgentContext
SubAgentPlugin - 依赖注入(瞬时) - KernelService在BuildKernel时创建 - 需要AgentContext
SubWorkflowPlugin - SubDeveloperAgent在创建时动态添加

** 服务 **
SettingsService - 依赖注入(单例) - 通过依赖注入创建
KernelService - 依赖注入(单例) - 通过依赖注入创建
MultiAgentOrchestrator - 依赖注入(瞬时) - MainForm创建会话时通过工厂创建 - 需要AgentContext

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