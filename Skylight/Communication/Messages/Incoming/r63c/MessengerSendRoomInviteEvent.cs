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
    class MessengerSendRoomInviteEvent : MessengerSendRoomInviteEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.SendTo = new uint[message.PopWiredInt32()];
            for(int i= 0; i < this.SendTo.Length; i++)
            {
                this.SendTo[i] = message.PopWiredUInt();
            }
            this.Message = message.PopFixedString();
            
            base.Handle(session, message);
        }
    }
}
