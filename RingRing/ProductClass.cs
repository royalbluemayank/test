using System;

namespace RingRing
{
    public enum ProductStatus
    {
        Pending = 0, Updated = 1
    }
    public class Product: OrderHistory.Product
    {
        private string Datetime;
        //private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public Product(ProductStatus productstatus = ProductStatus.Pending, string BorderColor = "White", string BackgroundColor = "White", decimal Amount = 0, string Image = "ok_Green")
        {
            this.productstatus = productstatus;
            this.BorderColor = BorderColor;
            this.BackgroundColor = BackgroundColor;
            this.Amount = Amount;
            this.Image = "/Resources/" + Image + ".png";
            this.ProductID = System.DateTime.UtcNow.Ticks.ToString();
            this.DateTime = System.DateTime.Now.ToString();
            this.Applicable = true;
        }
        public ProductStatus productstatus { get; set; }
        public string Image { get; private set; }
        public string BorderColor { get; private set; }
        public bool Applicable { get; private set; }
        public string BackgroundColor { get; private set; }
        public bool Addedinremovedproduct{get; private set;}
        public string DateTime
        {
            get
            {
                DateTime dt = Convert.ToDateTime(this.Datetime);
                return string.Format("{0:hh:mm tt MMMM dd}{1} {0:yyyy}", dt, ((dt.Day % 10 == 1 && dt.Day != 11) ? "st" : (dt.Day % 10 == 2 && dt.Day != 12) ? "nd"
                                                                                                                        : (dt.Day % 10 == 3 && dt.Day != 13) ? "rd" : "th"));
                //return System.Convert.ToDateTime(this.Datetime).ToString("hh:mm tt MMMM dd++ yyyy").Replace("++", "th"); //12:55 PM February 26th 2018
            }
            set
            {
                this.Datetime = value;
            }
        }
        public void Added()
        {
            //Console.WriteLine("changeselection start: {0}", this.ToString());
            this.BackgroundColor = "White";
            this.BorderColor = "White";
            this.Image = "/Resources/ok_Green.png";
            this.Addedinremovedproduct = false;
            //Console.WriteLine("changeselection stop: {0}", this.ToString());
        }
        public void Removed()
        {
            //Console.WriteLine("changeselection start: {0}", this.ToString());
            this.BackgroundColor = "#F9F6F6";
            this.BorderColor = "Red";
            this.Image = "/Resources/cross_Red.png";
            this.Addedinremovedproduct = true;
            //Console.WriteLine("changeselection stop: {0}", this.ToString());
        }
        public string ToSave()
        {
            return this.Barcode + "," + this.ProductName + "," + this.Amount.ToString("0.00") + "," + this.DateTime;
        }
    }
   
    //public class MainUser
    //{
    //    public MainUser()
    //    {
    //        product = new ObservableCollection<User>();
    //    }
    //    public string Amount { get; set; }
    //    public ObservableCollection<User> product { get; set; }

    //}

    //public class MainUser
    //{
    //    private int v;

    //    public int index { get; set; }
    //    public product product { get; set; }
    //    public MainUser(int v, product user)
    //    {
    //        this.v = v;
    //        this.product = user;
    //    }
    //}

    //class TopMenu1
    //{
    //    public ObservableCollection<MainUser> mainproduct { get; set; }
    //    public TopMenu1()
    //    {
    //        mainproduct = new ObservableCollection<MainUser>();
    //    }
    //}

    //class Menu
    //{
    //    public ObservableCollection<TopMenu> TopMenuItems { get; set; }
    //    public Menu()
    //    {
    //        TopMenuItems = new ObservableCollection<TopMenu>();
    //    }
    //}

    //class TopMenu
    //{
    //    public string GroupName { get; set; }
    //    public ObservableCollection<SubMenu> SubMenuItems { get; set; }
    //    public TopMenu()
    //    {
    //        SubMenuItems = new ObservableCollection<SubMenu>();
    //    }
    //}

    //class SubMenu
    //{
    //    public string ItemName { get; set; }
    //}
}