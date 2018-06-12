using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RingRing
{
    /// <summary>
    /// Interaction logic for Resized.xaml
    /// </summary>
    public partial class Resized : Window
    {
        private bool itemImageClicked = false;
        Order order;
        ICollectionView view;
        public Resized()
        {
            InitializeComponent();
            order = new Order("SampleOrder1");
            order.Add(new Product() { ProductName = "Complete this WPF tutorial", Barcode = "45" });
            order.Add(new Product() { ProductName = "Learn C#", Barcode = "80" });
            order.Add(new Product() { ProductName = "Wash the car", Barcode = "0" });
            order.Add(new Product() { ProductName = "Complete this WPF tutorial", Barcode = "45" });
            order.Add(new Product() { ProductName = "Learn C#", Barcode = "80" });
            order.Add(new Product() { ProductName = "Wash the car", Barcode = "0" });
            order.Add(new Product() { ProductName = "Complete this WPF tutorial", Barcode = "45" });
            order.Add(new Product() { ProductName = "Learn C#", Barcode = "80" });
            order.Add(new Product() { ProductName = "Wash the car", Barcode = "0" });
            order.Add(new Product() { ProductName = "Complete this WPF tutorial", Barcode = "45" });
            order.Add(new Product() { ProductName = "Learn C#", Barcode = "80" });
            order.Add(new Product() { ProductName = "Wash the car", Barcode = "0" });
            order.Add(new Product() { ProductName = "Complete this WPF tutorial", Barcode = "45" });
            order.Add(new Product() { ProductName = "Learn C#", Barcode = "80" });
            order.Add(new Product() { ProductName = "Wash the car", Barcode = "0" });
            view = CollectionViewSource.GetDefaultView(order.products);
            icTodoList.ItemsSource = view;

            //OrderP order = new OrderP();
            //order.ProductItems.Add(new ProductP() { ItemName = "Colgate Toothpaste Visible White" });
            //order.ProductItems.Add(new ProductP() { ItemName = "Bornvita Chocolate" });
            //order.ProductItems.Add(new ProductP() { ItemName = "Nescafe Sachet 1 gm" });
            //order.ProductItems.Add(new ProductP() { ItemName = "Nescafe Sachet" });

            //Order order1 = new Order("01");
            //order1.Add(new Product() { Barcode = 1100, ProductName = "Colgate Toothpaste Visible White", Amount = 2.51m });
            //order1.Add(new Product() { Barcode = 1101, ProductName = "Bornvita Chocolate", Amount = 0.50m });
            //order1.Add(new Product() { Barcode = 1102, ProductName = "Nescafe Sachet 1 gm", Amount = 2.00m });
            //order1.Add(new Product() { Barcode = 1103, ProductName = "Nescafe Sachet", Amount = 1.00m });


            //Order order2 = new Order("o2");
            //order2.Add(new Product() { Barcode = 1100, ProductName = "Colgate Toothpaste", Amount = 2.51m });
            //order2.Add(new Product() { Barcode = 1101, ProductName = "Bornvita", Amount = 0.50m });
            //order2.Add(new Product() { Barcode = 1102, ProductName = "Nescafe Sachet", Amount = 2.00m });
            //order2.Add(new Product() { Barcode = 1103, ProductName = "Nescafe", Amount = 3.00m });
            //order2.Add(new Product() { Barcode = 1103, ProductName = "Nescafe Jar", Amount = 3.00m });

            //Store.AllOrders.Add(order1);
            //Store.AllOrders.Add(order2);

            //OrderList list = order.AllOrders;


            //OrderList ol = new OrderList();
            //ol.OrderItems.Add(order1);
            //ol.OrderItems.Add(order2);
            //ol.OrderItems[0].GettotalAmount = order1.GettotalAmount;
            //ol.OrderItems[0].products.Add(order.products[0]);
            //ol.OrderItems.Add(order2);

            //view = CollectionViewSource.GetDefaultView(Store.AllOrders);
            //view.GroupDescriptions.Add(new PropertyGroupDescription("GettotalAmount"));
            //lvTxnHistory.ItemsSource = view;

            //Menu menu = new Menu();
            //menu.TopMenuItems.Add(new TopMenu() { GroupName = "Basic Reports - Mobile" });
            //menu.TopMenuItems[0].SubMenuItems.Add(new SubMenu() { ItemName = "Sales Reports - mobile" });
            //menu.TopMenuItems.Add(new TopMenu() { GroupName = "Enhanced Reports - Mobile" });
            //menu.TopMenuItems[1].SubMenuItems.Add(new SubMenu() { ItemName = "Subcategory Month - mobile" });
            //menu.TopMenuItems[1].SubMenuItems.Add(new SubMenu() { ItemName = "Top Categories - mobile" });
            //menu.TopMenuItems.Add(new TopMenu() { GroupName = "Basic Reports - Mobile" });
            //menu.TopMenuItems[2].SubMenuItems.Add(new SubMenu() { ItemName = "Sales Reports - mobile" });
            //view = CollectionViewSource.GetDefaultView(menu.TopMenuItems);
            //view.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
            //groupListView.ItemsSource = view;
        }

        private void OrderItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).ToolTip = ((TextBlock)sender).Text;
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

        private void innercanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (itemImageClicked)
            {
                itemImageClicked = false;
                return;
            }
            Product product = ((Product)((Border)sender).DataContext);
            if (!product.Applicable) return;
            if (!product.Addedinremovedproduct)
            {
                order.Remove(product);
            }
            else
            {
                order.Add(product);
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
    }

    public class TodoItem
    {
        public string Title { get; set; }
        public int Completion { get; set; }
    }

    class Menu  //order
    {
        public ObservableCollection<TopMenu> TopMenuItems { get; set; }  //product
        public Menu()
        {
            TopMenuItems = new ObservableCollection<TopMenu>();
        }
    }
    class TopMenu  //product
    {
        public string GroupName { get; set; }
        public ObservableCollection<SubMenu> SubMenuItems { get; set; }
        public TopMenu()
        {
            SubMenuItems = new ObservableCollection<SubMenu>();
        }
    }
    class SubMenu
    {
        public string ItemName { get; set; }
        public string ItemName1 { get; set; }
        public string ItemName2 { get; set; }
    }

    class OrderList  //order
    {
        public ObservableCollection<Order> OrderItems { get; set; }  //product
        public OrderList()
        {
            OrderItems = new ObservableCollection<Order>();
        }
    }

    class OrderP  //product
    {
        public decimal GettotalAmount
        {
            get; set;
        }
        public ObservableCollection<ProductP> ProductItems { get; set; }
        public OrderP()
        {
            ProductItems = new ObservableCollection<ProductP>();
        }
    }

    class ProductP //product
    {
        public string ItemName { get; set; }
    }
}