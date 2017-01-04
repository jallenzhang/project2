using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace WebNotifications.Helper
{
    public class ReceiptHelper
    {
        /// <summary>
        /// 0 means valid, other means invalid.
        /// </summary>
        /// <param name="receipt"></param>
        /// <returns></returns>
        public static string IsValid(string receipt)
        {
            string json = GenerateReceiptData(receipt);
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            HttpWebRequest myreq = PostJsonToAppStore(json, encode);
            string result = GetResponseResult(myreq, encode);
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            var receObj = (IDictionary<string, object>)json_serializer.DeserializeObject(result);

            return receObj["status"].ToString();
        }

        public static string GetJsonString(string receipt) {
            string json = GenerateReceiptData(receipt);
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            HttpWebRequest myreq = PostJsonToAppStore(json, encode);
            string result = GetResponseResult(myreq, encode);

            return result;        
        }

        public static string DecodeReceipt(string receipt) { 
            try
            {                
                string trasResult = DecodeBase64String(receipt);
                var receObj = GetKeyValue(trasResult);
                //string signatureValue = DecodeBase64String(receObj["signature"].ToString());
                string purchase_info = DecodeBase64String(receObj["purchase-info"].ToString());
                //receObj["signature"] = signatureValue;
                receObj["purchase-info"] = purchase_info;
                var json_serializer = new JavaScriptSerializer();               
                //string result = json_serializer.Serialize(receObj);
                //string result = json_serializer.Serialize(purchase_info);
                string result = purchase_info; 
                result = result.Replace(@"\n\t","");
                result = result.Replace(@"\", "");

                return result;
            }
            catch {
                return receipt;
            }        
        }

        private static string DecodeBase64String(string s) {
            byte[] content = Convert.FromBase64String(s);
            string trasResult = Encoding.UTF8.GetString(content);
            return trasResult;
        }

        private static Dictionary<string, string> GetKeyValue(string str) 
        {
            Dictionary<string, string> obj = new Dictionary<string, string>();
            List<int> pos = new List<int>();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '"') {
                    pos.Add(i);
                }
            }
            for (int i = 0; i < pos.Count; ) {
                string key = str.Substring(pos[i]+1, pos[i + 1]-pos[i]-1);
                string value = str.Substring(pos[i+2] + 1, pos[i + 3] - pos[i+2] - 1);
                obj.Add(key, value);
                i += 4;
            }
            return obj;
        }

        private static string GetResponseResult(HttpWebRequest myReq, Encoding encode)
        {
            WebResponse myResp = myReq.GetResponse();
            Stream ReceiveStream = myResp.GetResponseStream();
            StreamReader readStream = new StreamReader(ReceiveStream, encode);
            Char[] read = new Char[256];
            int count = readStream.Read(read, 0, 256);
            string str = string.Empty;
            while (count > 0)
            {
                str += new String(read, 0, count);
                count = readStream.Read(read, 0, 256);
            }
            readStream.Close();
            myResp.Close();

            return str;
        }

        private static HttpWebRequest PostJsonToAppStore(string json, Encoding encode)
        {
            string result = string.Empty;
            //string url = "https://buy.itunes.apple.com/verifyReceipt";
            string url = "https://sandbox.itunes.apple.com/verifyReceipt";
            byte[] arrB = encode.GetBytes(json);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            myReq.Method = "POST";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.ContentLength = arrB.Length;
            Stream outStream = myReq.GetRequestStream();
            outStream.Write(arrB, 0, arrB.Length);
            outStream.Close();

            return myReq;
        }

        private static string GenerateReceiptData(string receipt)
        {
            string json = "{ \"receipt-data\":\"" + receipt + "\" }";
            return json;
        }
    }
}