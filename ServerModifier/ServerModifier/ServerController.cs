using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MoonByte.IO.Server
{
    public class ServerController
    {

        #region Vars

        List<string> ConsoleLine = new List<string>();
        Process ServerInstance;
        public string Name;

        #endregion

        #region Starting Process

        public ServerController(string name) { this.Name = name; }
        public void StartServer(string StartFile, string ServerDirectory, string StartArgs)
        {
            ProcessStartInfo info = new ProcessStartInfo(StartFile, StartArgs);
            info.WorkingDirectory = ServerDirectory;
            Console.WriteLine("Server Directory : " + ServerDirectory);

            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;

            ServerInstance = new Process(); ServerInstance.StartInfo = info;

            ServerInstance.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);

            ServerInstance.Start();

            ServerInstance.BeginOutputReadLine();

        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            ServerInstance.Kill();
            ServerInstance.Dispose();
        }

        #endregion

        #region GetOutput

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outline)
        {
            ConsoleLine.Add(outline.Data);
        }
        public string GetServerOutput()
        {
            return string.Join(Environment.NewLine, ConsoleLine);
        }

        #endregion

        #region Import

        public void InputCommand(string Command)
        {
            StreamWriter sw = ServerInstance.StandardInput;
            sw.WriteLine(Command);
        }

        #endregion

        #region Stop Process

        public void StopServer()
        { try { ServerInstance.Kill(); } catch { } }

        #endregion

        #region Check if process is closed

        public bool IsOnline()
        { try { Process.GetProcessById(ServerInstance.Id); return true; } catch { return false; } }

        #endregion

        #region GetJavaPath

        private string GetJavaInstallationPath()
        {
            string environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(environmentPath))
            {
                return environmentPath;
            }

            string javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment\\";
            using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(javaKey))
            {
                string currentVersion = rk.GetValue("CurrentVersion").ToString();
                using (Microsoft.Win32.RegistryKey key = rk.OpenSubKey(currentVersion))
                {
                    return key.GetValue("JavaHome").ToString();
                }
            }
        }

        #endregion

    }
}
