using SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class RemoveBuddyMessageEvent : MessengerRemoveFriendsEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.RemovedFriends = new uint[message.PopWiredInt32()];
            for (int i = 0; i < this.RemovedFriends.Length; i++)
            {
                this.RemovedFriends[i] = message.PopWiredUInt();
            }

            base.Handle(session, message);
        }
    }
}
