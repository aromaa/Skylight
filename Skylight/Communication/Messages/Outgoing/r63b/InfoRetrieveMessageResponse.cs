using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class InfoRetrieveMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            GameClient session = valueHolder.GetValue<GameClient>("Session");
            
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            Message.Init(r63bOutgoing.SendUserInfo);
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
            return Message;
        }
    }
}
