namespace ReShCaller.UserControls
{
    partial class ScreenshotPage
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
            ScreenshotPictureBox = new PictureBox();
            TakeScreenshotButton = new Button();
            SaveScreenshotButton = new Button();
            ((System.ComponentModel.ISupportInitialize)ScreenshotPictureBox).BeginInit();
            SuspendLayout();
            // 
            // ScreenshotPictureBox
            // 
            ScreenshotPictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ScreenshotPictureBox.Location = new Point(0, 66);
            ScreenshotPictureBox.Name = "ScreenshotPictureBox";
            ScreenshotPictureBox.Size = new Size(657, 464);
            ScreenshotPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            ScreenshotPictureBox.TabIndex = 0;
            ScreenshotPictureBox.TabStop = false;
            // 
            // TakeScreenshotButton
            // 
            TakeScreenshotButton.FlatStyle = FlatStyle.Flat;
            TakeScreenshotButton.Location = new Point(3, 13);
            TakeScreenshotButton.Name = "TakeScreenshotButton";
            TakeScreenshotButton.Size = new Size(191, 34);
            TakeScreenshotButton.TabIndex = 1;
            TakeScreenshotButton.Text = "Take screenshot";
            TakeScreenshotButton.UseVisualStyleBackColor = true;
            TakeScreenshotButton.Click += TakeScreenshotButton_Click;
            // 
            // SaveScreenshotButton
            // 
            SaveScreenshotButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SaveScreenshotButton.FlatStyle = FlatStyle.Flat;
            SaveScreenshotButton.Location = new Point(466, 13);
            SaveScreenshotButton.Name = "SaveScreenshotButton";
            SaveScreenshotButton.Size = new Size(191, 34);
            SaveScreenshotButton.TabIndex = 2;
            SaveScreenshotButton.Text = "Save screenshot";
            SaveScreenshotButton.UseVisualStyleBackColor = true;
            SaveScreenshotButton.Click += SaveScreenshotButton_Click;
            // 
            // ScreenshotPage
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.Black;
            Controls.Add(SaveScreenshotButton);
            Controls.Add(TakeScreenshotButton);
            Controls.Add(ScreenshotPictureBox);
            Font = new Font("Impact", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.Gold;
            Name = "ScreenshotPage";
            Size = new Size(660, 530);
            ((System.ComponentModel.ISupportInitialize)ScreenshotPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox ScreenshotPictureBox;
        private Button TakeScreenshotButton;
        private Button SaveScreenshotButton;
    }
}
