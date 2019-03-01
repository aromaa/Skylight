using SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger;
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
    class AcceptBuddyMessageEvent : MessengerAcceptFriendRequestEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.FriendRequests = new uint[message.PopWiredInt32()];
            for (int i = 0; i < this.FriendRequests.Length; i++)
            {
                this.FriendRequests[i] = message.PopWiredUInt();
            }

            base.Handle(session, message);
        }
    }
}
