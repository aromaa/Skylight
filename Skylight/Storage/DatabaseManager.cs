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
				ConnString.Server = this.Server.Hostname;
				ConnString.Port = this.Server.Port;
				ConnString.UserID = this.Server.Username;
				ConnString.Password = this.Server.Password;
				ConnString.Database = this.Database.DatabaseName;
				ConnString.MinimumPoolSize = this.Database.PoolMinSize;
				ConnString.MaximumPoolSize = this.Database.PoolMaxSize;
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
