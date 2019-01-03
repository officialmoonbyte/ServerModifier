using MoonByte.PluginWrappers;
using System;
using System.Windows.Forms;

namespace TestProject
{
    public partial class Form1 : Form
    {

        string Username = "test";
        string Server = "Testing!!";
        string StartFile = "java.exe";
        string ServerArgs = "-Xmx1024M -Xms1024M -jar server.jar nogui";
        string ConsoleCommand = "say Hello!";

        ServerModifier mod = new ServerModifier("192.168.0.16", 4543);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.CreateServer(Username, Server));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.GetServerDirectory(Username, Server));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.IsOnline(Username, Server));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.StartServer(Username, Server, StartFile, ServerArgs));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.IsOnline(Username, Server));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.GetConsoleInfo(Username, Server));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.SendConsoleCommand(Username, Server, ConsoleCommand));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.GetConsoleInfo(Username, Server));
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.StopServer(Username, Server));
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.DeleteServer(Username, Server));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.WriteSetting(Username, Server, "TE", "est"));
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.ReadSetting(Username, Server, "TE"));
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mod.CheckSetting(Username, Server, "TE"));
        }
    }
}
