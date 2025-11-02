
using System.IO.Pipes;

namespace ReShServer
{

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            RSServer server;
            server = new RSServer(65432);
            MyLogger.LogDebug("ReSh Server is running...");
            await server.StartAsync();

        }
    }
}