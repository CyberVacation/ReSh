namespace ReShCaller.UserControls
{
    partial class ClientListItem
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
            ClientNameLabel = new Label();
            ClientIdLabel = new Label();
            SuspendLayout();
            // 
            // ClientNameLabel
            // 
            ClientNameLabel.AutoSize = true;
            ClientNameLabel.Location = new Point(0, 2);
            ClientNameLabel.Name = "ClientNameLabel";
            ClientNameLabel.Size = new Size(108, 25);
            ClientNameLabel.TabIndex = 0;
            ClientNameLabel.Text = "ClientName";
            // 
            // ClientIdLabel
            // 
            ClientIdLabel.AutoSize = true;
            ClientIdLabel.Location = new Point(24, 31);
            ClientIdLabel.Name = "ClientIdLabel";
            ClientIdLabel.Size = new Size(82, 25);
            ClientIdLabel.TabIndex = 1;
            ClientIdLabel.Text = "ClientId";
            // 
            // ClientListItem
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            Controls.Add(ClientIdLabel);
            Controls.Add(ClientNameLabel);
            Font = new Font("Ink Free", 10F);
            ForeColor = Color.Lavender;
            Name = "ClientListItem";
            Size = new Size(310, 61);
            Click += ClientListItem_Click;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label ClientNameLabel;
        private Label ClientIdLabel;
    }
}
