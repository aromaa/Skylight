using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Profiles;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class BullyReportStartComposer<T> : OutgoingHandlerPacket where T : BullyReportStartComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.BullyReportStart);
            message.AppendInt32((int)handler.Code);

            if (handler.Code == BullyReportStartCode.OngoingCase)
            {
                message.AppendInt32(3); //type, 3 = bully
                message.AppendInt32((int) (TimeUtilies.GetUnixTimestamp() - handler.Report.Timestamp));
                message.AppendBoolean(false); //pending guide sessions

                UserProfile profile = Skylight.GetGame().GetUserProfileManager().GetProfile(handler.Report.ReportedID);
                message.AppendString(profile.Username);
                message.AppendString(profile.Look);
                message.AppendString(Skylight.GetGame().GetRoomManager().TryGetRoomData(handler.Report.RoomID).Name);
            }
            return message;
        }
    }
}
