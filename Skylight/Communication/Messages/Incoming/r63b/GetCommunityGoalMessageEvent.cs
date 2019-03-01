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

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class GetCommunityGoalMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message_.Init(r63bOutgoing.CommunityGoal);
            message_.AppendBoolean(false); //finished
            message_.AppendInt32(6); //user amount
            message_.AppendInt32(1); //user rank
            message_.AppendInt32(2); //total
            message_.AppendInt32(1); //mater level
            message_.AppendInt32(4); //none
            message_.AppendInt32(50); //mater progress
            message_.AppendString("giftgiver_val2016"); //name
            message_.AppendInt32(1); //countdown
            message_.AppendInt32(0); //count
            //session.SendMessage(message_);
        }
    }
}
