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
    class SaveLookToWardrobeEvent : SaveLookToWardrobeEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.SlotID = message.PopWiredInt32();
            this.Look = message.PopFixedString();
            this.Gender = message.PopFixedString();

            base.Handle(session, message);
        }
    }
}
