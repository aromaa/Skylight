using SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class MessengerRemoveFriendsMessageEvent : MessengerRemoveFriendsEventHandler
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
