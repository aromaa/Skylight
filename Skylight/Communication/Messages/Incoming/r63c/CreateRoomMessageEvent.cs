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

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
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
                    string roomDescription = message.PopFixedString();
                    string roomModel = message.PopFixedString();
                    int categoryId = message.PopWiredInt32();
                    int maxUsers = message.PopWiredInt32();
                    int tradeType = message.PopWiredInt32();

                    RoomData roomData = Skylight.GetGame().GetRoomManager().CreateRoom(session, roomName, roomDescription, roomModel, categoryId, maxUsers, tradeType, RoomStateType.OPEN);
                    if (roomData != null)
                    {
                        ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
                        Message.Init(r63cOutgoing.RoomCreated);
                        Message.AppendUInt(roomData.ID);
                        Message.AppendString(roomData.Name);
                        session.SendMessage(Message);
                    }
                }
            }
        }
    }
}
