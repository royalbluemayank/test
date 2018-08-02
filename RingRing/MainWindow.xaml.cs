﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Sockets;
using System.Management;
using RawInput_dll;
using System.Windows.Interop;
using System.Timers;
using System.Collections;
using System.Security.Permissions;
//using gma.System.Windows;

namespace RingRing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Process posApp;
        private ManagementEventWatcher startWatch;
        private System.Timers.Timer keyboardTimer = new System.Timers.Timer(20.0);
        static private System.Timers.Timer TVSServiceTimer = new System.Timers.Timer(2000);
        //Fetch Main Window
        [DllImport("user32.dll",  CharSet = CharSet.Auto)]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);
        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();
            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
        private bool IsPOSinFocus()
        {
            if (this.posApp != null)
            {
                uint ProcessId;
                GetWindowThreadProcessId(GetForegroundWindow(), out ProcessId);
                if ((long)this.posApp.Id == (long)ProcessId)
                    return true;
            }
            return false;
        }

        private bool IsPOSRunning()
        {
            if (this.posApp != null)
                return true;
            Process[] processesByName = Process.GetProcessesByName(ActivePos);
            if (processesByName.Length == 0)
                return false;
            this.posApp = processesByName[0];
            this.posApp.EnableRaisingEvents = true;
            this.posApp.Exited += new EventHandler(this.PosApp_Exited);
            //mayank this.Invoke((Delegate) (() => this.labelStatus.ForeColor = Color.ForestGreen));
            return true;
        }

        private void PosApp_Exited(object sender, EventArgs e)
        {
            this.posApp = (Process)null;
            this.MonitorPOSAppStart();
        }

        private void MonitorPOSAppStart()
        {
            this.startWatch = new ManagementEventWatcher("SELECT *  FROM __InstanceOperationEvent WITHIN  1 WHERE TargetInstance ISA 'Win32_Process' AND TargetInstance.Name = '"+ PosExe+"'");
            this.startWatch.EventArrived += new EventArrivedEventHandler(this.StartWatch_EventArrived);
            this.startWatch.Start();
        }
        private void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            this.startWatch.EventArrived -= new EventArrivedEventHandler(this.StartWatch_EventArrived);
            this.startWatch.Stop();
            this.startWatch = (ManagementEventWatcher)null;
            this.IsPOSRunning();
        }


        ICollectionView view, Txnview;
        public bool clicked, sametext = true;
        private static object _lock = new object();
        private bool Isbackpressed, loginfailedflag = false;
        Store store;
        String text, previousText = string.Empty, textdash = " - ";
        int caretno;
        long _lastKeystroke = DateTime.Now.Ticks;
        Order order;
        private RawInput _rawinput;
        private ArrayList scanCode = new ArrayList();
        //UserActivityHook CaptureHook = null;
        //Hook CaptureHook = null;
        List<char> bcode = new List<char>(10);
        ///Product product = null;
        Order.UserInfo user;
        TVSService tvsService = null;
        Thread thread, threadforConnection = null;
        //int counterkeydown, counterkeypress = 0;
        //Socket ClientSocket;
        bool IsFirstCharacter = true;
        bool IsconnectedToService= false;
        private string vid = "";
        private string testCode = "";
        long TimeoutforKey = 200000;
        SocketClient pSocketClient;
        private static object _lockforProcessdata = new object();
        Dictionary<String, Product> tempProducts;
        String ActivePos, PosExe = null;
        
        public delegate void ClientReceivedHandler(byte[] data);
        public event ClientReceivedHandler Receiveddata;

        public delegate void AddProductHandler(Product value);
        private event AddProductHandler addProductEvent;
        public delegate void UpdatePro();
        private event UpdatePro m_updatePro;
        public MainWindow() 
        {
            InitializeComponent();
            this.Topmost = true;
            this.Left = SystemParameters.WorkArea.Width - this.Width - 20;
            this.Top = 5;
            long temp = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
            if (temp != 0)
                TimeoutforKey = temp;
            string tempPos = System.Configuration.ConfigurationManager.AppSettings["Pos"];
            if (tempPos != null)
                ActivePos = tempPos;
            string tempPosExe = System.Configuration.ConfigurationManager.AppSettings["PosExe"];
            if (tempPosExe != null)
                PosExe = tempPosExe;
            SettingStore();
        }

        private void KeyboardTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.keyboardTimer.Stop();
            this.scanCode.Clear();
            this.testCode = "";
        }
        private void SettingStore()
        {
            if (Transaction.Introspect())
            {
                if (Transaction.RetrieveStore())
                {
                    store = new Store(Transaction.retrievestoreresponse.oid.ToString(), Transaction.retrievestoreresponse.displayName);
                    //store = new Store(System.Configuration.ConfigurationManager.AppSettings["StoreId"], System.Configuration.ConfigurationManager.AppSettings["StoreName"]);
                    _lblStoreName.Content = store.StoreName;
                    _lblTvsId.Content = store.fullTvsId;

                    if (!Transaction.SetStore("1285091"))
                    {
                        System.Windows.Forms.MessageBox.Show("CLosing Application", "SetStore");
                        this.Close();
                    }
                    tempProducts = new Dictionary<string, Product>();
                    //textBox.Focus();
                    //Console.Write("Active Window :" + GetActiveWindowTitle());
                    this.MonitorPOSAppStart();
                    addProductEvent = new AddProductHandler(addProductToOrder);
                    m_updatePro = new UpdatePro(updateCart);
                    StartCapture();
                    //CreateRequest();
                    threadforConnection = new Thread(ConnectToTvsInLoop);
                    threadforConnection.IsBackground = true;
                    threadforConnection.Start();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("CLosing Application", "RetrieveStore");
                    this.Close();
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("CLosing Application", "Introspect");
                this.Close();
            }
            //String m = AppDomain.CurrentDomain.FriendlyName;
            //Console.WriteLine(m);
        }
        private void CreateRequestwithService()
        {
            try
            {
                 pSocketClient = new SocketClient(10240, null,
                    new Sockets.MessageHandler(MessageHandlerClient),
                    new Sockets.CloseHandler(CloseHandler),
                    new Sockets.ErrorHandler(ErrorHandler));

                pSocketClient.Connect("localhost", 8123);

                //for (int i = 0; i < 100; i++)
                //    pSocketClient.Send("My Message");

                //Console.ReadLine();
                //pSocketClient.Disconnect();
                //Console.ReadLine();
                //pSocketClient.Dispose();

                //ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //ClientSocket.Connect("127.0.0.1", 8);
                IsconnectedToService = true;
                Console.WriteLine("Connected to Server");
                ////byte[] buffer = Encoding.ASCII.GetBytes("hello from client 1");
                ////ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                //ClientSocket.BeginReceive(new byte[] { 0 }, 0, 0, 0, callback, null);
                //this.Receiveddata += ReceivedatafromService;
                ////byte[] buffer2 = Encoding.ASCII.GetBytes("hello from client 2");
                ////ClientSocket.Send(buffer2, 0, buffer2.Length, SocketFlags.None);
            }
            catch(Exception ex)
            {
                //Console.WriteLine("error:  " + ex.Message);
                //ClientSocket = null;
            }
        }
        //private void ReceivedatafromService(byte[] data)
        //{

        //    try
        //    {
        //        Console.WriteLine("data :" + data);
        //        thread = new Thread(() => ProcessCode(data));
        //        thread.IsBackground = true;
        //        thread.Start();
              
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("==================================Exception ex 1 :" + Environment.NewLine + ex.Message
        //            + Environment.NewLine + ex.StackTrace + Environment.NewLine + "===================================");
        //    }

           
        //    //tempProducts.TryGetValue(product.ProductID, out product);
        //    //Console.WriteLine("Applicable : true");
        //    //Console.WriteLine("Data from server : " + value);
        //    //Console.WriteLine("i Ring Ring : " + i++);
        //    //Application.Current.Dispatcher.Invoke(new Action(() => { addProduct(value); }));
        //    //Application.Current.Dispatcher.Invoke(new Action(() => { updateCart(); }));
        //}

        public void MessageHandlerClient(SocketBase pSocket, Int32 iNumberOfBytes)
        {
            try
            {
                pSocket = ((SocketClient)pSocket);
                // Find a complete message
                String strMessage = System.Text.ASCIIEncoding.ASCII.GetString(pSocket.RawBuffer, 0, iNumberOfBytes);
                //Console.WriteLine("data :" + strMessage);
                Thread thread = new Thread(() => ProcessCode(strMessage));
                thread.IsBackground = true;
                thread.Start();


                
                //thread = new Thread(() => ProcessCode(data));
                //thread.IsBackground = true;
                //thread.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine("==================================Exception ex 1 :" + Environment.NewLine + ex.Message
                    + Environment.NewLine + ex.StackTrace + Environment.NewLine + "===================================");
            }

            //try
            //{
            //    // Convert the message from a byte array to a string
            //    String strMessage = System.Text.ASCIIEncoding.ASCII.GetString(pSocket.RawBuffer, 0, iNumberOfBytes);

            //    // Display the string to the console window
            //    Console.WriteLine(strMessage);
            //}
            //catch (Exception pException)
            //{
            //    Console.WriteLine(pException.Message);
            //}
        }
        //*********************************************
        /// <summary> Called when a socket connection is closed </summary>
        /// <param name="pSocket"> The SocketClient object the message came from </param>
        public void CloseHandler(SocketBase pSocket)
        {
            IsconnectedToService = false;
            threadforConnection = new Thread(ConnectToTvsInLoop);
            threadforConnection.IsBackground = true;
            threadforConnection.Start();

            //Console.WriteLine("Server connection closed");
            //TVSServiceTimer.Elapsed += TVSServiceTimer_Elapsed;
            //TVSServiceTimer.Start();
            //Console.WriteLine("IpAddress: " + pSocket.IpAddress);
        }

        public void ConnectToTvsInLoop()
        {
            Console.WriteLine("Disconnected to Server");
            do
            {
                CreateRequestwithService();
            } while (!IsconnectedToService);
        }

        private void TVSServiceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CreateRequestwithService();
            if (IsconnectedToService)
            {
                TVSServiceTimer.Stop();
                TVSServiceTimer.Elapsed -= TVSServiceTimer_Elapsed;
            }
        }

        //**************************************************
        /// <summary> Called when a socket error occurs </summary>
        /// <param name="pSocket"> The SocketClient object the message came from </param>
        /// <param name="pException"> The reason for the error </param>
        static public void ErrorHandler(SocketBase pSocket, Exception pException)
        {
            Console.WriteLine(pException.Message);
        }

        //private void ProcessCode(byte[] data)
        private void ProcessCode(string data)
        {
            lock (_lockforProcessdata)
            {
                //String value = Encoding.ASCII.GetString(data);
                String value = data;
                Console.WriteLine("data received from server: " + value);
                Constants.Logger("data received from server: " + value);

                TVSService tvsService = JsonConvert.DeserializeObject<TVSService>(value);

                if (tempProducts.ContainsKey(tvsService.Product.ProductID) && tvsService.productType == TVSService.ProductType.ValidUserProduct)
                {
                    Product product = new Product();
                    tempProducts.TryGetValue(tvsService.Product.ProductID, out product);
                    product.Amount = tvsService.Product.Amount;
                    product.ProductName = tvsService.Product.ProductName;
                    this.Dispatcher.Invoke(addProductEvent, product);
                    tempProducts.Remove(tvsService.Product.ProductID);
                }
                else if (tvsService.productType == TVSService.ProductType.NotValidUserProduct)
                {
                    tempProducts.Remove(tvsService.Product.ProductID);
                }
                else
                {
                    //else condition code
                }
            }
        }

        private void evalCode()
        {
            if (this.scanCode.Count > 5)
            {
                String m = this.parseScanCode(this.scanCode);
                Console.WriteLine(m);
                thread = new Thread(() => SendtoTVSService(m));
                thread.IsBackground = true;
                thread.Start();
            }
            this.scanCode.Clear();
            this.testCode = "";
        }

        private string parseScanCode(ArrayList scancode)
        {
            string str1 = "";
            bool flag = false;
            for (int index = 0; index < scancode.Count; ++index)
            {
                if ((int)(uint)scancode[index] == 162 || (int)(uint)scancode[index] == 163 || (int)(uint)scancode[index] == 17)
                {
                    ++index;
                }
                else
                {
                    if ((int)(uint)scancode[index] == 13)
                        return str1;
                    char ch1;
                    if ((int)(uint)scancode[index] == 161 || (int)(uint)scancode[index] == 160 || (int)(uint)scancode[index] == 16)
                        flag = true;
                    else if ((uint)scancode[index] >= 32U && (uint)scancode[index] <= 64U)
                    {
                        byte num = (byte)(uint)scancode[index];
                        string str2 = str1;
                        ch1 = (char)num;
                        string str3 = ch1.ToString();
                        str1 = str2 + str3;
                    }
                    else if ((uint)scancode[index] >= 65U && (uint)scancode[index] <= 90U)
                    {
                        byte num = (byte)(uint)scancode[index];
                        if (!flag)
                        {
                            string str2 = str1;
                            ch1 = (char)((uint)num + 32U);
                            string str3 = ch1.ToString();
                            str1 = str2 + str3;
                        }
                        else
                        {
                            string str2 = str1;
                            ch1 = (char)num;
                            string str3 = ch1.ToString();
                            str1 = str2 + str3;
                        }
                    }
                    else if ((uint)scancode[index] >= 97U && (uint)scancode[index] <= 122U)
                    {
                        byte num = (byte)(uint)scancode[index];
                        string str2 = str1;
                        ch1 = (char)num;
                        string str3 = ch1.ToString();
                        str1 = str2 + str3;
                    }
                    else
                    {
                        str1 += (char)((uint)scancode[index]);
                        //char ch2;
                        //if ((int)(ch2 = this.getChar((uint)scancode[index])) != 0)
                        //    str1 += ch2.ToString();
                        //else
                        //    break;
                    }
                }
            }
            this.scanCode.Clear();
            return "";
        }

        private char getChar(uint code)
        {
            switch (code)
            {
                case 186:
                    return ';';
                case 187:
                    return '+';
                case 188:
                    return ',';
                case 189:
                    return '-';
                case 190:
                    return '.';
                case 191:
                    return '?';
                case 192:
                    return '`';
                case 219:
                    return '{';
                case 220:
                    return '|';
                case 221:
                    return '}';
                case 222:
                    return '\'';
                case 226:
                    return '\\';
                case 254:
                    return ' ';
                default:
                    return char.MinValue;
            }
        }
        void addProductToOrder(Product product)
        {
            if (product.Applicable)
            {
                order.AddProduct(product);
                updateCart();
            }
        }
        //void callback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        ClientSocket.EndReceive(ar);
        //        byte[] buf = new byte[1024];
        //        int rec = ClientSocket.Receive(buf, buf.Length, SocketFlags.None);
        //        if (rec < buf.Length)
        //        {
        //            Array.Resize<byte>(ref buf, rec);
        //        }
        //        if (Receiveddata != null)
        //        {
        //            Receiveddata(buf);
        //        }
        //        ClientSocket.BeginReceive(new byte[] { 0 }, 0, 0, 0, callback, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.Forms.MessageBox.Show(ex.Message);
        //    }
        //}
        public void CreateOrder()
        {
            if (order == null)
            {
                //TransactionStart = false;
                order = new Order(Transaction.createtxnresponse.oid.ToString());
                Console.WriteLine("Creating a new Order, No is " + order.OrderNumber);
                user = new Order.UserInfo(Transaction.createtxnresponse.consumerName, Transaction.createtxnresponse.consumerPin);
                _lbl_UserName.Content = user.Name.Substring(0, user.Name.IndexOf(' '));
                //cforKeyDown = '\0';
                bcode.Clear();
                view = CollectionViewSource.GetDefaultView(order.products);
                lvItems.ItemsSource = view;
                _lblHeader.Content = Constants.HeaderProductdescription;
                updateCart();
                //thread.Start();
                //System.Windows.Forms.MessageBox.Show("Test");
            }
        }
        private void StartCapture()
        {
            try
            {
                _rawinput = new RawInput(new WindowInteropHelper(this).EnsureHandle(), false);
                this._rawinput.AddMessageFilter();
                this._rawinput.KeyPressed += new RawKeyboard.DeviceEventHandler(this.OnKeyPressed);
                this.keyboardTimer.Elapsed += new ElapsedEventHandler(this.KeyboardTimer_Elapsed);
                //CaptureHook = new UserActivityHook();
                //CaptureHook.KeyUp += HookKeyUp;
                #region extra comments
                //CaptureHook.KeyPress += new System.Windows.Forms.KeyPressEventHandler(MyKeyPress);
                //CaptureHook.KeyDown += new System.Windows.Forms.KeyEventHandler(MyKeydown);
                //CaptureHook.KeyUp += new System.Windows.Forms.KeyEventHandler(MyKeyUp);
                //CaptureHook.Start();
                //CaptureHook = new Hook();
                #endregion
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void OnKeyPressed(object sender, RawInputEventArg e)
        {
            this.keyboardTimer.Stop();
            this.keyboardTimer.Start();
            string[] strArray = e.KeyPressEvent.DeviceName.Split('#');
            if (strArray.Length < 2)
                return;
            if (this.scanCode.Count == 0)
            {
                this.keyboardTimer.Stop();
                this.keyboardTimer.Start();
                this.vid = strArray[1];
            }
            else if (this.vid != strArray[1])
            {
                this.scanCode.Clear();
                this.keyboardTimer.Stop();
                this.keyboardTimer.Start();
                this.vid = strArray[1];
            }
            if ((int)e.KeyPressEvent.Message == 257)
                return;
            object j = (object)(uint)e.KeyPressEvent.VKey;
            //Console.WriteLine(j);
            this.scanCode.Add(j);
            this.testCode = this.testCode + e.KeyPressEvent.VKeyName + " ";
            if (e.KeyPressEvent.VKey != 13)
                return;
            this.keyboardTimer.Stop();
            this.evalCode();
        }


        //public void HookKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    long TicksNowbasic = DateTime.Now.Ticks;
        //    long difference = TicksNowbasic - _lastKeystroke;
        //    if (IsFirstCharacter || difference > TimeoutforKey)
        //    {
        //        bcode.Clear();
        //        //Console.WriteLine("---------------------------------------------------------------------------");
        //        Constants.Logger(String.Format("--------------------IsFirstCharacter : {0} = cforKeyDown : {1} = int keyupdata : {2} = char keyupdata : {3} = [ TicksNow : {4} - _lastKeystroke : {5} =  taken : {6} ]----------------"
        //          , IsFirstCharacter, (char)e.KeyValue, (int)e.KeyData, (char)e.KeyData, TicksNowbasic, _lastKeystroke, difference));
        //        //Console.WriteLine("---------------------------------------------------------------------------");
        //        IsFirstCharacter = false;
        //        _lastKeystroke = DateTime.Now.Ticks;
        //    }
        //    long TicksNow = DateTime.Now.Ticks;
        //    if (e.KeyValue == 13)
        //    {
        //        String value = new string(bcode.ToArray()).Trim();
        //        //value = value.Replace("\0", "");
        //        bcode.Clear();
        //        //Console.WriteLine("value : " + value);
        //        if (value != String.Empty)
        //        {
        //            //Validate(value);
        //            thread = new Thread(() => Validate(value));
        //            thread.IsBackground = true;
        //            thread.Start();
        //            //Console.WriteLine("thread start");
        //        }
        //        IsFirstCharacter = true;
        //    }
        //    else
        //    {
        //        long taken = TicksNow - _lastKeystroke;
        //        Constants.Logger(String.Format("cforKeyDown : {0} = int keyupdata : {1} = char keyupdata : {2} = [ TicksNow : {3} - _lastKeystroke : {4} =  taken : {5} ]"
        //          , (char)e.KeyValue, (int)e.KeyData, (char)e.KeyData, TicksNow, _lastKeystroke, taken));
        //        if (taken < TimeoutforKey)
        //        {
        //            bcode.Add((char)e.KeyValue);
        //        }
        //        else
        //        {
        //            bcode.Clear();
        //            Console.WriteLine("Clear called : " + taken);
        //            IsFirstCharacter = true;
        //        }
        //    }
        //    _lastKeystroke = DateTime.Now.Ticks;
        //}

        private void SendtoTVSService(string value)
        {

            //Console.WriteLine("validate : " + value);
            lock (_lock)
            {
                Console.WriteLine("Data send to server : " + value);
                Constants.Logger("Data send to server : " + value);

                Product product = new Product() { Barcode = value };
                //string Pos = GetActiveWindowTitle();
                
                if(!IsPOSRunning() || !IsPOSinFocus())
                {
                    Console.WriteLine("Configured Pos is not Active : " + ActivePos);
                    tvsService = new TVSService(TVSService.ProductType.AnonymousProduct, product);
                }
                else
                {
                    tvsService = new TVSService(Order.IsClosed ? TVSService.ProductType.AnonymousProduct : TVSService.ProductType.UserProduct, product);
                }
                //Console.WriteLine("product " + product.ProductID + " : " + product);
                if (tempProducts.ContainsKey(product.ProductID))
                {
                    lock (this)
                    {
                        product.ProductID = DateTime.Now.Ticks.ToString();
                        tempProducts.Add(product.ProductID, product);
                    }
                }
                else
                {
                    tempProducts.Add(product.ProductID, product);
                }
                if (IsconnectedToService)
                {
                    Console.WriteLine("data send to service : " + tvsService.ToJsonString());
                    //byte[] buffer = Encoding.ASCII.GetBytes(tvsService.ToJsonString());
                    //byte[] buffer = Encoding.ASCII.GetBytes("{\"Islogin\":" + IsUserLogin.ToString().ToLower() + ",\"value\":\"" + value + "\"}");
                    //ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                    pSocketClient.Send(tvsService.ToJsonString());
                    //Console.WriteLine("data send to server:" + tvsService.ToJsonString());
                }
            }
            //    if (IsUserLogin && order != null)
            //{
            //    //Console.WriteLine("validate : value" + value);
            //    lock(_lock)
            //    {
            //        this.Dispatcher.Invoke(addProductEvent, Jsondata);
            //    }
            //}
            //if (IsconnectedToService)
            //{
            //    byte[] buffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Product() {Barcode  = value}.ToJsonString()));
            //    //byte[] buffer = Encoding.ASCII.GetBytes("{\"Islogin\":" + IsUserLogin.ToString().ToLower() + ",\"value\":\"" + value + "\"}");
            //    ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            //}
        }
        //private void m_oWorkerdemo_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    OrderHistory.Product product = (OrderHistory.Product)e.Argument;
        //    if (Order.SaveAnonymousItem(product))
        //        e.Result = product;
        //    else
        //        e.Result = null;
        //}
        private void ButtonStopClick(object sender, EventArgs e)
        {
            //CaptureHook.Stop();
        }
        private void m_oWorkerdemo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (e.Result != null)
               //Console.WriteLine("saved..");
            //else
               //Console.WriteLine("Not saved..");
        }
        private void _bubbleImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
            /*if (e.ChangedButton == MouseButton.Right) { if (lvItems2.IsVisible) { lvItems2.Visibility = Visibility.Hidden; } else { lvItems2.Visibility = Visibility.Visible; } }
            */
        }
        private void _btnCalender_Click(object sender, RoutedEventArgs e)
        {

        }
        
        //private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //textBoxOtp.Text = string.Format("{0:[### - ### - ### - ###]}", double.Parse(textBoxOtp.Text));
        //}
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //IEnumerator t = e.Changes.GetEnumerator();
            //if (t.MoveNext())
            //{
            //    selectionstart = ((TextChange)t.Current).Offset + ((TextChange)t.Current).AddedLength;
            //}
            //text = textBoxOtp.Text.Replace("-", "").Replace(" ", "").Replace(Environment.NewLine, "");
            text = textBoxOtp.Text.Replace("-", "").Replace(" ", "").Replace(Environment.NewLine, "");
            //textBoxOtp.Text = Regex.Replace(textBoxOtp.Text, "[^0-9]", "");
            if (loginfailedflag)
            {
                _lbl_Pin.Content = "Enter PIN";
                _lbl_Pin.Foreground = (Brush)new BrushConverter().ConvertFromString("#FF3C4788");
                _enterPin.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/enterPin.png", UriKind.Absolute));
                loginfailedflag = false;
            }
            if (previousText != text)
            {
                previousText = text;
                caretno = ((System.Windows.Controls.TextBox)e.Source).CaretIndex;
                if (text.Length > 12)
                {
                    text = text.Substring(0, 12);
                }
                if (text.Length <= 3)
                {
                    text = text.Substring(0, text.Length) + "";
                }
                else if (text.Length > 3 && text.Length <= 6)
                {
                    text = text.Substring(0, 3) + textdash + text.Substring(3, text.Length - 3);
                }
                else if (text.Length > 6 && text.Length <= 9)
                {
                    text = text.Substring(0, 3) + textdash + text.Substring(3, 3) + Environment.NewLine + text.Substring(6, text.Length - 6);
                }
                else if (text.Length > 9 && text.Length <= 12)
                {
                    text = text.Substring(0, 3) + textdash + text.Substring(3, 3) + Environment.NewLine + text.Substring(6, 3) + textdash + text.Substring(9, text.Length - 9);
                }
                //else if (text.Length > 12)ring(3, 3) + Environment.NewLine + text.Substring(6, 3) + textdash + text.Substring(9, 3);
                //}
                textBoxOtp.Text = text;
                if(textBoxOtp.Text.Length == textBoxOtp.MaxLength)
                {
                    if (SendforValidUser(textBoxOtp.Text))
                    {
                        return;
                    }
                }
                //if (text.Replace("-", "").Replace(" ", "").Replace(Environment.NewLine, "").Equals("111111111111"))
                //{
                //    //LoginSuccess();
                //}
                //else if (text.Replace("-", "").Replace(" ", "").Replace(Environment.NewLine, "").Length == 12)
                //{
                //    //LoginFailed();
                //}
                else
                {
                    if (!Isbackpressed && sametext)
                    {
                        //{
                        //    text = text.Substring(0, 3) + textdash + text.Subst
                        textBoxOtp.SelectionStart = textBoxOtp.Text.Length; // add some logic if length is 0
                        textBoxOtp.SelectionLength = 0;
                        sametext = false;
                    }
                    else
                    {
                        textBoxOtp.SelectionStart = caretno; // add some logic if length is 0
                        textBoxOtp.SelectionLength = 0;
                        Isbackpressed = false;
                    }
                }
            }
            else
            {
                sametext = true;
            }
            //if (textchange)
            //{
            //textchange = false;
            //if (!Isbackpressed && !justpressed)
            //{
            //    textBox.SelectionStart = textBox.Text.Length; // add some logic if length is 0
            //    textBox.SelectionLength = 0;
            //    justpressed = false;
            //}
            //else if (!Isbackpressed && justpressed)
            //{
            //    textBox.SelectionStart = selectionstart; // add some logic if length is 0
            //    textBox.SelectionLength = 0;
            //    justpressed = false;
            //}
            //else
            //{
            //    textBox.SelectionStart = selectionstart;
            //    textBox.SelectionLength = 0;
            //    Isbackpressed = false;
            //    justpressed = true;
            //}
            //else
            //{
            //    textchange = true;
            //}
        }
        private bool SendforValidUser(string text)
        {
            text = text.Replace("-", "").Replace(" ", "").Replace(Environment.NewLine, "");
            if(Transaction.CreateTxn(text))
            {
                LoginSuccess();
                return true;
            }
            else
            {
                LoginFailed();
                return false;
            }
        }
        private void LoginSuccess()
        {
            //Console.WriteLine("Login success");
            //IsUserlogin = true;
            canvasEnterPin.Visibility = Visibility.Hidden;
            canvasUserInfo.Visibility = Visibility.Visible;
            CreateOrder();
            //cforKeyDown = '\0';
        }
        private void CloseOrder()
        { 
            Order.Close(ref order);
        }
        private void LoginFailed()
        {
            _lbl_Pin.Content = "Invalid PIN";
            _lbl_Pin.Foreground = Brushes.Red;
            loginfailedflag = true;
            _enterPin.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/enterPingrey.png", UriKind.Absolute));
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e) { }
        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (itemImageClicked)
            //{
            //    itemImageClicked = false;
            //    return;
            //}
            Product product = ((Product)((Border)sender).DataContext);
            //if (!product.Applicable) return;
            if (!product.Addedinremovedproduct)
            {
                order.RemoveProduct(product);
            }
            else
            {
                order.AddProduct(product);
            }
            view.Refresh();
            //if (e.AddedItems.Count != 0)
            //{
            //    product = e.AddedItems[0] as Product;
            //    if (!product.Applicable) return;
            //    order.Remove(product);
            //}
            //else if (e.RemovedItems.Count != 0)
            //{
            //    product = e.RemovedItems[0] as Product;
            //    if (!product.Applicable) return;
            //    order.Add(product);
            //}
            //else
            //    MessageBox.Show("lvItems_SelectionChanged : else");
            //updateCart();
        }
        private void _btn_AddItem_Click(object sender, RoutedEventArgs e)
        {
            //Product product = null;
            //int temp = ++value;
            //product = new Product() { Barcode = 1100 + temp +"", ProductName = filleditems[temp].Barcode + "" };
            //if (UserInfo.Islogin)
            //{
            //    if (temp == 0)
            //    {
            //        canvasUserInfo.Visibility = Visibility.Hidden;
            //        canvasCoupanDisplay.Visibility = Visibility.Visible;
            //        order = new Order("o0");
            //        view = CollectionViewSource.GetDefaultView(order.products);
            //        lvItems.ItemsSource = view;
            //        startWorkerProcess(product);
            //    }
            //    else
            //    {
            //        startWorkerProcess(product);
            //    }
            //}
            //else
            //{
            //    Order.SaveItem(product);
            //}

            //order.Add(new Product() { Barcode = value, Name = filleditems[value].Name });
            //Order o = new Order("1", p);
            //o.Delete(p);
            //p.Barcode = 12;
            //o.Delete(p);
            //Order.Clean(ref o);
            //startWorkerProcess(product);
        }
        private void TextBlock_NextCustomer(object sender, MouseButtonEventArgs e)
        {
            if (lvItems.Visibility == Visibility.Visible)
            {
                lvItems.Visibility = Visibility.Hidden;
                _lblHeader.Content = Constants.HeaderTxndescription;
                lvTxnHistory.Visibility = Visibility.Visible;
            }
            else
            {
                _lblHeader.Content = Constants.HeaderProductdescription;
                lvItems.Visibility = Visibility.Visible;
                lvTxnHistory.Visibility = Visibility.Hidden;
            }
        }
        private void _lbl_editorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvasEditOrder.Visibility = Visibility.Visible;
            canvasCoupanDisplay.Visibility = Visibility.Hidden;
            BorderTransactionPanel.Visibility = Visibility.Visible;
            //if (BorderTransactionPanel.Visibility == Visibility.Visible)
            //    BorderTransactionPanel.Visibility = Visibility.Hidden;
            //else
            //    BorderTransactionPanel.Visibility = Visibility.Visible;
        }
        private void _btnRedeem_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Txn Redeemed No is" + order.OrderNumber);
            if (Transaction.RedeemTxn())
            {
                if (Transaction.redeemtxnresponse != null && Transaction.redeemtxnresponse.status.ToLower() == "committed")
                {
                    if (Transaction.GetRedeemTxn())
                    {
                        decimal TotalAmount = 0;
                        if (order != null)
                        {
                            order.UpdateProducts();
                        }
                        canvasCoupanDisplay.Visibility = Visibility.Hidden;
                        BorderTransactionPanel.Visibility = Visibility.Visible;
                        canvasFinishTxn.Visibility = Visibility.Visible;
                        canvasEditOrder.Visibility = Visibility.Hidden;
                        BorderFooterPanel.Visibility = Visibility.Hidden;
                        BorderFooterHistoryPanel.Visibility = Visibility.Visible;

                        OrderHistory oh0 = new OrderHistory(Transaction.transactionresponselist[0].oid.ToString(), TotalAmount,Constants.ConvertToDateTimefromISO8601(Transaction.redeemtxnresponse.finished));

                        //OrderHistory oh1 = new OrderHistory("or1", order.GettotalAmount + 1, DateTime.Now.AddDays(27).AddHours(2).ToString());
                        foreach (var item in Transaction.transactionresponselist)
                        {
                            oh0.products.Add(new OrderHistory.Product() { Amount = item.savingsValue, Barcode = item.couponOid.ToString(), ProductName = item.name });
                            TotalAmount += item.savingsValue;
                        }

                        oh0.OrderAmount = TotalAmount;
                        Store.Orders.Add(oh0);
                        TotalAmount = 0;
                        foreach (var item in Store.Orders)
                        {
                            TotalAmount += item.OrderAmount;
                        }
                        _lblTotalDiscountAmountHistory.Content = Store.Currency + TotalAmount;
                        Order.IsClosed = true;
                        //CloseOrder();
                        #region extras
                        //Store.Orders.Add(oh1);

                        //OrderHistory o;
                        //OrderHistory Previous;
                        //OrderProduct product;

                        //for (int i = 1; i < 5; i++)
                        //{
                        //    Previous = oh0;
                        //    oh0 = new OrderHistory("oh" + );

                        //    foreach (var item in Previous.products)
                        //    {
                        //        o.products.Add(item);
                        //    }
                        //    product = new Product() { Barcode = 1105 + i+"", ProductName = filleditems[5 + i].Barcode + "" };
                        //    product.Applicable = true;
                        //    product.productstatus = ProductStatus.Updated;
                        //    product.Amount = 5.00m * i;
                        //   o.Add(product);
                        //   Store.AllOrders.Add(o);
                        //}

                        //Order.Clean(ref order);
                        #endregion

                        lvItems.Visibility = Visibility.Hidden;
                        _lblHeader.Content = Constants.HeaderTxndescription;
                        lvTxnHistory.Visibility = Visibility.Visible;
                        if (Txnview == null)
                        {
                            Txnview = CollectionViewSource.GetDefaultView(Store.Orders);
                            Txnview.GroupDescriptions.Clear();
                            Txnview.GroupDescriptions.Add(new PropertyGroupDescription("DateTime"));
                            lvTxnHistory.ItemsSource = Txnview;
                        }
                        else
                        {
                            Txnview.Refresh();
                        }
                        Console.WriteLine("Order is Redeemed now.!!");
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Product count is 0", "GetRedeemTxn");
                    }
                }
            }
        }
        private void ResetScreen()
        {
            canvasEnterPin.Visibility = Visibility.Visible;
            canvasUserInfo.Visibility = Visibility.Hidden;
            canvasCoupanDisplay.Visibility = Visibility.Hidden;
            canvasFinishTxn.Visibility = Visibility.Hidden;
            BorderTransactionPanel.Visibility = Visibility.Hidden;
            textBoxOtp.Text = String.Empty;
            lvItems.Visibility = Visibility.Visible;
            _lblHeader.Content = Constants.HeaderProductdescription;
            lvTxnHistory.Visibility = Visibility.Hidden;
            BorderFooterPanel.Visibility = Visibility.Visible;
            BorderFooterHistoryPanel.Visibility = Visibility.Hidden;
        }
        private void Back_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Do you want to Close the Current Txn ?", "Confirmation", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                //if (order.GetProductCount == 0)
                //{
                BackImage_MouseDown(null, null);
                CloseOrder();
                Order.IsClosed = true;
                ResetScreen();
                //}
                //else
                //{
                //    System.Windows.Forms.MessageBox.Show("Product Count : " + order.GetProductCount);
                //}
            }
        }
        private void Forward_MouseDown(object sender, MouseButtonEventArgs e)
        {
            canvasCoupanDisplay.Visibility = Visibility.Visible;
            canvasUserInfo.Visibility = Visibility.Hidden;
            //order.AddProduct(new Product() { Barcode = "123", ProductName = "123" });
            //order.AddProduct(new Product() { Barcode = "1234", ProductName = "123" });
            //order.AddProduct(new Product() { Barcode = "12345", ProductName = "123" });
            //order.AddProduct(new Product() { Barcode = "123456", ProductName = "123" });
            //updateCart();
        }
        private void textBoxOtp_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void textBoxOtp_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            if(e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
        private void _btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Order.orderStatus == OrderStatus.Open)
            {
                order.UpdateProducts();
                updateCart();
                BackImage_MouseDown(null, null);
            }
            //Product product = ((Product)((Image)sender).DataContext);
            //if (!product.Applicable || product.Addedinremovedproduct)
            //{
            //    itemImageClicked = true;
            //    order.Delete(product);
            //    updateCart();
            //    view.Refresh();
            //    return;
            //}
        }
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).ToolTip = ((TextBlock)sender).Text;
        }
        private void _btnSaveHistory_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            //if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    if (Directory.Exists(fd.SelectedPath.Trim()))
            //    {
            //        if (!order.SaveData(fd.SelectedPath.Trim()))
            //        {
            //            System.Windows.Forms.MessageBox.Show("Error occured while saving");
            //        }
            //    }
            //    else
            //    {
            //        System.Windows.Forms.MessageBox.Show("Please select folder to save the file.");
            //    }
            //}
        }
        private void _btnFinishTxn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIPermission clipBoard = new UIPermission(PermissionState.None);
                clipBoard.Clipboard = UIPermissionClipboard.AllClipboard;
                Clipboard.SetText(order.GettotalAmount.ToString());
            }
            catch( Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("CLipboard Copy Error" + ex.Message);
            }
            CloseOrder();
            Console.WriteLine("Order is Finished now.!!");
            ResetScreen();
        }
        private void BackImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            canvasEditOrder.Visibility = Visibility.Hidden;
            canvasCoupanDisplay.Visibility = Visibility.Visible;
            BorderTransactionPanel.Visibility = Visibility.Hidden;
        }
        private void TextBlock_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

        }
        private void updateCart()
        {
            //if (!TransactionStart)
            //{
            //    canvasCoupanDisplay.Visibility = Visibility.Visible;
            //    canvasUserInfo.Visibility = Visibility.Hidden;
            //    TransactionStart = true;
            //}
            _lblTotalDiscountAmount.Content = Store.Currency + order.GettotalAmount;
            _lbl_coupan.Content = order.products.Count;// + " coupan(s)";
        }

        //private void startWorkerProcess(String productBarcode)
        //{
        //    m_oWorker = new BackgroundWorker();
        //    m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
        //    m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
        //    m_oWorker.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
        //    m_oWorker.WorkerReportsProgress = true;
        //    m_oWorker.WorkerSupportsCancellation = true;
        //    m_oWorker.RunWorkerAsync(productBarcode);
        //    //m_oWorker = new BackgroundWorker();
        //    //if (UserInfo.Islogin)
        //    //{
        //    //    product = new Product() { Barcode = productBarcode, ProductName = productBarcode };
        //    //    order.Add(product);
        //    //    view.Refresh();
        //    //    m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
        //    //    m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
        //    //    m_oWorker.WorkerReportsProgress = true;
        //    //    m_oWorker.WorkerSupportsCancellation = true;
        //    //    m_oWorker.RunWorkerAsync(product);
        //    //}
        //    //else
        //    //{
        //    //    m_oWorker.DoWork += new DoWorkEventHandler(m_oWorkerdemo_DoWork);
        //    //    m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorkerdemo_RunWorkerCompleted);
        //    //    m_oWorker.WorkerReportsProgress = true;
        //    //    m_oWorker.WorkerSupportsCancellation = true;
        //    //    m_oWorker.RunWorkerAsync(new OrderHistory.Product() { Barcode = productBarcode });
        //    //}
        //}
        //private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    //selectionstart = ((TextBox)e.Source).CaretIndex;
        //    if (e.Key == Key.Back)
        //    {
        //        Isbackpressed = true;
        //    }
        //    else
        //    {

        //    }
        //}
        #region Extra

        //private void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if (e.Cancelled)
        //    {
        //        // StatusTextBox.Text = "Task Cancelled.";
        //    }
        //    // Check to see if an error occurred in the background process.
        //    else if (e.Error != null)
        //    {
        //        //StatusTextBox.Text = "Error while performing background operation.";
        //    }
        //    else
        //    {
        //        updateCart();
        //    }
        //    view.Refresh();
        //    //lvItems.SelectedItems.Add(selecteditems);
        //    //lvItems.Focus();
        //    // lvItems.Items.DeferRefresh();
        //    //_lblTotalDiscountAmount.Content = "Rs." + totalAmount;
        //    // Everything completed normally.
        //    //StatusTextBox.Text = "Task Completed...";
        //}
        //private void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    String Barcode = (String)e.Argument;
        //    Product product = new Product() { Barcode = Barcode, ProductName = Barcode };
        //    m_oWorker.ReportProgress(0, product);

        //    Thread.Sleep(2000);
        //    if (product.productstatus == ProductStatus.Pending)
        //    {
        //        product.productstatus = ProductStatus.Updated;
        //        if (product.productstatus != ProductStatus.Pending && !product.Addedinremovedproduct)
        //        {
        //            product.Amount = 1.00m;
        //        }
        //    }
        //    m_oWorker.ReportProgress(100, product);
        //    e.Result = product;
        //    //    for (int i = 0; i < filleditems.Count; i++)
        //    //{
        //    //    Thread.Sleep(1000);
        //    //    if (product.Barcode == filleditems[i].Barcode)
        //    //    {
        //    //        product.Amount = filleditems[i].Amount;
        //    //        if (product.productstatus == ProductStatus.Pending)
        //    //        {
        //    //            product.productstatus = ProductStatus.Updated;
        //    //            if (product.productstatus != ProductStatus.Pending && !product.Addedinremovedproduct)
        //    //            {
        //    //                product.ProductName = filleditems.Single(data => data.Barcode == product.Barcode).ProductName;
        //    //                if (i == 3)
        //    //                {
        //    //                    product.Applicable = false;
        //    //                    product.Amount = -1.00m;
        //    //                    order.Remove(product);
        //    //                    //changeselection(product);
        //    //                }
        //    //                //totaldiscountedAmount += product.Amount;
        //    //                //Console.WriteLine("m_oWorker_DoWork totaldiscountedAmount stop: {0} - {1}", product.ToString(), product.Amount);
        //    //            }
        //    //            //changeselection(product);  // added mayank
        //    //            //if (!product.Addedinremovedproduct)   // commented mayank
        //    //            //totaldiscountedAmount += Convert.ToDecimal(product.Amount); commented mayank
        //    //        }
        //    //        e.Result = product;
        //    //        break;
        //    //    }
        //    //    //m_oWorker.ReportProgress(i, user);
        //    //    //if (m_oWorker.CancellationPending)
        //    //    //{
        //    //    //    e.Cancel = true;
        //    //    //    //m_oWorker.ReportProgress(0);
        //    //    //    return;
        //    //    //}
        //    //}
        //    //m_oWorker.ReportProgress(100);
        //}
        //private void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    if (e.ProgressPercentage == 0)
        //    {
        //        Product product = (Product)e.UserState;
        //        order.Add(product);
        //        view.Refresh();
        //    }
        //    //if (product != null)
        //    //  ContainerPanel.Controls.Add(new Label { Text = oEmp.Name });
        //    //ListBox1.Items.Add(new ListBoxItem { Content = oEmp.Name + " item added" + e.ProgressPercentage.ToString() + "%" });
        //    //StatusTextBox.Text = "Processing......" + e.ProgressPercentage.ToString() + "%";
        //}


        //private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    canvasEnterPin.Visibility = Visibility.Hidden;
        //    ScrollBar s = new ScrollBar();
        //    s.Width = 10;

        //    if (clicked)
        //    {
        //        _btnSaveImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/exportCsv.png", UriKind.Absolute));
        //        _enterPin.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/enterPingrey.png", UriKind.Absolute));
        //        _wifiImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/wifi_Red.png", UriKind.Absolute));
        //        _okImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/RingRinglogo.png", UriKind.Absolute));
        //        _btnCalender.Visibility = Visibility.Hidden;
        //        clicked = false;
        //    }
        //    else
        //    {
        //        _btnSaveImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/save_Green.png", UriKind.Absolute));
        //        _enterPin.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/enterPin.png", UriKind.Absolute));
        //        _okImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/ok_Green.png", UriKind.Absolute));
        //        _wifiImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/wifi_Green.png", UriKind.Absolute));
        //        clicked = true;
        //        _btnCalender.Visibility = Visibility.Visible;
        //    }
        //}

        /*public void MyKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
           //Console.WriteLine("MyKeyPress int :" + (int)e.KeyChar);
           //Console.WriteLine("MyKeyPress char :" + (char)e.KeyChar);
            //Console.WriteLine("MyKeyPress bool : " + AppendBarcode);
            if (AppendBarcode)
            {
                if (e.KeyChar != 13)
                    _barcode.Add((char)e.KeyChar);
                //Debug.WriteLine("AppendBarcode : " + (char)e.KeyChar);
               //Console.WriteLine("_barcode value : " + new String(_barcode.ToArray()));
            }
            if (e.KeyChar == 13 && _barcode.Count > 0)
            {
                //Console.WriteLine("_barcode.Count " + _barcode.Count);
                string BarCodeData = string.Empty;
                BarCodeData = new string(_barcode.ToArray());
               //Console.WriteLine(String.Format("{0}[{1}]", "BarCodeData before : ", BarCodeData));
                _barcode.Clear();
                if (Console.CapsLock == true)
                    BarCodeData = changeCase(BarCodeData);
                keyPressCounter = keydownCounter = 0;
                //Debug.WriteLine(String.Format("[{0}]", BarCodeData));
               //Console.WriteLine(String.Format("{0}[{1}]", "BarCodeData after : ", BarCodeData));
                order.Add(new Product() { Barcode = BarCodeData, ProductName = BarCodeData });
                view.Refresh();
                //startWorkerProcess(BarCodeData);
                //_barcode.Clear();
                //lvItems.Items.MoveCurrentToLast();
                //m_oWorkerdemo.RunWorkerAsync(product);

            }
            else
            {
                keyPressCounter++;
                _lastKeystroke = DateTime.Now.Millisecond;
            }
            //cforKeyDown = (char)e.KeyChar;
            //Console.WriteLine("MyKeyPress : " + (int)e.KeyChar);
        }
        public void MyKeydown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
           //Console.WriteLine("MyKeydown :" + (int)e.KeyCode);
            AppendBarcode = true;
            _lastKeystroke = DateTime.Now.Millisecond;
            if (e.KeyData == System.Windows.Forms.Keys.LShiftKey || e.KeyData == System.Windows.Forms.Keys.RShiftKey ||
                     e.KeyData == System.Windows.Forms.Keys.ShiftKey || e.KeyData == System.Windows.Forms.Keys.Shift)
            {
                return;
            }
            //else if (e.KeyData == System.Windows.Forms.Keys.Capital || e.KeyData == System.Windows.Forms.Keys.CapsLock)
            //{
            //    return;
            //}
            keydownCounter++;
            cforKeyDown = ((char)e.KeyCode);
            //Console.WriteLine("MyKeydown : " + (int)e.KeyData);
        }
        public void MyKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
           //Console.WriteLine("MyKeyUp :" + (int)e.KeyCode);
            if ((keydownCounter == keyPressCounter) && keydownCounter > 1)
            {
               //Console.WriteLine(keydownCounter);
               //Console.WriteLine("_barcode.Clear();");
                _barcode.Clear();
                keydownCounter = 0;
                keyPressCounter = 0;
                return;
            }
            keydownCounter = 0;
            keyPressCounter = 0;
            //Console.WriteLine("MyKeyUp : " + (int)e.KeyCode);
            //Console.WriteLine("cforKeyDown : " + cforKeyDown);
            if (e.KeyData == System.Windows.Forms.Keys.LShiftKey || e.KeyData == System.Windows.Forms.Keys.RShiftKey ||
                     e.KeyData == System.Windows.Forms.Keys.ShiftKey || e.KeyData == System.Windows.Forms.Keys.Shift)
            {
                return;
            }
            if (cforKeyDown != (char)e.KeyCode || cforKeyDown == '\0')
            {
                //Debug.WriteLine("cforKeyDown : MyKeyUp =>" + (int)cforKeyDown + " : " + (int)e.KeyCode);
                //Debug.WriteLine("MyKeyUp bool AppendBarcode : " + AppendBarcode);
                cforKeyDown = '\0';
                _barcode.Clear();
                AppendBarcode = false;
                //Debug.WriteLine("MyKeyUp bool AppendBarcode : " + AppendBarcode);
                return;
            }
            AppendBarcode = true;
           //Console.WriteLine("_lastKeystroke :" + _lastKeystroke);
            int elapsed = (DateTime.Now.Millisecond - _lastKeystroke);
            if (elapsed > 17)
            {
               //Console.WriteLine("clear : " + elapsed);
                AppendBarcode = false;
                _barcode.Clear();
            }
           //Console.WriteLine("elapsed : " + elapsed);
            //if(e.KeyCode == System.Windows.Forms.Keys.Return)
            //{
            //    AppendBarcode = false;
            //    //_barcode.Add((char)e.KeyData);
            //}
        }

        */

        //private string changeCase(String barcodeData)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (char c in barcodeData)
        //    {
        //        if (Char.IsLower(c))
        //            sb.Append(c.ToString().ToUpper());
        //        else if (char.IsUpper(c))
        //            sb.Append(c.ToString().ToLower());
        //        else
        //            sb.Append(c.ToString());
        //    }
        //    return sb.ToString();
        //}

        //private void _okImage1_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    Product product = ((Product)((Image)sender).DataContext);
        //    if (!product.Applicable || product.Addedinremovedproduct)
        //    {
        //        itemImageClicked = true;
        //        order.Delete(product);
        //        updateCart();
        //        view.Refresh();
        //        return;
        //    }
        //}

        //public void MyKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    cforKeyUp = (char)e.KeyCode;
        //    //Debug.WriteLine("MyKeyUp : " + e.KeyData);
        //}
        //public void MyKeydown(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    cforKeydown = (char)e.KeyCode;
        //     //Debug.WriteLine("MyKeydown : " + e.KeyData);
        //}
        //public void MyKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        //{
        //    //Debug.WriteLine("MyKeyPress : " + e.KeyChar);
        //    int elapsed = (DateTime.Now.Millisecond - _lastKeystroke);
        //    ////Debug.WriteLine("elapsed : " + elapsed);
        //    if (elapsed > 17)
        //        _barcode.Clear();
        //    ////Debug.WriteLine("e._lastKeystroke" + _lastKeystroke);
        //    ////Debug.WriteLine("e.char" + (char)e.KeyChar);
        //    _barcode.Add(e.KeyChar);
        //    if (e.KeyChar == 13 && _barcode.Count > 1)
        //    {
        //        _barcode.Remove(e.KeyChar);
        //        string BarCodeData = new String(_barcode.ToArray());
        //        //Debug.Print("BarCodeData : " + BarCodeData);
        //        _barcode.Clear();
        //    }
        //    _lastKeystroke = DateTime.Now.Millisecond;
        //}

        //Console.WriteLine("-------------------------------------------------------------------------------");
        //Console.WriteLine("lvItems_SelectionChanged fired : {0} ", true);

        //Console.WriteLine("e.AddedItems.Count : {0} ", e.AddedItems.Count);
        //removedproductfromCart.Add(product);
        //Console.WriteLine("removedproductfromCart.Add(product) : {0} ", product.ToString());

        //Console.WriteLine("e.RemovedItems.Count : {0} ", e.RemovedItems.Count);

        //for (int i = 0; i < order.Rejectedproducts.Count; i++) //int i = 0; i < removedproductfromCart.Count; i++
        //{
        //    if (product.Barcode == order.Rejectedproducts[i].Barcode)  //product.Barcode == removedproductfromCart[i].Barcode
        //    {
        //        //Console.WriteLine("removedproductfromCart.Remove(product) : {0} ", product.ToString());
        //        if (itemImageClicked && product.Addedinremovedproduct)
        //        {
        //            if (order.products.Where(item => item.Barcode == product.Barcode).Count() == 1)
        //            {
        //                order.Delete(product);
        //                view.Refresh();
        //                itemImageClicked = false;
        //                return;
        //            }
        //            //foreach (var item in order.products)
        //            //{
        //            //    if (item.Barcode == product.Barcode)
        //            //    {
        //            //        order.Delete(product);
        //            //        order.products.Remove(product);
        //            //        //order.products.RemoveAt(order.products.IndexOf(item));
        //            //       //Console.WriteLine("deleted item : {0} ", product.ToString());
        //            //        view.Refresh();
        //            //        itemImageClicked = false;
        //            //        return;
        //            //    }
        //            //}
        //        }
        //        order.Add(product);                                    //removedproductfromCart.RemoveAt(i);
        //        break;
        //    }
        //}
        //changeselection(product);
        //itemImageClicked = false;
        //Console.WriteLine("removedproductfromCart.Count : {0}", order.Rejectedproducts.Count);
        //Console.WriteLine("totalitems.Count : {0}", order.products.Count);
        //Console.WriteLine("totaldiscountedAmount : {0}", order.GettotalAmount);
        //Console.WriteLine("-------------------------------------------------------------------------------");

        //if (e.AddedItems.Count != 0)
        //{
        //    Product u = e.AddedItems[0] as Product;
        //    selecteditems.Add(u);
        //    if (u.productstatus == ProductStatus.Updated && !u.Added)
        //        removedProductAmount = Convert.ToDecimal(u.Amount);
        //    changeselection(u);
        //}
        //if (e.RemovedItems.Count != 0)
        //{
        //    for (int i = 0; i < selecteditems.Count; i++)
        //    {
        //        Product u = (Product)e.RemovedItems[0];
        //        if (u.index == selecteditems[i].index)
        //        {
        //            if (u.productstatus == ProductStatus.Updated && u.Added)
        //                removedProductAmount = -Convert.ToDecimal(u.Amount);
        //            changeselection(u);
        //            selecteditems.RemoveAt(i);
        //            break;
        //        }
        //    }
        //}
        //private void changeselection(Product product)
        //{
        //   //Console.WriteLine("changeselection start: {0}", product.ToString());
        //    if (product.Addedinremovedproduct)
        //    {
        //        product.Added();
        //        ////if (product.productstatus != ProductStatus.Pending)
        //        ////{
        //        ////    updateCart();
        //        ////    //Console.WriteLine("before totaldiscountedAmount: {0}", totaldiscountedAmount);
        //        ////    //totaldiscountedAmount += product.Amount;
        //        ////    //Console.WriteLine("changeselection totaldiscountedAmount: {0}", product.Amount);
        //        ////    //Console.WriteLine("after totaldiscountedAmount: {0}", totaldiscountedAmount);
        //        ////}
        //    }
        //    else
        //    {
        //        product.Removed();
        //        ////if (product.productstatus != ProductStatus.Pending) {
        //        ////    updateCart();
        //        ////    //Console.WriteLine("before totaldiscountedAmount: {0}", totaldiscountedAmount);
        //        ////    //totaldiscountedAmount -= product.Amount;
        //        ////    //Console.WriteLine("changeselection totaldiscountedAmount: {0}", product.Amount);
        //        ////    //Console.WriteLine("after totaldiscountedAmount: {0}", totaldiscountedAmount);
        //        ////}
        //    }
        //   //Console.WriteLine("changeselection stop: {0}", product.ToString());

        //}
        //private void lvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (itemImageClicked)
        //    {
        //        itemImageClicked = false;
        //        return;
        //    }
        //    Product product = null;
        //    if (e.AddedItems.Count != 0)
        //    {
        //        product = e.AddedItems[0] as Product;
        //        if (!product.Applicable) return;
        //        order.Remove(product);
        //    }
        //    else if (e.RemovedItems.Count != 0)
        //    {
        //        product = e.RemovedItems[0] as Product;
        //        if (!product.Applicable) return;
        //        order.Add(product);
        //    }
        //    else
        //        MessageBox.Show("lvItems_SelectionChanged : else");
        //    updateCart();
        //    lvItems.Focusable = false;
        //    view.Refresh();
        //}
        //public void MyKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    long TicksNow = DateTime.UtcNow.Ticks;
        //    //Order.Logger(String.Format("{0} {1} {2} {3}", "MyKeyUp :", (int)e.KeyData, (char)e.KeyData, cforKeyDown));
        //    // check if the same key pressed for long time
        //    if (counterkeydown == counterkeypress && counterkeydown > 1)
        //    {
        //        bcode.Clear();
        //        counterkeydown = counterkeypress = 0;
        //        cforKeyDown = '\0';
        //        return;
        //    }

        //    //reset the counter for key press
        //    counterkeydown = counterkeypress = 0;

        //    // ignore shift up
        //    if (e.KeyData == System.Windows.Forms.Keys.LShiftKey || e.KeyData == System.Windows.Forms.Keys.RShiftKey ||
        //            e.KeyData == System.Windows.Forms.Keys.ShiftKey || e.KeyData == System.Windows.Forms.Keys.Shift)
        //    {
        //        _lastKeystroke = DateTime.UtcNow.Ticks;
        //        return;
        //    }
        //    if (e.KeyValue == 13)
        //    {
        //        String value = new string(bcode.ToArray()).Trim();
        //        value = value.Replace("\0", "");
        //        //System.Windows.Forms.MessageBox.Show(intvalue);
        //        //Debug.WriteLine(String.Format("{0}\n{1}\n{2}\n\n","-----------------------------------","BarCode int value : " + intvalue, "-----------------------------------"));
        //        cforKeyDown = '\0';
        //        bcode.Clear();
        //        if (value != String.Empty)
        //        {
        //            String intvalue = string.Empty;
        //            foreach (char item in value)
        //            {
        //                intvalue += "[ " + ((int)item).ToString() + " , " + item + " ] ";
        //            }
        //            Order.Logger(String.Format("{0} {1}", "BarCode values with int : ", intvalue));
        //            if (order == null)
        //            {
        //                Order.SaveAnonymousItem(new OrderHistory.Product() { Barcode = value });
        //                return;
        //            }
        //            else
        //            {
        //                order.AddProduct(new Product() { Barcode = value, ProductName = value });
        //            }
        //            //if ( GetActiveWindowTitle() != null &&  GetActiveWindowTitle().ToLower().Contains("notepad"))
        //            //else
        //            //    order.Add(new Product() { Barcode = "no notepad", ProductName = "no notepad" });
        //            //updateCart();
        //            //Application.Current.Dispatcher.Invoke(new Action(() => { updateCart(); }));
        //        }
        //    }
        //    else
        //    {
        //        long taken = TicksNow - _lastKeystroke;
        //        //Order.Logger(String.Format("cforKeyDown : {0} = int keyupdata : {1} = char keyupdata : {2} = [ TicksNow : {3} - _lastKeystroke : {4} =  taken : {5} ]"
        //        //       , cforKeyDown, (int)e.KeyData, (char)e.KeyData, TicksNow, _lastKeystroke, taken));
        //        if (taken < 100000)
        //        {
        //            bcode.Add(cforKeyDown);
        //        }
        //        else
        //        {
        //            bcode.Clear();
        //            cforKeyDown = '\0';
        //        }
        //    }
        //    return;
        //    /*
        //    //Console.WriteLine("========== Key Up ==========" + (char)e.KeyValue);
        //    //Console.WriteLine("========== Key up int ==========" + e.KeyValue);
        //    //return;
        //    //Console.WriteLine("========== Key Up ==========" + (int)cforKeyDown);
        //    //Console.WriteLine("========== Key up int ==========" + e.KeyValue);
        //    //Console.WriteLine("==== Barcode Read ========== " + Environment.NewLine + new string(bcode.ToArray()) + Environment.NewLine + "**********************" + Environment.NewLine);
        //   //Console.WriteLine("keydownCounter : " + keydownCounter);
        //   //Console.WriteLine("keyPressCounter : " + keyPressCounter);
        //    if ((keydownCounter == keyPressCounter) && keydownCounter > 1)
        //    {
        //       //Console.WriteLine("_barcode.Clear();");
        //        _barcode.Clear();
        //        keydownCounter = 0;
        //        keyPressCounter = 0;
        //        return;
        //    }
        //    keydownCounter = 0;
        //    keyPressCounter = 0;
        //   //Console.WriteLine("MyKeyUp : " + (char)e.KeyCode);
        //    //Console.WriteLine("cforKeyDown : " + cforKeyDown);
        //    if (e.KeyData == System.Windows.Forms.Keys.LShiftKey || e.KeyData == System.Windows.Forms.Keys.RShiftKey ||
        //             e.KeyData == System.Windows.Forms.Keys.ShiftKey || e.KeyData == System.Windows.Forms.Keys.Shift)
        //    {
        //        return;
        //    }
        //    if (cforKeyDown != (char)e.KeyCode || cforKeyDown == '\0')
        //    {
        //        //Console.WriteLine("cforKeyDown : MyKeyUp =>" + (int)cforKeyDown + " : " + (int)e.KeyCode);
        //        //Console.WriteLine("MyKeyUp bool : " + AppendBarcode);
        //        cforKeyDown = '\0';
        //        _barcode.Clear();
        //        AppendBarcode = false;
        //        return;
        //    }
        //    //AppendBarcode = true;
        //    //int elapsed = (DateTime.Now.Millisecond - _lastKeystroke);
        //    //if (elapsed > 17)
        //    //{
        //    //   //Console.WriteLine("_barcode.Clear();" + elapsed);
        //    //    AppendBarcode = false;
        //    //    _barcode.Clear();
        //    //}
        //    //if(e.KeyCode == System.Windows.Forms.Keys.Return)
        //    //{
        //    //    AppendBarcode = false;
        //    //    //_barcode.Add((char)e.KeyData);
        //    //}
        //    */
        //}
        //public void MyKeydown(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    //Order.Logger(String.Format("{0} {1} {2} {3}", "MyKeydown :", (int)e.KeyData, (char)e.KeyData, cforKeyDown));
        //    _lastKeystroke = DateTime.UtcNow.Ticks;
        //    //Console.WriteLine("========== Key down ==========" + (char)e.KeyValue);
        //    //Console.WriteLine("========== Key down int ==========" + e.KeyValue);
        //    //return;
        //    //AppendBarcode = true;
        //    //if (e.KeyData == System.Windows.Forms.Keys.LShiftKey || e.KeyData == System.Windows.Forms.Keys.RShiftKey ||
        //    //         e.KeyData == System.Windows.Forms.Keys.ShiftKey || e.KeyData == System.Windows.Forms.Keys.Shift)
        //    //{
        //    //    //_lastKeystroke = DateTime.UtcNow.Ticks;
        //    //    //Console.WriteLine("my down key store shift :" + _lastKeystroke);
        //    //    return;
        //    //}

        //    //else if (e.KeyData == System.Windows.Forms.Keys.Capital || e.KeyData == System.Windows.Forms.Keys.CapsLock)
        //    //{
        //    //    return;
        //    //}
        //    //keydownCounter++;
        //    //cforKeyDown = ((char)e.KeyCode);
        //    //Console.WriteLine("MyKeydown : " + e.KeyValue);
        //    //Console.WriteLine("my down key store :" + _lastKeystroke);
        //}
        //public void MyKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        //{
        //    cforKeyDown = (char)e.KeyChar;
        //    //Order.Logger(String.Format("{0} {1} {2} {3}", "MyKeyPress :", (int)e.KeyChar, (char)e.KeyChar, cforKeyDown));
        //    //Console.WriteLine("========== Key Press ==========" + (char)e.KeyChar);
        //    //Console.WriteLine("========== Key Press int ==========" + (int)e.KeyChar);
        //    //return;
        //    /*
        //    //Console.WriteLine("MyKeyPress bool : " + AppendBarcode);
        //    if ((DateTime.Now.Millisecond - _lastKeystroke) < 17)
        //    {
        //        if (e.KeyChar != 13)
        //            _barcode.Add((char)e.KeyChar);
        //        //Console.WriteLine("AppendBarcode : " + (char)e.KeyChar);
        //        //Console.WriteLine("_barcode : " + new String(_barcode.ToArray()));


        //        if (e.KeyChar == 13 && _barcode.Count > 0)
        //        {
        //            //Console.WriteLine("_barcode.Count " + _barcode.Count);
        //            string BarCodeData = new string(_barcode.ToArray());
        //            if (Console.CapsLock == true)
        //                BarCodeData = changeCase(BarCodeData);
        //            keydownCounter = keyPressCounter = 0;
        //           //Console.WriteLine(String.Format("{0}{1}{2}", "[", BarCodeData, "]"));
        //            order.Add(new Product() { Barcode = BarCodeData, ProductName = BarCodeData });
        //            view.Refresh();
        //            //product = new Product() { Barcode = BarCodeData, ProductName = BarCodeData };
        //            //order.Add(product);
        //            //lvItems.Items.MoveCurrentToLast();
        //            //m_oWorkerdemo.RunWorkerAsync(product);
        //            //view.Refresh();
        //            _barcode.Clear();
        //            return;
        //        }
        //    } else {
        //        _barcode.Clear(); return;
        //    }

        //    keyPressCounter++;
        //    //_lastKeystroke = DateTime.Now.Millisecond;
        //    //cforKeyDown = (char)e.KeyChar;
        //   //Console.WriteLine("MyKeyPress : " + (char)e.KeyChar);
        //    */
        //}
        #endregion
    }
}