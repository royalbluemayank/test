using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RingRing
{
    class TVSService
    {
        public TVSService(ProductType productType, OrderHistory.Product product)
        {
            this.productType = productType;
            this.Product = product;
        }
        public enum ProductType
        {
            AnonymousProduct = 0, UserProduct = 1, ValidUserProduct = 2, NotValidUserProduct = 4
        }
        public OrderHistory.Product Product { get; private set; }
        public ProductType productType { get; private set; }
        //public bool IsClosed { get; private set; }
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
