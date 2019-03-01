using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users
{
    public class UserStats
    {
        public readonly uint HabboID;
        public readonly double LoginTimestamp;

        public int DailyRespectPoints;
        public int DailyPetRespectPoints;
        public int AchievementPoints;
        public double OnlineTime;
        public int RespectReceived;
        public int RegularVisitor;
        public int RespectGiven;
        public int GiftsGived;
        public int GiftsReceived;
        public int NotesLeft;
        public int NotesReceived;
        public int FireworksCharger;
        public int EquestrianTrackHost;
        public int HorseJumper;
        public int StableOwner;
        public int PetRespectGiven;
        public int BattleBanzaiTilesLocked;
        public int FootballGoalScorer;
        public int FootballGoalHost;
        public int FreezeFigter;
        public int FreezePowerUpCollector;
        public int SkateboardJumper;
        public int SkateboardSlider;
        public double RoomHost;
        public int RoomRaider;
        public double IceIceBaby;
        public int CaughtOnIceTag;
        public double RollerDerby;
        public int FloorDesigner;
        public int LandscapeDesigner;
        public int WallDesigner;
        public int StaffPicks;
        public int GuideRequester;
        public int GuideFeedbackGiver;
        public double GuideOnDutyPresence;
        public int GuideRecommendations;
        public int GuideRequestsHandled;
        public int GuideChatsReviewed;

        public int RoomBuilder; //SESSION ONLY
        public int FurniCollector; //SESSION ONLY
        public int RoomArchitect; //SESSION ONLY
        public int Equestrian; //SESSION ONLY
        public int IceRinkBuilder; //SESSION ONLY
        public int RollerRinkBuilder; //SESSION ONLY
        public int BunnyRunBuilder; //SESSION ONLY
        public int SnowboardingBuilder; //SESSION ONLY
        public double LastOnlineTimeCheck; //SESSION ONLY;
        public double GuideOnDutyPresenceCheckStart; //SESSION ONLY

        public UserStats(uint ID)
        {
            this.LoginTimestamp = this.LastOnlineTimeCheck = TimeUtilies.GetUnixTimestamp();
            this.HabboID = ID;

            this.DailyRespectPoints = 3;
            this.DailyPetRespectPoints = 3;
            this.AchievementPoints = 0;
            this.OnlineTime = 0.0;
            this.RespectReceived = 0;
            this.RegularVisitor = 1;
            this.RespectGiven = 0;
            this.GiftsGived = 0;
            this.GiftsReceived = 0;
            this.NotesLeft = 0;
            this.NotesReceived = 0;
            this.FireworksCharger = 0;
            this.EquestrianTrackHost = 0;
            this.HorseJumper = 0;
            this.StableOwner = 0;
            this.Equestrian = 0;
            this.PetRespectGiven = 0;
            this.BattleBanzaiTilesLocked = 0;
            this.FootballGoalScorer = 0;
            this.FootballGoalHost = 0;
            this.FreezeFigter = 0;
            this.FreezePowerUpCollector = 0;
            this.SkateboardJumper = 0;
            this.SkateboardSlider = 0;
            this.RoomBuilder = 0;
            this.FurniCollector = 0;
            this.RoomArchitect = 0;
            this.RoomHost = 0.0;
            this.RoomRaider = 0;
            this.IceRinkBuilder = 0;
            this.IceIceBaby = 0;
            this.CaughtOnIceTag = 0;
            this.RollerRinkBuilder = 0;
            this.RollerDerby = 0;
            this.BunnyRunBuilder = 0;
            this.SnowboardingBuilder = 0;
            this.FloorDesigner = 0;
            this.LandscapeDesigner = 0;
            this.WallDesigner = 0;
            this.StaffPicks = 0;
            this.GuideRequester = 0;
            this.GuideFeedbackGiver = 0;
            this.GuideOnDutyPresence = 0;
            this.GuideRecommendations = 0;
            this.GuideRequestsHandled = 0;
            this.GuideChatsReviewed = 0;
        }

        public bool Fill(DataRow dataRow)
        {
            if (dataRow != null)
            {
                this.DailyRespectPoints = (int)dataRow["daily_respect_points"];
                this.DailyPetRespectPoints = (int)dataRow["daily_pet_respect_points"];
                this.AchievementPoints = (int)dataRow["achievement_points"];
                this.OnlineTime = (double)dataRow["online_time"];
                this.RespectReceived = (int)dataRow["respect_received"];
                this.RegularVisitor = (int)dataRow["regular_visitor"];
                this.RespectGiven = (int)dataRow["respect_given"];
                this.GiftsGived = (int)dataRow["gifts_given"];
                this.GiftsReceived = (int)dataRow["gifts_received"];
                this.NotesLeft = (int)dataRow["notes_left"];
                this.NotesReceived = (int)dataRow["notes_received"];
                this.FireworksCharger = (int)dataRow["fireworks_charger"];
                this.EquestrianTrackHost = (int)dataRow["equestrian_track_host"];
                this.HorseJumper = (int)dataRow["horse_jumper"];
                this.StableOwner = (int)dataRow["stable_owner"];
                this.PetRespectGiven = (int)dataRow["pet_respect_given"];
                this.BattleBanzaiTilesLocked = (int)dataRow["battle_banzai_tiles_locked"];
                this.FootballGoalScorer = (int)dataRow["football_goal_scorer"];
                this.FootballGoalHost = (int)dataRow["football_goal_host"];
                this.FreezeFigter = (int)dataRow["freeze_figter"];
                this.FreezePowerUpCollector = (int)dataRow["freeze_power_up_collector"];
                this.SkateboardJumper = (int)dataRow["skateboard_jumper"];
                this.SkateboardSlider = (int)dataRow["skateboard_slider"];
                this.RoomHost = (double)dataRow["room_host"];
                this.RoomRaider = (int)dataRow["room_raider"];
                this.IceIceBaby = (double)dataRow["ice_ice_baby"];
                this.CaughtOnIceTag = (int)dataRow["caught_on_ice_tag"];
                this.RollerDerby = (double)dataRow["roller_derby"];
                this.FloorDesigner = (int)dataRow["floor_designer"];
                this.LandscapeDesigner = (int)dataRow["landscape_designer"];
                this.WallDesigner = (int)dataRow["wall_designer"];
                this.StaffPicks = (int)dataRow["staff_picks"];
                this.GuideRequester = (int)dataRow["guide_requester"];
                this.GuideFeedbackGiver = (int)dataRow["guide_feedback_giver"];
                this.GuideOnDutyPresence = (double)dataRow["guide_on_duty_presence"];
                this.GuideRecommendations = (int)dataRow["guide_recommendations"];
                this.GuideRequestsHandled = (int)dataRow["guide_requests_handled"];
                this.GuideChatsReviewed = (int)dataRow["guide_chats_reviewed"];

                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateOnlineTime()
        {
            double now = TimeUtilies.GetUnixTimestamp();

            this.OnlineTime += now - this.LastOnlineTimeCheck;
            this.LastOnlineTimeCheck = now;
        }

        public void UpdateGuideOnDutyPresence()
        {
            if (this.GuideOnDutyPresenceCheckStart > 0)
            {
                double now = TimeUtilies.GetUnixTimestamp();

                this.GuideOnDutyPresence += now - this.GuideOnDutyPresenceCheckStart;
                this.GuideOnDutyPresenceCheckStart = now;
            }
        }

        public void Disconnect()
        {
            this.UpdateOnlineTime();

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userId", this.HabboID);
                dbClient.AddParamWithValue("dailyRespectPoints", this.DailyRespectPoints);
                dbClient.AddParamWithValue("dailyPetRespectPoints", this.DailyPetRespectPoints);
                dbClient.AddParamWithValue("onlineTime", this.OnlineTime);
                dbClient.AddParamWithValue("respectReceived", this.RespectReceived);
                dbClient.AddParamWithValue("regularVisitor", this.RegularVisitor);
                dbClient.AddParamWithValue("achievementPoints", this.AchievementPoints);
                dbClient.AddParamWithValue("respectGiven", this.RespectGiven);
                dbClient.AddParamWithValue("giftsGiven", this.GiftsGived);
                dbClient.AddParamWithValue("giftsReceived", this.GiftsReceived);
                dbClient.AddParamWithValue("notesLeft", this.NotesLeft);
                dbClient.AddParamWithValue("notesReceived", this.NotesReceived);
                dbClient.AddParamWithValue("fireworksCharger", this.FireworksCharger);
                dbClient.AddParamWithValue("equestrianTrackHost", this.EquestrianTrackHost);
                dbClient.AddParamWithValue("horseJumper", this.HorseJumper);
                dbClient.AddParamWithValue("stableOwner", this.StableOwner);
                dbClient.AddParamWithValue("petRespectGiven", this.PetRespectGiven);
                dbClient.AddParamWithValue("battleBanzaiTilesLocked", this.BattleBanzaiTilesLocked);
                dbClient.AddParamWithValue("footballGoalScorer", this.FootballGoalScorer);
                dbClient.AddParamWithValue("footballGoalHost", this.FootballGoalHost);
                dbClient.AddParamWithValue("freezeFigter", this.FreezeFigter);
                dbClient.AddParamWithValue("freezePowerUpCollector", this.FreezePowerUpCollector);
                dbClient.AddParamWithValue("skateboardJumper", this.SkateboardJumper);
                dbClient.AddParamWithValue("skateboardSlider", this.SkateboardSlider);
                dbClient.AddParamWithValue("roomHost", this.RoomHost);
                dbClient.AddParamWithValue("roomRaider", this.RoomRaider);
                dbClient.AddParamWithValue("iceIceBaby", this.IceIceBaby);
                dbClient.AddParamWithValue("caughtOnIceTag", this.CaughtOnIceTag);
                dbClient.AddParamWithValue("rollerDerby", this.RollerDerby);
                dbClient.AddParamWithValue("floorDesigner", this.FloorDesigner);
                dbClient.AddParamWithValue("landscapeDesigner", this.LandscapeDesigner);
                dbClient.AddParamWithValue("wallDesigner", this.WallDesigner);
                dbClient.AddParamWithValue("staffPicks", this.StaffPicks);
                dbClient.AddParamWithValue("guideRequester", this.GuideRequester);
                dbClient.AddParamWithValue("guideFeedbackGiver", this.GuideFeedbackGiver);
                dbClient.AddParamWithValue("guideOnDutyPresence", this.GuideOnDutyPresence);
                dbClient.AddParamWithValue("guideRecommendations", this.GuideRecommendations);
                dbClient.AddParamWithValue("guideRequestsHandled", this.GuideRequestsHandled);
                dbClient.AddParamWithValue("guideChatsReviewed", this.GuideChatsReviewed);

                dbClient.ExecuteQuery("UPDATE user_stats SET daily_respect_points = @dailyRespectPoints, daily_pet_respect_points = @dailyPetRespectPoints, achievement_points = @achievementPoints, " +
                "online_time = @onlineTime, respect_received = @respectReceived, regular_visitor = @regularVisitor, respect_given = @respectGiven, gifts_given = @giftsGiven, gifts_received = @giftsReceived, " +
                "notes_left = @notesLeft, notes_received = @notesReceived, fireworks_charger = @fireworksCharger, equestrian_track_host = @equestrianTrackHost, horse_jumper = @horseJumper, " +
                "stable_owner = @stableOwner, pet_respect_given = @petRespectGiven, battle_banzai_tiles_locked = @battleBanzaiTilesLocked, football_goal_scorer = @footballGoalScorer, " +
                "football_goal_host = @footballGoalHost, freeze_figter = @freezeFigter, freeze_power_up_collector = @freezePowerUpCollector, skateboard_jumper = @skateboardJumper, " +
                "skateboard_slider = @skateboardSlider, room_host = @roomHost, room_raider = @roomRaider, ice_ice_baby = @iceIceBaby, caught_on_ice_tag = @caughtOnIceTag, roller_derby = @rollerDerby, " +
                "floor_designer = @floorDesigner, landscape_designer = @landscapeDesigner, wall_designer = @wallDesigner, staff_picks = @staffPicks, guide_requester = @guideRequester, " +
                "guide_feedback_giver = @guideFeedbackGiver, guide_on_duty_presence = @guideOnDutyPresence, guide_recommendations = @guideRecommendations, guide_requests_handled = @guideRequestsHandled, " + "" +
                "guide_chats_reviewed = @guideChatsReviewed WHERE user_id = @userId");
            }
        }
    }
}
