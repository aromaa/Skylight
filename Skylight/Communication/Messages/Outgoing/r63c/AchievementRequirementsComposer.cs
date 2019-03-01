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
    class AchievementRequirementsComposer<T> : OutgoingHandlerPacket where T : AchievementRequirementsComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.AchievementRequirements);
            message.AppendInt32(Skylight.GetGame().GetAchievementManager().GetAchievements().Count);
            foreach(Achievement achievement in Skylight.GetGame().GetAchievementManager().GetAchievements())
            {
                message.AppendString(achievement.Levels[1].DynamicBadgeLevel ? achievement.Levels[1].Badge.Substring(4) : achievement.Levels[1].Badge);

                message.AppendInt32(achievement.LevelsCount);
                foreach(AchievementLevel level in achievement.Levels.Values)
                {
                    message.AppendInt32(level.Level);
                    message.AppendInt32(level.ProgressNeeded);
                }
            }
            return message;
        }
    }
}
