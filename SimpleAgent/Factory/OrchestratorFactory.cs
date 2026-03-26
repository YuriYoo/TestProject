using Microsoft.Extensions.DependencyInjection;
using SimpleAgent.Agents;
using SimpleAgent.Models;
using SimpleAgent.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAgent.Factory
{
    public interface IOrchestratorFactory
    {
        MultiAgentOrchestrator CreateOrchestrator(AgentContext context);
    }

    public class OrchestratorFactory : IOrchestratorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public OrchestratorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MultiAgentOrchestrator CreateOrchestrator(AgentContext context)
        {
            return ActivatorUtilities.CreateInstance<MultiAgentOrchestrator>(_serviceProvider, context);
        }
    }
}
