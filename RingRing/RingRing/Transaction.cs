using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RingRing
{
    class Transaction
    {
        //internal bool TransactionforLogin()
        //{
        //    try
        //    {
        //        string postData = JsonConvert.SerializeObject(totp);
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Constants.url_for_otp);
        //        webRequest.Method = "POST";
        //        webRequest.Accept = "application/json";
        //        webRequest.Headers.Add("mid", totp.request.merchantGuid);
        //        webRequest.Headers.Add("phone", totp.MobileNo);
        //        webRequest.Headers.Add("otp", totp.otp);
        //        webRequest.Headers.Add("checksumhash", paytm.CheckSum.generateCheckSumByJson(Constants.key, JsonConvert.SerializeObject(totp)));

        //        webRequest.ContentLength = postData.Length;

        //        using (StreamWriter requestWriter2 = new StreamWriter(webRequest.GetRequestStream()))
        //        {
        //            requestWriter2.Write(postData);
        //        }

        //        string responseData = string.Empty;

        //        using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
        //        {
        //            responseData = responseReader.ReadToEnd();
        //            otpresponse = JsonConvert.DeserializeObject<TotpResponse>(responseData);
        //            if (otpresponse != null)
        //            {
        //                MainForm.PaytmTxnStatus = otpresponse.status;
        //                MainForm.ErroMessage = otpresponse.statusMessage;
        //                if (otpresponse.response != null)
        //                {
        //                    MainForm.PaytmTxnId = otpresponse.response.walletSystemTxnId;
        //                    MainForm.PosOrderId = MainForm.PaytmOrderId = otpresponse.response.merchantOrderId;
        //                    //MainForm.PosOrderId = "1234567890123456789012345678901234567890123456789012345678901234";
        //                    //MainForm.PaytmOrderId = "1234567890123456789012345678901234567890123456789012345678901234";
        //                    //MainForm.PaytmTxnId = "1234567890123456789012345678901234567890123456789012345678901234";
        //                }
        //                return true;
        //            }
        //            else
        //            {
        //                MainForm.PaytmTxnStatus = "FAILURE";
        //                MainForm.ErroMessage = "Blank Response from Paytm";
        //                // MessageErrorBox.Show(Program.mainform, "Otp Response", MainForm.ErroMessage);
        //                return false;
        //            }
        //            //else if(otpresponse.status.Trim().ToUpper() == "SUCCESS")
        //            //{
        //            //    MainForm.PaytmTxnId = otpresponse.response.walletSystemTxnId;
        //            //    MainForm.PosOrderId = MainForm.PaytmOrderId = otpresponse.response.merchantOrderId;
        //            //    return true;
        //            //}
        //            //else
        //            //{
        //            //    MainForm.ErroMessage = otpresponse.statusMessage;
        //            //    MessageErrorBox.Show(Program.mainform, "Otp Response", MainForm.ErroMessage);
        //            //    return false;
        //            //}
        //        }
        //    }
        //    catch (WebException S)
        //    {
        //        try
        //        {
        //            MainForm.PaytmTxnStatus = "FAILURE";
        //            HttpWebResponse W = S.Response as HttpWebResponse;
        //            if (W == null)
        //            {
        //                MainForm.PaytmTxnStatus = S.Status.ToString();
        //                MainForm.ErroMessage = "Message : " + S.Message;
        //                return false;
        //            }
        //            using (Stream data = S.Response.GetResponseStream())
        //            using (var reader = new StreamReader(data))
        //            {
        //                String resp = reader.ReadToEnd();
        //                otpresponse = JsonConvert.DeserializeObject<TotpResponse>(resp);
        //                if (otpresponse != null)
        //                    MainForm.ErroMessage = otpresponse.statusMessage;
        //                else
        //                    MainForm.ErroMessage = S.Message;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MainForm.IsExceptionFired = true;
        //            MainForm.PaytmTxnStatus = ex.Message;
        //            MainForm.ErroMessage = ex.StackTrace;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MainForm.IsExceptionFired = true;
        //        MainForm.PaytmTxnStatus = ex.Message;
        //        MainForm.ErroMessage = ex.StackTrace;
        //    }
        //    return false;
        //}

        public static IntrospectResponse introspectresponse = null;
        public static RetrieveStoreResponse retrievestoreresponse = null;
        public static SetStoreResponse setstoreresponse = null;
        public static CreateTxnResponse createtxnresponse = null;
        public static RedeemTxnResponse redeemtxnresponse = null;
        public static List<TransactionResponse> transactionresponselist = null;
        internal static bool RetrieveStore()
        {
            try
            {
                List<RetrieveStoreResponse> list = JsonConvert.DeserializeObject<List<RetrieveStoreResponse>>
                    (Get(Constants.UrlforRetriveStore, "?subject=" + Transaction.introspectresponse.sub.ToString(), "application /json",
Constants.GetAuthorization));

                if (list.Count < 1)
                {
                    System.Windows.Forms.MessageBox.Show("RetrieveStore is null");
                    return false;
                }
                retrievestoreresponse = list[0];
                return true;
            }
            catch (WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("RetrieveStore WebException : " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("RetrieveStore Exception : " + ex.Message);
            }
            return false;
        }
        internal static bool Introspect()
        {
            try
            {
                introspectresponse = JsonConvert.DeserializeObject<IntrospectResponse>
                    (Post(Constants.UrlforIntrospect, "token=" + HttpUtility.UrlEncode(Constants.token), "application/x-www-form-urlencoded", null));
                return true;
            }
            catch (WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect WebException : " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect Exception : " + ex.Message);
            }
            return false;
        }
        internal static bool SetStore(string StoreId = null)
        {
            try
            {
                if (StoreId != null)
                {
                    List<SetStoreResponse> list = JsonConvert.DeserializeObject<List<SetStoreResponse>>
                   (Get(Constants.UrlforSetStore, "/" + StoreId, "application/json",
                       Constants.GetAuthorization));

                    if (list.Count < 1)
                    {
                        System.Windows.Forms.MessageBox.Show("Store Configuration is null");
                        return false;
                    }
                    setstoreresponse = list[0];
                }
                else
                {
                    string PostData = "{\"storeUserOid\":" + Transaction.retrievestoreresponse.oid + ", \"name\": \"Demo TVS device\", \"type\": \"tvs\", \"info\":{\"macAddress\": \"52:54:00:a2:1e:31\", \"posApplicationName\": \"No Pos\",\"posApplicationVersion\":\"0.0.0\"}}";
                    setstoreresponse = JsonConvert.DeserializeObject<SetStoreResponse>
                        (Post(Constants.UrlforSetStore, PostData, "application/json",
                        new Dictionary<string, string>() { { "Authorization", "Bearer " + Transaction.introspectresponse.sub.ToString() } }));

                }
                return true;
            }
            catch (WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect WebException : " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect Exception : " + ex.Message);
            }
            return false;
        }
        internal static bool CreateTxn(string Pin)
        {
            try
            {
                string PostData = "{\"txdeviceOid\":" + setstoreresponse.oid + ", \"storeUserOid\":" + setstoreresponse.storeUserOid + ", \"ownerUserOid\":" + setstoreresponse.storeUserOid + ",\"consumerPin\":\"" + Pin + "\"}";
                createtxnresponse = JsonConvert.DeserializeObject<CreateTxnResponse>
                    (Post(Constants.UrlforCreateTxn, PostData, "application/json", Constants.GetAuthorization));
                return true;
            }
            catch (WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect WebException : " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect Exception : " + ex.Message);
            }
            return false;
        }
        internal static bool RedeemTxn()
        {
            try
            {
                string PostData = "{\"status\":\"committed\"}";
                redeemtxnresponse = JsonConvert.DeserializeObject<RedeemTxnResponse>
                    (Post(Constants.UrlforRedeemTxn + "/" + createtxnresponse.oid, PostData, "application/json",
                       Constants.GetAuthorization, "PUT"));
                return true;
            }
            catch (WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect WebException : " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect Exception : " + ex.Message);
            }
            return false;
        }
        internal static bool GetRedeemTxn()
        {
            try
            {
                transactionresponselist = JsonConvert.DeserializeObject<List<TransactionResponse>>
                   (Get(Constants.UrlforGetRedeemTxn, "?txOid=" + createtxnresponse.oid + "&status=redeemed", "application/json",
                       Constants.GetAuthorization));

                //string data = "[{\"id\": \"00000000-0000-4000-8000-00000013c62d\",\"oid\": 1295917,\"txOid\": 1295915,\"txItemOid\": 1295916,\"storeUserOid\": 1281887,\"ownerUserOid\": 1281887,\"clipOid\": 1295908,\"couponOid\": 1248910,\"status\": \"redeemed\",\"activeIndex\": null,\"name\": \"Fritz-Kola\",\"description\": \"Sugar free caffeine drink\",\"savingsValue\": 10,\"savingsCurrency\": \"INR\",\"reimbursementValue\": \"10\",\"expiresOn\": \"2018-08-31T21:59:00Z\",\"rejectReason\":null},{\"id\":\"00000000-0000-4000-8000-00000013c62f\",    \"oid\":1295919,\"txOid\":1295915,\"txItemOid\":1295918,\"storeUserOid\":1281887,\"ownerUserOid\":1281887,\"clipOid\":1295858,\"couponOid\":1283751,\"status\":\"redeemed\",\"activeIndex\":null,\"name\":\"Whisper ultra clean large 30 pads\",\"description\":\"Whisper ultra clean large 30 pads\",\"savingsValue\":10,\"savingsCurrency\":\"INR\",\"reimbursementValue\":\"10\",\"expiresOn\":\"2018-08-31T21:59:00Z\",\"rejectReason\":null},{\"id\":\"00000000-0000-4000-8000-00000013c631\",\"oid\":1295921,\"txOid\":1295915,\"txItemOid\":1295920,\"storeUserOid\":1281887,\"ownerUserOid\":1281887,\"clipOid\":1295882,\"couponOid\":1283742,\"status\":\"redeemed\",\"activeIndex\":null,\"name\":\"Vim cocentrated gel 500 ml  \",\"description\":\"Vim cocentrated gel 500 ml  \",\"savingsValue\":10,\"savingsCurrency\":\"INR\",\"reimbursementValue\":\"10\",\"expiresOn\":\"2018-08-31T21:59:00Z\",\"rejectReason\":null}]";
                //transactionresponselist = JsonConvert.DeserializeObject<List<TransactionResponse>>(data);

                if (transactionresponselist.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect WebException : " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Introspect Exception : " + ex.Message);
            }
            return false;
        }
        public static string Get(string uri, string querystring, string ContentType, Dictionary<String, String> Header = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri + querystring);
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
                string value = reader.ReadToEnd();
                //Console.WriteLine("------------------------------");
                //Console.WriteLine("Server Response : " + value);
                //Console.WriteLine("------------------------------");
                return value;
            }
        }
        public static string Post(string uri, string data, string contentType, Dictionary<String, String> Header = null, string method = "POST")
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
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
                string value = reader.ReadToEnd();
                //Console.WriteLine("--------------");
                //Console.WriteLine("Server Response : " + value);
                //Console.WriteLine("--------------");
                return value;
            }
        }

        /*
         public async Task<string> PostAsync(string uri, string data, string contentType, string method = "POST")
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = dataBytes.Length;
            request.ContentType = contentType;
            request.Method = method;

            using (Stream requestBody = request.GetRequestStream())
            {
                await requestBody.WriteAsync(dataBytes, 0, dataBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
        public void sc(string data)
        {
            string url = "http://myserver.com/?page=hello&param2=val2";
            // HTTP web request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(data);
            IAsyncResult result = null;
            result =
                httpWebRequest.BeginGetRequestStream(ar =>
                {
                    Stream postStream = httpWebRequest.EndGetRequestStream(result);
                    postStream.Write(requestBytes, 0, data.Length);
                    postStream.Close();
                }
                , httpWebRequest);

            httpWebRequest.BeginGetResponse(new AsyncCallback(GetResponseCallback), httpWebRequest);

            //httpWebRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), httpWebRequest);
        }

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;

            using (var postStream = webRequest.EndGetRequestStream(asynchronousResult))
            {
                //send yoour data here
            }
            webRequest.BeginGetResponse(new AsyncCallback(GetResponseCallback), webRequest);
        }

        void GetResponseCallback(IAsyncResult asynchronousResult)
        {

            try
            {
                string data;
                HttpWebRequest myrequest = (HttpWebRequest)asynchronousResult.AsyncState;
                using (HttpWebResponse response = (HttpWebResponse)myrequest.EndGetResponse(asynchronousResult))
                {
                    System.IO.Stream responseStream = response.GetResponseStream();
                    using (var reader = new System.IO.StreamReader(responseStream))
                    {
                        data = reader.ReadToEnd();
                    }
                    responseStream.Close();
                }
            }
            catch (Exception e)
            {
            }
        }*/
    }
    class RedeemTxnResponse
    {
        public string id { get; set; }
        public int oid { get; set; }
        public int deviceOid { get; set; }
        public int storeUserOid { get; set; }
        public int ownerUserOid { get; set; }
        public int consumerUserOid { get; set; }
        public string consumerName { get; set; }
        public string consumerPin { get; set; }
        public string status { get; set; }
        public string endType { get; set; }
        public string finished { get; set; }
    }

    class TransactionResponse
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

    class CreateTxnResponse
    {
        public string id { get; set; }
        public int oid { get; set; }
        public int deviceOid { get; set; }
        public int storeUserOid { get; set; }
        public int ownerUserOid { get; set; }
        public int consumerUserOid { get; set; }
        public string consumerName { get; set; }
        public string consumerPin { get; set; }
        public string status { get; set; }
    }

    class SetStoreResponse
    {
        public string id { get; set; }
        public int oid { get; set; }
        public int storeUserOid { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public internalSetStoreResponse info { get; set; }
    }

    internal class internalSetStoreResponse
    {
        public string macAddress { get; set; }
        public string posApplicationName { get; set; }
        public string posApplicationVersion { get; set; }
    }

    class RetrieveStoreResponse
    {
        public string id { get; set; }
        public int oid { get; set; }
        public int shortId { get; set; }
        public string type { get; set; }
        public string login { get; set; }
        public string subject { get; set; }
        public string displayName { get; set; }
        public string registeredOn { get; set; }
        public string redemptionPinInvariant { get; set; }
        public string redemptionPinSecretKey { get; set; }
    }

    class IntrospectResponse
    {
        public bool active { get; set; }
        public bool banned { get; set; }
        public bool verified { get; set; }
        public bool email_verified { get; set; }
        public string phone_number { get; set; }
        public bool phone_number_verified { get; set; }
        public string scope { get; set; }
        public int exp { get; set; }
        public string sub { get; set; }
        public string sms_provider { get; set; }
    }
}
