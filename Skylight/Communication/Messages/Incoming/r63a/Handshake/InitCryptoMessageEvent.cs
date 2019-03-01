using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class InitCryptoMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            //TROOLRLOROLROLROLROLROLROLROLRLOROLROLROLROLROLROLOLROLROLROLROLROLROLROLROLROLROLROLRLROLROLROLOLROLOLRLORROLOLROLRLRLORLORLRLORLORLRLRLRLOROLOLROLROLROLROLROLROLROLROLOLROLROLROLROLROLOLROLROLROLROLROLROLROLOLROLROLROLROLROLROLROLOLROLROLROLROLROLROLRLROLOLROLROLROLOLROLOLR
            //THIS IS COPY & PASTE FROM PHOENIX, or eh... almost (cool)
            IncomingPacket incoming;
            if (Skylight.GetPacketManager().HandleIncoming(r63aIncoming.GetSessionParameters, out incoming))
            {
                incoming.Handle(session, null);
            }
        }
    }
}
