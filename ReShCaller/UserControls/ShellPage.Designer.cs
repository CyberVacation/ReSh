namespace ReShCaller.UserControls
{
    partial class ShellPage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            outputTextbox = new TextBox();
            inputTextbox = new TextBox();
            sendButton = new Button();
            SuspendLayout();
            // 
            // outputTextbox
            // 
            outputTextbox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            outputTextbox.BackColor = Color.Black;
            outputTextbox.BorderStyle = BorderStyle.FixedSingle;
            outputTextbox.Font = new Font("Ink Free", 14F);
            outputTextbox.ForeColor = Color.Gold;
            outputTextbox.Location = new Point(3, 3);
            outputTextbox.Multiline = true;
            outputTextbox.Name = "outputTextbox";
            outputTextbox.ReadOnly = true;
            outputTextbox.ScrollBars = ScrollBars.Vertical;
            outputTextbox.Size = new Size(654, 461);
            outputTextbox.TabIndex = 0;
            // 
            // inputTextbox
            // 
            inputTextbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            inputTextbox.BackColor = Color.Black;
            inputTextbox.BorderStyle = BorderStyle.FixedSingle;
            inputTextbox.Font = new Font("Ink Free", 14F);
            inputTextbox.ForeColor = Color.Gold;
            inputTextbox.Location = new Point(3, 481);
            inputTextbox.Name = "inputTextbox";
            inputTextbox.Size = new Size(544, 42);
            inputTextbox.TabIndex = 1;
            inputTextbox.KeyDown += inputTextbox_KeyDown;
            // 
            // sendButton
            // 
            sendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sendButton.BackColor = SystemColors.ControlDarkDark;
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.Font = new Font("Impact", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sendButton.Location = new Point(553, 470);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(104, 53);
            sendButton.TabIndex = 2;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = false;
            sendButton.Click += SendButton_Click;
            // 
            // ShellPage
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.Black;
            Controls.Add(sendButton);
            Controls.Add(inputTextbox);
            Controls.Add(outputTextbox);
            Font = new Font("Impact", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.Gold;
            Name = "ShellPage";
            Size = new Size(660, 530);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox outputTextbox;
        private TextBox inputTextbox;
        private Button sendButton;
    }
}
