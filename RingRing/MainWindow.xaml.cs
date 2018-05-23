using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using gma.System.Windows;
using System.Diagnostics;
using System.Windows.Interop;
using Microsoft.Win32;

namespace RingRing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Productdummy> filleditems;
        ICollectionView view, Txnview;
        BackgroundWorker m_oWorker, m_oWorkerdemo;
        //private Object thisLock = new Object();
        public bool clicked, sametext = true;
        private bool itemImageClicked, Isbackpressed, loginfailedflag = false;
        int value = -1;
        Store store;
        String text, previousText = string.Empty, textdash = " - ";
        int caretno = 0;
        Order order;
        UserActivityHook CaptureHook = null;
        String BarCodeData = String.Empty;
        char cforKeyDown = '\0';
        int _lastKeystroke = DateTime.Now.Millisecond;
        List<char> _barcode = new List<char>(50);
        Product product = null;
        public MainWindow()
        {
            InitializeComponent();
            //this.Topmost = true;
            this.Left = SystemParameters.WorkArea.Width - this.Width - 20;
            this.Top = 5;
            //textBox.Focus();
            store = new Store("0987654321", "Ali Hyper Market");
            filleditems = Productdummy.getDummyData;
            _lblStoreName.Content = store.StoreName;
            _lblTvsId.Content = store.fullTvsId;
            order = new Order("SampleOrder1");
            view = CollectionViewSource.GetDefaultView(order.products);
            lvItems.ItemsSource = view;
            _lblHeader.Content = Constants.HeaderProductdescription;
            StartCapture();
            m_oWorkerdemo = new BackgroundWorker();
            m_oWorkerdemo.DoWork += new DoWorkEventHandler(m_oWorkerdemo_DoWork);
            m_oWorkerdemo.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorkerdemo_RunWorkerCompleted);
            m_oWorkerdemo.WorkerReportsProgress = true;
            m_oWorkerdemo.WorkerSupportsCancellation = true;
        }
        private void StartCapture()
        {
            try
            {
                CaptureHook = new UserActivityHook();
                CaptureHook.KeyPress += new System.Windows.Forms.KeyPressEventHandler(MyKeyPress);
                CaptureHook.KeyDown += new System.Windows.Forms.KeyEventHandler(MyKeydown);
                CaptureHook.KeyUp += new System.Windows.Forms.KeyEventHandler(MyKeyUp);
                CaptureHook.Start();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        bool AppendBarcode = false;
        public void MyKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //Console.WriteLine("MyKeyPress bool : " + AppendBarcode);
            if (AppendBarcode)
            {
                if (e.KeyChar != 13)
                    _barcode.Add((char)e.KeyChar);
                //Console.WriteLine("AppendBarcode : " + (char)e.KeyChar);
                //Console.WriteLine("_barcode : " + new String(_barcode.ToArray()));
            }
            if (e.KeyChar == 13 && _barcode.Count > 0)
            {
                //Console.WriteLine("_barcode.Count " + _barcode.Count);
                string BarCodeData = string.Empty;
                if (Console.CapsLock == true)
                    BarCodeData = changeCase(_barcode.ToArray());
                else
                    BarCodeData = new String(_barcode.ToArray());
                //Console.WriteLine(String.Format("{0}{1}{2}", "[", BarCodeData, "]"));
                product = new Product() { Barcode = BarCodeData, ProductName = BarCodeData, productstatus = ProductStatus.Updated };
                order.Add(product);
                if (canvasUserInfo.Visibility == Visibility.Visible)
                {
                    canvasUserInfo.Visibility = Visibility.Hidden;
                    canvasCoupanDisplay.Visibility = Visibility.Visible;
                }
                //lvItems.Items.MoveCurrentToLast();
                //m_oWorkerdemo.RunWorkerAsync(product);
                view.Refresh();
                _barcode.Clear();
            }
            _lastKeystroke = DateTime.Now.Millisecond;
            //cforKeyDown = (char)e.KeyChar;
            //Console.WriteLine("MyKeyPress : " + (int)e.KeyChar);
        }

        private string changeCase(char[] barcodeData)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in barcodeData)
            {
                if (Char.IsLower(c))
                    sb.Append(c.ToString().ToUpper());
                else if (char.IsUpper(c))
                    sb.Append(c.ToString().ToLower());
                else
                    sb.Append(c.ToString());
            }
            return sb.ToString();
        }

        public void MyKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //Console.WriteLine("MyKeyUp : " + (int)e.KeyCode);
            //Console.WriteLine("cforKeyDown : " + cforKeyDown);
            if (e.KeyData == System.Windows.Forms.Keys.LShiftKey || e.KeyData == System.Windows.Forms.Keys.RShiftKey ||
                     e.KeyData == System.Windows.Forms.Keys.ShiftKey || e.KeyData == System.Windows.Forms.Keys.Shift)
            {
                return;
            }
            if (cforKeyDown != (char)e.KeyCode || cforKeyDown == '\0')
            {
                //Console.WriteLine("cforKeyDown : MyKeyUp =>" + (int)cforKeyDown + " : " + (int)e.KeyCode);
                //Console.WriteLine("MyKeyUp bool : " + AppendBarcode);
                cforKeyDown = '\0';
                _barcode.Clear();
                AppendBarcode = false;
                return;
            }
            AppendBarcode = true;
            int elapsed = (DateTime.Now.Millisecond - _lastKeystroke);
            if (elapsed > 20)
            {
                Console.WriteLine("_barcode.Clear();" + elapsed);
                AppendBarcode = false;
                _barcode.Clear();
            }
            //if(e.KeyCode == System.Windows.Forms.Keys.Return)
            //{
            //    AppendBarcode = false;
            //    //_barcode.Add((char)e.KeyData);
            //}
        }

        private void m_oWorkerdemo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            updateCart();
            view.Refresh();
            Console.WriteLine("m_oWorkerdemo_RunWorkerCompleted : lvItems.count " + lvItems.Items.Count);
        }

        private void m_oWorkerdemo_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("m_oWorkerdemo_DoWork");
        }

        public void MyKeydown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            AppendBarcode = true;
            _lastKeystroke = DateTime.Now.Millisecond;
            if (e.KeyData == System.Windows.Forms.Keys.LShiftKey || e.KeyData == System.Windows.Forms.Keys.RShiftKey ||
                     e.KeyData == System.Windows.Forms.Keys.ShiftKey || e.KeyData == System.Windows.Forms.Keys.Shift)
            {
                return;
            }
            else if (e.KeyData == System.Windows.Forms.Keys.Capital || e.KeyData == System.Windows.Forms.Keys.CapsLock)
            {
                return;
            }
            cforKeyDown = ((char)e.KeyCode);
            //Console.WriteLine("MyKeydown : " + (int)e.KeyData);
        }
        private void ButtonStopClick(object sender, System.EventArgs e)
        {
            CaptureHook.Stop();
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //selectionstart = ((TextBox)e.Source).CaretIndex;
            if (e.Key == Key.Back)
            {
                Isbackpressed = true;
            }
            else
            {

            }
        }
        private void _bubbleImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
            /*if (e.ChangedButton == MouseButton.Right) { if (lvItems2.IsVisible) { lvItems2.Visibility = Visibility.Hidden; } else { lvItems2.Visibility = Visibility.Visible; } }
            */
        }
        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvasEnterPin.Visibility = Visibility.Hidden;
            ScrollBar s = new ScrollBar();
            s.Width = 10;

            if (clicked)
            {
                _btnSaveImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/exportCsv.png", UriKind.Absolute));
                _enterPin.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/enterPingrey.png", UriKind.Absolute));
                _wifiImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/wifi_Red.png", UriKind.Absolute));
                _okImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/RingRinglogo.png", UriKind.Absolute));
                _btnCalender.Visibility = Visibility.Hidden;
                clicked = false;
            }
            else
            {
                _btnSaveImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/save_Green.png", UriKind.Absolute));
                _enterPin.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/enterPin.png", UriKind.Absolute));
                _okImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/ok_Green.png", UriKind.Absolute));
                _wifiImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/wifi_Green.png", UriKind.Absolute));
                clicked = true;
                _btnCalender.Visibility = Visibility.Visible;
            }
        }
        private void _btnCalender_Click(object sender, RoutedEventArgs e)
        {

        }
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).ToolTip = ((TextBlock)sender).Text;
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //IEnumerator t = e.Changes.GetEnumerator();
            //if (t.MoveNext())
            //{
            //    selectionstart = ((TextChange)t.Current).Offset + ((TextChange)t.Current).AddedLength;
            //}
            text = textBox.Text.Replace("-", "").Replace(" ", "").Replace(Environment.NewLine, "");
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
                caretno = ((TextBox)e.Source).CaretIndex;
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
                    if (text.Replace("-", "").Replace(" ", "").Replace(Environment.NewLine, "").Equals("111111111111"))
                    {
                        LoginSuccess();
                    }
                    else if (text.Replace("-", "").Replace(" ", "").Replace(Environment.NewLine, "").Length == 12)
                    {
                        LoginFailed();
                    }
                    else
                    {

                    }
                }
                //else if (text.Length > 12)ring(3, 3) + Environment.NewLine + text.Substring(6, 3) + textdash + text.Substring(9, 3);
                //}
                textBox.Text = text;
                if (!Isbackpressed && sametext)
                {
                    //{
                    //    text = text.Substring(0, 3) + textdash + text.Subst
                    textBox.SelectionStart = textBox.Text.Length; // add some logic if length is 0
                    textBox.SelectionLength = 0;
                    sametext = false;
                }
                else
                {
                    textBox.SelectionStart = caretno; // add some logic if length is 0
                    textBox.SelectionLength = 0;
                    Isbackpressed = false;
                }
            }
            else
            {
                sametext = true;
            }
        }
        private void LoginSuccess()
        {
            canvasEnterPin.Visibility = Visibility.Hidden;
            canvasUserInfo.Visibility = Visibility.Visible;
            UserInfo user = new UserInfo(true, "Mayank", "1234567890");
        }
        private void LoginFailed()
        {
            _lbl_Pin.Content = "Invalid PIN";
            _lbl_Pin.Foreground = Brushes.Red;
            loginfailedflag = true;
            _enterPin.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/enterPingrey.png", UriKind.Absolute));
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e) { }
        private void _okImage1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Product product = ((Product)((Image)sender).DataContext);
            if (!product.Applicable || product.Addedinremovedproduct)
            {
                itemImageClicked = true;
                order.Delete(product);
                view.Refresh();
                return;
            }
        }
        private void lvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemImageClicked)
            {
                itemImageClicked = false;
                return;
            }
            Product product = null;
            if (e.AddedItems.Count != 0)
            {
                product = e.AddedItems[0] as Product;
                if (!product.Applicable) return;
                order.Remove(product);
            }
            else if (e.RemovedItems.Count != 0)
            {
                product = e.RemovedItems[0] as Product;
                if (!product.Applicable) return;
                order.Add(product);
            }
            else
                MessageBox.Show("lvItems_SelectionChanged : else");
            updateCart();
            view.Refresh();
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
            if (BorderTransactionPanel.Visibility == Visibility.Visible)
                BorderTransactionPanel.Visibility = Visibility.Hidden;
            else
                BorderTransactionPanel.Visibility = Visibility.Visible;
        }
        private void _btnRedeem_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("Order is Closed now.!!");
            order.Close(ref order);

            OrderHistory oh0 = new OrderHistory(order.OrderNumber, order.GettotalAmount, DateTime.Now.ToString());
            //OrderHistory oh1 = new OrderHistory("or1", order.GettotalAmount + 1, DateTime.Now.AddHours(2).ToString());
            foreach (var item in order.products)
            {
                oh0.products.Add(new OrderProduct() { ProductAmount = item.Amount, ProductBarcode = item.Barcode, ProductName = item.ProductName });
                //oh1.products.Add(new OrderProduct() { ProductAmount = item.Amount, ProductBarcode = item.Barcode, ProductName = item.ProductName });
            }

            Store.Orders.Add(oh0);
            lvItems.Visibility = Visibility.Hidden;
            _lblHeader.Content = Constants.HeaderTxndescription;
            lvTxnHistory.Visibility = Visibility.Visible;
            _btnSave.Visibility = Visibility.Visible;
            Txnview = CollectionViewSource.GetDefaultView(Store.Orders);
            Txnview.GroupDescriptions.Add(new PropertyGroupDescription("OrderAmount"));
            lvTxnHistory.ItemsSource = Txnview;
        }
        private void _btnSave_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(fd.SelectedPath.Trim()))
                {
                    if (!order.SaveData(fd.SelectedPath.Trim()))
                    {
                        System.Windows.Forms.MessageBox.Show("Error occured while saving");
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Please select folder to save the file.");
                }
            }
        }
        private void TextBlock_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

        }
        private void startWorkerProcess(Product product)
        {
            order.Add(product);
            view.Refresh();
            m_oWorker = new BackgroundWorker();
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;
            m_oWorker.RunWorkerAsync(product);
        }
        private void updateCart()
        {
            _lblTotalDiscountAmount.Content = "₹" + (order.GettotalAmount);
            _lbl_coupan.Content = order.products.Count - order.Rejectedproducts.Count + " coupan(s)";
        }
        private void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // StatusTextBox.Text = "Task Cancelled.";
            }
            // Check to see if an error occurred in the background process.
            else if (e.Error != null)
            {
                //StatusTextBox.Text = "Error while performing background operation.";
            }
            else
            {
                updateCart();
            }
            view.Refresh();
        }
        private void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Product product = (Product)e.Argument;
            for (int i = 0; i < filleditems.Count; i++)
            {
                Thread.Sleep(1000);
                if (product.Barcode == filleditems[i].Barcode)
                {
                    product.Amount = filleditems[i].Amount;
                    if (product.productstatus == ProductStatus.Pending)
                    {
                        product.productstatus = ProductStatus.Updated;
                        if (product.productstatus != ProductStatus.Pending && !product.Addedinremovedproduct)
                        {
                            product.ProductName = filleditems.Single(data => data.Barcode == product.Barcode).ProductName;
                            if (i == 3)
                            {
                                product.Applicable = false;
                                product.Amount = -1.00m;
                                order.Remove(product);
                            }
                        }
                    }
                    e.Result = product;
                    break;
                }
            }
        }
    }
}