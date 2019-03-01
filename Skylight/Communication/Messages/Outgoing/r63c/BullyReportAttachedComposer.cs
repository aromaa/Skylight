using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Support;
using System.Collections.Concurrent;
using SkylightEmulator.HabboHotel.Guardian;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class BullyReportAttachedComposer<T> : OutgoingHandlerPacket where T : BullyReportAttachedComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ConcurrentDictionary<uint, int> users = new ConcurrentDictionary<uint, int>();

            DateTime date = TimeUtilies.UnixTimestampToDateTime(handler.Report.Timestamp);

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.BullyReportAttached);
            message.AppendInt32(BullyReport.VoteTime);

            StringBuilder stringBuidler = new StringBuilder(date.Year + " " + date.Month + " " + date.Day + " " + date.Hour + " " + date.Minute + " " + date.Second + ";\r");
            foreach(RoomMessage message_ in handler.Report.Chatlog)
            {
                stringBuidler.Append(";" + users.GetOrAdd(message_.UserID, users.Count) + ";" + message_.Message + "\r");
            }

            message.AppendString(stringBuidler.ToString());
            return message;
        }
    }
}
