using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RingRing
{
    public enum OrderStatus
    {
        Close = 0, Open = 1
    }
    public class Order
    {
        public readonly string OrderNumber = string.Empty;
        public static bool IsClosed = true;
        private string Datetime;
        private static string FileStartText { get { return "{\"JsonData\":["; } }
        private static string FileEndText { get { return "]}"; } }
        private static string FileHeadersAllTxn { get { return "SNo,Product,Name,Amount,DateTime"; } }
        public static OrderStatus orderStatus { get; private set; }
        public Order(String OrderNumber)
        {
            this.OrderNumber = OrderNumber;
            this.DateTime = System.DateTime.Now.ToString();
            products = new ObservableCollection<Product>();
            Rejectedproducts = new List<Product>();
            Deletedproducts = new List<Product>();
            IsClosed = false;
            Order.orderStatus = OrderStatus.Open;
        }
        public ObservableCollection<Product> products
        {
            get; set;
        }
        public List<Product> Rejectedproducts
        {
            get; private set;
        }
        public List<Product> Deletedproducts
        {
            get; private set;
        }
        public void AddProduct(Product product)
        {
            if (!products.Contains(product))
            {
                products.Add(product);
            }
            else if (Rejectedproducts.Contains(product))
            {
                Rejectedproducts.Remove(product);
            }
            product.Added();
        }
        public void RemoveProduct(Product product)
        {
            if (products.Contains(product))
            {
                Rejectedproducts.Add(product);
                product.Removed();
            }
            //products.Remove(products.FirstOrDefault (x => x.Barcode == product.Barcode));
            //products.RemoveAll(x => x.Barcode == product.Barcode);
        }
        public void DeleteProduct(Product product)
        {
            //if (products.Contains(product))
            //{
            //    products.Remove(product);
            //}
            //if (Rejectedproducts.Contains(product))
            //{
            //    Rejectedproducts.Remove(product);
            //}
            //if (!Deletedproducts.Contains(product))
            //{
            //    Deletedproducts.Add(product);
            //}
        }
        public void UpdateProducts()
        {
            if (Rejectedproducts.Count > 0)
            {
                Deletedproducts.AddRange(Rejectedproducts);
                foreach (var item in Rejectedproducts)
                {
                    products.Remove(item);
                }
                Rejectedproducts.Clear();
            }
        }
        public decimal GettotalAmount
        {
            get { return products.Where(e => !e.Addedinremovedproduct && e.Applicable).Select(x => x.Amount).Sum(); }
        }
        public int GetProductCount
        {
            get { return products.Count - Rejectedproducts.Count; }
        }
        public string DateTime
        {
            get
            {
                DateTime dt = Convert.ToDateTime(this.Datetime);
                return string.Format("{0:hh:mm:ss tt MMMM dd}{1} {0:yyyy}", dt, ((dt.Day % 10 == 1 && dt.Day != 11) ? "st" : (dt.Day % 10 == 2 && dt.Day != 12) ? "nd"
                                                                                                                        : (dt.Day % 10 == 3 && dt.Day != 13) ? "rd" : "th"));
                //return Convert.ToDateTime(this.Datetime).ToString("hh:mm tt MMMM dd++ yyyy").Replace("++", "th"); //12:55 PM February 26th 2018
            }
            private set
            {
                this.Datetime = value;
            }
        }
        public bool SaveData(string filepath)
        {
            try
            {
                filepath = filepath + "\\" + OrderNumber + ".csv";
                using (StreamWriter sw = new StreamWriter(File.Open(filepath, FileMode.Append)))
                {
                    if (new FileInfo(filepath).Length == 0)
                    {
                        sw.WriteLine(FileHeadersAllTxn);
                    }
                    StringBuilder sb = new StringBuilder();
                    int i = 1;
                    foreach (Product item in products.Where(e => !e.Addedinremovedproduct && e.Applicable))
                    {
                        sb.AppendLine(String.Format("{0},{1}", i++, item.ToString()));
                    }
                    sb.AppendLine(string.Format(",,,{0}", GettotalAmount.ToString("0.00")));
                    sw.WriteLine(sb);
                }
                Process.Start(filepath);
                MessageBox.Show("file saved.!!");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
                return false;
            }
        }
        public static bool SaveItem(Product product)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(Constants.PathforSaveItem, FileMode.Append)))
                {
                    if (new FileInfo(Constants.PathforSaveItem).Length == 0)
                    {
                        sw.WriteLine(FileHeadersAllTxn);
                    }
                    sw.WriteLine(String.Format("{0}", product.ToString()));
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
                return false;
            }
        }
        public static bool SaveAnonymousItem(OrderHistory.Product product)
        {
            Console.WriteLine("SaveAnonymousItem");
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(Constants.PathforSaveItem, FileMode.Append)))
                {
                    Debug.WriteLine("Constants.PathforSaveItem");
                    sw.Write(String.Format("{0},", product.ToJson()));
                    Debug.WriteLine(String.Format("{0},", product.ToJson()));
                }
                SendAnonymousList();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
                return false;
            }
        }
        public static void Logger(String logs)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(Constants.Pathforlogger, FileMode.Append)))
                {
                    sw.WriteLine(String.Format("[ {0} ] ------ {1}", System.DateTime.Now.ToString("yyyy MMMM dd hh:mm:ss tt"),logs));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }
        public static bool SendAnonymousList()
        {
            try
            {
                Debug.WriteLine("SendAnonymousList");
                String Data = File.ReadAllText(Constants.PathforSaveItem);
                Debug.WriteLine("Data : " + Data);
                int count = Data.Count(character => character == '}');
                Debug.WriteLine("count : " + count);
                if (count > 50)
                {
                    Data = Data.Substring(0, Data.Length - 1);
                    Data = FileStartText + Data + FileEndText;
                    //File.Delete(Constants.PathforSaveItem);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            return false;
        }
        public static void Close(ref Order order)
        {
            Order.orderStatus = OrderStatus.Close;
            order = null;
            //SaveData(".");
            //AllOrders.Add(order);
        }
        public class UserInfo
        {
            public UserInfo(string Name, string PhoneNo)
            {
                this.Name = Name;
                this.PhoneNo = PhoneNo;
            }
            public string Name { get; }
            public string PhoneNo { get; }
        }

        //public void Edit(Product product)
        //{
        //    products.Add(product);
        //}
        //private static List<Product> products;
        //public static int GetUnitCount()
        //{
        //    //return Order.Products.Select(x => x.Quantity).Sum();
        //}
        //public static decimal GetInventoryValue()
        //{
        //    //return Order.Products.Select(x => (x.Price * x.Quantity)).Sum();
        //}
        //private static void Load()
        //{
        //    Products = DataManager.LoadProducts();
        //}
        //private static void Save()
        //{
        //    DataManager.SaveProducts(Products);
        //}
        //public static void RemoveProduct(int productId)
        //{
        //    Order.Products.RemoveAll(x => x.Id == productId);
        //    Save();
        //}
        //public static void ClearInventory()
        //{
        //    Order.Products.Clear();
        //    Save();
        //}
    }
}

//static class DataManager
//{
//    private static string dataPath = "data.json";

//    public static List<Product> LoadProducts()
//    {
//        List<Product> listOfProducts = new List<Product>();

//        if (File.Exists(dataPath))
//        {
//            string json = File.ReadAllText("data.json");
//            if (!string.IsNullOrWhiteSpace(json))
//            {
//                listOfProducts = JsonConvert.DeserializeObject<List<Product>>(json);
//            }
//        };

//        return listOfProducts;
//    }

//    public static void SaveProducts(List<Product> productsToSave)
//    {
//        if (!File.Exists(dataPath))
//            File.Create(dataPath);

//        string json = JsonConvert.SerializeObject(productsToSave);

//        File.WriteAllText(dataPath, json);
//    }
//}
