using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Core;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class SendInstantMessageMessageEvent : MessengerSendPrivateMessageEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.UserID = message.PopWiredUInt();
            this.Message = message.PopFixedString();

            base.Handle(session, message);
        }
    }
}
