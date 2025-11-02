
namespace ReShCaller
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ScreenshotPageButton = new Button();
            FilePageButton = new Button();
            InfoPageButton = new Button();
            ShellPageButton = new Button();
            PagePanel = new Panel();
            ReconnectButton = new Button();
            ConnectionStatusLabel = new Label();
            ClientFlowLayoutPanel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // ScreenshotPageButton
            // 
            ScreenshotPageButton.BackColor = Color.Black;
            ScreenshotPageButton.FlatStyle = FlatStyle.Flat;
            ScreenshotPageButton.Font = new Font("Impact", 9F);
            ScreenshotPageButton.Location = new Point(383, 58);
            ScreenshotPageButton.Name = "ScreenshotPageButton";
            ScreenshotPageButton.Size = new Size(118, 40);
            ScreenshotPageButton.TabIndex = 2;
            ScreenshotPageButton.Text = "Screenshot";
            ScreenshotPageButton.UseVisualStyleBackColor = false;
            ScreenshotPageButton.Click += ScreenshotPageButton_Click;
            // 
            // FilePageButton
            // 
            FilePageButton.BackColor = Color.Black;
            FilePageButton.FlatStyle = FlatStyle.Flat;
            FilePageButton.Font = new Font("Impact", 12F);
            FilePageButton.Location = new Point(494, 58);
            FilePageButton.Name = "FilePageButton";
            FilePageButton.Size = new Size(117, 40);
            FilePageButton.TabIndex = 3;
            FilePageButton.Text = "File";
            FilePageButton.UseVisualStyleBackColor = false;
            // 
            // InfoPageButton
            // 
            InfoPageButton.BackColor = Color.Black;
            InfoPageButton.FlatStyle = FlatStyle.Flat;
            InfoPageButton.Font = new Font("Impact", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            InfoPageButton.Location = new Point(607, 58);
            InfoPageButton.Name = "InfoPageButton";
            InfoPageButton.Size = new Size(117, 40);
            InfoPageButton.TabIndex = 4;
            InfoPageButton.Text = "Info";
            InfoPageButton.UseVisualStyleBackColor = false;
            // 
            // ShellPageButton
            // 
            ShellPageButton.BackColor = Color.Black;
            ShellPageButton.FlatStyle = FlatStyle.Flat;
            ShellPageButton.Font = new Font("Impact", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ShellPageButton.Location = new Point(274, 58);
            ShellPageButton.Name = "ShellPageButton";
            ShellPageButton.Size = new Size(117, 40);
            ShellPageButton.TabIndex = 1;
            ShellPageButton.Text = "Shell";
            ShellPageButton.UseVisualStyleBackColor = false;
            ShellPageButton.Click += ShellPageButton_Click;
            // 
            // PagePanel
            // 
            PagePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PagePanel.BackColor = Color.Black;
            PagePanel.Font = new Font("Ink Free", 9.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PagePanel.Location = new Point(274, 87);
            PagePanel.Name = "PagePanel";
            PagePanel.Size = new Size(1092, 665);
            PagePanel.TabIndex = 6;
            PagePanel.Resize += PagePanel_Resize;
            // 
            // ReconnectButton
            // 
            ReconnectButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ReconnectButton.BackColor = SystemColors.ActiveCaptionText;
            ReconnectButton.FlatStyle = FlatStyle.Flat;
            ReconnectButton.Font = new Font("Impact", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ReconnectButton.ForeColor = Color.Fuchsia;
            ReconnectButton.Location = new Point(58, 583);
            ReconnectButton.Name = "ReconnectButton";
            ReconnectButton.Size = new Size(162, 40);
            ReconnectButton.TabIndex = 9;
            ReconnectButton.Text = "Reconnect";
            ReconnectButton.UseVisualStyleBackColor = false;
            ReconnectButton.Click += ReconnectButton_Click;
            // 
            // ConnectionStatusLabel
            // 
            ConnectionStatusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ConnectionStatusLabel.AutoSize = true;
            ConnectionStatusLabel.Location = new Point(58, 626);
            ConnectionStatusLabel.Name = "ConnectionStatusLabel";
            ConnectionStatusLabel.Size = new Size(88, 22);
            ConnectionStatusLabel.TabIndex = 10;
            ConnectionStatusLabel.Text = "Connected";
            // 
            // ClientFlowLayoutPanel
            // 
            ClientFlowLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ClientFlowLayoutPanel.BackColor = Color.Black;
            ClientFlowLayoutPanel.Location = new Point(31, 87);
            ClientFlowLayoutPanel.Name = "ClientFlowLayoutPanel";
            ClientFlowLayoutPanel.Size = new Size(208, 490);
            ClientFlowLayoutPanel.TabIndex = 11;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(1378, 764);
            Controls.Add(ClientFlowLayoutPanel);
            Controls.Add(ConnectionStatusLabel);
            Controls.Add(ReconnectButton);
            Controls.Add(PagePanel);
            Controls.Add(ShellPageButton);
            Controls.Add(InfoPageButton);
            Controls.Add(FilePageButton);
            Controls.Add(ScreenshotPageButton);
            Font = new Font("Impact", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.Gold;
            Name = "MainForm";
            Text = "ReShCaller";
            Load += MainForm_Load;
            MouseDown += MainForm_MouseDown;
            MouseMove += MainForm_MouseMove;
            MouseUp += MainForm_MouseUp;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button ScreenshotPageButton;
        private Button FilePageButton;
        private Button InfoPageButton;
        private Button ShellPageButton;
        private Panel PagePanel;
        private Button ReconnectButton;
        private Label ConnectionStatusLabel;
        private FlowLayoutPanel ClientFlowLayoutPanel;
    }
}
