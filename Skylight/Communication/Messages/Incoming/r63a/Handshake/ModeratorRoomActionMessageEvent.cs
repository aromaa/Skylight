using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class ModeratorRoomActionMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("acc_supporttool"))
            {
                uint roomId = message.PopWiredUInt();
                Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(roomId);
                if (room != null)
                {
                    bool lockRoom = message.PopWiredBoolean();
                    bool changeName = message.PopWiredBoolean();
                    bool kickUsers = message.PopWiredBoolean();

                    if (lockRoom)
                    {
                        room.RoomData.State = RoomStateType.LOCKED;

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("roomId", roomId);
                            dbClient.ExecuteQuery("UPDATE rooms SET state = 'locked' WHERE id = @roomId LIMIT 1");
                        }
                    }

                    if (changeName)
                    {
                        room.RoomData.Name = "Inappropriate to hotel managment";
                        room.RoomData.Description = "Inappropriate to hotel managment";
                        room.RoomData.Tags.Clear();

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("roomId", roomId);
                            dbClient.AddParamWithValue("roomName", room.RoomData.Name);
                            dbClient.AddParamWithValue("roomDesc", room.RoomData.Description);
                            dbClient.ExecuteQuery("UPDATE rooms SET name = @roomName, description = @roomDesc, tags = '' WHERE id = @roomId LIMIT 1");
                        }
                    }

                    if (kickUsers)
                    {
                        foreach (RoomUnitUser user in room.RoomUserManager.GetRealUsers())
                        {
                            if (user.Session.GetHabbo().Rank < session.GetHabbo().Rank)
                            {
                                room.RoomUserManager.KickUser(user.Session, false);
                            }
                        }
                    }
                }
            }
        }
    }
}
