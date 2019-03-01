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
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Users.Messenger;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger
{
    public class MessengerSendFriendRequestEventHandler : IncomingPacket
    {
        protected string Username;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetMessenger() != null)
            {
                uint userId = 0;
                bool blockFriendRequests = false;

                GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(this.Username);
                if (target?.GetHabbo()?.GetUserSettings() == null)
                {
                    DataRow dataRow = null;
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("username", this.Username);
                        dataRow = dbClient.ReadDataRow("SELECT id, block_newfriends FROM users WHERE username = @username LIMIT 1");
                    }

                    if (dataRow != null)
                    {
                        userId = (uint)dataRow["id"];
                        blockFriendRequests = TextUtilies.StringToBool((string)dataRow["block_newfriends"]);
                    }
                }
                else
                {
                    userId = target.GetHabbo().ID;
                    blockFriendRequests = target.GetHabbo().GetUserSettings().BlockNewFriends;
                }

                if (userId > 0 && userId != session.GetHabbo().ID)
                {
                    if (blockFriendRequests)
                    {
                        session.SendMessage(new MessengerSendFriendRequestErrorComposerHandler(MessengerSendFriendRequestErrorCode.FriendRequestsDisabled));
                    }
                    else
                    {
                        if (session.GetHabbo().GetMessenger().TrySendFriendRequestTo(userId) && !(target?.GetHabbo()?.GetMessenger()?.HasFriendRequestPendingFrom(session.GetHabbo().ID) ?? true))
                        {
                            bool insertSuccess = false;
                            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                            {
                                dbClient.AddParamWithValue("toid", userId);
                                dbClient.AddParamWithValue("userid", session.GetHabbo().ID);
                                insertSuccess = dbClient.ExecuteNonQuery("INSERT INTO messenger_requests (to_id, from_id) VALUES (@toid, @userid)") > 0;
                            }

                            if (insertSuccess)
                            {
                                target?.GetHabbo()?.GetMessenger()?.AddFriendRequest(new MessengerRequest(userId, session.GetHabbo().ID, session.GetHabbo().Username, session.GetHabbo().Look));
                            }
                        }
                    }
                }
                else
                {
                    session.SendMessage(new MessengerSendFriendRequestErrorComposerHandler(MessengerSendFriendRequestErrorCode.RequestNotFound));
                }
            }
        }
    }
}
