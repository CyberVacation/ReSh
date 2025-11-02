using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReShCaller.UserControls
{
    public partial class ScreenshotPage : UserControl
    {
        private Guid _clientId;
        public ScreenshotPage(Guid clientId)
        {
            _clientId = clientId;
            InitializeComponent();
            ReShCaller.OnCommandResultReceived += OnScreenshotReceived;
            Disposed += ScreenshotPage_Disposed;
        }
        private void ScreenshotPage_Disposed(object? sender, EventArgs e)
        {
            ReShCaller.OnCommandResultReceived -= OnScreenshotReceived;
        }
        public Bitmap CapturedImage { get; private set; }
        public void OnScreenshotReceived(CommandResult result)
        {
            if (_clientId != result.ClientId)
                return;
            if (result.Type != CommandType.Screenshot)
                return;
            if (!result.TryGetData(out ScreenshotResult screenshotResult))
                return;
            try
            {
                using (MemoryStream stream = new MemoryStream(screenshotResult.Screenshot))
                {
                    Image image = Image.FromStream(stream);
                    CapturedImage = (Bitmap)image.Clone();
                    ScreenshotPictureBox.Image = CapturedImage;
                }
            }
            catch (Exception ex)
            {
                // Handle other potential errors (e.g., corrupted image data)
                MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void TakeScreenshotButton_Click(object sender, EventArgs e)
        {
            ReShCaller.TakeScreenshot();
        }

        private void SaveScreenshotButton_Click(object sender, EventArgs e)
        {
            if (CapturedImage != null)
                CapturedImage.Save($"screenshot{DateTime.Now:'-'HH'-'mm'-'ss'-'fff}.png");

        }
    }
}
