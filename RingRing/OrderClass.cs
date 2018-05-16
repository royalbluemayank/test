using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RingRing
{
    public class Order
    {
        //private static List<Product> products;
        public readonly string OrderNumber = string.Empty;

        public static bool IsOrderAnonymous = false;
        private static string FileHeadersAllTxn { get { return "SNo,Product,Name,Amount,DateTime"; } }
        public Order(String OrderNumber)
        {
            this.OrderNumber = OrderNumber;
            products = new List<Product>();
            Rejectedproducts = new List<Product>();
        }
        public List<Product> products
        {
            get; private set;
        }
        public List<Product> Rejectedproducts
        {
            get; private set;
        }
        public void Add(Product product)
        {
            if (!products.Contains(product)){
                products.Add(product);
            }
            else if (Rejectedproducts.Contains(product)){
                Rejectedproducts.Remove(product);
            }
            product.Added();
        }

        //public void Edit(Product product)
        //{
        //    products.Add(product);
        //}
        public void Remove(Product product)
        {
            if (products.Contains(product))
            {
                Rejectedproducts.Add(product);
                product.Removed();
            }
            //products.Remove(products.FirstOrDefault (x => x.Barcode == product.Barcode));
            //products.RemoveAll(x => x.Barcode == product.Barcode);
        }
        public void Delete(Product product)
        {
            if (products.Contains(product))
            {
                products.Remove(product);
            }
            if (Rejectedproducts.Contains(product))
            {
                Rejectedproducts.Remove(product);
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
        internal static void Clean(ref Order o)
        {
            o = null;
        }
        internal bool SaveData(string filepath)
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
                    int i = 0;
                    foreach (Product item in products.Where(e => !e.Addedinremovedproduct && e.Applicable))
                    {
                        sb.AppendLine(String.Format("{0},{1}", i++, item.ToString()));
                    }
                    sb.AppendLine(string.Format(",,,{0}", GettotalAmount.ToString("0.00")));
                    sw.WriteLine(sb);
                }
                Process.Start(filepath);
                System.Windows.Forms.MessageBox.Show("file saved.!!");
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.StackTrace);
                return false;
            }
        }
       
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
