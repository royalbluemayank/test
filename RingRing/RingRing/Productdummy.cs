using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RingRing
{
    public class Productdummy
    {
        private static int ID = -1;
        private string Datetime;
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public Productdummy(ProductStatus productstatus = ProductStatus.Pending, string BorderColor = "White", string BackgroundColor = "White", decimal Amount = 0.00m, string Image = "ok_Green")
        {
            ID++;
            this.productstatus = ProductStatus.Pending;
            this.BorderColor = BorderColor;
            this.BackgroundColor = BackgroundColor;
            this.Amount = Amount;
            this.Image = "/Resources/" + Image + ".png";
            this.DateTime = System.DateTime.Now.ToString();
            this.Applicable = true;
        }
        public ProductStatus productstatus { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public decimal Amount { get; set; }
        public string BorderColor { get; set; }
        public string BackgroundColor { get; set; }
        public bool Added { get; set; }
        public bool Applicable { get; set; }
        public string DateTime
        {
            get
            {
                return System.Convert.ToDateTime(this.Datetime).ToString("hh:mm tt MMMM dd++ yyyy").Replace("++", "th"); //12:55 PM February 26th 2018
            }
            set
            {
                this.Datetime = value;
            }
        }
        public override string ToString()
        {
            return this.Barcode + " , " + this.ProductName + " , " + this.Amount;
        }

        public static List<Productdummy> getDummyData
        {
            get { return createData(); }
        }
        private static List<Productdummy> createData()
        {
            List<Productdummy> filleditems = new List<Productdummy>();
            filleditems.Add(new Productdummy() { Barcode = "1100", ProductName = "Colgate Toothpaste Visible White", Amount = 2.51m });
            filleditems.Add(new Productdummy() { Barcode = "1101", ProductName = "Bornvita Chocolate", Amount = 0.50m });
            filleditems.Add(new Productdummy() { Barcode = "1102", ProductName = "Nescafe Sachet 1 gm", Amount = 2.00m });
            filleditems.Add(new Productdummy() { Barcode = "1103", ProductName = "Nescafe Sachet", Amount = 1.00m });
            filleditems.Add(new Productdummy() { Barcode = "1104", ProductName = "Jelly Belly", Amount = 2.00m });
            filleditems.Add(new Productdummy() { Barcode = "1105", ProductName = "Mother Dairy Ghee", Amount = 39.00m });
            filleditems.Add(new Productdummy() { Barcode = "1106", ProductName = "Britannia Bourbon Cream Biscuit", Amount = 13.76m });
            filleditems.Add(new Productdummy() { Barcode = "1107", ProductName = "Colgate Toothpaste Visible White 100 gm pack", Amount = 124222.90m });
            filleditems.Add(new Productdummy() { Barcode = "1108", ProductName = "Bornvita Chocolate", Amount = 39.19m });
            filleditems.Add(new Productdummy() { Barcode = "1109", ProductName = "Nescafe Sachet", Amount = 13.09m });
            filleditems.Add(new Productdummy() { Barcode = "1110", ProductName = "Jelly Belly", Amount = 2.00m });
            filleditems.Add(new Productdummy() { Barcode = "1111", ProductName = "Mother Dairy Ghee", Amount = 39.00m });
            filleditems.Add(new Productdummy() { Barcode = "1112", ProductName = "Haldiram's Tomato Ketchup", Amount = 13.76m });
            filleditems.Add(new Productdummy() { Barcode = "1113", ProductName = "Colgate Toothpaste Red", Amount = 2.51m });
            filleditems.Add(new Productdummy() { Barcode = "1114", ProductName = "Bornvita Chocolate 50 gm", Amount = 0.50m });
            filleditems.Add(new Productdummy() { Barcode = "1115", ProductName = "Nescafe Sachet 10 gm", Amount = 2.00m });
            filleditems.Add(new Productdummy() { Barcode = "1116", ProductName = "Britannia Bourbon Cream Biscuit", Amount = 13.76m });
            filleditems.Add(new Productdummy() { Barcode = "1117", ProductName = "Colgate Toothpaste Visible White 500 gm pack", Amount = 1222.90m });
            filleditems.Add(new Productdummy() { Barcode = "1118", ProductName = "Bornvita Vanilla 2 gm", Amount = 39.19m });
            filleditems.Add(new Productdummy() { Barcode = "1119", ProductName = "Nescafe Sachet 2 gm", Amount = 13.09m });
            filleditems.Add(new Productdummy() { Barcode = "1120", ProductName = "Jelly Belly", Amount = 2.00m });
            filleditems.Add(new Productdummy() { Barcode = "1121", ProductName = "Mother Dairy Ghee 1 kg", Amount = 39.00m });
            filleditems.Add(new Productdummy() { Barcode = "1122", ProductName = "Britannia Bourbon Cream 25 gm", Amount = 13.76m });
            return filleditems;
        }
    }
}
