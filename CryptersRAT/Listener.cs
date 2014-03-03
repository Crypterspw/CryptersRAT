using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

class Listener
{
    Socket s;
    string IP;
    public List<Info> clients;
    public delegate void ReceivedEventHandler(Listener l, Info i, string received);
    public event ReceivedEventHandler Received;
    public delegate void DisconnectedEventHandler(Listener l, Info i);
    public event DisconnectedEventHandler Disconnected;
    bool listening = false;
    public Listener()
    {
        clients = new List<Info>();
        s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public bool Running
    {
        get { return listening; }
    }

    public void BeginListen(int port)
    {
        s.Bind(new IPEndPoint(IPAddress.Any, port));
        s.Listen(100);
        s.BeginAccept(new AsyncCallback(AcceptCallback), s);
        listening = true;
    }

    public void StopListen()
    {
        if (listening == true)
        {
            s.Close();
        }
    }

    void AcceptCallback(IAsyncResult ar)
    {
        Socket handler = (Socket)ar.AsyncState;
        Socket sock = handler.EndAccept(ar);
        Info i = new Info(sock);
        clients.Add(i);

        Console.WriteLine("New Connection: " + i.ID.ToString());
        sock.BeginReceive(i.buffer, 0, i.buffer.Length, SocketFlags.None, new AsyncCallback(ReadCallback), i); 
        handler.BeginAccept(new AsyncCallback(AcceptCallback), handler); 
    }

    void ReadCallback(IAsyncResult ar)
    {
        Info i = (Info)ar.AsyncState;
        try
        {
            int rec = i.sock.EndReceive(ar);
            if (rec != 0)
            {
                string data = Encoding.ASCII.GetString(i.buffer, 0, rec);
                Received(this, i, data);
            }
            else
            {
                Disconnected(this, i);
                return;
            }

            i.sock.BeginReceive(i.buffer, 0, i.buffer.Length, SocketFlags.None, new AsyncCallback(ReadCallback), i); 
        }
        catch
        {
            Disconnected(this, i);
            i.sock.Close();
            clients.Remove(i);
        }
    }
}
