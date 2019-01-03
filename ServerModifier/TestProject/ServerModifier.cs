using IndieGoat.Net.Tcp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoonByte.PluginWrappers
{
    public class ServerModifier
    {
        UniversalClient client = new UniversalClient();
        string Command = "ServerModifier";
        public ServerModifier(string IP, int Port)
        {
            client.ConnectToRemoteServer(new UniversalConnectionObject(IP, Port));
        }

        public string GetServerDirectory(string Username, string Server) { return client.ClientSender.SendCommand(Command, new string[] { "GETSERVERDIRECTORY", Username, Server }); }
        public string StartServer(string Username, string Server, string StartFile, string ServerArgs) { return client.ClientSender.SendCommand(Command, new string[] { "STARTSERVER", Username, Server, StartFile, ServerArgs }); }
        public string SendConsoleCommand(string Username, string Server, string ConsoleCommand) { return client.ClientSender.SendCommand(Command, new string[] { "SENDCONSOLECOMMAND", Username, Server, ConsoleCommand }); }
        public string StopServer(string Username, string Server) { return client.ClientSender.SendCommand(Command, new string[] { "STOPSERVER", Username, Server }); }
        public string CreateServer(string Username, string Server) { return client.ClientSender.SendCommand(Command, new string[] { "CREATESERVER", Username, Server }); }
        public string DeleteServer(string Username, string Server) { return client.ClientSender.SendCommand(Command, new string[] { "DELETESERVER", Username, Server }); }
        public string IsOnline(string Username, string Server) { return client.ClientSender.SendCommand(Command, new string[] { "ISONLINE", Username, Server }); }
        public List<string> GetUserServers(string Username) { string s = client.ClientSender.SendCommand(Command, new string[] { "GETUSERSERVERS", Username, "none" }); return s.Split(new string[] { "%20%" }, StringSplitOptions.None).ToList(); }
        public List<string> GetConsoleInfo(string Username, string Server) { string s = client.ClientSender.SendCommand(Command, new string[] { "GETCONSOLEINFO", Username, Server }); return s.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList(); }
        public string ChangeExpirationDate(string Username, string Server, DateTime date) { return client.ClientSender.SendCommand(Command, new string[] { "CHANGEEXPIREDATE", Username, Server, date.ToShortDateString() }); }
        public string GetExpireDate(string Username, string Server) { return client.ClientSender.SendCommand(Command, new string[] { "GETEXPIREDATE", Username, Server }); }
        public string ChangeImageURL(string Username, string Server, string ImageURL) { return client.ClientSender.SendCommand(Command, new string[] { "CHANGEIMAGEURL", Username, Server, ImageURL }); }
        public string GetImageURL(string Username, string Server) { return client.ClientSender.SendCommand(Command, new string[] { "GETIMAGEURL", Username, Server }); }
        public string WriteSetting(string Username, string Server, string SettingName, string SettingValue)  { return client.ClientSender.SendCommand(Command, new string[] { "WRITESETTING", Username, Server, SettingName, SettingValue }); }
        public string ReadSetting(string Username, string Server, string SettingName) { return client.ClientSender.SendCommand(Command, new string[] { "READSETTING", Username, Server, SettingName }); }
        public string CheckSetting(string Username, string Server, string SettingName) { return client.ClientSender.SendCommand(Command, new string[] { "CHECKSETTING", Username, Server, SettingName }); }
    }
}
