﻿using SkylightEmulator.Communication.Messages.Incoming.Handlers.Guide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class GuideRecommendHelperEvent : GuideRecommendHelperEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.Recommend = message.PopWiredBoolean();

            base.Handle(session, message);
        }
    }
}
