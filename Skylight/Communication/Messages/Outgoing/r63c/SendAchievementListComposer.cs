using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Achievements;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class SendAchievementListComposer<T> : OutgoingHandlerPacket where T : SendAchievementListComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.AchievementList);
            message.AppendInt32(Skylight.GetGame().GetAchievementManager().GetAchievements().Count);
            foreach(Achievement achievement in Skylight.GetGame().GetAchievementManager().GetAchievements())
            {
                AchievementLevel level = handler.UserAchievements.GetAchievementLevel(achievement.GroupName) == achievement.LevelsCount ? achievement.GetLevel(handler.UserAchievements.GetAchievementLevel(achievement.GroupName)) : achievement.GetLevel(handler.UserAchievements.GetAchievementLevel(achievement.GroupName) + 1);
                AchievementLevel oldlevel = achievement.GetLevel(handler.UserAchievements.GetAchievementLevel(achievement.GroupName) - 1);

                message.AppendInt32(achievement.ID);
                message.AppendInt32(level.Level);
                message.AppendString(level.LevelBadge);
                message.AppendInt32(oldlevel?.ProgressNeeded ?? 0);
                message.AppendInt32(level.ProgressNeeded);
                message.AppendInt32(level.ActivityPoints);
                message.AppendInt32(level.ActivityPointsType);
                message.AppendInt32(handler.UserAchievements.GetAchievementProgress(achievement.GroupName));
                message.AppendBoolean(handler.UserAchievements.GetAchievementLevel(achievement.GroupName) == achievement.LevelsCount);
                message.AppendString(achievement.Category);
                message.AppendString("");
                message.AppendInt32(achievement.LevelsCount);
                message.AppendInt32(0);
            }
            message.AppendString(""); //open category
            return message;
        }
    }
}
