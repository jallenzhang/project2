using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Net;
using System.IO;
using System.Xml;
using DataPersist;
using ExitGames.Logging;

namespace LilyServer
{
    internal class JavApiService
    {
        private static string JavApi = ConfigurationManager.AppSettings["JavApi"];
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private const string tagID = "杭州感遇";
        private const string sortid = "423";
        private const string key = "123";
        private const string defaultpassword = "poker123";

        public static bool checkNickName(string nickName) {
            Log.DebugFormat("######################checkNickName:{0}", nickName);
            string method = "CheckNickName";
            Hashtable pars = new Hashtable();
            pars.Add("nickName", nickName);
            pars.Add("key", key);
            XmlDocument doc = QueryPostWebService(method, pars);

            Log.DebugFormat("######################QueryPostWebService:{0}", nickName);

            int resultCode = int.Parse((doc).DocumentElement.ChildNodes[0].InnerText);
            //string resultString = (doc).DocumentElement.ChildNodes[1].InnerText;
            bool ret = false;
            Log.DebugFormat("######################QueryPostWebServiceResultCode:{0}", resultCode);
            if (resultCode > -1)
            {
                ret = true;
            }

            return ret;
        }

        public static bool ChangePassword(string userName,string oldpwd,string newpwd) {
            Log.DebugFormat("######################ChangePassword:{0},{1},{2}", userName,oldpwd,newpwd);
            string method = "ResetPassWordByLoginName";
            Hashtable pars = new Hashtable();
            pars.Add("loginName", userName);
            pars.Add("oldpwd", oldpwd);
            pars.Add("newpwd", newpwd);
            pars.Add("key", key);
            pars.Add("IP", "");
            XmlDocument doc = QueryPostWebService(method, pars);

            Log.DebugFormat("######################QueryPostWebService:{0}", userName);

            int resultCode = int.Parse((doc).DocumentElement.ChildNodes[0].InnerText);
            //string resultString = (doc).DocumentElement.ChildNodes[1].InnerText;
            bool ret = false;
            Log.DebugFormat("######################QueryPostWebServiceResultCode:{0}", resultCode);
            if (resultCode > -1)
            {
                ret = true;
            }

            return ret;
        }


        public static bool ResetUserName(string userId,string username,string nickname) {
            Log.DebugFormat("######################ResetUserName:{0},username,{1},nickname,{2}", userId,username,nickname);

            string method = "ResetUserName";
            Hashtable pars = new Hashtable();
            pars.Add("uid", userId);
            pars.Add("username", username);
            pars.Add("nickname", nickname ?? "");
            pars.Add("key", key);
            XmlDocument doc = QueryPostWebService(method, pars);

            Log.DebugFormat("######################QueryPostWebService:{0}", userId);

            int resultCode = int.Parse((doc).DocumentElement.ChildNodes[0].InnerText);
            //string resultString = (doc).DocumentElement.ChildNodes[1].InnerText;
            bool ret = false;
            Log.DebugFormat("######################QueryPostWebServiceResultCode:{0}", resultCode);
            if (resultCode > -1)
            {
                ret = true;
            }
            return ret;
        }


        public static bool UpGrade(string userid,string userName,string nickName) {
            return ResetUserName(userid, userName, nickName);             
        }

        public static bool UpGradeChangePassword(string username,string password) {
            return ChangePassword(username, defaultpassword, password);
        }

        public static int QuickCreateAccount(UserData user) {
            Log.DebugFormat("######################CreateAccountByPhone:{0}", user.NickName);

            string method = "CreateAccountByPhone";
            Hashtable pars = new Hashtable();
            pars.Add("Password", defaultpassword);
            pars.Add("IP", "");
            pars.Add("sortid", sortid);
            pars.Add("tgID", tagID);
            pars.Add("key", key);
            XmlDocument doc = QueryPostWebService(method, pars);

            Log.DebugFormat("######################QueryPostWebService:{0},{1}", user.NickName, user.Mail);

            int resultCode = int.Parse((doc).DocumentElement.ChildNodes[0].InnerText);
            //string resultString = (doc).DocumentElement.ChildNodes[1].InnerText;
            //int ret = false;
            Log.DebugFormat("######################QueryPostWebServiceResultCode:{0}", resultCode);
            int id = 0;
            if (resultCode > -1)
            {
                id = int.Parse((doc).DocumentElement.ChildNodes[2].InnerText);
                string tempnickname = (doc).DocumentElement.ChildNodes[5].InnerText;
                user.UserName = tempnickname;
            }

            return id;
        }

