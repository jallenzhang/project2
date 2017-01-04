using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace EricUtility.Data
{
    public abstract class AbstractConnector
    {
        private string m_Path;
		private OleDbConnection m_Connexion;
		public string Path
		{
			get
			{
				return m_Path;
			}
		}
		protected OleDbConnection Connexion
		{
			get
			{
				return m_Connexion;
			}
			set
			{
				m_Connexion = value;
			}
		}
        public AbstractConnector(string path)
		{
			m_Path = path;
		}
		protected abstract void Connect();
		protected virtual void Disconnect()
		{
			Connexion.Close();
		}
		public virtual DataSet ExecuteQuery( string query )
		{
			Connect();
			OleDbCommand commande = new OleDbCommand( query, Connexion );
			OleDbDataAdapter adapter = new OleDbDataAdapter();
			DataSet datas = new DataSet();
			adapter.SelectCommand = commande;
			adapter.Fill( datas );
			Disconnect();
			return datas;
		}
		public virtual int ExecuteNonQuery( string query )
		{
			Connect();
			OleDbCommand commande = new OleDbCommand( query, Connexion );
			int returnValue = commande.ExecuteNonQuery();
			Disconnect();
			return returnValue;
		}
    }
}
