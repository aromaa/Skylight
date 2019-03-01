using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger
{
    public class MessengerFollowFriendEventHandler : IncomingPacket
    {
        protected uint UserID;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetMessenger() != null)
            {
                if (session.GetHabbo().GetMessenger().IsFriendWith(this.UserID))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.UserID);
                    if (target?.GetHabbo()?.GetRoomSession() != null)
                    {
                        if (!target.GetHabbo().GetUserSettings().HideInRoom)
                        {
                            if (target.GetHabbo().GetRoomSession().GetRoom() != null)
                            {
                                session.SendMessage(new MessengerFollowUserComposerHandler(target.GetHabbo().GetRoomSession().GetRoom().ID));
                            }
                            else
                            {
                                session.SendMessage(new MessengerFollowFriendErrorComposerHandler(MessengerFollowFriendErrorCode.Hotelview));
                            }
                        }
                        else
                        {
                            session.SendMessage(new MessengerFollowFriendErrorComposerHandler(MessengerFollowFriendErrorCode.Prevented));
                        }
                    }
                    else
                    {
                        session.SendMessage(new MessengerFollowFriendErrorComposerHandler(MessengerFollowFriendErrorCode.Offline));
                    }
                }
                else
                {
                    session.SendMessage(new MessengerFollowFriendErrorComposerHandler(MessengerFollowFriendErrorCode.NotFriend));
                }
            }
        }
    }
}
