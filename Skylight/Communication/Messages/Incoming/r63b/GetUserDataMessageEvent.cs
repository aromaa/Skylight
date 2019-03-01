using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class GetUserDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            session.SendMessage(BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.InfoRetrieve).Handle(new ValueHolder().AddValue("Session", session)));
        }
    }
}
