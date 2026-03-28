using SimpleAgent.Services;
using System;
using System.Windows.Forms;

namespace SimpleAgent
{
    public partial class SettingsForm : Form
    {
        private readonly ISettingsService settingsService;
        private readonly Models.AppSettings originalSettings;

        public SettingsForm(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
            originalSettings = CloneSettings(settingsService.Current);
            InitializeComponent();
            LoadSettings();
        }

        private Models.AppSettings CloneSettings(Models.AppSettings source)
        {
            return new Models.AppSettings
            {
                ApiKey = source.ApiKey,
                ApiBaseUrl = source.ApiBaseUrl,
                ModelId = source.ModelId,
                TerminalTruncation = source.TerminalTruncation,
                TerminalTimeout = source.TerminalTimeout,
                HttpTerminal = source.HttpTerminal,
                HttpTimeout = source.HttpTimeout,
                MaxThinkingRounds = source.MaxThinkingRounds,
                SubMaxThinkingRounds = source.SubMaxThinkingRounds,
                MaxTokens = source.MaxTokens,
                ContextCompressionThreshold = source.ContextCompressionThreshold,
                WorkingDirectory = source.WorkingDirectory
            };
        }

        private void LoadSettings()
        {
            var settings = settingsService.Current;

            txtApiKey.Text = settings.ApiKey;
            txtApiBaseUrl.Text = settings.ApiBaseUrl;
            txtModelId.Text = settings.ModelId;

            numTerminalTruncation.Value = settings.TerminalTruncation;
            numTerminalTimeout.Value = settings.TerminalTimeout;
            numHttpTerminal.Value = settings.HttpTerminal;
            numHttpTimeout.Value = settings.HttpTimeout;
            numMaxThinkingRounds.Value = settings.MaxThinkingRounds;
            numSubMaxThinkingRounds.Value = settings.SubMaxThinkingRounds;
            numMaxTokens.Value = settings.MaxTokens;
            numContextCompressionThreshold.Value = settings.ContextCompressionThreshold;

            txtWorkingDirectory.Text = settings.WorkingDirectory;
        }

        private void ApplySettings()
        {
            var settings = settingsService.Current;

            settings.ApiKey = txtApiKey.Text;
            settings.ApiBaseUrl = txtApiBaseUrl.Text;
            settings.ModelId = txtModelId.Text;

            settings.TerminalTruncation = (int)numTerminalTruncation.Value;
            settings.TerminalTimeout = (int)numTerminalTimeout.Value;
            settings.HttpTerminal = (int)numHttpTerminal.Value;
            settings.HttpTimeout = (int)numHttpTimeout.Value;
            settings.MaxThinkingRounds = (int)numMaxThinkingRounds.Value;
            settings.SubMaxThinkingRounds = (int)numSubMaxThinkingRounds.Value;
            settings.MaxTokens = (int)numMaxTokens.Value;
            settings.ContextCompressionThreshold = (int)numContextCompressionThreshold.Value;

            settings.WorkingDirectory = txtWorkingDirectory.Text;
        }

        private void RestoreOriginalSettings()
        {
            var settings = settingsService.Current;

            settings.ApiKey = originalSettings.ApiKey;
            settings.ApiBaseUrl = originalSettings.ApiBaseUrl;
            settings.ModelId = originalSettings.ModelId;
            settings.TerminalTruncation = originalSettings.TerminalTruncation;
            settings.TerminalTimeout = originalSettings.TerminalTimeout;
            settings.HttpTerminal = originalSettings.HttpTerminal;
            settings.HttpTimeout = originalSettings.HttpTimeout;
            settings.MaxThinkingRounds = originalSettings.MaxThinkingRounds;
            settings.SubMaxThinkingRounds = originalSettings.SubMaxThinkingRounds;
            settings.MaxTokens = originalSettings.MaxTokens;
            settings.ContextCompressionThreshold = originalSettings.ContextCompressionThreshold;
            settings.WorkingDirectory = originalSettings.WorkingDirectory;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            ApplySettings();
            settingsService.Save();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            RestoreOriginalSettings();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "确定要重置所有设置为默认值吗？",
                "确认重置",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                settingsService.Reset();
                LoadSettings();
                MessageBox.Show("设置已重置为默认值", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnBrowseWorkingDir_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog dialog = new();
            dialog.Description = "选择默认工作目录";
            dialog.ShowNewFolderButton = true;

            if (!string.IsNullOrEmpty(txtWorkingDirectory.Text) && System.IO.Directory.Exists(txtWorkingDirectory.Text))
            {
                dialog.SelectedPath = txtWorkingDirectory.Text;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtWorkingDirectory.Text = dialog.SelectedPath;
            }
        }
    }
}
