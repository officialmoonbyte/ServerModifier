using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Timers;

namespace MoonByte.IO.Server
{
    public class ServerController
    {

        #region Vars

        List<string> ConsoleLine = new List<string>();
        Process ServerInstance;
        public string Name;
        Timer expiredTimer = new Timer(960000);

        #region Paths

        string ServerDirectory;

        #endregion
        #endregion

        #region Startup

        public ServerController(string name, string ServerDirectory)
        {
            this.Name = name;
            this.ServerDirectory = ServerDirectory;
            expiredTimer.Elapsed += new ElapsedEventHandler(CheckExpireAuto);
            expiredTimer.Enabled = true;
        }

        #endregion

        #region Settings

        private void WriteSetting(string SettingName, string SettingValue)
        {
            string FilePath = ServerDirectory + @"\" + SettingName + ".set";
            if (!File.Exists(FilePath)) File.Create(FilePath).Close();
            File.WriteAllText(FilePath, SettingValue);
        }

        private string ReadSetting(string SettingName)
        {
            string FilePath = ServerDirectory + @"\" + SettingName + ".set";
            if (File.Exists(FilePath)) { return File.ReadAllText(FilePath); }
            else { return "file does not exists!"; }
        }

        #endregion

        #region Starting Process

        public void StartServer(string StartFile, string ServerDirectory, string StartArgs)
        {
            if (!RunExpireCheck()) { return; }
            ProcessStartInfo info = new ProcessStartInfo(StartFile, StartArgs);
            info.WorkingDirectory = ServerDirectory;

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
        { if (this.IsOnline()) { try { ServerInstance.Kill(); } catch { } } }

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

        #region Expire

        private void CheckExpireAuto(object source, ElapsedEventArgs e) { if (!RunExpireCheck()) { if (IsOnline()) { StopServer(); } } }

        DateTime expireTime;

        private bool RunExpireCheck() { if (expireTime == null) expireTime = GetExpireTime(); if (expireTime < DateTime.Now) { return false; } else { return true; } }
        private DateTime GetExpireTime() { return DateTime.Parse(ReadSetting("expire")); }
        public bool ChangeExpireTime(DateTime time)
        {
            try
            {
                WriteSetting("expire", time.ToShortDateString());
                expireTime = time;

                return true;
            } catch { return false; }
        }

        #endregion

    }
}
