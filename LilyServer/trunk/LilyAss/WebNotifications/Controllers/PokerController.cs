using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.IO;
using System.Text;
using WebNotifications.Permission;

namespace WebNotifications.Controllers
{
    [Description("上传Android (底下权限可以继承当前权限)")]
    public class PokerController : BaseController
    {
        //
        // GET: /Poker/
        [Description("上传Android apk")]
        public ActionResult UploadAndroidApk()
        {
            return View();
        }

        [Description("上传Android apk")]
        [HttpPost]
        public ActionResult UploadAndroidApk(HttpPostedFileBase uploadFile)
        {
            if (uploadFile != null)
            {
                StreamReader sr = new StreamReader(uploadFile.InputStream, Encoding.ASCII);
                StreamWriter sw = new StreamWriter(Server.MapPath("..//SoftwareDisk//LilyGame.apk"), false);                
                sw.Write(sr.ReadToEnd());
                sw.Close();
                sr.Close();

                //uploadFile.SaveAs(filename);
                ViewData["Message"] = "上传成功";
            }
            else
            {
                ViewData["Message"] = "文件不能为空";
            }

            return View();
        }
    }
}
