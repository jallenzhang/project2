using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;

namespace EricUtility.Data
{
    public class AccessConnector : AbstractConnector
    {
        public AccessConnector(string path)
            : base(path)
        {
        }
        protected override void Connect()
        {
            string Provider = "Microsoft.ACE.OLEDB.12.0";
            string connectionString = String.Format("Provider={0};Data Source={1}", Provider, Path);
            Connexion = new OleDbConnection(connectionString);
            Connexion.Open();
        }
    }
}
