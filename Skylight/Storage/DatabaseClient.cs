using MySql.Data.MySqlClient;
using System;
using System.Data;
using SkylightEmulator.Storage;
namespace SkylightEmulator.Storage
{
	public sealed class DatabaseClient : IDisposable
	{
		private DatabaseManager Manager;
		private MySqlConnection Connection;
		private MySqlCommand Command;
		public DatabaseClient(DatabaseManager _Manager)
		{
			Manager = _Manager;
			Connection = new MySqlConnection(_Manager.ConnectionString);
			Command = this.Connection.CreateCommand();
			Connection.Open();
		}
		public void Dispose()
		{
			Connection.Close();
			Command.Dispose();
			Connection.Dispose();
		}
		public void AddParamWithValue(string sParam, object val)
		{
			Command.Parameters.AddWithValue(sParam, val);
		}
		public void ExecuteQuery(string sQuery)
		{
			Command.CommandText = sQuery;
			Command.ExecuteScalar();
			Command.CommandText = null;
		}
		public DataSet ReadDataSet(string Query)
		{
			DataSet dataSet = new DataSet();
			Command.CommandText = Query;
			using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(this.Command))
			{
				mySqlDataAdapter.Fill(dataSet);
			}
			Command.CommandText = null;
			return dataSet;
		}
        public DataTable ReadDataTable(string Query)
		{
			DataTable dataTable = new DataTable();
            Command.CommandText = Query;
			using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(this.Command))
			{
				mySqlDataAdapter.Fill(dataTable);
			}
			Command.CommandText = null;
			return dataTable;
		}
		public DataRow ReadDataRow(string Query)
		{
			DataTable dataTable = this.ReadDataTable(Query);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable.Rows[0];
			}
			return null;
		}
		public string ReadString(string Query)
		{
			Command.CommandText = Query;
			string result = this.Command.ExecuteScalar().ToString();
			Command.CommandText = null;
			return result;
		}
		public int ReadInt32(string Query)
		{
			Command.CommandText = Query;
			int result = int.Parse(this.Command.ExecuteScalar().ToString());
			Command.CommandText = null;
			return result;
		}
		public uint ReadUInt32(string Query)
		{
			Command.CommandText = Query;
			uint result = (uint)this.Command.ExecuteScalar();
			Command.CommandText = null;
			return result;
		}
        public double ReadDouble(string Query)
        {
            Command.CommandText = Query;
            double result = (double)this.Command.ExecuteScalar();
            Command.CommandText = null;
            return result;
        }
        public bool HasResults(string Query)
        {
            bool Found = false;
            Command.CommandText = Query;
            MySqlDataReader dReader = this.Command.ExecuteReader();
            Found = dReader.HasRows;
            dReader.Close();
            return Found;
        }

        public long GetID()
        {
            return Command.LastInsertedId;
        }
	}
}
