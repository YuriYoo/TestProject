using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAgent.Models
{
    public class ConversationTreeNode
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsProject { get; set; }

        public Guid ConversationId { get; set; }

        public List<ConversationTreeNode> Children { get; set; }
    }
}
