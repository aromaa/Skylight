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
    class GetAchievementCompetionHOFMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string code = message.PopFixedString();

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message_.Init(r63bOutgoing.AchievementCompetion);
            message_.AppendString(code);
            message_.AppendInt32(10); //count
            for (int i = 1; i < 11; i++ )
            {
                message_.AppendInt32(i); //id
                message_.AppendString("Jonny Is The Best #" + i); //username
                message_.AppendString("cc-3289-110.ca-3187-82.ha-3478-1408.lg-3401-110-1408.hr-828-1403.sh-906-1408.fa-3276-1290.ch-3438-1408-82.hd-209-2"); //figure
                message_.AppendInt32(i); //position
                message_.AppendInt32(100 - (int)(i * 2.5)); //points
            }
            session.SendMessage(message_);
        }
    }
}
