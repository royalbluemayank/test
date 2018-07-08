using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sockets;

namespace TVSService
{
    public partial class TVSService : ServiceBase
    {
        
        #region Variables

        //mayank static Listener listener;
        //Client client;
        //Thread thread;
        System.Timers.Timer timer;
        private static object _lockforProcessdata = new object();
        private static object _lockforSendData = new object();
        #endregion 
        int i = 0;
        static int m_Count = 0;
        SocketServer socketServer;
        SocketClient pSocket;
        int Port = 8123;

        #region Constructor
        public TVSService()
        {
            InitializeComponent();
            try
            {
                // Instantiate a CSocketServer object
                socketServer = new SocketServer();
                // Start listening for connections
                socketServer.Start("localhost", Port, 1024,null,
                    new Sockets.MessageHandler(MessageHandlerServer),
                    new Sockets.SocketServer.AcceptHandler(AcceptHandler),
                    new Sockets.CloseHandler(CloseHandler),
                    new Sockets.ErrorHandler(ErrorHandler));
                Console.WriteLine("Waiting for client on Machine: {0} Port: {1}", System.Environment.MachineName, Port);
                // Stay here until you are ready to shutdown the server    
                //Console.ReadLine();
                //socketServer.Dispose();

                //mayank listener = new Listener(8);
                //mayank listener.SocketAccepted += L_SocketAccepted;
                //mayank listener.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex :" + ex.StackTrace);
            }
        }
        #endregion

