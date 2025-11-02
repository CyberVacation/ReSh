using ReShCaller.UserControls;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ReShCaller
{
    public enum PageType
    {
        ShellPage,
        ScreenshotPage,
        FilePage,
        InfoPage
    }
    public partial class MainForm : Form
    {
        private ClientListItem? _currentlySelectedItem = null;
        public MainForm()
        {
            InitializeComponent();

            ReShCaller.OnClientListReceived += UpdateClientList;
            ReShCaller.OnClientOnline += AddClient;
            ReShCaller.OnClientOffline += RemoveClient;
            ReShCaller.OnDisconnnected += () =>
            {
                Invoke(new Action(() =>
                {
                    ClientFlowLayoutPanel.Controls.Clear();
                    PagePanel.Controls.Clear();
                    ConnectionStatusLabel.Text = "Disconnected.";
                    ShellPageButton.BackColor = Color.Black;
                    ScreenshotPageButton.BackColor = Color.Black;
                    FilePageButton.BackColor = Color.Black;
                    InfoPageButton.BackColor = Color.Black;
                }));
            };

        }
        private void CreateClientItem(ClientInfo clientInfo)
        {
            var clientItem = new ClientListItem(clientInfo);
            clientItem.ItemClicked += ClientListItem_ItemClicked;
            ClientFlowLayoutPanel.Controls.Add(clientItem);
            foreach (Control page in clientItem.Pages.Values)
            {
                page.Size = PagePanel.ClientSize;
                page.Hide();
                PagePanel.Controls.Add(page);
            }
        }
        private void RemoveClientListItem(Guid clientId)
        {
            if (_currentlySelectedItem != null && _currentlySelectedItem.ClientInfo.ClientId == clientId)
            {
                _currentlySelectedItem = null;
                ReShCaller.SelectedClientId = Guid.Empty;
                ShellPageButton.BackColor = Color.Black;
                ScreenshotPageButton.BackColor = Color.Black;
                FilePageButton.BackColor = Color.Black;
                InfoPageButton.BackColor = Color.Black;
            }
            foreach (ClientListItem clientItem in ClientFlowLayoutPanel.Controls)
            {
                if (clientItem.ClientInfo.ClientId == clientId)
                {
                    foreach (Control page in clientItem.Pages.Values)
                    {
                        PagePanel.Controls.Remove(page);
                    }
                    ClientFlowLayoutPanel.Controls.Remove(clientItem);
                    break;
                }
            }
        }

        private void AddClient(ClientInfo clientInfo)
        {
            Invoke(new Action(() =>
            {
                CreateClientItem(clientInfo);
            }));
        }
        private void RemoveClient(Guid clientId)
        {
            Invoke(new Action(() =>
            {
                RemoveClientListItem(clientId);
            }));
        }
        private void UpdateClientList()
        {
            Invoke(new Action(() =>
            {
                ClientFlowLayoutPanel.Controls.Clear();
                foreach (var client in ReShCaller.Clients.Values)
                {
                    CreateClientItem(client);
                }
            }));
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            if (!await ReShCaller.ConnectAsync())
            {
                ConnectionStatusLabel.Text = "Disconnected.";
            }
        }

        private void ShellPageButton_Click(object sender, EventArgs e)
        {
            if (ReShCaller.SelectedClientId == Guid.Empty)
            {
                return;
            }
            foreach (Control control in PagePanel.Controls)
            {
                control.Hide();
            }
            _currentlySelectedItem?.Pages[PageType.ShellPage].Show();
            ShellPageButton.BackColor = Color.Peru;
            ScreenshotPageButton.BackColor = Color.Black;
            FilePageButton.BackColor = Color.Black;
            InfoPageButton.BackColor = Color.Black;
        }

        private void ScreenshotPageButton_Click(object sender, EventArgs e)
        {
            if (ReShCaller.SelectedClientId == Guid.Empty)
            {
                return;
            }
            foreach (Control control in PagePanel.Controls)
            {
                control.Hide();
            }
            _currentlySelectedItem?.Pages[PageType.ScreenshotPage].Show();
            ShellPageButton.BackColor = Color.Black;
            ScreenshotPageButton.BackColor = Color.Peru;
            FilePageButton.BackColor = Color.Black;
            InfoPageButton.BackColor = Color.Black;
        }

        private async void ReconnectButton_Click(object sender, EventArgs e)
        {
            ReShCaller.Disconnect();
            ConnectionStatusLabel.Text = "Connecting.";
            bool isConnected = await ReShCaller.ConnectAsync();
            if (isConnected)
            {
                ConnectionStatusLabel.Text = "Connected.";
            }
            else
            {
                ConnectionStatusLabel.Text = "Failed to Connect.";
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            ReShCaller.Disconnect();
            Close();
        }


        bool isMouseDown = false;
        Point OriginalPoint = new Point(0, 0);

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OriginalPoint = e.Location;
                isMouseDown = true;
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point NewPoint = e.Location;
                Location = new Point(Location.X + NewPoint.X - OriginalPoint.X, Location.Y + NewPoint.Y - OriginalPoint.Y);
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private void PagePanel_Resize(object sender, EventArgs e)
        {
            foreach (Control control in PagePanel.Controls)
            {
                control.Size = PagePanel.ClientSize;
            }
        }

        private void ClientListItem_ItemClicked(object sender, EventArgs e)
        {
            if (sender is ClientListItem clickedControl)
            {
                if (_currentlySelectedItem != null && _currentlySelectedItem != clickedControl)
                {
                    _currentlySelectedItem.IsSelected = false;
                }
                clickedControl.IsSelected = true;
                _currentlySelectedItem = clickedControl;

                ReShCaller.SelectedClientId = clickedControl.ClientInfo.ClientId;
            }
        }
    }
}
