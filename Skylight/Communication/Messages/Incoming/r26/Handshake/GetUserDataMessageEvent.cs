using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26.Handshake
{
    class GetUserDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            Message.Init(r26Outgoing.SendUserInfo);
            Message.AppendString(session.GetHabbo().ID.ToString());
            Message.AppendString(session.GetHabbo().Username);
            Message.AppendString(session.GetHabbo().Look);
            Message.AppendString(session.GetHabbo().Gender.ToUpper());
            Message.AppendString(session.GetHabbo().Motto);
            Message.AppendString(session.GetHabbo().RealName);
            Message.AppendString(""); //unknown
            Message.AppendString("PCch=s02/53,51,44");
            Message.AppendBoolean(false);
            Message.AppendBoolean(true);
            session.SendMessage(Message);
        }
    }
}
