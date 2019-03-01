using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class GetOwnRoomsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().UserRooms.Count > 0)
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                message_.Init(r26Outgoing.Ownrooms);
                foreach(uint roomId in session.GetHabbo().UserRooms)
                {
                    RoomData data = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId);
                    message_.AppendString(data.ID.ToString(), 9);
                    message_.AppendString(data.Name, 9);
                    message_.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(data.OwnerID), 9);
                    message_.AppendString(data.State == RoomStateType.OPEN ? "open" : data.State == RoomStateType.LOCKED ? "locked" : "password", 9);
                    message_.AppendString("x", 9);
                    message_.AppendString(data.UsersNow.ToString(), 9);
                    message_.AppendString(data.UsersMax.ToString(), 9);
                    message_.AppendString("null", 9);
                    message_.AppendString(data.Description, 9);
                    message_.AppendString(data.Description, 9);
                    message_.AppendString("", 13);
                }
                session.SendMessage(message_);
            }
            else
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                message_.Init(r26Outgoing.NoOwnRooms);
                session.SendMessage(message_);
            }
        }
    }
}
