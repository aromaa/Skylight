using SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class LookAtPointEvent : LookAtPointEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.X = message.PopWiredInt32();
            this.Y = message.PopWiredInt32();

            base.Handle(session, message);
        }
    }
}
