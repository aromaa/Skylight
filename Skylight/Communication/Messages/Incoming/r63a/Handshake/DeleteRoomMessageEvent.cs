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
    class DeleteRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint roomId = message.PopWiredUInt();

            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.HaveOwnerRights(session))
            {
                Skylight.GetGame().GetRoomManager().UnloadRoom(room);

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("roomId", room.ID);

                    dbClient.ExecuteQuery("DELETE FROM rooms WHERE id = @roomId LIMIT 1");
                    dbClient.ExecuteQuery("DELETE FROM user_favorites WHERE room_id = @roomId");
                    dbClient.ExecuteQuery("DELETE FROM room_rights WHERE room_id = @roomId");
                    dbClient.ExecuteQuery("UPDATE items SET room_id = '0' WHERE room_id = @roomId");
                    dbClient.ExecuteQuery("UPDATE users SET home_room = '0' WHERE home_room = @roomId");
                    dbClient.ExecuteQuery("UPDATE users SET newbie_status = '2', newbie_room = '0' WHERE newbie_room = @roomId");
                    dbClient.ExecuteQuery("UPDATE user_pets SET room_id = '0' WHERE room_id = @roomId");
                }

                session.GetHabbo().LoadRooms();
                session.GetHabbo().GetInventoryManager().UpdateInventoryItems(true);
                session.GetHabbo().GetInventoryManager().UpdateInventryPets(true);
                session.SendMessage(Skylight.GetGame().GetNavigatorManager().GetMyRooms(session));
            }
        }
    }
}
