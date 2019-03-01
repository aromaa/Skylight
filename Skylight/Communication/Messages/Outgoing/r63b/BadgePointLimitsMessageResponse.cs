using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Achievements;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class BadgePointLimitsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<Achievement> achievements = valueHolder.GetValue<List<Achievement>>("Achievements");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.BadgePointLimits);
            message.AppendInt32(achievements.Count);
            foreach (Achievement achievement in achievements)
            {
                message.AppendString(achievement.GetLevel(1).Badge.Substring("ACH_".Length));
                message.AppendInt32(achievement.LevelsCount);
                foreach (AchievementLevel level in achievement.Levels.Values)
                {
                    message.AppendInt32(level.Level);
                    message.AppendInt32(level.ProgressNeeded);
                }
            }
            return message;
        }
    }
}
