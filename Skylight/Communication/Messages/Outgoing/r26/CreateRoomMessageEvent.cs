using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class CreateRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                if (session.GetHabbo().UserRooms.Count <= ServerConfiguration.MaxRoomsPerUser)
                {
                    string[] data = message.PopStringUntilBreak(null).Split('/');
                    if (data[0] == "" && data[1] == "first floor")
                    {
                        string roomName = data[2];
                        string roomModel = data[3];
                        RoomStateType state = data[4] == "open" ? RoomStateType.OPEN : data[4] == "closed" ? RoomStateType.LOCKED : RoomStateType.PASSWORD;
                        string showName = data[5];

                        RoomData roomData = Skylight.GetGame().GetRoomManager().CreateRoom(session, roomName, "", roomModel, 0, 25, 0, state);
                        if (roomData != null)
                        {
                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                            message_.Init(r26Outgoing.CreatedRoom);
                            message_.AppendString(roomData.ID.ToString(), 13);
                            message_.AppendString(roomData.Name);
                            session.SendMessage(message_);
                        }
                    }
                }
            }
        }
    }
}
