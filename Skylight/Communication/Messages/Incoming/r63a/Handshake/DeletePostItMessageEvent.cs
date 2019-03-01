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
    class DeletePostItMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.HaveOwnerRights(session))
            {
                uint roomId = message.PopWiredUInt();
                RoomItem item = room.RoomItemManager.TryGetRoomItem(roomId);
                if (item != null && item.GetBaseItem().InteractionType.ToLower() == "postit")
                {
                    room.RoomItemManager.RemoveItem(session, item);

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("itemId", item.ID);
                        dbClient.ExecuteQuery("DELETE FROM items WHERE id = @itemId LIMIT 1");
                    }
                }
            }
        }
    }
}
