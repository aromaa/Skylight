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

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class CreateRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                if (session.GetHabbo().UserRooms.Count <= ServerConfiguration.MaxRoomsPerUser)
                {
                    string roomName = message.PopFixedString();
                    string roomModel = message.PopFixedString();
                    RoomData roomData = Skylight.GetGame().GetRoomManager().CreateRoom(session, roomName, "", roomModel, 0, 25, 0, RoomStateType.OPEN);
                    if (roomData != null)
                    {
                        ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                        Message.Init(r63bOutgoing.FlatRoomCreated);
                        Message.AppendUInt(roomData.ID);
                        Message.AppendString(roomData.Name);
                        session.SendMessage(Message);
                    }
                }
            }
        }
    }
}
