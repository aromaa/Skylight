using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Achievements;
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
    class UpdateAchievementMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            GameClient session = valueHolder.GetValue<GameClient>("Session");
            Achievement achievement = valueHolder.GetValue<Achievement>("Achievement");
            AchievementLevel level = valueHolder.GetValue<AchievementLevel>("Level");
            AchievementLevel lastLevel = valueHolder.GetValue<AchievementLevel>("LastLevel");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.AchievementUpdate);
            message.AppendInt32(achievement.ID); //id
            message.AppendInt32(level.Level); //current level
            message.AppendString(level.LevelBadge); //badge code
            message.AppendInt32(lastLevel != null ? lastLevel.ProgressNeeded : 0); //require
            message.AppendInt32(level.ProgressNeeded); //progress needed
            message.AppendInt32(level.ActivityPoints); //pixes
            message.AppendInt32(level.ActivityPointsType); //currency type
            message.AppendInt32(session.GetHabbo().GetUserAchievements().GetAchievementProgress(achievement.GroupName)); //current progress
            message.AppendBoolean(session.GetHabbo().GetUserAchievements().GetAchievementLevel(achievement.GroupName) == achievement.LevelsCount); //completed or not
            message.AppendString(achievement.Category); //category
            message.AppendString(""); //unused
            message.AppendInt32(achievement.LevelsCount); //how many levels
            message.AppendInt32(0); //dont show progress bar
            return message;
        }
    }
}