        #region Methods
        public void OnDebug()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                Console.WriteLine("Service Started");
                Constants.Log("OnStart", "-----------------------------------------------------------------" + 
                    Environment.NewLine + "Service Started @" + DateTime.Now.ToString());
                //mayank listener.Start();
                //timer = new System.Timers.Timer();
                //timer.Interval = 1000;
                //timer.Elapsed += Timer_Elapsed;
                //timer.Enabled = true;
                //timer.Start();
                Constants.Log("OnStart", "listener : Start");
            }
            catch (Exception ex)
            {
                Constants.Log("OnStart exception", ex.StackTrace);
            }
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("" + i++);
        }

        //private void HookManager_KeyDown(object sender, KeyEventArgs e)
        //{
        //    Console.WriteLine(string.Format("KeyDown - {0}", e.KeyCode));
        //}
        //private void HookManager_KeyUp(object sender, KeyEventArgs e)
        //{
        //    Console.WriteLine(string.Format("KeyUp event - {0}", (char)e.KeyValue));
        //}
        //private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    Console.WriteLine(string.Format("KeyPress - {0}", e.KeyChar));
        //}
        //private void L_SocketAccepted(Socket e)
        //{
        //    //client = new Client(e);
        //    //Console.WriteLine("L_SocketAccepted" ,"Client ID : " + client.ID + " EndPoint : " + client.EndPoint);
        //    //Client.IsClientConnected = true;
        //    //client.Received += C_Received;
        //    //client.Disconnected += C_Disconnected;
        //    //Console.WriteLine("Hello from Server start");
        //    //byte[] buffer = Encoding.ASCII.GetBytes("Hello from Server");
        //    //e.Send(buffer, 0, buffer.Length, SocketFlags.None);
        //    //Console.WriteLine("Hello from Server stop");
        //}

        //private void C_Disconnected(Client sender)
        //{
        //    Console.WriteLine("Client C_Disconnected : ");
        //    Constants.Log("C_Disconnected","Client ID : " + sender.ID + " EndPoint : " + sender.EndPoint);
        //    Client.IsClientConnected = false;
        //}

        //private void C_Received(Client sender, byte[] data)
        //{
        //    try
        //    {
        //        Console.WriteLine("Received " + Encoding.ASCII.GetString(data));
        //        lock (this)
        //        {
        //            Thread thread = new Thread(() => ProcessCode(data));
        //            thread.IsBackground = true;
        //            thread.Start();
        //        }
        //        #region extras
        //        //Console.WriteLine("C_Received start");
        //        // "{\"Islogin\":" + Login + ",\"Value\":\"" + value + "\"}"
        //        //Console.WriteLine("data from client : " + value);
        //        //ClientService datavalue = JsonConvert.DeserializeObject<ClientService>(value);
        //        ////Console.WriteLine("Data IsLogin : " + datavalue.IsLogin);
        //        //if (datavalue.Islogin) //login  
        //        //{
        //        //    Console.WriteLine("Send to Server :" + datavalue.Product.Barcode);
        //        //    thread = new Thread(() => ProcessCode(datavalue.Product));
        //        //    thread.IsBackground = true;
        //        //    thread.Start();
        //        //    //Send to Server
        //        //}
        //        //else
        //        //{
        //        //    Constants.SaveAnonymousItem(datavalue.Product.Barcode);
        //        //    Console.WriteLine("Saved SaveAnonymousItem");
        //        //}
        //        //sender.Send(datavalue.ToJsonString());
        //        //Console.WriteLine("C_Received stop");
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        Constants.Log("C_Received exception", ex.StackTrace);
        //        //Console.WriteLine("==================================Exception ex 1 :" + Environment.NewLine + ex.Message
        //        //    + Environment.NewLine + ex.StackTrace+ Environment.NewLine + "===================================");
        //    }
        //}

        //String value = string.Empty;
        ClientService datavalue;
        //private void ProcessCode(byte[] data)
        private void ProcessCode(string value)
        {
            lock (this)
            {
                //value = Encoding.ASCII.GetString(data);
                datavalue = JsonConvert.DeserializeObject<ClientService>(value);
                if (datavalue.productType == ProductType.UserProduct) //login  
                {
                    //Constants.Log("ProcessCode for UserProduct", value);
                    Console.WriteLine("Received form client: " + datavalue.Product.Barcode);
                    Thread.Sleep(2000);
                    datavalue.Product.Amount = 5.00m;
                    datavalue.productType = ProductType.ValidUserProduct;
                    datavalue.Product.ProductName = datavalue.Product.Barcode;
                    Send(datavalue.ToJsonString());
                    //Send to Server
                }
                else
                {
                    //Constants.Log("ProcessCode for AnonymousProduct", value);
                    Constants.SaveAnonymousItem(datavalue.Product.Barcode);
                    Console.WriteLine("Saved as Anonymous : " + datavalue.Product.Barcode);
                }
            }
        }
        protected override void OnStop()
        {
            //listener.Stop();
            //Console.WriteLine("Service Stopped");
            Constants.Log("OnStop", "Service Stopped @" + DateTime.Now.ToString() + 
                Environment.NewLine + "-----------------------------------------------------------------");
        }
        public void Send(String Data)
        {
            lock (_lockforSendData)
            {
                String json = Data;
                //Console.WriteLine("Send back to Client : " + json);
                pSocket.Send(json);
            }
        }


        public static void ErrorHandler(SocketBase socket, Exception pException)
        {
            Console.WriteLine(pException.Message);
        }
        public static void CloseHandler(SocketBase socket)
        {
            Console.WriteLine("Connection Closed" + Environment.NewLine + "------------------------------");
            //Console.WriteLine("IpAddress: " + socket.IpAddress);
        }
        /// <summary> Called when a message is extracted from the socket </summary>
        /// <param name="pSocket"> The SocketClient object the message came from </param>
        /// <param name="iNumberOfBytes"> The number of bytes in the RawBuffer inside the SocketClient </param>
        public void MessageHandlerServer(SocketBase socket, int iNumberOfBytes)
        {
            try
            {
                pSocket = ((SocketClient)socket);
                // Find a complete message
                String strMessage = System.Text.ASCIIEncoding.ASCII.GetString(pSocket.RawBuffer, 0, iNumberOfBytes);
                Thread thread = new Thread(() => ProcessCode(strMessage));
                thread.IsBackground = true;
                thread.Start();
                //Console.WriteLine("Message=<{1}> Received {0} messages", m_Count++, strMessage);
            }
            catch (Exception pException)
            {
                Console.WriteLine(pException.Message);
            }
        }

        /// <summary> Called when a socket connection is accepted </summary>
        /// <param name="pSocket"> The SocketClient object the message came from </param>
        static public void AcceptHandler(SocketClient pSocket)
        {
            Console.WriteLine("------------------------------" + Environment.NewLine + "Client connected..");
            //Console.WriteLine("IpAddress: " + pSocket.IpAddress);
        }




        #endregion

        //public void MyKeyUp(object sender, KeyEventArgs e)
        //{
        //    Constants.Log(this.GetType().Name, "MyKeyUp");
        //    //================================================================================
        //    if (IsFirstCharacter || (DateTime.UtcNow.Ticks - _lastKeystroke) > 200000)
        //    {
        //        //bcode.Add((char)e.KeyValue);
        //        bcode.Clear();
        //        _lastKeystroke = DateTime.UtcNow.Ticks;
        //        IsFirstCharacter = false;
        //        //return;
        //    }
        //    long TicksNow = DateTime.UtcNow.Ticks;
        //    if (e.KeyValue == 13)
        //    {
        //        String value = new string(bcode.ToArray()).Trim();
        //        value = value.Replace("\0", "");
        //        bcode.Clear();
        //        if (value != String.Empty)
        //        {
        //            Send(value);
        //            //Console.WriteLine("value != Empty : " + value);
        //            //thread = new Thread(() => Send(value));
        //            //thread.IsBackground = true;
        //            //thread.Start();
        //        }
        //        IsFirstCharacter = true;
        //    }
        //    else
        //    {
        //        long taken = TicksNow - _lastKeystroke;
        //        //Console.WriteLine(String.Format("cforKeyDown : {0} = int keyupdata : {1} = char keyupdata : {2} = [ TicksNow : {3} - _lastKeystroke : {4} =  taken : {5} ]"
        //        //        , (char)e.KeyValue, (int)e.KeyData, (char)e.KeyData, TicksNow, _lastKeystroke, taken));
        //        if (taken < 200000)
        //        {
        //            bcode.Add((char)e.KeyValue);
        //        }
        //        else
        //        {
        //            bcode.Clear();
        //            IsFirstCharacter = true;
        //            //cforKeyDown = '\0';
        //        }
        //    }
        //    _lastKeystroke = DateTime.UtcNow.Ticks;
        //    return;
        //}

        //public void MyKeydown(object sender, KeyEventArgs e)
        //{
        //    //Console.WriteLine("MyKeydown : " + (int)e.KeyCode);
        //    _lastKeystroke = DateTime.UtcNow.Ticks;
        //}
        //public void MyKeyPress(object sender, KeyPressEventArgs e)
        //{
        //    //cforKeyDown = (char)e.KeyChar;
        //    //Console.WriteLine("MyKeyPress : " + (int)cforKeyDown);
        //}
    }
}
