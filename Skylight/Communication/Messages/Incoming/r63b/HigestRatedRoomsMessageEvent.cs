using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Utilies;
using System.Data;
using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class HigestRatedRoomsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage popularRooms = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            popularRooms.Init(r63bOutgoing.Navigator);
            popularRooms.AppendInt32(2);
            popularRooms.AppendString("");

            DataTable dataTable = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dataTable = dbClient.ReadDataTable("SELECT * FROM rooms WHERE score > 0 AND type = 'private' ORDER BY score DESC LIMIT 50");
            }

            List<RoomData> rooms = new List<RoomData>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    rooms.Add(Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData((uint)dataRow["id"], dataRow));
                }
            }

            popularRooms.AppendInt32(rooms.Count);
            foreach (RoomData room in rooms)
            {
                room.Serialize(popularRooms, false);
            }

            List<PublicItem> itemsThatsNotCategory = Skylight.GetGame().GetNavigatorManager().GetPublicRoomItems().Where(i => i.Type != PublicItemType.CATEGORY).ToList();
            if (itemsThatsNotCategory.Count > 0)
            {
                popularRooms.AppendBoolean(true);
                itemsThatsNotCategory.ElementAt(new Random().Next(0, itemsThatsNotCategory.Count)).Serialize(popularRooms);
            }
            else
            {
                popularRooms.AppendBoolean(false);
            }

            session.SendMessage(popularRooms);
        }
    }
}
