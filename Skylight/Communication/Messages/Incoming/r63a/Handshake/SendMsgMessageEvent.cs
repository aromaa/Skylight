using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class SendMsgMessageEvent : MessengerSendPrivateMessageEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.UserID = message.PopWiredUInt();
            this.Message = message.PopFixedString();

            base.Handle(session, message);
        }
    }
}
