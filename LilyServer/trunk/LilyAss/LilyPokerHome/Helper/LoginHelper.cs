using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;

namespace LilyPokerHome.Helper
{
    public class LoginHelper
    {
        public static string SESSION_USERNAME = "username";
        public static string SESSION_PWD = "password";

        public static void LoginInSession(string username, string pwd)
        {
            HttpContext.Current.Session[SESSION_USERNAME] = username;
            HttpContext.Current.Session[SESSION_PWD] = pwd;           
        }

        public static void LogoutInSession()
        {
            HttpContext.Current.Session[SESSION_USERNAME] = string.Empty;
            HttpContext.Current.Session[SESSION_PWD] = string.Empty;           
        }

        public static bool ValidateUser(string username, string pwd)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pwd))
                return false;

            if (username.Equals(ConfigurationManager.AppSettings["username"])
                && pwd.Equals(ConfigurationManager.AppSettings["pwd"]))
            {
                return true;
            }

            return false;
        }

        public static bool ValidateUserInSession() {
            string username = HttpContext.Current.Session[SESSION_USERNAME] as string;
            string pwd = HttpContext.Current.Session[SESSION_PWD] as string;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pwd))
                return false;

            if(username.Equals(ConfigurationManager.AppSettings["username"])
                && pwd.Equals(ConfigurationManager.AppSettings["pwd"])){
                return true;
            }

            return false; // should not come here
        }
    }
}