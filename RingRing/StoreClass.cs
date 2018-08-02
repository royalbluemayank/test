using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace RingRing
{
    public class Store
    {
        private static int _Id = -1;
        private string _TvsId;
        public Store(string TvsId, string StoreName, string Currency = "Rs.")
        {
            _Id++;
            this._TvsId = TvsId;
            this.StoreName = StoreName;
            Store.Currency = Currency + " ";
            Orders = new ObservableCollection<OrderHistory>();
        }
        public int Id { get { return _Id; } }
        public string TvsId
        {
            get
            {
                string value = "";
                for (int i = 0; i < this._TvsId.Length; i++)
                {
                    value += this._TvsId[i];
                    if (i % 3 == 2)
                    {
                        value += " ";
                    }
                }
                return value.Trim();
            }
        }
        public static string Currency
        {
            get; private set;
        }
        public string fullTvsId
        {
            get
            {
                return "TVS ID: " + TvsId;
            }
        }
        public string StoreName { get; set; }
        public static ObservableCollection<OrderHistory> Orders
        {
            get; set;
        }
        public override string ToString()
        {
            return this._TvsId + " , " + this.StoreName;
        }
    }

    public class OrderHistory
    {
        //private string Datetime;
        public string OrderNumber { get; }
        public decimal OrderAmount { set; get; }
        public string DateTime
        {
            get; private set;
            //get
            //{
            //    DateTime dt = Convert.ToDateTime(this.Datetime);
            //    return string.Format("{0:hh:mm:ss tt MMMM dd}{1} {0:yyyy}", dt , ((dt.Day % 10 == 1 && dt.Day != 11) ? "st": (dt.Day % 10 == 2 && dt.Day != 12) ? "nd"
            //                                                                                                            : (dt.Day % 10 == 3 && dt.Day != 13) ? "rd": "th"));
            //    //return Convert.ToDateTime(this.Datetime).ToString("hh:mm tt MMMM dd++ yyyy").Replace("++", "th"); //12:55 PM February 26th 2018
            //}
            //private set
            //{
            //    this.Datetime = value;
            //}
        }
        public ObservableCollection<Product> products
        {
            get; set;
        }
        public OrderHistory(string OrderNumber, decimal OrderAmount, string OrderDateTime)
        {
            this.OrderNumber = OrderNumber;
            this.OrderAmount = OrderAmount;
            this.DateTime = OrderDateTime;
            products = new ObservableCollection<Product>();
        }
        public class Product
        {
            public string ProductID { get; set; }
            public string Barcode { get; set; }
            public string ProductName { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get { return Store.Currency; } }
            public string ToJson()
            {
                return "{\"ProductBarcode\":\"" + this.Barcode + "\",\"TimeStamp\":\"" + System.DateTime.Now.ToString("MM/dd/yyyy H:mm:ss:ffff zzz") + "\"}"; //yyyyMMddHHmmssffff
            }
        }
    }
}
