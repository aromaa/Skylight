using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class RateRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && !room.HaveOwnerRights(session) && !session.GetHabbo().RatedRooms.Contains(room.ID))
            {
                int rating = message.PopWiredInt32();
                if (rating == 1)
                {
                    room.RoomData.Score++;
                }
                else if (rating == 0)
                {
                    //idk how
                    return;
                }
                else if (rating == -1)
                {
                    //sctiper, dislikes not avaible anymore
                    return;
                }

                session.GetHabbo().RatedRooms.Add(room.ID);

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("roomId", room.ID);
                    dbClient.AddParamWithValue("score", room.RoomData.Score);
                    dbClient.ExecuteQuery("UPDATE rooms SET score = @score WHERE id = @roomId LIMIT 1");

                    if (session.GetHabbo().GetUserSettings().FriendStream)
                    {
                        dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                        dbClient.ExecuteQuery("INSERT INTO user_friend_stream(type, user_id, timestamp, extra_data) VALUES('1', @userId, UNIX_TIMESTAMP(), @roomId)");
                    }
                }

                ServerMessage roomRating = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                roomRating.Init(r63aOutgoing.RoomRating);
                roomRating.AppendInt32(room.RoomData.Score);
                session.SendMessage(roomRating);
            }
        }
    }
}
