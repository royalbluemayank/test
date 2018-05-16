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

namespace RingRing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Productdummy> filleditems;
        ICollectionView view, Txnview;
        BackgroundWorker m_oWorker;
        //private Object thisLock = new Object();
        public bool clicked, sametext = true;
        private bool itemImageClicked, Isbackpressed, loginfailedflag = false;
        int value = -1;
        Store store;
        String text, previousText = string.Empty, textdash = " - ";
        int caretno = 0;
        Order order;
        public MainWindow()
        {
            InitializeComponent();
            this.Left = SystemParameters.WorkArea.Width - this.Width - 20;
            this.Top = 5;
            textBox.Focus();
            createData();
            _lblStoreName.Content = store.StoreName;
            _lblTvsId.Content = store.fullTvsId;
            order = new Order("order1");
            view = CollectionViewSource.GetDefaultView(order.products);
            lvItems.ItemsSource = view;
            Txnview = CollectionViewSource.GetDefaultView(filleditems);
            Txnview.GroupDescriptions.Add(new PropertyGroupDescription("Amount"));
            //view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            lvTxnHistory.ItemsSource = Txnview;
            _lblHeader.Content = Constants.HeaderProductdescription;
        }
        public void createData()
        {
            //totalitems = new List<Product>();
            //removedproductfromCart = new List<Product>();
            filleditems = new List<Productdummy>();
            store = new Store("0987654321", "Ali Hyper Market");
            filleditems.Add(new Productdummy() { index = 1100, Name = "Colgate Toothpaste Visible White", Amount = 2.51m });
            filleditems.Add(new Productdummy() { index = 1101, Name = "Bornvita Chocolate", Amount = 0.50m });
            filleditems.Add(new Productdummy() { index = 1102, Name = "Nescafe Sachet", Amount = 2.00m });
            filleditems.Add(new Productdummy() { index = 1103, Name = "Nescafe Sachet", Amount = 1.00m });
            filleditems.Add(new Productdummy() { index = 1104, Name = "Jelly Belly", Amount = 2.00m });
            filleditems.Add(new Productdummy() { index = 1105, Name = "Mother Dairy Ghee", Amount = 39.00m });
            filleditems.Add(new Productdummy() { index = 1106, Name = "Britannia Bourbon Cream Biscuit", Amount = 13.76m });
            filleditems.Add(new Productdummy() { index = 1107, Name = "Colgate Toothpaste Visible White 100 gm pack", Amount = 124222.90m });
            filleditems.Add(new Productdummy() { index = 1108, Name = "Bornvita Chocolate", Amount = 39.19m });
            filleditems.Add(new Productdummy() { index = 1109, Name = "Nescafe Sachet", Amount = 13.09m });
            filleditems.Add(new Productdummy() { index = 1110, Name = "Jelly Belly", Amount = 2.00m });
            filleditems.Add(new Productdummy() { index = 1111, Name = "Mother Dairy Ghee", Amount = 39.00m });
            filleditems.Add(new Productdummy() { index = 1112, Name = "Britannia Bourbon Cream Biscuit", Amount = 13.76m });
            //items.Add(new User() { index = 0, Name = "Colgate Toothpaste Visible White", Amount = "2.40" });
            //items.Add(new User() { index = 1, Name = "Bornvita Chocolate", Amount = "Pending" });
            //items.Add(new User() { index = 2, Name = "Nescafe Sachet", Amount = "Pending" });
            //items.Add(new User() { index = 3, Name = "Nescafe Sachet", Amount = "Pending" });
            //items.Add(new User() { index = 4, Name = "Jelly Belly", Amount = "Pending" });
            //items.Add(new User() { index = 5, Name = "Mother Dairy Ghee", Amount = "Pending" });
            //items.Add(new User() { index = 6, Name = "Britannia Bourbon Cream Biscuit", Amount = "Pending" });
            //items.Add(new User() { index = 7, Name = "Colgate Toothpaste Visible White", Amount = "Pending" });
            //items.Add(new User() { index = 8, Name = "Bornvita Chocolate", Amount = "Pending" });
            //items.Add(new User() { index = 9, Name = "Nescafe Sachet", Amount = "Pending" });
            //items.Add(new User() { index = 10, Name = "Jelly Belly", Amount = "Pending" });
            //items.Add(new User() { index = 11, Name = "Mother Dairy Ghee", Amount = "Pending" });
            //items.Add(new User() { index = 12, Name = "Britannia Bourbon Cream Biscuit", Amount = "Pending" });
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
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
        }
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
        private void _btn_AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (!UserInfo.Islogin)
            {
                System.Windows.Forms.MessageBox.Show("Please login");
                return;
            }
            if (value == -1)
            {
                canvasUserInfo.Visibility = Visibility.Hidden;
                canvasCoupanDisplay.Visibility = Visibility.Visible;
            }
            if (value++ == filleditems.Count - 1)
            {
                if (System.Windows.Forms.MessageBox.Show("reload Items as filled items list is empty", "Refill items", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    value = 0;
                    Order.Clean(ref order);
                    order = new Order("order1");
                    view = CollectionViewSource.GetDefaultView(order.products);
                    lvItems.ItemsSource = view;
                }
                else
                {
                    return;
                }
            }

            //order.Add(new Product() { Barcode = value, Name = filleditems[value].Name });
            //Order o = new Order("1", p);
            //o.Delete(p);
            //p.Barcode = 12;
            //o.Delete(p);
            //Order.Clean(ref o);
            startWorkerProcess(new Product() { Barcode = 1100 + value, Name = filleditems[value].Name });
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

        }

        private void _btnSave_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(fd.SelectedPath.Trim()))
                {
                    if (!order.SaveData(fd.SelectedPath.Trim())) {
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
            //lvItems.SelectedItems.Add(selecteditems);

            //lvItems.Focus();


            // lvItems.Items.DeferRefresh();
            //_lblTotalDiscountAmount.Content = "₹" + totalAmount;
            // Everything completed normally.
            //StatusTextBox.Text = "Task Completed...";
        }
        private void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Product product = (Product)e.Argument;
            for (int i = 0; i < filleditems.Count; i++)
            {
                Thread.Sleep(1000);
                if (product.Barcode == filleditems[i].index)
                {
                    product.Amount = filleditems[i].Amount;
                    if (product.productstatus == ProductStatus.Pending)
                    {
                        product.productstatus = ProductStatus.Updated;
                        if (product.productstatus != ProductStatus.Pending && !product.Addedinremovedproduct)
                        {
                            if (i == 3)
                            {
                                product.Applicable = false;
                                product.Amount = -1.00m;
                                order.Remove(product);
                                //changeselection(product);
                            }
                            //totaldiscountedAmount += product.Amount;
                            //Console.WriteLine("m_oWorker_DoWork totaldiscountedAmount stop: {0} - {1}", product.ToString(), product.Amount);
                        }
                        //changeselection(product);  // added mayank
                        //if (!product.Addedinremovedproduct)   // commented mayank
                        //totaldiscountedAmount += Convert.ToDecimal(product.Amount); commented mayank
                    }
                    break;
                }
                //m_oWorker.ReportProgress(i, user);
                //if (m_oWorker.CancellationPending)
                //{
                //    e.Cancel = true;
                //    //m_oWorker.ReportProgress(0);
                //    return;
                //}
            }
            //m_oWorker.ReportProgress(100);
        }
        //private void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    lock (thisLock)
        //    {
        //        product product = (User)e.UserState;
        //        //if (product != null)
        //        //  ContainerPanel.Controls.Add(new Label { Text = oEmp.Name });
        //        //ListBox1.Items.Add(new ListBoxItem { Content = oEmp.Name + " item added" + e.ProgressPercentage.ToString() + "%" });
        //        //StatusTextBox.Text = "Processing......" + e.ProgressPercentage.ToString() + "%";
        //    }
        //}
    }
}