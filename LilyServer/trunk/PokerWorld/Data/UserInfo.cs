using System;
using System.Collections.Generic;
using System.Text;

namespace PokerWorld.Data
{
    public class UserInfo
    {
        private string m_Username;
        private string m_Password;
        private string m_Email;
        private string m_DisplayName;
        private double m_TotalMoney;

        public double TotalMoney
        {
            get { return m_TotalMoney; }
            set { m_TotalMoney = value; }
        }

        public string Username
        {
            get { return m_Username; }
            set { m_Username = value; }
        }

        public string Password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }

        public string Email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }

        public string DisplayName
        {
            get { return m_DisplayName; }
            set { m_DisplayName = value; }
        }

        public UserInfo()
        {
        }

        public UserInfo(string username, string password, string email, string displayname, double totalmoney)
        {
            m_DisplayName = displayname;
            m_Email = email;
            m_Password = password;
            m_TotalMoney = totalmoney;
            m_Username = username;
        }
    }
}
