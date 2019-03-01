using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Achievements;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class AchievementUnlockedMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            Achievement achievement = valueHolder.GetValue<Achievement>("Achievement");
            AchievementLevel nextLevel = valueHolder.GetValue<AchievementLevel>("NextLevel");
            AchievementLevel currentLevel = valueHolder.GetValue<AchievementLevel>("CurrentLevel");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.AchievementUnlocked);
            message.AppendInt32(achievement.ID);
            message.AppendInt32(nextLevel.Level);
            message.AppendInt32(1337); //idk
            message.AppendString(nextLevel.LevelBadge);
            message.AppendInt32(nextLevel.Score);
            message.AppendInt32(nextLevel.ActivityPoints);
            message.AppendInt32(nextLevel.ActivityPointsType);
            message.AppendInt32(0); //fb(?)
            message.AppendInt32(0); //fb(?)
            if (currentLevel != null)
            {
                message.AppendString(currentLevel.LevelBadge);
            }
            else
            {
                message.AppendString("");
            }
            message.AppendString(achievement.Category);
            message.AppendBoolean(true);
            return message;
        }
    }
}
