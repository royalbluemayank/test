using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        ObservableCollection<Product> items;
        ICollectionView view;

        //ObservableCollection<Person> myList;
        public Window1()
        {
            InitializeComponent();
            items = new ObservableCollection<Product>();
            //items.Add(new Product() { Barcode = 0, Name = "Colgate Toothpaste Visible White", Amount = "Rs.42.0" });
            //items.Add(new Product() { Barcode = 1, Name = "Bornvita Chocolate", Amount = "Rs.39.19" });
            //items.Add(new Product() { Barcode = 2, Name = "Nescafe Sachet", Amount = "Rs.13.09" });
            //items.Add(new Product() { Barcode = 3, Name = "Nescafe Jar", Amount = "Rs.130.09" });
            //items.Add(new Product() { Barcode = 4, Name = "Jelly Belly", Amount = "Rs.2.00" });
            //items.Add(new Product() { Barcode = 5, Name = "Mother Dairy Ghee", Amount = "Rs.1.09" });
            //items.Add(new Product() { Barcode = 6, Name = "Britannia Bourbon Cream Biscuit", Amount = "Rs.13.76" });
            //items.Add(new Product() { Barcode = 7, Name = "Colgate Toothpaste Visible White1", Amount = "Rs.42.90" });
            //items.Add(new Product() { Barcode = 8, Name = "Bornvita Chocolate1", Amount = "Rs.39.19" });
            //items.Add(new Product() { Barcode = 9, Name = "Nescafe Sachet1", Amount = "Rs.13.09" });
            //items.Add(new Product() { Barcode = 10, Name = "Nescafe Jar1", Amount = "Rs.130.09" });
            //items.Add(new Product() { Barcode = 11, Name = "Jelly Belly1", Amount = "Rs.2.00" });
            //items.Add(new Product() { Barcode = 12, Name = "Mother Dairy Ghee1", Amount = "Rs.1.09" });
            //items.Add(new Product() { Barcode = 13, Name = "Britannia Bourbon Cream Biscuit1", Amount = "Rs.13.76" });


            //myList = new ObservableCollection<Person>()
            //{
            //    new Person{ Name="Name 1", Age=24, Country="Japan"},
            //    new Person{ Name="Name 2", Age=24, Country="India"},
            //    new Person{ Name="Name 3", Age=24, Country="China"},
            //    new Person{ Name="Name 4", Age=24, Country="Japan"},
            //    new Person{ Name="Name 5", Age=24, Country="India"},
            //    new Person{ Name="Name 6", Age=24, Country="US"},
            //    new Person{ Name="Name 7", Age=24, Country="US"},
            //    new Person{ Name="Name 8", Age=24, Country="India"},
            //    new Person{ Name="Name 9", Age=24, Country="India"},
            //    new Person{ Name="Name 10", Age=24, Country="India"},
            //    new Person{ Name="Name 11", Age=24, Country="India"},
            //    new Person{ Name="Name 12", Age=24, Country="China"},
            //    new Person{ Name="Name 13", Age=24, Country="India"},
            //    new Person{ Name="Name 14", Age=24, Country="India"},
            //    new Person{ Name="Name 15", Age=24, Country="India"},
            //    new Person{ Name="Name 16", Age=24, Country="China"},
            //    new Person{ Name="Name 17", Age=24, Country="India"},
            //    new Person{ Name="Name 18", Age=24, Country="India"},
            //    new Person{ Name="Name 19", Age=24, Country="India"},
            //    new Person{ Name="Name 20", Age=24, Country="US"},
            //    new Person{ Name="Name 21", Age=24, Country="US"},
            //    new Person{ Name="Name 22", Age=24, Country="India"},
            //};
            ////lvItems.ItemsSource = myList;

            view = CollectionViewSource.GetDefaultView(items);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Amount"));
            //view.SortDescriptions.Add(new SortDescription("Amount", ListSortDirection.Ascending));
            //view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            TxnHistory.ItemsSource = view;
            //MessageBox.Show("count : " + items.Count);

        }

        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Country { get; set; }
        }

        private void expand_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           // Product u = new Product() { Barcode = 0, ProductName = "Colgate Toothpaste Visible White", Amount = 2.90m };

           // foreach (var item in items)
           // {
           //     if (item.Amount== 42.90m)
           //     {
           //         item.Amount = u.Amount;
           //     }
           // }
           //// items.Add(new User() { index = 0, Name = "Colgate Toothpaste Visible White", Amount = "Rs.2.90" });
           ////items.Add(new User() { index = 15, Name = "Bornvita Chocolate", Amount = "Rs.1.90" });
           ////items.Add(new User() { index = 16, Name = "Nescafe Sachet", Amount = "Rs.1.90" });
           ////items.Add(new User() { index = 17, Name = "Nescafe Jar", Amount = "Rs.1.90" });
           // //lvItems.Items.Refresh();
           // view.Refresh();
            //MessageBox.Show("count : " + items.Count);
        }


        //foreach (User item in items)
        //{
        //    topmenu.mainUser.Add(new MainUser() { Amount = item.Amount });
        //    topmenu.mainUser[i].user.Add(item);
        //    i++;
        //}

        //this.DataContext = topmenu;

        //lvItems.ItemsSource = items;


        //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvItems.ItemsSource);
        //PropertyGroupDescription groupDescription = new PropertyGroupDescription("User");
        //view.GroupDescriptions.Add(groupDescription);
        //PropertyGroupDescription groupDescription2 = new PropertyGroupDescription("Amount");
        //view.GroupDescriptions.Add(groupDescription2);

    }
}
