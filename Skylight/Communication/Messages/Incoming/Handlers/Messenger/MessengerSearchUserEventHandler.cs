using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System.Data;
using SkylightEmulator.Storage;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger
{
    public class MessengerSearchUserEventHandler : IncomingPacket
    {
        protected string Username;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetMessenger() != null)
            {
                DataTable matchUsers = null;
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("query", this.Username + "%");
                    matchUsers = dbClient.ReadDataTable("SELECT id, username, look, motto, online FROM users WHERE username LIKE @query LIMIT 50");
                }

                if (matchUsers?.Rows.Count > 0)
                {
                    List<DataRow> friends = new List<DataRow>();
                    List<DataRow> users = new List<DataRow>();
                    foreach (DataRow dataRow in matchUsers.Rows)
                    {
                        MessengerFriend friend = session.GetHabbo().GetMessenger().GetFriend((uint)dataRow["Id"]);
                        if (friend != null)
                        {
                            friends.Add(dataRow);
                        }
                        else
                        {
                            users.Add(dataRow);
                        }
                    }

                    session.SendMessage(new MessengerSearchUserResultsComposerHandler(friends, users));
                }
                else
                {
                    session.SendMessage(new MessengerSearchUserResultsComposerHandler());
                }
            }
        }
    }
}
