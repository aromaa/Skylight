using MySql.Data.MySqlClient;
using System;
namespace SkylightEmulator.Storage
{
    public class DatabaseManager
	{
		public DatabaseServer Server;
		public Database Database;
		public string ConnectionString
		{
			get
			{
				MySqlConnectionStringBuilder ConnString = new MySqlConnectionStringBuilder();
				ConnString.Server = Server.Hostname;
				ConnString.Port = Server.Port;
				ConnString.UserID = Server.Username;
				ConnString.Password = Server.Password;
				ConnString.Database = Database.DatabaseName;
				ConnString.MinimumPoolSize = Database.PoolMinSize;
				ConnString.MaximumPoolSize = Database.PoolMaxSize;
				ConnString.Pooling = true;
				return ConnString.ToString();
			}
		}
		public DatabaseManager(DatabaseServer _Server, Database _Database)
		{
			this.Server = _Server;
			this.Database = _Database;
		}
		public DatabaseClient GetClient()
		{
			return new DatabaseClient(this);
		}
	}
}
