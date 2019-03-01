using MySql.Data.MySqlClient;
using System;
using System.Data;
using SkylightEmulator.Storage;
using System.IO;
using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System.IO.Compression;
namespace SkylightEmulator.Storage
{
	public sealed class DatabaseClient : IDisposable
	{
		private DatabaseManager Manager;
		private MySqlConnection Connection;
        private MySqlCommand Command;
        private MySqlTransaction Transaction;

        private double LastTotalRowsProgressChange;

		public DatabaseClient(DatabaseManager _Manager)
		{
            this.Manager = _Manager;
            this.Connection = new MySqlConnection(_Manager.ConnectionString);
            this.Command = this.Connection.CreateCommand();
            this.Connection.Open();
		}
		public void Dispose()
		{
            this.Connection.Close();
            this.Command.Dispose();
            this.Connection.Dispose();
		}
		public void AddParamWithValue(string sParam, object val)
		{
            this.Command.Parameters.AddWithValue(sParam, val);
		}
        public void ClearParams()
        {
            this.Command.Parameters.Clear();
        }
		public long ExecuteQuery(string sQuery)
		{
            long lastInsertedId = 0L;

            this.Command.CommandText = sQuery;
            this.Command.ExecuteScalar();
            lastInsertedId = this.Command.LastInsertedId;
            this.Command.CommandText = null;

            return lastInsertedId;
		}

        public int ExecuteNonQuery(string query)
        {
            this.Command.CommandText = query;
            int effectedRows = this.Command.ExecuteNonQuery();
            this.Command.CommandText = null;

            return effectedRows;
        }

		public DataSet ReadDataSet(string Query)
		{
			DataSet dataSet = new DataSet();
            this.Command.CommandText = Query;
			using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(this.Command))
			{
				mySqlDataAdapter.Fill(dataSet);
			}
            this.Command.CommandText = null;
			return dataSet;
		}
        public DataTable ReadDataTable(string Query)
		{
			DataTable dataTable = new DataTable();
            this.Command.CommandText = Query;
			using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(this.Command))
			{
				mySqlDataAdapter.Fill(dataTable);
			}
            this.Command.CommandText = null;
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
            this.Command.CommandText = Query;
			string result = this.Command.ExecuteScalar().ToString();
            this.Command.CommandText = null;
			return result;
		}
		public int ReadInt32(string Query)
		{
            this.Command.CommandText = Query;
			int result = int.Parse(this.Command.ExecuteScalar().ToString());
            this.Command.CommandText = null;
			return result;
		}
		public uint ReadUInt32(string Query)
		{
            this.Command.CommandText = Query;
			uint result = (uint)this.Command.ExecuteScalar();
            this.Command.CommandText = null;
			return result;
		}
        public double ReadDouble(string Query)
        {
            this.Command.CommandText = Query;
            double result = (double)this.Command.ExecuteScalar();
            this.Command.CommandText = null;
            return result;
        }
        public bool HasResults(string Query)
        {
            bool Found = false;
            this.Command.CommandText = Query;
            MySqlDataReader dReader = this.Command.ExecuteReader();
            Found = dReader.HasRows;
            dReader.Close();
            return Found;
        }

        public void StartTransaction()
        {
            this.Command.Transaction = this.Transaction = this.Connection.BeginTransaction();
        }

        public void Rollback()
        {
            this.Transaction.Rollback();
        }

        public void Commit()
        {
            this.Transaction.Commit();
        }

        public long GetID()
        {
            return this.Command.LastInsertedId;
        }

        public void TakeBackup(bool compress = false)
        {
            try
            {
                using (MySqlBackup mySqlBackup = new MySqlBackup(this.Command))
                {
                    string path = Path.Combine(Environment.CurrentDirectory, "Backups");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    mySqlBackup.ExportProgressChanged += new MySqlBackup.exportProgressChange(this.ExportProgressStatusChanged);
                    mySqlBackup.GetTotalRowsProgressChanged += new MySqlBackup.getTotalRowsProgressChange(this.GetTotalRowsProgressStatusChanged);
                    mySqlBackup.ExportInfo.GetTotalRowsBeforeExport = true;

                    if (compress)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                            {
                                ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry(DateTime.Now.ToString().Replace(':', '-') + ".sql"); //we need replace : with - becase windows dont support : on file names
                                using (Stream zipArchiveEntryStream = zipArchiveEntry.Open())
                                {
                                    using (BinaryWriter binaryWriter = new BinaryWriter(zipArchiveEntryStream))
                                    {
                                        MemoryStream memoryStream2 = new MemoryStream();
                                        using (memoryStream2)
                                        {
                                            mySqlBackup.ExportToMemoryStream(memoryStream2);
                                        }
                                        binaryWriter.Write(memoryStream2.ToArray());
                                    }
                                }
                            }

                            using (FileStream file = File.Create(path + @"\" + DateTime.Now.ToString("d.M.yyyy HH-mm") + ".zip")) //we need replace : with - becase windows dont support : on file names
                            {
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                memoryStream.CopyTo(file);
                            }
                        }
                    }
                    else
                    {
                        mySqlBackup.ExportToFile(path + @"\" + DateTime.Now.ToString("d.M.yyyy HH-mm") + ".sql"); //we need replace : with - becase windows dont support : on file names
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CRITICAL ERROR WHEN TRYING GET BACKUPS! " + ex.ToString());
            }
        }

        public void ExportProgressStatusChanged(object sender, ExportProgressArgs e)
        {
            Logging.WriteLine("MySQL backup status: " + e.CurrentRowIndexInAllTables + "/" + e.TotalRowsInAllTables);
        }

        public void GetTotalRowsProgressStatusChanged(object sender, GetTotalRowsArgs e)
        {
            double currentTime = TimeUtilies.GetUnixTimestamp();
            if (currentTime - this.LastTotalRowsProgressChange >= 1) //1s
            {
                this.LastTotalRowsProgressChange = currentTime;

                Logging.WriteLine("MySQL backup status: Loading database information!");
            }
        }
	}
}
