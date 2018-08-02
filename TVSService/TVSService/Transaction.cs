using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TVSService
{
    class Transaction
    {
        public MakeTransactionResponse maketransactionresponse = null;
        public GetTransactionResponse gettransactionresponse = null;
        //internal bool makeTransaction(string BarCode, string token, int txOid,int ownerUserOid, int storeUserOid)
        internal ClientService MakeTransaction(ClientService datavalue)
        {
            try
            {
                ClientService sc = datavalue;
                string data = "{\"txOid\":" + sc.txOid + ", \"storeUserOid\":" + sc.storeUserOid + ", \"ownerUserOid\":" + sc.ownerUserOid + ",\"barcode\":\"" + datavalue.Product.Barcode + "\"}";
                maketransactionresponse = JsonConvert.DeserializeObject<MakeTransactionResponse>
                    (Post(Constants.UrlforMakeTransaction, data, "application/json", new Dictionary<string, string>() { { "Authorization", "Bearer " + sc.token } }));
                if (maketransactionresponse != null)
                {
                    //gettransactionresponse = JsonConvert.DeserializeObject<List<GetTransactionResponse>>("[{\"id\":\"00000000-0000-4000-8000-000000139b0e\",\"oid\": 1284878,\"txOid\": 1284825,\"txItemOid\": 1284877,\"storeUserOid\": 1284787,\"ownerUserOid\": 1284787,\"clipOid\": 1284869,\"couponOid\": 1248910,\"status\": \"redeemed\",\"activeIndex\": null,\"name\": \"Fritz-Kola\",\"description\":\"Sugar free caffeine drink\",\"savingsValue\": 10,\"savingsCurrency\": \"INR\",\"reimbursementValue\": \"10\",\"expiresOn\": \"2018-08-31T21:59:00Z\",\"rejectReason\": null}]")[0];
                    List<GetTransactionResponse> list = JsonConvert.DeserializeObject<List<GetTransactionResponse>>
                        (Get(Constants.UrlforGetTransaction, maketransactionresponse.oid, "application/json", new Dictionary<string, string>()
                    { { "Authorization", "Bearer " + sc.token } }));

                    if (list.Count < 1)
                        return null;
                    gettransactionresponse = list[0];

                    if (gettransactionresponse != null)
                    {
                        sc.Product.ProductName = gettransactionresponse.name;
                        sc.Product.Amount = gettransactionresponse.savingsValue;
                        sc.productType = ProductType.ValidUserProduct;
                        return sc;
                    }
                }
            }
            catch (WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect WebException : " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect Exception : " + ex.Message);
            }
            return null;
        }

        public  string Post(string uri, string data, string contentType, Dictionary<String, String> Header = null, string method = "POST")
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentLength = dataBytes.Length;
            if (Header != null)
            {
                foreach (var item in Header)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }
            request.ContentType = contentType;
            request.Method = method;

            using (Stream requestBody = request.GetRequestStream())
            {
                requestBody.Write(dataBytes, 0, dataBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public  string Get(string uri, int txItemOid, string ContentType, Dictionary<String, String> Header = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri + "?txItemOid=" + txItemOid);
            request.ContentType = ContentType;
            if (Header != null)
            {
                foreach (var item in Header)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string PG_TxnStatus_Click(int i, string body1)
        {
            try
            {
                //Thread.Sleep(i * 1000);
                body1 = "JsonData=" + body1;
                string uri = "https://pguat.paytm.com/oltp/HANDLER_INTERNAL/getTxnStatus";
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                string data = "";
                using (StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    streamWriter.Write(body1);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (var reader = new System.IO.StreamReader(stream))
                {
                    return i + " : " + reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return ("Exception : " + ex.Message);
            }
        }
    }

    class MakeTransactionResponse
    {
        public string id { get; set; }
        public int oid { get; set; }
        public int txOid { get; set; }
        public int storeUserOid { get; set; }
        public int ownerUserOid { get; set; }
        public string barcode { get; set; }
    }

    class GetTransactionResponse
    {
        public string id { get; set; }
        public int oid { get; set; }
        public int txOid { get; set; }
        public int txItemOid { get; set; }
        public int storeUserOid { get; set; }
        public int ownerUserOid { get; set; }
        public int clipOid { get; set; }
        public int couponOid { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal savingsValue { get; set; }
        public string savingsCurrency { get; set; }
        public string reimbursementValue { get; set; }
        public string expiresOn { get; set; }
        public string rejectReason { get; set; }
    }
}
