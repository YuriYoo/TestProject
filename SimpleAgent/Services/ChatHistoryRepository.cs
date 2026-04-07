using SimpleAgent.Models;
using SimpleAgent.UserControls;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAgent.Services
{
    public class ChatHistoryRepository
    {
        private readonly string storageDirectory;

        public FlowLayoutPanel PlannerPanel { get; set; }
        public FlowLayoutPanel DeveloperPanel { get; set; }
        public FlowLayoutPanel ReviewerPanel { get; set; }

        public ChatHistoryRepository()
        {
            // 获取本地 AppData 目录
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            storageDirectory = Path.Combine(appDataPath, "SimpleAgent", "Conversations");

            // 如果目录不存在，初始化时自动创建
            if (!Directory.Exists(storageDirectory))
            {
                Directory.CreateDirectory(storageDirectory);
            }
        }

        private List<string> SortedFiles(string path)
        {
            return Directory.GetFiles(path)
            .Select(f => new { Path = f, Name = Path.GetFileNameWithoutExtension(f) })
            .Where(x => int.TryParse(x.Name, out _)) // 安全过滤：确保文件名是纯数字
            .Select(x => new { x.Path, Index = int.Parse(x.Name) })
            .OrderBy(x => x.Index) // 按照数字大小排序（0, 1, 2, 10...）
            .Select(x => x.Path)   // 提取出路径
            .ToList();
        }

        public void Load(Guid guid)
        {
            PlannerPanel.Controls.Clear();
            DeveloperPanel.Controls.Clear();
            ReviewerPanel.Controls.Clear();

            var loadPath = Path.Combine(storageDirectory, guid.ToString(), "History");
            if (!Directory.Exists(loadPath)) return;

            var plannerPath = Path.Combine(loadPath, "Planner");
            var files = SortedFiles(plannerPath);
            //PlannerPanel.SuspendLayout();
            for (int i = 0; i < files.Count; i++)
            {
                var item = new ChatMessageItem(files[i])
                {
                    Width = ReviewerPanel.ClientSize.Width,
                };
                PlannerPanel.Controls.Add(item);
            }
            PlannerPanel.PerformLayout();

            var developerPath = Path.Combine(loadPath, "Developer");
            files = SortedFiles(developerPath);
            //DeveloperPanel.SuspendLayout();
            for (int i = 0; i < files.Count; i++)
            {
                var item = new ChatMessageItem(files[i])
                {
                    Width = ReviewerPanel.ClientSize.Width,
                };
                DeveloperPanel.Controls.Add(item);
            }
            DeveloperPanel.PerformLayout();

            var reviewerPath = Path.Combine(loadPath, "Reviewer");
            files = SortedFiles(reviewerPath);
            //ReviewerPanel.SuspendLayout();
            for (int i = 0; i < files.Count; i++)
            {
                var item = new ChatMessageItem(files[i])
                {
                    Width = ReviewerPanel.ClientSize.Width,
                };
                ReviewerPanel.Controls.Add(item);
            }
            ReviewerPanel.PerformLayout();
        }

        public void Save(Guid guid)
        {
            var savePath = Path.Combine(storageDirectory, guid.ToString(), "History");
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

            var plannerPath = Path.Combine(savePath, "Planner");
            if (!Directory.Exists(plannerPath)) Directory.CreateDirectory(plannerPath);

            for (int i = 0; i < PlannerPanel.Controls.Count; i++)
            {
                if (PlannerPanel.Controls[i] is ChatMessageItem item)
                {
                    var filePath = Path.Combine(plannerPath, $"{i}.{item.type}");
                    item.SaveMessage(filePath);
                }
            }

            var developerPath = Path.Combine(savePath, "Developer");
            if (!Directory.Exists(developerPath)) Directory.CreateDirectory(developerPath);

            for (int i = 0; i < DeveloperPanel.Controls.Count; i++)
            {
                if (DeveloperPanel.Controls[i] is ChatMessageItem item)
                {
                    var filePath = Path.Combine(developerPath, $"{i}.{item.type}");
                    item.SaveMessage(filePath);
                }
            }

            var reviewerPath = Path.Combine(savePath, "Reviewer");
            if (!Directory.Exists(reviewerPath)) Directory.CreateDirectory(reviewerPath);

            for (int i = 0; i < ReviewerPanel.Controls.Count; i++)
            {
                if (ReviewerPanel.Controls[i] is ChatMessageItem item)
                {
                    var filePath = Path.Combine(reviewerPath, $"{i}.{item.type}");
                    item.SaveMessage(filePath);
                }
            }
        }
    }
}
