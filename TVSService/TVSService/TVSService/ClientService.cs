using Newtonsoft.Json;

namespace TVSService
{
    class ClientService
    {
        public Product Product { get; set; }
        public ProductType productType { get; set; }
        public int txOid { get;  set; }
        public int ownerUserOid { get;  set; }
        public int storeUserOid { get;  set; }
        public string token { get;  set; }
        public string status { get; set; }
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public enum ProductType
    {
        AnonymousProduct = 0,
        UserProduct = 1,
        ValidUserProduct = 2,
        NotValidUserProduct = 4,
        DeleteValidUserProduct = 8
    }
    public class Product
    {
        public string ProductID { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
