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
    public partial class ClientListItem : UserControl
    {

        public ClientInfo ClientInfo { get; }

        public Dictionary<PageType, UserControl> Pages { get; set; }

        public event EventHandler ItemClicked;
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                BackColor = value ? Color.Navy : Color.Black;
            }
        }
        public ClientListItem(ClientInfo clientInfo)
        {
            InitializeComponent();
            foreach (Control c in Controls)
            {
                c.Click += ClientListItem_Click;
            }

            ClientInfo = clientInfo;
            ClientNameLabel.Text = clientInfo.Name;
            ClientIdLabel.Text = clientInfo.ClientId.ToString();
            Pages = new Dictionary<PageType, UserControl>
            {
                { PageType.ShellPage, new ShellPage(clientInfo.ClientId) },
                { PageType.ScreenshotPage, new ScreenshotPage(clientInfo.ClientId) }
            };
        }

        private void ClientListItem_Click(object sender, EventArgs e)
        {
            ItemClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
