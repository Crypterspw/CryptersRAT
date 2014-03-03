using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Info
{
  
    public Socket sock;
    public Guid ID;
    public string RemoteAddress;
    public byte[] buffer = new byte[8192];

    public Info(Socket sock)
    {
        this.sock = sock; 
        ID = Guid.NewGuid(); 
        RemoteAddress = sock.RemoteEndPoint.ToString(); 
    }

    public void Send(string data)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        sock.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback((ar) =>
        {
            sock.EndSend(ar);
        }), buffer);
    }
}
