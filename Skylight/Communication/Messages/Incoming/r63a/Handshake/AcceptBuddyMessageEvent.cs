using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class AcceptBuddyMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetMessenger() != null)
            {
                int amount = message.PopWiredInt32();
                for (int i = 0; i < amount; i++)
                {
                    uint userId = message.PopWiredUInt();
                    MessengerRequest request = session.GetHabbo().GetMessenger().GetFriendRequest(userId);
                    if (request != null)
                    {
                        if (request.ToID != session.GetHabbo().ID)
                        {
                            continue;
                        }

                        if (!session.GetHabbo().GetMessenger().IsFriend(request.FromID))
                        {
                            session.GetHabbo().GetMessenger().AcceptFriend(request.FromID);
                        }
                        session.GetHabbo().GetMessenger().RemoveFriendRequest(request.FromID);
                    }
                }
            }
        }
    }
}
