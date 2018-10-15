using MoonByte.PluginWrappers;
using System;
using System.Windows.Forms;

namespace TestProject
{
    public partial class Form1 : Form
    {

        string Username = "Test User";
        string Server = "Test Server";
        string StartFile = "java.exe";
        string ServerArgs = "-Xmx1024M -Xms1024M -jar server.jar nogui";
        string ConsoleCommand = "say Hello!";

        ServerModifier mod = new ServerModifier("localhost", 7777);

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
    }
}
