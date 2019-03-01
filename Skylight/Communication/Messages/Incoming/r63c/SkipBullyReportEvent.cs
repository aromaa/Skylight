using SkylightEmulator.Communication.Messages.Incoming.Handlers.Guardian;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class SkipBullyReportEvent : SkipBullyReportEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            base.Handle(session, message);
        }
    }
}
