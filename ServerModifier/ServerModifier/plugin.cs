using IndieGoat.UniversalServer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace MoonByte.IO.Server
{
    public class ServerModifier : IServerPlugin
    {

        #region Vars

        #region Default Vars

        public string Name { get { return "ServerModifier"; } }

        public event EventHandler<SendMessageEventArgs> SendMessage;

        #endregion Default Vars

        string UserDirectory;
        string PluginDirectory;
        List<string> Users = new List<string>();
        public string GetPersonalUserDirectory(string User)
        {
            return UserDirectory + "\\" + User;
        }

        List<serverControlObject> controllers = new List<serverControlObject>();

        #endregion Vars

        public class serverControlObject { public string Username; public ServerController serverController; }

        public void onLoad(string ServerDirectory)
        {

            PluginDirectory = ServerDirectory + "ServerModifier";
            Console.WriteLine(PluginDirectory);
            UserDirectory = PluginDirectory + "\\" + "Users";
            Console.WriteLine(UserDirectory);

            if (!Directory.Exists(UserDirectory)) Directory.CreateDirectory(UserDirectory);

            //Updates the Users list for the directories located. It is assumed you checked from UserDirectory if the user exist and is login.
            UpdateLocalData();

            //To dispose of the list of servers
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            //Initializing servers
            foreach(string user in Users)
            {
                DirectoryInfo[] servers = new DirectoryInfo(UserDirectory + "\\" + user).GetDirectories();
                foreach (DirectoryInfo server in servers)
                {
                    serverControlObject serverControl = new serverControlObject();
                    serverControl.Username = user;

                    string serverdirectory = GetServerDirectory(user, server.Name);

                    serverControl.serverController = new ServerController(server.Name, serverdirectory);
                    Console.WriteLine("Initialized " + server.Name + " controller for " + user);
                }
            }
        }

        public void Invoke(ClientSocketWorkload workload, ClientContext context, int port, List<string> Args, string ServerDirectory)
        {
            //ServerModifier [Command] [Username] [Server]
            string Command = Args[1].ToUpper();
            string UserProfile = Args[2];
            string ServerUI = Args[3];

            if (Command == "GETSERVERDIRECTORY")
            {
                List<string> UserServers = GetUserServers(UserProfile);

                bool s = false; foreach (string file in UserServers)
                {
                    if (file == ServerUI) { workload.SendMessage(context, UserDirectory + "\\" + UserProfile + "\\" + file); s = true; break; }
                } if (s == false) { workload.SendMessage(context, "invalid"); }
            }
            if(Command == "CHANGEENDDATE")
            {

            }
            if (Command == "STARTSERVER")
            {
                ServerController userServerController = GetServerController(UserProfile, ServerUI);
                if (userServerController != null)
                {
                    string startFile = Args[4];
                    string serverdirectory = GetServerDirectory(UserProfile, ServerUI);
                    string serverStartArgs = Args[5];
                    userServerController.StartServer(startFile, serverdirectory, serverStartArgs);
                    workload.SendMessage(context, "true");
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if (Command == "GETCONSOLEINFO")
            {
                ServerController userServerController = GetServerController(UserProfile, ServerUI);
                
                if (userServerController != null)
                {
                    workload.SendMessage(context, userServerController.GetServerOutput());
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if(Command == "SENDCONSOLECOMMAND")
            {
                ServerController userServerController = GetServerController(UserProfile, ServerUI);

                string consoleCommand = Args[4];
                if(userServerController != null)
                {
                    userServerController.InputCommand(consoleCommand);
                    workload.SendMessage(context, "true");
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if (Command == "STOPSERVER")
            {
                ServerController userServerController = GetServerController(UserProfile, ServerUI);

                if (userServerController != null)
                {
                    userServerController.StopServer();
                    workload.SendMessage(context, "true");
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if (Command == "CREATESERVER")
            {
                string UserDirectory = GetPersonalUserDirectory(UserProfile);

                if (Directory.Exists(UserDirectory + "\\" + ServerUI))
                {
                    workload.SendMessage(context, "invalid");
                }
                else
                {
                    string serverdirectory = GetServerDirectory(UserProfile, ServerUI);

                    Directory.CreateDirectory(UserDirectory + "\\" + ServerUI);
                    serverControlObject controllerObject = new serverControlObject();
                    ServerController controller = new ServerController(ServerUI, serverdirectory);
                    controllerObject.serverController = controller;
                    controllerObject.Username = UserProfile;
                    controllers.Add(controllerObject);
                    workload.SendMessage(context, "true");
                }
            }
            if (Command == "DELETESERVER")
            {
                string UserDirectory = GetPersonalUserDirectory(UserProfile);

                if (Directory.Exists(UserDirectory + "\\" + ServerUI))
                {
                    Directory.Delete(UserDirectory + "\\" + ServerUI, true);
                    workload.SendMessage(context, "true");
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if (Command == "ISONLINE")
            {
                ServerController controller = GetServerController(UserProfile, ServerUI);

                if (controller != null)
                {
                    bool isOn = controller.IsOnline();
                    if (isOn == true) workload.SendMessage(context, "yes");
                    if (isOn == false) workload.SendMessage(context, "no");
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if(Command == "GETUSERSERVERS")
            {
                workload.SendMessage(context, string.Join("%20%", GetUserServers(UserProfile)));
            }
            if (Command == "CHANGEEXPIREDATE")
            {
                string dateString = Args[4];
                DateTime incomingDate = DateTime.Today;
                try { incomingDate = DateTime.Parse(dateString); }
                catch { workload.SendMessage(context, "Failed to parse DateString"); }

                ServerController controller = GetServerController(UserProfile, ServerUI);
                
                if(controller != null)
                {
                    if (controller.ChangeExpireTime(incomingDate) == true)
                    { workload.SendMessage(context, "true");
                    } else { workload.SendMessage(context, "falsec"); }
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if (Command == "GETEXPIREDATE")
            {
                ServerController controller = GetServerController(UserProfile, ServerUI);

                if (controller != null)
                {
                    workload.SendMessage(context, controller.GetExpireTime().ToString());
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if (Command == "CHANGEIMAGEURL")
            {
                string ImageURL = Args[4];
                ServerController controller = GetServerController(UserProfile, ServerUI);

                if (controller != null)
                {
                    if (controller.ChangeImageURL(ImageURL)) { workload.SendMessage(context, "true"); }
                    else { workload.SendMessage(context, "falsec"); }
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if (Command == "GETIMAGEURL")
            {
                ServerController controller = GetServerController(UserProfile, ServerUI);

                if (controller != null)
                {
                    workload.SendMessage(context, controller.GetImageURL());
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if (Command == "WRITESETTING")
            {
                string SettingName = Args[4];
                string SettingValue = Args[5];
                ServerController controller = GetServerController(UserProfile, ServerUI);

                if (controller != null)
                {
                    if (controller.WriteSetting(SettingName, SettingValue)) { workload.SendMessage(context, "true"); }
                    else { workload.SendMessage(context, "falsec"); }
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if (Command == "READSETTING")
            {
                string SettingName = Args[4];

                ServerController controller = GetServerController(UserProfile, ServerUI);

                if (controller != null)
                {
                    workload.SendMessage(context, controller.ReadSetting(SettingName));
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
            if (Command == "CHECKSETTING")
            {
                string SettingName = Args[4];

                ServerController controller = GetServerController(UserProfile, ServerUI);

                if (controller != null)
                {
                    if (controller.CheckSetting(SettingName))
                    {
                        workload.SendMessage(context, "true");
                    }
                    else { workload.SendMessage(context, "false"); }
                }
                else
                {
                    workload.SendMessage(context, "invalid");
                }
            }
        }

        public void Unload()
        {
        }

        #region Update local data

        private void UpdateLocalData()
        {
            DirectoryInfo[] dirInfo = new DirectoryInfo(UserDirectory).GetDirectories();
            foreach(DirectoryInfo dir in dirInfo) { Users.Add(dir.Name); }
        }

        #endregion

        #region Get User Servers

        public List<string> GetUserServers(string Username)
        {
            List<string> returnList = new List<string>();
            DirectoryInfo[] dirInfo = new DirectoryInfo(GetPersonalUserDirectory(Username)).GetDirectories();
            foreach(DirectoryInfo dir in dirInfo) { returnList.Add(dir.Name); } return returnList;
        }

        public ServerController GetServerController(string Username, string ServerName)
        {
            foreach(serverControlObject obj in controllers)
            {
                if (obj.Username == Username & obj.serverController.Name == ServerName)
                {
                    return obj.serverController;
                }
            }

            return null;
        }

        public string GetServerDirectory(string Username, string ServerName)
        {
            string returnString = null;
            bool s = false; foreach (string file in GetUserServers(Username))
            {
                if (file == ServerName) { returnString = UserDirectory + "\\" + Username + "\\" + file;  }
            }
            return returnString;
        }

        #endregion

        #region Dispose when application exit

        public void CurrentDomain_ProcessExit(object sender, EventArgs e) { foreach (serverControlObject controller in controllers) { controller.serverController.Dispose(); } }

        #endregion
    }
}
