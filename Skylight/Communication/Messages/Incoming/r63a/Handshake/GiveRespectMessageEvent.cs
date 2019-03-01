using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GiveRespectMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().GetUserStats().DailyRespectPoints > 0)
            {
                Room room = session.GetHabbo().GetRoomSession().GetRoom();
                if (room != null)
                {
                    uint userId = message.PopWiredUInt();
                    RoomUnitUser target = room.RoomUserManager.GetUserByID(userId);
                    if (target != null && target.Session.GetHabbo().ID != session.GetHabbo().ID)
                    {
                        session.GetHabbo().GetUserStats().DailyRespectPoints--;
                        session.GetHabbo().GetUserStats().RespectGiven++;
                        target.Session.GetHabbo().GetUserStats().RespectReceived++;

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("giverId", session.GetHabbo().ID);
                            dbClient.AddParamWithValue("giverRespectsGiven", session.GetHabbo().GetUserStats().RespectGiven);
                            dbClient.AddParamWithValue("giverDailyRespects", session.GetHabbo().GetUserStats().DailyRespectPoints);
                            dbClient.ExecuteQuery("UPDATE user_stats SET daily_respect_points = @giverDailyRespects, respect_given = @giverRespectsGiven WHERE user_id = @giverId LIMIT 1");

                            dbClient.AddParamWithValue("receiverId", target.Session.GetHabbo().ID);
                            dbClient.AddParamWithValue("receiverRespects", target.Session.GetHabbo().GetUserStats().RespectReceived);
                            dbClient.ExecuteQuery("UPDATE user_stats SET respect_received = @receiverRespects WHERE user_id = @receiverId LIMIT 1");
                        }

                        room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.GiveRespect, new ValueHolder("UserID", target.Session.GetHabbo().ID, "Total", target.Session.GetHabbo().GetUserStats().RespectReceived)));

                        session.GetHabbo().GetUserAchievements().CheckAchievement("RespectGiven");
                        target.Session.GetHabbo().GetUserAchievements().CheckAchievement("RespectReceived");
                    }
                }
            }
        }
    }
}
