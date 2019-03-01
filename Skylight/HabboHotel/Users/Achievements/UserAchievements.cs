using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Achievements;
using SkylightEmulator.HabboHotel.Pets;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Achievements
{
    public class UserAchievements
    {
        private readonly Habbo Habbo;
        private readonly Dictionary<string, int> AchievementLevels;

        public UserAchievements(Habbo habbo)
        {
            this.AchievementLevels = new Dictionary<string, int>();
            this.Habbo = habbo;
            
            foreach (DataRow dataRow in this.Habbo.GetUserDataFactory().GetAchievements()?.Rows)
            {
                this.AchievementLevels.Add((string)dataRow["achievement_group"], (int)dataRow["achievement_level"]);
            }
        }

        public int GetAchievementProgress(string achievement)
        {
            switch (achievement)
            {
                case "EmailVerification":
                    {
                        return 0;
                    }
                case "AvatarLook":
                    {
                        return 0;
                    }
                case "ChangeMotto":
                    {
                        return 0;
                    }
                case "Tags":
                    {
                        return this.Habbo.Tags.Count;
                    }
                case "ChangeName":
                    {
                        return 0;
                    }
                case "HappyHour":
                    {
                        return 0;
                    }
                case "Student":
                    {
                        return 0;
                    }
                case "Graduate":
                    {
                        return 0;
                    }
                case "RegistrationDuration":
                    {
                        return (int)Math.Floor((TimeUtilies.GetUnixTimestamp() - this.Habbo.AccountCreated) / 86400);
                    }
                case "OnlineTime":
                    {
                        return (int)Math.Floor(this.Habbo.GetUserStats().OnlineTime / 60); //minutes
                    }
                case "RegularVisitor":
                    {
                        return this.Habbo.GetUserStats().RegularVisitor;
                    }
                case "HCMember":
                    {
                        return (int)Math.Floor((this.Habbo.GetSubscriptionManager().SubscriptionTime("habbo_club") + this.Habbo.GetSubscriptionManager().SubscriptionTime("habbo_vip")) / 2678400);
                    }
                case "VIPMember":
                    {
                        return (int)Math.Floor((this.Habbo.GetSubscriptionManager().SubscriptionTime("habbo_vip")) / 2678400);
                    }
                case "RespectReceived":
                    {
                        return this.Habbo.GetUserStats().RespectReceived;
                    }
                case "GiftGiver":
                    {
                        return this.Habbo.GetUserStats().GiftsGived;
                    }
                case "GiftReceiver":
                    {
                        return this.Habbo.GetUserStats().GiftsReceived;
                    }
                case "NotesLeft":
                    {
                        return this.Habbo.GetUserStats().NotesLeft;
                    }
                case "NotesReceived":
                    {
                        return this.Habbo.GetUserStats().NotesReceived;
                    }
                case "FriendListSize":
                    {
                        return this.Habbo.GetMessenger().GetFriends().Count;
                    }
                case "RespectGiven":
                    {
                        return this.Habbo.GetUserStats().RespectGiven;
                    }
                case "FireworksCharger":
                    {
                        return this.Habbo.GetUserStats().FireworksCharger;
                    }
                case "PetOwner":
                    {
                        return this.Habbo.Pets.Count;
                    }
                case "PetTrainer":
                    {
                        return this.Habbo.Pets.Values.Sum(p => p.Level - 1);
                    }
                case "PetFreeder": //make pets smarter first
                    {
                        return -1;
                    }
                case "PetRespectReceiver":
                    {
                        return this.Habbo.Pets.Values.Sum(p => p.Respect);
                    }
                case "EquestrianTrackHost":
                    {
                        return this.Habbo.GetUserStats().EquestrianTrackHost;
                    }
                case "HorseJumper":
                    {
                        return this.Habbo.GetUserStats().HorseJumper;
                    }
                case "StableOwner":
                    {
                        return this.Habbo.GetUserStats().StableOwner;
                    }
                case "Equestrian":
                    {
                        return this.Habbo.GetUserStats().Equestrian;
                    }
                case "PetRespectGiven":
                    {
                        return this.Habbo.GetUserStats().PetRespectGiven;
                    }
                case "BattleBanzaiTilesLocked":
                    {
                        return this.Habbo.GetUserStats().BattleBanzaiTilesLocked;
                    }
                case "GamePlayer": //game logic
                    {
                        return -1;
                    }
                case "CaughtOnIceTag":
                    {
                        return this.Habbo.GetUserStats().CaughtOnIceTag;
                    }
                case "FreezeFigter":
                    {
                        return this.Habbo.GetUserStats().FreezeFigter;
                    }
                case "SkateboardJumper":
                    {
                        return this.Habbo.GetUserStats().SkateboardJumper;
                    }
                case "SkateboardSlider":
                    {
                        return this.Habbo.GetUserStats().SkateboardSlider;
                    }
                case "BattleBanzaiQuester": //quests
                    {
                        return -1;
                    }
                case "FreezeQuester": //quests
                    {
                        return -1;
                    }
                case "BattleBanzaiWinner": //game logic
                    {
                        return -1;
                    }
                case "BattleBallPlayer": //game logic
                    {
                        return -1;
                    }
                case "FreezeWinner": //game logic
                    {
                        return -1;
                    }
                case "FreezePlayer": //game logic
                    {
                        return -1;
                    }
                case "FreezePowerUpCollector":
                    {
                        return this.Habbo.GetUserStats().FreezePowerUpCollector;
                    }
                case "FootballGoalScorer":
                    {
                        return this.Habbo.GetUserStats().FootballGoalScorer;
                    }
                case "FootballGoalHost":
                    {
                        return this.Habbo.GetUserStats().FootballGoalHost;
                    }
                case "GameArcadeOwner":  //game logic
                    {
                        return -1;
                    }
                case "IceRinkBuilder":
                    {
                        return this.Habbo.GetUserStats().IceRinkBuilder;
                    }
                case "StaffPick":
                    {
                        return this.Habbo.GetUserStats().StaffPicks;
                    }
                case "RollerRinkBuilder":
                    {
                        return this.Habbo.GetUserStats().RollerRinkBuilder;
                    }
                case "RoomBuilder":
                    {
                        return this.Habbo.GetUserStats().RoomBuilder;
                    }
                case "FurniCollector":
                    {
                        return this.Habbo.GetUserStats().FurniCollector;
                    }
                case "RoomHost":
                    {
                        return (int)Math.Floor(this.Habbo.GetUserStats().RoomHost / 60); //minutes
                    }
                case "FloorDesigner":
                    {
                        return this.Habbo.GetUserStats().FloorDesigner;
                    }
                case "LandscapeDesigner":
                    {
                        return this.Habbo.GetUserStats().LandscapeDesigner;
                    }
                case "WallDesigner":
                    {
                        return this.Habbo.GetUserStats().WallDesigner;
                    }
                case "BunnyRunBuilder":
                    {
                        return this.Habbo.GetUserStats().BunnyRunBuilder;
                    }
                case "RoomArchitect":
                    {
                        return this.Habbo.GetUserStats().RoomArchitect;
                    }
                case "SnowboardingBuilder":
                    {
                        return this.Habbo.GetUserStats().SnowboardingBuilder;
                    }
                case "IceIceBaby":
                    {
                        return (int)Math.Floor(this.Habbo.GetUserStats().IceIceBaby / 60); //minutes
                    }
                case "RollerDerby":
                    {
                        return (int)Math.Floor(this.Habbo.GetUserStats().RollerDerby / 60); //minutes
                    }
                case "RoomRaider":
                    {
                        return this.Habbo.GetUserStats().RoomRaider;
                    }
                case "MusicPlayer":
                    {
                        return -1;
                    }
                case "MusicCollector":
                    {
                        return -1;
                    }
                case "GuideRequester":
                    {
                        return this.Habbo.GetUserStats().GuideRequester;
                    }
                case "GuideFeedbackGiver":
                    {
                        return this.Habbo.GetUserStats().GuideFeedbackGiver;
                    }
                case "GuideEnrollmentLifetime":
                    {
                        return this.Habbo.GuideEnrollmentTimestamp > 0 ? (int)Math.Floor((TimeUtilies.GetUnixTimestamp() - this.Habbo.GuideEnrollmentTimestamp) / 86400) : 0;
                    }
                case "GuideOnDutyPresence":
                    {
                        return (int)Math.Floor(this.Habbo.GetUserStats().GuideOnDutyPresence / 60); //minutes;
                    }
                case "GuideRecommendation":
                    {
                        return this.Habbo.GetUserStats().GuideRecommendations;
                    }
                case "GuideRequestHandler":
                    {
                        return this.Habbo.GetUserStats().GuideRequestsHandled;
                    }
                case "GuideChatReviewer":
                    {
                        return this.Habbo.GetUserStats().GuideChatsReviewed;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        public void CheckAchievement(string achievement)
        {
            Achievement achievement_ = Skylight.GetGame().GetAchievementManager().GetAchievement(achievement);
            if (achievement_ != null)
            {
                for (int i = 1; i <= achievement_.LevelsCount; i++)
                {
                    if (this.GetAchievementProgress(achievement) >= achievement_.GetLevel(i).ProgressNeeded)
                    {
                        Skylight.GetGame().GetAchievementManager().AddAchievement(this.Habbo.GetSession(), achievement, i);
                    }
                    else
                    {
                        break; //why we would continue?
                    }
                }
            }

            Skylight.GetGame().GetAchievementManager().UpdateAchievement(this.Habbo.GetSession(), achievement); //update it too!
        }
            

        public int GetAchievementLevel(string achievement)
        {
            this.AchievementLevels.TryGetValue(achievement, out int level);
            return level;
        }

        public void AchievementUnlocked(string achievement, int level)
        {
            if (this.AchievementLevels.ContainsKey(achievement))
            {
                this.AchievementLevels[achievement] = level;

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", this.Habbo.ID);
                    dbClient.AddParamWithValue("group", achievement);
                    dbClient.AddParamWithValue("level", level);
                    dbClient.ExecuteQuery("UPDATE user_achievements SET achievement_level = @level WHERE user_id = @userId AND achievement_group = @group LIMIT 1");
                }
            }
            else
            {
                this.AchievementLevels.Add(achievement, level);

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", this.Habbo.ID);
                    dbClient.AddParamWithValue("group", achievement);
                    dbClient.AddParamWithValue("level", level);
                    dbClient.ExecuteQuery("INSERT INTO user_achievements(user_id, achievement_group, achievement_level) VALUES(@userId, @group, @level)");
                }
            }
        }
    }
}
