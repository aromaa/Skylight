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
    class WhisperEvent : WhisperEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            string[] splitd = message.PopFixedString().Split(new char[] { ' ' }, 2);
            this.Username = splitd[0];
            this.Message = splitd[1];
            this.Bubble = message.PopWiredInt32();
            
            base.Handle(session, message);
        }
    }
}
