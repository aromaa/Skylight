using System;
namespace SkylightEmulator.Storage
{
	public sealed class DatabaseServer
	{
		private readonly string _Hostname;
		private readonly uint _Port;
		private readonly string _Username;
		private readonly string _Password;
		public string Hostname
		{
			get
			{
				return this._Hostname;
			}
		}
		public uint Port
		{
			get
			{
				return this._Port;
			}
		}
		public string Username
		{
			get
			{
				return this._Username;
			}
		}
		public string Password
		{
			get
			{
				return this._Password;
			}
		}
		public DatabaseServer(string Hostname, uint Port, string Username, string Password)
		{
			if (Hostname == null || Hostname.Length == 0)
			{
				throw new ArgumentException("sHost");
			}
			if (Username == null || Username.Length == 0)
			{
				throw new ArgumentException("sUser");
			}
			this._Hostname = Hostname;
			this._Port = Port;
			this._Username = Username;
			this._Password = ((Password != null) ? Password : "");
		}
		public override string ToString()
		{
			return this._Username + "@" + this._Hostname;
		}
	}
}
