using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace RingRing
{
    class Constants
    {

        public static string token = "d9WqHCfhLxm3JJxr0nx9wwK5xryts48EDHkAn_A=";

        public static string HeaderProductdescription = "Product description                                                Discount            Select";
        public static string HeaderTxndescription = "Product description                                                                      Discount";
        public static string PathforSaveItem { get { return @"C:\ItemHistory.csv"; } }
        public static string Pathforlogger { get { return @"C:\logs.txt"; } }

        public static string UrlforRetriveStore { get { return "https://api.dev.xaos.aintu.io/rest/user"; } }
        public static string UrlforIntrospect { get { return "https://api.dev.xaos.aintu.io/auth/v1/introspect"; } }
        public static string UrlforSetStore { get { return "https://api.dev.xaos.aintu.io/rest/txDevice"; } }
        public static string UrlforCreateTxn { get { return "https://api.dev.xaos.aintu.io/rest/tx"; } }
        public static string UrlforRedeemTxn { get { return "https://api.dev.xaos.aintu.io/rest/tx"; } }
        public static string UrlforGetRedeemTxn { get { return "https://api.dev.xaos.aintu.io/rest/txClip"; } }
        public static Dictionary<string, string> GetAuthorization { get { return new Dictionary<string, string>() { { "Authorization", "Bearer " + token } }; } }

        public static string ConvertToDateTimefromISO8601(string ISOdateTime)
        {
            DateTime dt = DateTime.Parse(ISOdateTime);
            return string.Format("{0:hh:mm:ss tt MMMM dd}{1} {0:yyyy}", dt, ((dt.Day % 10 == 1 && dt.Day != 11) ? "st" : (dt.Day % 10 == 2 && dt.Day != 12) ? "nd"
                                                                                                                    : (dt.Day % 10 == 3 && dt.Day != 13) ? "rd" : "th"));
        }
        public static void Logger(String logs)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(Constants.Pathforlogger, FileMode.Append)))
                {
                    sw.WriteLine(String.Format("[ {0} ] ------ {1}", System.DateTime.Now.ToString("yyyy MMMM dd hh:mm:ss tt"), logs));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }
    }
}
