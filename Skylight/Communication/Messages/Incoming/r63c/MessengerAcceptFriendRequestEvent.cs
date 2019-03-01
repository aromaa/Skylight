using SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class MessengerAcceptFriendRequestEvent : MessengerAcceptFriendRequestEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.FriendRequests = new uint[message.PopWiredInt32()];
            for(int i = 0; i < this.FriendRequests.Length; i++)
            {
                this.FriendRequests[i] = message.PopWiredUInt();
            }

            base.Handle(session, message);
        }
    }
}
