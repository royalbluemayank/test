using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TVSService
{
    class Constants
    {

        #region Constructor
        public Constants()
        {

        }
        #endregion

        #region Constant Properties 

        private static string FolderpathValue = string.Empty;

        private static int ItemsCount = 0;
        private static string Folderpath
        {
            get
            {
                if (FolderpathValue == string.Empty)
                {
                    object value = ConfigurationManager.AppSettings["Path"];
                    if (value != null && value.ToString() != "")
                    {
                        FolderpathValue = value.ToString();
                    }
                    else
                    {
                        FolderpathValue = AppDomain.CurrentDomain.BaseDirectory;
                    }
                }
                return FolderpathValue;
            }
        }
        private static string PathforSaveItem { get { return Folderpath + "ItemHistory.csv"; } }
        private static string PathforSendItem { get { return Folderpath + "SendItemHistory.csv"; } }
        public static string Pathforlogger { get { return Folderpath + "logs.txt"; } }
        private static string FileStartText { get { return "{\"Items\":["; } }
        private static string FileEndText { get { return "]}"; } }
        public static string UrlforMakeTransaction { get { return "https://api.dev.xaos.aintu.io/rest/txItem"; } }
        public static string UrlforGetTransaction { get { return "https://api.dev.xaos.aintu.io/rest/txClip"; } }
        public static string UrlforDeleteTransaction { get { return "https://api.dev.xaos.aintu.io/rest/txClip"; } }

        #endregion

        #region Methods
        public static bool SaveAnonymousItem(String BarCode)
        {
            try
            {
                File.AppendAllText(PathforSaveItem, String.Format("{0},", ProductToJson(BarCode)));
                SendAnonymousList();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("message : " + ex.Message);
            }
            return false;
        }
        private static bool SendAnonymousList()
        {
            try
            {
                if (ItemsCount == 0)
                {
                    ItemsCount = getItemCount();
                }
                if (ItemsCount < 100)
                    return true;
                //ItemsCount = 0;
                File.Move(PathforSaveItem, PathforSendItem);
                //File.Create(PathforSaveItem);
                String Data = File.ReadAllText(PathforSendItem);
                Data = Data.Substring(0, Data.Length - 1);
                Data = FileStartText + Data + FileEndText;
                if (SendListToServer(Data))
                {
                    //File.Delete(PathforSendItem);
                }
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        private static int getItemCount()
        {
            String value = "";
            using (StreamReader sr = new StreamReader(PathforSaveItem))
            {
                value = sr.ReadToEnd();
            }
            return value.Count(character => character == '}');
        }
        public static bool SendListToServer(String Data)
        {
            return true;
        }
        public static void Log(String Caption , String logs)
        {
            try
            {
                FileInfo f = new FileInfo(Pathforlogger);
                using (StreamWriter sw = new StreamWriter(File.Open(Pathforlogger, FileMode.Append)))
                {
                    sw.WriteLine(String.Format("[ {0} ] - {1} : {2}", System.DateTime.Now.ToString("yyyy MMMM dd hh:mm:ss tt"), Caption, logs));
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        private static string ProductToJson(String Barcode)
        {
            return "{\"ProductBarcode\":\"" + Barcode + "\",\"TimeStamp\":\"" + DateTime.Now.ToString("MM/dd/yyyy H:mm:ss:ffff zzz") + "\"}"; //yyyyMMddHHmmssffff
        }
        #endregion
    }
}
