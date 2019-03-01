using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Users;
using SkylightEmulator.Net;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.GameClients
{
    public class GameClientManager
    {
        private Dictionary<long, GameClient> Clients;
        private Dictionary<uint, string> CachedUsernames;
        private Dictionary<string, uint> CachedIDs;

        public GameClientManager()
        {
            this.Clients = new Dictionary<long, GameClient>();
            this.CachedUsernames = new Dictionary<uint, string>();
            this.CachedIDs = new Dictionary<string, uint>();
        }

        public int OnlineCount
        {
            get
            {
                return this.Clients.Values.Count(c => c != null && c.GetHabbo() != null);
            }
        }

        public void Connection(SocketsConnection connection, Revision revision, Crypto crypto)
        {
            GameClient gameClient = new GameClient(connection.GetID(), connection, revision, crypto);
            this.Clients.Add(connection.GetID(), gameClient);
            gameClient.Start();
        }

        public void Disconnection(long id)
        {
            GameClient gameClient = null;
            if (this.Clients.TryGetValue(id, out gameClient))
            {
                gameClient.HandleDisconnection();
            }
            this.Clients.Remove(id);
        }

        public void DisconnectDoubleSession(uint id)
        {
            foreach(GameClient gameClient in this.Clients.Values.ToList())
            {
                if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().ID == id)
                {
                    gameClient.Stop("Another user entered hotel with same ID");
                }
            }
        }

        public void UpdateCachedUsername(uint id, string username)
        {
            this.CachedUsernames[id] = username;
        }

        public void UpdateCachedID(uint id, string username)
        {
            this.CachedIDs[username] = id;
        }

        public string GetUsernameByID(uint id)
        {
            if (this.CachedUsernames.ContainsKey(id))
            {
                return this.CachedUsernames[id];
            }
            else
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userid", id);
                    DataRow usernameRow = dbClient.ReadDataRow("SELECT username FROM users WHERE id = @userid LIMIT 1");
                    if (usernameRow != null)
                    {
                        string username = (string)usernameRow["username"];
                        this.UpdateCachedUsername(id, username);
                        this.UpdateCachedID(id, username);
                        return username;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public bool UsernameExits(string username)
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("username", username);
                DataRow usernameRow = dbClient.ReadDataRow("SELECT username FROM users WHERE username = @username LIMIT 1");
                if (usernameRow != null && !string.IsNullOrEmpty((string)usernameRow["username"]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public uint GetIDByUsername(string username)
        {
            if (this.CachedIDs.ContainsKey(username))
            {
                return this.CachedIDs[username];
            }
            else
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("username", username);
                    DataRow idRow = dbClient.ReadDataRow("SELECT id FROM users WHERE username = @username LIMIT 1");
                    if (idRow != null)
                    {
                        uint id = (uint)idRow["id"];
                        this.UpdateCachedID(id, username);
                        this.UpdateCachedUsername(id, username);
                        return id;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public GameClient GetGameClientById(uint id)
        {
            return this.Clients.Values.FirstOrDefault(g => g.GetHabbo() != null && g.GetHabbo().ID == id);
        }

        public GameClient GetGameClientByUsername(string username)
        {
            return this.Clients.Values.FirstOrDefault(g => g.GetHabbo() != null && g.GetHabbo().Username.ToLower() == username.ToLower());
        }

        public List<GameClient> GetClients()
        {
            return this.Clients.Values.ToList();
        }
    }
}
