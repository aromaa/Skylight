using SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class SetActiveBadgesEvent : SetActiveBadgesEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.Badges = new string[5];
            while(message.GetRemainingLength() > 0)
            {
                this.Badges[message.PopWiredInt32() - 1] = message.PopFixedString();
            }

            base.Handle(session, message);
        }
    }
}
