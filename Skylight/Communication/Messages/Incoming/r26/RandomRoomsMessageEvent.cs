using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class RandomRoomsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int needLoadRooms = 9 - Skylight.GetGame().GetRoomManager().LoadedRoomData.Count; //9 loaded, 3 shown, this should be bit random
            if (needLoadRooms > 0)
            {
                DataTable rooms = null;
                using (DatabaseClient client = Skylight.GetDatabaseManager().GetClient())
                {
                    rooms = client.ReadDataTable("SELECT * FROM rooms ORDER BY RAND() LIMIT " + needLoadRooms);
                }

                if (rooms != null && rooms.Rows.Count > 0)
                {
                    foreach(DataRow dataRow in rooms.Rows)
                    {
                        //Skylight.GetGame().GetRoomManager().LoadRoomData((uint)dataRow["id"], dataRow);
                    }
                }
            }

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message_.Init(r26Outgoing.GetRandomRooms);
            message_.AppendInt32(Math.Min(3, Skylight.GetGame().GetRoomManager().LoadedRoomData.Count)); //count
            foreach (RoomData roomData in Skylight.GetGame().GetRoomManager().LoadedRoomData.Values.OrderBy(r => RandomUtilies.GetRandom(int.MinValue, int.MaxValue - 1)))
            {
                message_.AppendUInt(roomData.ID);
                message_.AppendString(roomData.Name);
                message_.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(roomData.OwnerID));
                message_.AppendString(roomData.State == RoomStateType.OPEN ? "open" : roomData.State == RoomStateType.LOCKED ? "closed" : "password");
                message_.AppendInt32(roomData.UsersNow);
                message_.AppendInt32(roomData.UsersMax);
                message_.AppendString(roomData.Description);
            }
            session.SendMessage(message_);
        }
    }
}
