﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class RequestCatalogModeEventHandler : IncomingPacket
    {
        protected string Mode;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            session.SendMessage(new SendCatalogModeComposerHandler(0));
        }
    }
}
