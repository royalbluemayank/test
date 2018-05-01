using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RingRing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public String textdash = " - ";
        public bool Isbackpressed = false;
        List<User> items;
        Decimal totalAmount = 0;
        Decimal SelectedAmout = 0;
        public MainWindow()
        {
            InitializeComponent();
            this.Left = SystemParameters.WorkArea.Width - this.Width - 20;
            this.Top = 5;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.Focus();

            items = new List<User>();
            items.Add(new User() { index = 0, Name = "Colgate Toothpaste Visible White", Amount = "42.90" });
            items.Add(new User() { index = 1, Name = "Bornvita Chocolate", Amount = "39.19" });
            items.Add(new User() { index = 2, Name = "Nescafe Sachet", Amount = "13.09" });
            items.Add(new User() { index = 4, Name = "Jelly Belly", Amount = "2.00" });
            items.Add(new User() { index = 5, Name = "Mother Dairy Ghee", Amount = "39.00" });
            items.Add(new User() { index = 6, Name = "Britannia Bourbon Cream Biscuit", Amount = "13.76" });
            lvDataBinding.ItemsSource = items;

            foreach (User item in items)
            {
                totalAmount += Convert.ToDecimal(item.Amount);
            }
            _lblTotalDiscountAmount.Content = "₹" + totalAmount;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                Isbackpressed = true;
            }
        }
        private void _bubbleImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
            /*if (e.ChangedButton == MouseButton.Right) { if (lvDataBinding2.IsVisible) { lvDataBinding2.Visibility = Visibility.Hidden; } else { lvDataBinding2.Visibility = Visibility.Visible; } }
            */
        }

        public bool clicked = true;
        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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
        private void _btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Test");
        }
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).ToolTip = ((TextBlock)sender).Text;
            //MessageBox.Show(((TextBlock)sender).Text);
        }
        
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (textBox.Text.Replace("-", "").Replace(" ", "").Length > 6)
            {
                textBox.Text = textBox.Text.Replace("-", "").Replace(" ", "").Substring(0, textBox.Text.Replace("-", "").Replace(" ", "").Length);
            }
            if (textBox.Text.Replace("-", "").Replace(" ", "").Length > 3)
            {
                textBox.Text = textBox.Text.Replace("-", "").Replace(" ", "").Substring(0, 3) + textdash +
                    textBox.Text.Replace("-", "").Replace(" ", "").Substring(3, textBox.Text.Replace("-", "").Replace(" ", "").Length - 3);
            }
            else
            {
                textBox.Text = textBox.Text.Replace("-", "").Replace(" ", "").Substring(0, textBox.Text.Replace("-", "").Replace(" ", "").Length);
            }
            if (!Isbackpressed)
            {
                textBox.SelectionStart = textBox.Text.Length; // add some logic if length is 0
                textBox.SelectionLength = 0;
            }
            else
            {
                Isbackpressed = false;
            }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox1.Text.Replace("-", "").Replace(" ", "").Length > 6)
            {
                textBox1.Text = textBox1.Text.Replace("-", "").Replace(" ", "").Substring(0, textBox1.Text.Replace("-", "").Replace(" ", "").Length);
            }
            if (textBox1.Text.Replace("-", "").Replace(" ", "").Length > 3)
            {
                textBox1.Text = textBox1.Text.Replace("-", "").Replace(" ", "").Substring(0, 3) + textdash +
                    textBox1.Text.Replace("-", "").Replace(" ", "").Substring(3, textBox1.Text.Replace("-", "").Replace(" ", "").Length - 3);
            }
            else
            {
                textBox1.Text = textBox1.Text.Replace("-", "").Replace(" ", "").Substring(0, textBox1.Text.Replace("-", "").Replace(" ", "").Length);
            }
            if (!Isbackpressed)
            {
                textBox1.SelectionStart = textBox1.Text.Length; // add some logic if length is 0
                textBox1.SelectionLength = 0;
            }
            else
            {
                Isbackpressed = false;
            }
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Back))
            {
                MessageBox.Show("Test2");
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Back))
            {
                MessageBox.Show("Test");
            }
            if (e.Key == Key.Back)
            {
                Isbackpressed = true;
            }
        }
        private void _okImage1_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void lvDataBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAmout = 0;
            foreach (User item in lvDataBinding.SelectedItems)
            {
                SelectedAmout += Convert.ToDecimal(item.Amount);
            }
            _lblTotalDiscountAmount.Content = "₹" + (totalAmount - SelectedAmout);
        //    User selectedUsed = lvDataBinding.SelectedItem as User;
        //    if (!items[selectedUsed.index].Selected)
        //        items[selectedUsed.index].Selected = true;
        //    else
        //        items[selectedUsed.index].Selected = false;
        }
    }

    public class User
    {
        public int index { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public bool Selected { get; set; }

        public override string ToString()
        {
            return this.Name + "," + this.Amount;
        }
    }

    public class MainUser
    {
        public MainUser()
        {
            user = new ObservableCollection<User>();
        }
        public string Amount { get; set; }
        public ObservableCollection<User> user { get; set; }

    }

    class TopMenu
    {
        public ObservableCollection<MainUser> mainUser { get; set; }
        public TopMenu()
        {
            mainUser = new ObservableCollection<MainUser>();
        }
    }

}
