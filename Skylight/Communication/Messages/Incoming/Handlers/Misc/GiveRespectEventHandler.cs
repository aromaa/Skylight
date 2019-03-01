using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Storage;
using SkylightEmulator.Core;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class GiveRespectEventHandler : IncomingPacket
    {
        protected uint UserID;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetUserStats()?.DailyRespectPoints > 0)
            {
                RoomUnitUser target = session?.GetHabbo()?.GetRoomSession()?.GetRoom()?.RoomUserManager?.GetUserByID(this.UserID);
                if (target != null && target.Session.GetHabbo().ID != session.GetHabbo().ID)
                {
                    session.GetHabbo().GetRoomSession().GetRoomUser().Unidle();

                    session.GetHabbo().GetUserStats().DailyRespectPoints--;
                    session.GetHabbo().GetUserStats().RespectGiven++;
                    session.GetHabbo().GetUserAchievements().CheckAchievement("RespectGiven");

                    target.Session.GetHabbo().GetUserStats().RespectReceived++;
                    target.Session.GetHabbo().GetUserAchievements().CheckAchievement("RespectReceived");

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("giverId", session.GetHabbo().ID);
                        dbClient.AddParamWithValue("giverRespectsGiven", session.GetHabbo().GetUserStats().RespectGiven);
                        dbClient.AddParamWithValue("giverDailyRespects", session.GetHabbo().GetUserStats().DailyRespectPoints);
                        dbClient.AddParamWithValue("receiverId", target.Session.GetHabbo().ID);
                        dbClient.AddParamWithValue("receiverRespects", target.Session.GetHabbo().GetUserStats().RespectReceived);
                        dbClient.ExecuteQuery("UPDATE user_stats SET daily_respect_points = @giverDailyRespects, respect_given = @giverRespectsGiven WHERE user_id = @giverId LIMIT 1; UPDATE user_stats SET respect_received = @receiverRespects WHERE user_id = @receiverId LIMIT 1;");
                    }

                    session.GetHabbo().GetRoomSession().GetRoom().SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.GiveRespect, new ValueHolder("UserID", this.UserID, "Total", target.Session.GetHabbo().GetUserStats().RespectReceived)));
                    session.GetHabbo().GetRoomSession().GetRoom().SendToAll(new UserActionComposerHandler(this.UserID, ActionType.Thumb));
                }
            }
        }
    }
}
