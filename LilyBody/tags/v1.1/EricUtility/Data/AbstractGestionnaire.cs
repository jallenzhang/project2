using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace EricUtility.Data
{
    public abstract class AbstractGestionnaire
    {
		public abstract string TableName
		{
			get;
		}
        protected AbstractGestionnaire()
		{
		}
        protected virtual int Insert(AbstractConnector connector, Dictionary<string, string> fieldsAndValues)
		{
			string fields = "";
			string values = "";
			foreach ( KeyValuePair<string, string> kvp in fieldsAndValues )
			{
				fields = string.Format( "{0}{1},", fields, kvp.Key );
				values = string.Format( "{0}{1},", values, kvp.Value );
			}
			string query = string.Format( "INSERT INTO {0}({1}) VALUES({2})", TableName, fields.Remove( fields.Length - 1 ), values.Remove( values.Length - 1 ) );
			return connector.ExecuteNonQuery( query );
		}
        protected virtual int Update(AbstractConnector connector, Dictionary<string, string> fieldsAndValues, string whereCond)
		{
			string setparms = "";
			foreach ( KeyValuePair<string, string> kvp in fieldsAndValues )
				setparms = string.Format( "{0}{1}={2},", setparms, kvp.Key, kvp.Value );
			string query = string.Format( "UPDATE {0} SET {1}{2}", TableName, setparms.Remove( setparms.Length - 1 ), whereCond == null ? "" : String.Format( " WHERE {0}", whereCond ) );
			return connector.ExecuteNonQuery( query );
		}
        protected virtual int Delete(AbstractConnector connector, string whereCond)
		{
			string query = string.Format( "DELETE FROM {0}{1}", TableName, whereCond == null ? "" : String.Format( " WHERE {0}", whereCond ) );
			return connector.ExecuteNonQuery( query );
		}
        protected virtual DataTable Select(AbstractConnector connector, List<string> fieldsList, string whereCond)
		{
			string fields = "";
			foreach ( string field in fieldsList )
				fields = string.Format( "{0}{1},", fields, field );
			string query = string.Format( "SELECT {1} FROM {0}{2}", TableName, fields.Remove( fields.Length - 1 ), whereCond == null ? "" : String.Format( " WHERE {0}", whereCond ) );
			DataSet results = connector.ExecuteQuery( query );
			return results.Tables[0];
		}
	}
}
