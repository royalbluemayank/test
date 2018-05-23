using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace RingRing
{
    public class Store
    {
        private static int _Id = -1;
        private string _TvsId;
        public Store(string TvsId, string StoreName)
        {
            _Id++;
            this._TvsId = TvsId;
            this.StoreName = StoreName;
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
            return this.TvsId + " , " + this.StoreName;
        }
    }

    public class OrderHistory
    {
        private string Datetime;
        public string OrderNumber { get; }
        public decimal OrderAmount { get; }
        public string DateTime 
        {
            get
            {
                return Convert.ToDateTime(this.Datetime).ToString("hh:mm tt MMMM dd++ yyyy").Replace("++", "th"); //12:55 PM February 26th 2018
            }
            set
            {
                this.Datetime = value;
            }
        }
        public ObservableCollection<OrderProduct> products
        {
            get; set;
        }
        public OrderHistory(string OrderNumber, decimal OrderAmount, String DateTime )
        {
            this.OrderNumber = OrderNumber;
            this.OrderAmount = OrderAmount;
            this.DateTime = DateTime;
            products = new ObservableCollection<OrderProduct>();
        }
    }

    public class OrderProduct
    {
        public string ProductBarcode { get; set; }
        public string ProductName { get; set; }
        public decimal ProductAmount { get; set; }

    }
}
