using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace TVSService
{
    //internal class Listener
    //{
    //    Socket s;
    //    public bool listening { get; private set; }
    //    public int Port { get; private set; }
    //    public Listener(int port)
    //    {
    //        Port = port;
    //        s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //    }
    //    public void Start()
    //    {
    //        if (listening)
    //            return;
    //        s.Bind(new IPEndPoint(0, Port));
    //        s.Listen(0);

    //        s.BeginAccept(callback, null);
    //        listening = true;
    //    }

    //    public void Stop()
    //    {
    //        if (!listening)
    //            return;

    //        s.Close();
    //        s.Dispose();
    //        s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //    }

    //    public void callback(IAsyncResult ar)
    //    {
    //        try
    //        {
    //            Socket s = this.s.EndAccept(ar);
    //            if (SocketAccepted != null)
    //                SocketAccepted(s);

    //            this.s.BeginAccept(callback, null);
    //        }
    //        catch (Exception ex)
    //        {
    //            System.Windows.Forms.MessageBox.Show(ex.Message);
    //        }
    //    }

    //    public delegate void SocketAcceptedHandler(Socket e);
    //    public event SocketAcceptedHandler SocketAccepted;
    //}
}