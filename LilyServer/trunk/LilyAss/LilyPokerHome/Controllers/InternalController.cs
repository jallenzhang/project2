using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LilyPokerHome.Models;
using System.IO;
using LilyPokerHome.Helper;

namespace LilyPokerHome.Controllers
{
    [AuthorizeFilterAttribute]
    public class InternalController : Controller
    {        
        private static string ANDROID_TEST_URL = string.Empty;
        private static string URLLOG_PATH = "urllog.txt";

        //
        // GET: /Internal/        
        public ActionResult ConfigAPk()
        {
            GetTestUrl();
            ViewData["tbUrl"] = ANDROID_TEST_URL;

            return View();
        }

        [HttpPost]
        public ActionResult ConfigAPk(string tbUrl)
        {
            ANDROID_TEST_URL = tbUrl;
            this.WriteToUrlLog(tbUrl);
            
            return View();
        }

        #region Private Method

        private void GetTestUrl()
        {
            if (ANDROID_TEST_URL == string.Empty) {
                ANDROID_TEST_URL = ReadUrlLog();
            }
        }

        private string ReadUrlLog()
        {
            string st = string.Empty;
            using (StreamReader objReader = new StreamReader(GetCurrentFileLocation()))
            {
                while (!objReader.EndOfStream)
                {
                    st = objReader.ReadLine();
                }
                objReader.Close();
            }

            return st;
        }

        private void WriteToUrlLog(string txt)
        {
            using (StreamWriter sw = System.IO.File.AppendText(GetCurrentFileLocation()))
            {
                sw.WriteLine(txt);
                sw.Flush();
                sw.Close();
            }
        }

        private string GetCurrentFileLocation()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, URLLOG_PATH);
            return path;
        }

        #endregion        
    }
}
