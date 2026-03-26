using Microsoft.Extensions.DependencyInjection;
using SimpleAgent.Agents;
using SimpleAgent.Models;
using SimpleAgent.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAgent.Factory
{
    public interface IAgentFactory
    {
        TAgent CreateAgent<TAgent>(AgentContext context) where TAgent : BaseAgent;
    }

    public class AgentFactory : IAgentFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AgentFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TAgent CreateAgent<TAgent>(AgentContext context) where TAgent : BaseAgent
        {
            // 实例化 TAgent，传入需要的参数，如果没有传就会去DI中查找
            return ActivatorUtilities.CreateInstance<TAgent>(_serviceProvider, context);
        }
    }
}
