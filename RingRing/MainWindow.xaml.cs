using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            ListViewItem list;
            for (int i = 0; i < 15; i++)
            {
                list = new ListViewItem();
                
                ListView1.Items.Add(i);
            }
        }
        private void _bubbleImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (BorderPanel.IsVisible)
            {
                BorderPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                BorderPanel.Visibility = Visibility.Visible;
            }
        }
    }
}
