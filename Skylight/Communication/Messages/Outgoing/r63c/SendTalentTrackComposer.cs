using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Talent;
using SkylightEmulator.HabboHotel.Achievements;
using SkylightEmulator.Core;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class SendTalentTrackComposer<T> : OutgoingHandlerPacket where T : SendTalentTrackComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.SendTalentTrack);
            message.AppendString(handler.Track.Name);

            List<string> perks = null;
            Dictionary<uint, int> items = null;

            bool lastCompleted = true;
            message.AppendInt32(handler.Track.GetLevels().Count);
            foreach (TalentTrackLevel level in handler.Track.GetLevels())
            {
                bool completed = handler.Track.GetLevels().Count - 1 == level.Level ? false : lastCompleted && level.HasCompleted(handler.Habbo);

                message.AppendInt32(level.Level);
                message.AppendInt32(lastCompleted ? completed ? 2 : 1 : 0);
                message.AppendInt32(level.AchievementsRequired.Count);
                foreach(KeyValuePair<string, int> achievement in level.AchievementsRequired)
                {
                    AchievementLevel achievementLevel = Skylight.GetGame().GetAchievementManager().GetAchievement(achievement.Key).GetLevel(achievement.Value);

                    message.AppendInt32(achievementLevel.ID);
                    message.AppendInt32(achievementLevel.Level);
                    message.AppendString(achievementLevel.LevelBadge);
                    message.AppendInt32(lastCompleted ? completed || handler.Habbo.GetUserAchievements().GetAchievementLevel(achievement.Key) >= achievementLevel.Level ? 2 : 1 : 0); //state
                    message.AppendInt32(handler.Habbo.GetUserAchievements().GetAchievementProgress(achievement.Key));
                    message.AppendInt32(achievementLevel.ProgressNeeded);
                }

                message.AppendInt32(perks?.Count ?? 0);
                if (perks != null)
                {
                    foreach (string perk in perks)
                    {
                        message.AppendString(perk);
                    }
                }

                message.AppendInt32(items?.Count ?? 0);
                if (items != null)
                {
                    foreach (KeyValuePair<uint, int> item in items)
                    {
                        message.AppendString(item.Key.ToString());
                        message.AppendInt32(item.Value);
                    }
                }

                lastCompleted = completed;
                perks = level.PrizePerks;
                items = level.PrizeItems;
            }
            return message;
        }
    }
}
