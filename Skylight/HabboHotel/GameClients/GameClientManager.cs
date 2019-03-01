using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Users;
using SkylightEmulator.Net;
using SkylightEmulator.Storage;
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

        public GameClientManager()
        {
            this.Clients = new Dictionary<long, GameClient>();
            this.CachedUsernames = new Dictionary<uint, string>();
        }

        public void Connection(SocketsConnection connection)
        {
            GameClient gameClient = new GameClient(connection.GetID(), connection);
            this.Clients.Add(connection.GetID(), gameClient);
            gameClient.Start();
        }

        public void Disconnection(SocketsConnection connection)
        {
            GameClient gameClient = this.Clients[connection.GetID()];
            if (gameClient != null)
            {
                gameClient.HandleDisconnection();
            }
            this.Clients.Remove(connection.GetID());
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
            if (this.CachedUsernames.ContainsKey(id))
            {
                this.CachedUsernames[id] = username;
            }
            else
            {
                this.CachedUsernames.Add(id, username);
            }
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
                    DataRow usernameRow = dbClient.ReadDataRow("SELECT username FROM users WHERE id = @userid");
                    if (usernameRow != null)
                    {
                        string username = (string)usernameRow["username"];
                        this.UpdateCachedUsername(id, username);
                        return username;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public GameClient GetGameClientById(uint id)
        {
            if (this.Clients.Values.Any(g => g.GetHabbo() != null && g.GetHabbo().ID == id))
            {
                return this.Clients.Values.Where(g => g.GetHabbo() != null && g.GetHabbo().ID == id).First();
            }
            else
            {
                return null;
            }
        }

        public GameClient GetGameClientByUsername(string username)
        {
            if (this.Clients.Values.Any(g => g.GetHabbo() != null && g.GetHabbo().Username == username))
            {
                return this.Clients.Values.Where(g => g.GetHabbo() != null && g.GetHabbo().Username == username).First();
            }
            else
            {
                return null;
            }
        }
    }
}
