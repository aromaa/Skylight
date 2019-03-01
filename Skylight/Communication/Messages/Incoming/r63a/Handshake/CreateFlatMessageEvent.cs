using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class CreateFlatMessageEvent : IncomingPacket
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
                        ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        Message.Init(r63aOutgoing.FlatRoomCreated);
                        Message.AppendUInt(roomData.ID);
                        Message.AppendString(roomData.Name);
                        session.SendMessage(Message);
                    }
                }
            }
        }
    }
}
