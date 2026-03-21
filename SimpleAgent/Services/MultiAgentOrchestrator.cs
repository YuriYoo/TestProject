using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SimpleAgent.Agents;
using SimpleAgent.Models;
using SimpleAgent.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Services
{
	/// <summary>
	/// 多智能体编排
	/// </summary>
	public class MultiAgentOrchestrator
	{
		private KernelService kernelService;

		private WorkflowState _currentState = WorkflowState.Idle;
		private readonly AgentContext _context = new();

		private PlannerAgent _plannerAgent;
		private DeveloperAgent _developerAgent;
		private ReviewerAgent _reviewerAgent;
		private RouterAgent _routerAgent;
		private ChatUIService chatUIService;
		private MainForm mainForm;

		private TaskCompletionSource<string>? _userInputTcs;

		public MultiAgentOrchestrator(KernelService kernelService, ChatUIService chatUIService, MainForm mainForm)
		{
			this.chatUIService = chatUIService;
			this.kernelService = kernelService;
			this.mainForm = mainForm;

			_plannerAgent = new(kernelService, (plan) =>
			{
				_context.DetailedPlan = plan;
				_currentState = WorkflowState.Developing; // 完成时切换状态
			});

			_developerAgent = new(kernelService, (summary) =>
			{
				_context.DeveloperSummary = summary;
				_currentState = WorkflowState.Reviewing; // 完成时切换状态
			});

			_reviewerAgent = new(kernelService, () =>
			{
				_currentState = WorkflowState.Completed;
			},
			(feedback) =>
			{
				_context.ReviewerFeedback = feedback;
				_currentState = WorkflowState.Developing; // 打回重做
			});

			_routerAgent = new(kernelService, () =>
			{
				_plannerAgent.AddUserMessage(_context.OriginalRequest);
				_currentState = WorkflowState.Planning;
			},
			() =>
			{
				// 直接用用户输入作为开发计划
				_context.DetailedPlan = _context.OriginalRequest;
				_currentState = WorkflowState.Developing;
			});
		}

		/// <summary>
		/// 向UI界面发送消息
		/// </summary>
		/// <param name="messageType"></param>
		/// <param name="agentType"></param>
		/// <param name="message"></param>
		private void SendUIMessage(MessageType messageType, AgentType agentType, string message, bool addNewLine = false, Color? color = null)
		{
			chatUIService.SendMessage(messageType, agentType, message, addNewLine, color);
		}

		/// <summary>
		/// 向UI界面发送工具调用消息（专门处理模型调用插件时的显示）
		/// </summary>
		/// <param name="agentType"></param>
		/// <param name="name"></param>
		/// <param name="arguments"></param>
		private void SendToolMessage(AgentType agentType, string? name, string? arguments)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				SendUIMessage(MessageType.AI, agentType, $"[正在调用未知工具]", true, Color.Gray);
				return;
			}
			if (string.IsNullOrEmpty(arguments) || arguments.Length <= 2)
			{
				arguments = "";
			}
			else if (arguments.Length > 30)
			{
				arguments = $"{arguments[..30]}...}}";
			}
			SendUIMessage(MessageType.AI, agentType, $"[正在调用工具] {name} {arguments}", true, Color.Gray);
		}

		/// <summary>
		/// 接收来自 UI 的用户输入（用于在规划阶段恢复工作流）
		/// </summary>
		/// <param name="userInput">用户输入的反馈或确认信息</param>
		public void ProvideUserInput(string userInput)
		{
			// 如果当前正在等待用户输入，则将用户的内容设置进去，这会触发 await 后的代码继续执行
			if (_userInputTcs != null && !_userInputTcs.Task.IsCompleted)
			{
				_userInputTcs.TrySetResult(userInput);
			}
		}

		/// <summary>
		/// 运行工作流
		/// </summary>
		/// <param name="userInput"></param>
		/// <returns></returns>
		public async Task RunWorkflowAsync(string userInput)
		{
			// 记录用户的原始请求
			_context.OriginalRequest = userInput;

			// 重置
			_context.DevCycleCount = 0;

			// 收到消息，进入路由状态
			_currentState = WorkflowState.Routing;

			while (_currentState != WorkflowState.Idle && _currentState != WorkflowState.Completed)
			{
				Trace.WriteLine($"开始状态: {_currentState}");
				switch (_currentState)
				{
					// 路由判断
					case WorkflowState.Routing:
						await HandleRoutingAsync();
						break;

					// 讨论规划
					case WorkflowState.Planning:
						await HandlePlanningAsync();
						SaveChatHistory(AgentType.Planner, _plannerAgent.chatHistory);
						break;

					// 开发测试
					case WorkflowState.Developing:
						await HandleDevelopingAsync();
						SaveChatHistory(AgentType.Developer, _developerAgent.chatHistory);
						break;

					// 验收修改
					case WorkflowState.Reviewing:
						await HandleReviewingAsync();
						SaveChatHistory(AgentType.Reviewer, _reviewerAgent.chatHistory);
						break;
				}
				Trace.WriteLine($"结束状态: {_currentState}");
			}

			Trace.WriteLine("本轮任务已全部处理完毕，等待下一次用户输入。");
		}

		private void SaveChatHistory(AgentType agentType, ChatHistory chatHistory)
		{
			var path = $"logs\\{DateTime.Now:yyyyMMdd_HHmmss}_{agentType}.txt";
			StringBuilder sb = new();
			foreach (var message in chatHistory)
			{
				sb.AppendLine($"===== {message.Role} =====");
				sb.AppendLine($"[Items]");
				if (message.Items.Count > 0)
				{
					foreach (var item in message.Items)
					{
						sb.AppendLine($"- Mime:{item.MimeType}  Inner:{item.InnerContent}");
						if (item.Metadata != null)
						{
							foreach (var meta in item.Metadata)
							{
								sb.AppendLine($"- {meta.Key}: {meta.Value}");
								if (meta.Key == "ChatResponseMessage.FunctionToolCalls")
								{
									if (meta.Value is List<OpenAI.Chat.ChatToolCall> toolCalls)
									{
										foreach (var toolCall in toolCalls)
										{
											sb.AppendLine($"  - ID:{toolCall.Id}  Kind:{toolCall.Kind}  FuncName:{toolCall.FunctionName}  FuncArgs:{toolCall.FunctionArguments}");
										}
									}
									else
									{
										sb.AppendLine($"  - 转换失败: {meta.Value?.GetType()}");
									}
								}
							}
						}
					}
				}
				if (message.Metadata != null && message.Metadata.Count > 0)
				{
					sb.AppendLine($"[Metadata]");
					foreach (var meta in message.Metadata)
					{
						sb.AppendLine($"- {meta.Key}: {meta.Value}");
						if (meta.Key == "ChatResponseMessage.FunctionToolCalls")
						{
							if (meta.Value is List<OpenAI.Chat.ChatToolCall> toolCalls)
							{
								foreach (var toolCall in toolCalls)
								{
									sb.AppendLine($"  - ID:{toolCall.Id}  Kind:{toolCall.Kind}  FuncName:{toolCall.FunctionName}  FuncArgs:{toolCall.FunctionArguments}");
								}
							}
							else
							{
								sb.AppendLine($"  - 转换失败: {meta.Value?.GetType()}");
							}
						}
					}
				}
				sb.AppendLine($"[Content]");
				sb.AppendLine($"{message.Content}");
			}
			File.WriteAllText(path, sb.ToString());
		}

		/// <summary>
		/// 处理路由判断
		/// </summary>
		/// <returns></returns>
		private async Task HandleRoutingAsync()
		{
			Trace.WriteLine("正在路由...");
			_routerAgent.AddUserMessage($"用户输入: {_context.OriginalRequest}");

			StringBuilder sb = new();
			await foreach (var chunk in _routerAgent.GetChatMessageContentAsync())
			{
				sb.Append(chunk.Content);

				// 如果插件回调已经将状态改为了非 Routing 立即强制跳出循环。这会直接 Dispose 底层的异步流，强行掐断大模型的无限调用！
				if (_currentState != WorkflowState.Routing)
				{
					Trace.WriteLine("路由目标已确定，主动中断 Router 后续输出...");
					break;
				}
			}

			Trace.WriteLine($"路由消息: {sb}");
		}

		/// <summary>
		/// 处理需求规划
		/// </summary>
		/// <returns></returns>
		private async Task HandlePlanningAsync()
		{
			Trace.WriteLine("Planner: 正在与您规划需求...");

			StringBuilder sb = new();
			await foreach (var chunk in _plannerAgent.GetChatMessageContentAsync())
			{
				sb.Append(chunk.Content);
				//SendUIMessage(MessageType.AI, AgentType.Planner, chunk.Content ?? "");

				// 遍历当前 chunk 中的所有内容项
				foreach (var item in chunk.Items)
				{
					switch (item)
					{
						// 普通的消息文本内容
						case StreamingTextContent textContent:
							//sb.Append(textContent.Text);
							SendUIMessage(MessageType.AI, AgentType.Planner, textContent.Text ?? "");
							break;

						// 工具/函数调用的内容（模型决定调用插件时会触发）
						case StreamingFunctionCallUpdateContent functionCallUpdate:
							// 这里会流式输出函数名、参数等, Arguments 参数是一段段 JSON 字符串流式到达的
							SendToolMessage(AgentType.Planner, functionCallUpdate.Name, functionCallUpdate.Arguments);
							break;

						// 你还可以按需处理其他类型，例如 StreamingFileReferenceContent 等
						default:
							SendUIMessage(MessageType.AI, AgentType.Planner, $"未知的流内容: {item.GetType()}", true, Color.Red);
							break;
					}
				}

				if (_currentState != WorkflowState.Planning)
				{
					Trace.WriteLine("计划已完成，主动中断 Planner 后续输出...");
					break;
				}
			}

			if (sb.Length > 0)
			{
				_plannerAgent.AddAssistantMessage(sb.ToString());
			}
			SendUIMessage(MessageType.System, AgentType.Planner, "END");

			// 如果状态变了（因为模型调用了 FinalizePlan 触发了回调），循环会进入下一阶段
			// 如果没变，说明还需要向用户提问，可以在这里挂起，等待用户输入并加入 _plannerHistory
			if (_currentState == WorkflowState.Planning)
			{
				Trace.WriteLine("等待用户输入反馈...");

				// 通知 UI 开启输入框
				mainForm.RecoveryState();

				// 初始化一个新的 TaskCompletionSource
				_userInputTcs = new TaskCompletionSource<string>();

				// 异步等待，此时 RunWorkflowAsync 的执行会在这里暂停，并且交出控制权，不会阻塞 UI 线程
				string userFeedback = await _userInputTcs.Task;

				// 用户输入后，代码从这里恢复执行，将用户的回复加入 Planner 的上下文
				_plannerAgent.AddUserMessage(userFeedback);

				// 循环结束，外层的 while 会再次进入 HandlePlanningAsync，让 Planner 根据用户反馈继续对话
			}
		}

		/// <summary>
		/// 处理开发测试
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private async Task HandleDevelopingAsync()
		{
			Trace.WriteLine("Developer: 正在编写和测试代码...");
			_context.DevCycleCount++;

			if (_context.DevCycleCount > 500)
			{
				throw new Exception("开发-测试循环次数超限，强制中止！");
			}

			// 首次发送的为执行计划, 后续的为 Reviewer 修改
			if (_context.DevCycleCount == 1)
			{
				_developerAgent.AddUserMessage($"请根据以下计划开发：\n{_context.DetailedPlan}");
			}
			else if (!string.IsNullOrEmpty(_context.ReviewerFeedback))
			{
				_developerAgent.AddUserMessage($"Reviewer 打回，要求修改：{_context.ReviewerFeedback}");
				_context.ReviewerFeedback = string.Empty;
			}
			else
			{
				_developerAgent.AddUserMessage("【系统提示】如果你确定已经完成计划，也已经进行过测试且编译没有问题，请调用 `submit_for_review` 提交审查。如果没有完成任务，请不要停止，根据上一个消息继续你的工作。");
			}

			StringBuilder sb = new();
			await foreach (var chunk in _developerAgent.GetChatMessageContentAsync())
			{
				//Trace.WriteLine($"Developer 输出: Role:{(chunk.Role == null ? "Unknown" : chunk.Role.Value)}  Items:{string.Join(',', chunk.Items)}  Meta:{string.Join(',', chunk.Metadata)}");
				sb.Append(chunk.Content);
				//SendUIMessage(MessageType.AI, AgentType.Developer, chunk.Content ?? "");

				// 遍历当前 chunk 中的所有内容项
				foreach (var item in chunk.Items)
				{
					switch (item)
					{
						// 普通的消息文本内容
						case StreamingTextContent textContent:
							//sb.Append(textContent.Text);
							SendUIMessage(MessageType.AI, AgentType.Developer, textContent.Text ?? "");
							break;

						// 工具/函数调用的内容（模型决定调用插件时会触发）
						case StreamingFunctionCallUpdateContent functionCallUpdate:
							// 这里会流式输出函数名、参数等, Arguments 参数是一段段 JSON 字符串流式到达的
							SendToolMessage(AgentType.Developer, functionCallUpdate.Name, functionCallUpdate.Arguments);
							break;

						// 你还可以按需处理其他类型，例如 StreamingFileReferenceContent 等
						default:
							SendUIMessage(MessageType.AI, AgentType.Developer, $"未知的流内容: {item.GetType()}", true, Color.Red);
							break;
					}
				}

				if (_currentState != WorkflowState.Developing)
				{
					Trace.WriteLine("开发已完成，主动中断 Developer 后续输出...");
					break;
				}
			}

			if (sb.Length > 0)
			{
				_developerAgent.AddAssistantMessage(sb.ToString());
			}
			SendUIMessage(MessageType.System, AgentType.Developer, "END");
		}

		/// <summary>
		/// 处理验收修改
		/// </summary>
		/// <returns></returns>
		private async Task HandleReviewingAsync()
		{
			Trace.WriteLine("Reviewer: 正在验收...");
			_reviewerAgent.AddUserMessage($"原计划：{_context.DetailedPlan}\n开发者提交摘要：{_context.DeveloperSummary}");

			StringBuilder sb = new();
			await foreach (var chunk in _reviewerAgent.GetChatMessageContentAsync())
			{
				sb.Append(chunk.Content);
				SendUIMessage(MessageType.AI, AgentType.Reviewer, chunk.Content ?? "");

				if (_currentState != WorkflowState.Developing)
				{
					Trace.WriteLine("审查已完成，主动中断 Reviewer 后续输出...");
					break;
				}
			}

			if (sb.Length > 0)
			{
				_reviewerAgent.AddAssistantMessage(sb.ToString());
			}
			SendUIMessage(MessageType.System, AgentType.Reviewer, "END");
		}
	}
}
