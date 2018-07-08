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
        public static string HeaderProductdescription = "Product description                                                Discount            Select";
        public static string HeaderTxndescription = "Product description                                                                      Discount";
        public static string PathforSaveItem { get { return @"C:\ItemHistory.csv"; } }
        public static string Pathforlogger { get { return @"C:\logs.txt"; } }
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