        public static int CreateAccount(UserData user){

            Log.DebugFormat("######################CreateAccount:{0}",user.NickName);

            string method = "CreateAccount";
            Hashtable pars = new Hashtable();
            pars.Add("loginName", user.Mail);
            pars.Add("nickName", user.NickName);
            pars.Add("Password", user.Password);
            pars.Add("sex", "true");
            pars.Add("IP", "");
            pars.Add("sortid", sortid);
            pars.Add("tgID", tagID);
            pars.Add("key", key);
            XmlDocument doc = QueryPostWebService(method, pars);

            Log.DebugFormat("######################QueryPostWebService:{0},{1}", user.NickName, user.Mail);
            
            int resultCode = int.Parse((doc).DocumentElement.ChildNodes[0].InnerText);
            //string resultString = (doc).DocumentElement.ChildNodes[1].InnerText;
            //bool ret = false;
            Log.DebugFormat("######################QueryPostWebServiceResultCode:{0}", resultCode);
            //if (resultCode > -1)
            //{
            //    ret = true;
            //}

            return resultCode;
        }


        public static bool UserLoginGetInfo(UserData user) {
            string method = "UserLoginGetInfo";
            Log.DebugFormat("######################UserLoginGetInfo NickName:{0}", user.NickName);
            Log.DebugFormat("######################UserLoginGetInfo Password:{0}", user.Password);
            Hashtable pars = new Hashtable();
            pars.Add("loginName", user.Mail);
            pars.Add("Password", user.Password);
            pars.Add("IP", "");
            pars.Add("key", key);
            XmlDocument doc = QueryPostWebService(method, pars);
            int resultCode = int.Parse((doc).DocumentElement.ChildNodes[0].InnerText);
            //string resultString = (doc).DocumentElement.ChildNodes[1].InnerText;
            bool ret = false;

            if (resultCode > -1)
            {
                ret = true;
            }

            if (ret)
            {
                user.NickName = (doc).DocumentElement.ChildNodes[3].InnerText;
                user.UserId = (doc).DocumentElement.ChildNodes[1].InnerText;
            }

            return ret;
        }


        /// <summary>
        /// 需要WebService支持Post调用
        /// </summary>
        private static XmlDocument QueryPostWebService(String MethodName, Hashtable Pars)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(JavApi + "/" + MethodName);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            byte[] data = EncodePars(Pars);
            request.ContentLength = data.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();

            return ReadXmlResponse(request.GetResponse());
        }

        private static void SetWebRequest(HttpWebRequest request)
        {
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 10000;
        }
        private static byte[] EncodePars(Hashtable Pars)
        {
            return Encoding.UTF8.GetBytes(ParsToString(Pars));
        }
        private static String ParsToString(Hashtable Pars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string k in Pars.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.Append(k + "=" + Pars[k].ToString());
            }
            return sb.ToString();
        }
        private static XmlDocument ReadXmlResponse(WebResponse response)
        {
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String retXml = sr.ReadToEnd();
            sr.Close();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(retXml);


            //XmlNodeList nodes = doc.SelectNodes("/students/student");

            //int resultCode = int.Parse((doc).DocumentElement.ChildNodes[0].InnerText);
            //string resultString = (doc).DocumentElement.ChildNodes[1].InnerText;
            //bool ret = false;

            //if (resultCode > -1)
            //{
            //    ret = true;
            //}

            return doc;
        }
    }
}
