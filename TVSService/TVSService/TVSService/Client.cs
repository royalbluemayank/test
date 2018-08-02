using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TVSService
{
    internal class Client
    {
        #region Events
        public event ClientReceivedHandler Received;
        public event ClientDisconnectedHandler Disconnected;
        #endregion

        #region Delegates
        public delegate void ClientReceivedHandler(Client sender, byte[] data);
        public delegate void ClientDisconnectedHandler(Client sender);
        #endregion

        #region Properties and Variables
        Socket sck;
        public static bool IsClientConnected = false;
        public string ID { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        #endregion

        #region Constructor
        public Client(Socket accepted)
        {
            sck = accepted;
            ID = Guid.NewGuid().ToString();
            EndPoint = (IPEndPoint)accepted.RemoteEndPoint;
            sck.BeginReceive(new byte[] { 0 }, 0, 0, 0, callback, null);
        }
        #endregion

        #region Methods
        public void Send(string Data)
        {
            Console.WriteLine("Send start");
            byte[] buffer = Encoding.ASCII.GetBytes(Data);
            sck.Send(buffer, 0, buffer.Length, SocketFlags.None);
            Console.WriteLine("Send stop");
        }
        void callback(IAsyncResult ar)
        {
            try
            {
                sck.EndReceive(ar);
                byte[] buf = new byte[1024];
                int rec = sck.Receive(buf, buf.Length, SocketFlags.None);
                if (rec < buf.Length)
                {
                    Array.Resize<byte>(ref buf, rec);
                }
                if (Received != null)
                {
                    Received(this, buf);
                }
                sck.BeginReceive(new byte[] { 0 }, 0, 0, 0, callback, null);
            }
            catch (Exception ex)
            {
                close();
                if (Disconnected != null)
                {
                    Disconnected(this);
                }
            }
        }
        public void close()
        {
            sck.Close();
            sck.Dispose();
        }
        #endregion

    }
}