using System;
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
    public partial class ShellPage : UserControl
    {
        private Guid _clientId;
        public ShellPage(Guid clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            ReShCaller.OnCommandResultReceived += OnShellResultReceived;
            Disposed += ShellPage_Disposed;
        }

        public void ShellPage_Disposed(object? sender, EventArgs e)
        {
            ReShCaller.OnCommandResultReceived -= OnShellResultReceived;
        }

        private void OnShellResultReceived(CommandResult result)
        {
            if (_clientId != result.ClientId)
                return;
            if (result.Type != CommandType.ShellCommand)
                return;
            if (!result.TryGetData(out ShellResult shellResult))
                return;
            outputTextbox.AppendText(shellResult.Result);
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (inputTextbox.Text.Trim().Length > 0)
            {
                ReShCaller.SendShellCommand(inputTextbox.Text);
                inputTextbox.Clear();
            }
        }

        private void inputTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendButton_Click(this, new EventArgs());
                e.SuppressKeyPress = true; // Prevents the ding sound
            }
        }
    }
}
