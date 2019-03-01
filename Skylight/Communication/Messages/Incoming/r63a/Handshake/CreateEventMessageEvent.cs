using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class CreateEventMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
            if (room != null && room.RoomEvent == null && room.HaveOwnerRights(session) && room.RoomData.State == RoomStateType.OPEN && ServerConfiguration.EventsEnabled)
            {
                foreach (uint roomId in session.GetHabbo().UserRooms)
                {
                    Room room_ = Skylight.GetGame().GetRoomManager().TryGetRoom(roomId);
                    if (room_ != null && room_.RoomEvent != null) //if any room events is already going
                    {
                        return;
                    }
                }

                //Create event :)
                int category = message.PopWiredInt32();
                string name = message.PopFixedString();
                string desc = message.PopFixedString();
                if (!string.IsNullOrEmpty(name))
                {
                    List<string> tags = new List<string>();
                    int tagsCount = message.PopWiredInt32();
                    for(int i = 0; i < tagsCount; i++)
                    {
                        tags.Add(message.PopFixedString());
                    }

                    room.RoomEvent = new RoomEvent(room.ID, session.GetHabbo().ID, name, desc, category, tags);
                    room.SendToAll(room.RoomEvent.Serialize());
                }
            }
        }
    }
}
