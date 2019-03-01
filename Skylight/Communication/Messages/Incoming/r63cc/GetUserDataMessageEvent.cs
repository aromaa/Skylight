using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r63cc
{
    class GetUserDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            Message.Init(r63ccOutgoing.UserData);
            Message.AppendUInt(session.GetHabbo().ID);
            Message.AppendString(session.GetHabbo().Username);
            Message.AppendString(session.GetHabbo().Look);
            Message.AppendString(session.GetHabbo().Gender.ToUpper());
            Message.AppendString(session.GetHabbo().Motto);
            Message.AppendString(session.GetHabbo().RealName);
            Message.AppendBoolean(false); //unused
            Message.AppendInt32(session.GetHabbo().GetUserStats().RespectReceived);
            Message.AppendInt32(session.GetHabbo().GetUserStats().DailyRespectPoints);
            Message.AppendInt32(session.GetHabbo().GetUserStats().DailyPetRespectPoints);
            Message.AppendBoolean(session.GetHabbo().GetUserSettings().FriendStream);
            Message.AppendString(TimeUtilies.UnixTimestampToDateTime(session.GetHabbo().LastOnline).ToString());
            Message.AppendBoolean(session.GetHabbo().HasPermission("cmd_flagme")); //can change name
            Message.AppendBoolean(false); //unused
            session.SendMessage(Message);
        }
    }
}
