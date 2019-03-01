using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Users.Badges;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Achievements
{
    public class AchievementManager
    {
        private Dictionary<string, Achievement> Achievements;

        public AchievementManager()
        {
            this.Achievements = new Dictionary<string, Achievement>();
        }

        public void LoadAchievements(DatabaseClient dbClient)
        {
            Logging.Write("Loading achievements... ");
            Dictionary<string, Achievement> newAchievemetns = new Dictionary<string, Achievement>();

            DataTable achievements = dbClient.ReadDataTable("SELECT * FROM achievements");
            if (achievements != null && achievements.Rows.Count > 0)
            {
                foreach(DataRow dataRow in achievements.Rows)
                {
                    string groupName = (string)dataRow["group_name"];

                    if (!newAchievemetns.TryGetValue(groupName, out Achievement achievement))
                    {
                        achievement = new Achievement((string)dataRow["category"], groupName);
                        newAchievemetns.Add(groupName, achievement);
                    }

                    achievement.AddLevel(new AchievementLevel((int)dataRow["id"], (int)dataRow["level"], TextUtilies.StringToBool((string)dataRow["dynamic_badgelevel"]), (string)dataRow["badge"], (int)dataRow["activity_points_type"], (int)dataRow["activity_points"], (int)dataRow["score"], (int)dataRow["progress_needed"]));
                }
            }

            this.Achievements = newAchievemetns;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public ServerMessage GetAchievements(GameClient session)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.Achievements);
            message.AppendInt32(this.Achievements.Count);
            foreach(Achievement achievement in this.Achievements.Values)
            {
                AchievementLevel level = null;
                if (session.GetHabbo().GetUserAchievements().GetAchievementLevel(achievement.GroupName) == achievement.LevelsCount)
                {
                    level = achievement.GetLevel(session.GetHabbo().GetUserAchievements().GetAchievementLevel(achievement.GroupName)); //current level
                }
                else
                {
                    level = achievement.GetLevel(session.GetHabbo().GetUserAchievements().GetAchievementLevel(achievement.GroupName) + 1); //next level
                }

                message.AppendInt32(achievement.ID); //id
                message.AppendInt32(level.Level); //current level
                message.AppendString(level.LevelBadge); //badge code
                message.AppendInt32(level.ProgressNeeded); //progress needed
                message.AppendInt32(level.ActivityPoints); //pixes
                message.AppendInt32(level.ActivityPointsType); //currency type
                message.AppendInt32(session.GetHabbo().GetUserAchievements().GetAchievementProgress(achievement.GroupName)); //current progress
                message.AppendBoolean(session.GetHabbo().GetUserAchievements().GetAchievementLevel(achievement.GroupName) == achievement.LevelsCount); //completed or not
                message.AppendString(achievement.Category); //category
                message.AppendInt32(achievement.LevelsCount); //how many levels
            }
            message.AppendString(""); //idk
            return message;
        }

        public void AddAchievement(GameClient session, string group)
        {
            this.AddAchievement(session, group, session.GetHabbo().GetUserAchievements().GetAchievementLevel(group) + 1);
        }

        public void AddAchievement(GameClient session, string group, int level)
        {
            if (this.Achievements.ContainsKey(group))
            {
                Achievement achievement = this.Achievements[group];
                if (achievement != null)
                {
                    AchievementLevel currentLevel = achievement.GetLevel(session.GetHabbo().GetUserAchievements().GetAchievementLevel(group));
                    AchievementLevel nextLevel = achievement.GetLevel(level);
                    if (nextLevel != null && nextLevel.Level > session.GetHabbo().GetUserAchievements().GetAchievementLevel(group))
                    {
                        session.GetHabbo().GetUserAchievements().AchievementUnlocked(group, level);
                        if (currentLevel != null)
                        {
                            session.GetHabbo().GetBadgeManager().RemoveBadge(currentLevel.LevelBadge);
                        }
                        session.GetHabbo().GetBadgeManager().AddBadge(nextLevel.LevelBadge, 0, true);
                        session.GetHabbo().AddActivityPoints(nextLevel.ActivityPointsType, nextLevel.ActivityPoints);
                        session.GetHabbo().UpdateActivityPoints(nextLevel.ActivityPointsType, true);
                        session.GetHabbo().GetUserStats().AchievementPoints += nextLevel.Score;

                        if (session.Revision > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
                        {
                            session.SendMessage(BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.AchievementUnlocked).Handle(new ValueHolder().AddValue("Achievement", achievement).AddValue("NextLevel", nextLevel).AddValue("CurrentLevel", currentLevel)));
                        }

                        this.UpdateAchievement(session, group);

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            if (session.GetHabbo().GetUserSettings().FriendStream)
                            {
                                dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                                dbClient.AddParamWithValue("badge", nextLevel.LevelBadge);
                                dbClient.ExecuteQuery("INSERT INTO user_friend_stream(type, user_id, timestamp, extra_data) VALUES('2', @userId, UNIX_TIMESTAMP(), @badge)");
                            }
                        }
                    }
                }
            }
        }

        public void UpdateAchievement(GameClient session, string group)
        {
            if (session.Revision > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                if (this.Achievements.ContainsKey(group))
                {
                    Achievement achievement = this.Achievements[group];
                    if (achievement != null)
                    {
                        AchievementLevel level = session.GetHabbo().GetUserAchievements().GetAchievementLevel(achievement.GroupName) == achievement.LevelsCount ? achievement.GetLevel(session.GetHabbo().GetUserAchievements().GetAchievementLevel(achievement.GroupName)) : achievement.GetLevel(session.GetHabbo().GetUserAchievements().GetAchievementLevel(achievement.GroupName) + 1); //next level progress
                        if (level != null)
                        {
                            session.SendMessage(BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.UpdateAchievement).Handle(new ValueHolder().AddValue("Session", session).AddValue("Achievement", achievement).AddValue("Level", level).AddValue("LastLevel", achievement.GetLevel(level.Level - 1))));
                        }
                    }
                }
            }
        }

        public void Shutdown()
        {
            if (this.Achievements != null)
            {
                this.Achievements.Clear();
            }
            this.Achievements = null;
        }

        public Achievement GetAchievement(string achievement)
        {
            this.Achievements.TryGetValue(achievement, out Achievement achievement_);
            return achievement_;
        }

        public ServerMessage BadgePointLimits(Revision revision)
        {
            return BasicUtilies.GetRevisionPacketManager(revision).GetOutgoing(OutgoingPacketsEnum.BadgePointLimits).Handle(new ValueHolder().AddValue("Achievements", this.Achievements.Values.ToList()));
        }

        public ICollection<Achievement> GetAchievements()
        {
            return this.Achievements.Values;
        }
    }
}
