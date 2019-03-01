using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Core;
using SkylightEmulator.Storage;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class ModifyRoomCategoryMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint roomId = message.PopWiredUInt();

            RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId);
            if (roomData != null && roomData.OwnerID == session.GetHabbo().ID)
            {
                int categoryId = message.PopWiredInt32();
                if (Skylight.GetGame().GetNavigatorManager().GetFlatCat(categoryId) != null)
                {
                    roomData.Category = categoryId;

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.ExecuteQuery("UPDATE rooms SET category = '" + categoryId + "' WHERE id = '" + roomId + "' LIMIT 1");
                    }
                }
            }
        }
    }
}
