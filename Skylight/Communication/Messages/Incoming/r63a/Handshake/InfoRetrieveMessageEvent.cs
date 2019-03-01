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

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class InfoRetrieveMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.SendUserInfo);
            Message.AppendStringWithBreak(session.GetHabbo().ID.ToString());
            Message.AppendStringWithBreak(session.GetHabbo().Username);
            Message.AppendStringWithBreak(session.GetHabbo().Look);
            Message.AppendStringWithBreak(session.GetHabbo().Gender.ToUpper());
            Message.AppendStringWithBreak(session.GetHabbo().Motto);
            Message.AppendStringWithBreak(session.GetHabbo().RealName);
            Message.AppendInt32(0); //unknown
            Message.AppendInt32(session.GetHabbo().GetUserStats().RespectReceived);
            Message.AppendInt32(session.GetHabbo().GetUserStats().DailyRespectPointsLeft);
            Message.AppendInt32(session.GetHabbo().GetUserStats().DailyRespectPointsLeft);
            Message.AppendBoolean(false); //friend stream enabled
            session.SendMessage(Message);
        }
    }
}
