using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class InfoRetrieveMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            GameClient session = valueHolder.GetValue<GameClient>("Session");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.SendUserInfo);
            Message.AppendString(session.GetHabbo().ID.ToString());
            Message.AppendString(session.GetHabbo().Username);
            Message.AppendString(session.GetHabbo().Look);
            Message.AppendString(session.GetHabbo().Gender.ToUpper());
            Message.AppendString(session.GetHabbo().Motto);
            Message.AppendString(session.GetHabbo().RealName);
            Message.AppendInt32(0); //unused
            Message.AppendInt32(session.GetHabbo().GetUserStats().RespectReceived);
            Message.AppendInt32(session.GetHabbo().GetUserStats().DailyRespectPoints);
            Message.AppendInt32(session.GetHabbo().GetUserStats().DailyPetRespectPoints);
            Message.AppendBoolean(session.GetHabbo().GetUserSettings().FriendStream);
            return Message;
        }
    }
}
