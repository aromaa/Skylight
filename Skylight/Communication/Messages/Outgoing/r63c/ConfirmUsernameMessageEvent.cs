using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Utilies;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class ConfirmUsernameMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message_.Init(r63cOutgoing.ValidUsername);
            message_.AppendInt32(0); //result
            message_.AppendString(session.GetHabbo().Username);
            message_.AppendInt32(0); //suggested names
            session.SendMessage(message_);
        }
    }
}
