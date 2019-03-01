using SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class CompletedQuizEvent : CompletedQuizEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.Type = message.PopFixedString();
            
            base.Handle(session, message);
        }
    }
}
