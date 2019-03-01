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
    class GetRoomSettingsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
                if (room != null && room.HaveOwnerRights(session))
                {
                    ServerMessage roomSettings = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    roomSettings.Init(r63aOutgoing.RoomSettings);
                    roomSettings.AppendUInt(room.ID);
                    roomSettings.AppendString(room.RoomData.Name);
                    roomSettings.AppendString(room.RoomData.Description);
                    roomSettings.AppendInt32((int)room.RoomData.State);
                    roomSettings.AppendInt32(room.RoomData.Category);
                    roomSettings.AppendInt32(room.RoomData.UsersMax);
                    roomSettings.AppendInt32(100);
                    roomSettings.AppendInt32(room.RoomData.Tags.Count);
                    foreach (string current in room.RoomData.Tags)
                    {
                        roomSettings.AppendString(current);
                    }

                    roomSettings.AppendInt32(room.UsersWithRights.Count);
                    foreach (uint userId in room.UsersWithRights)
                    {
                        roomSettings.AppendUInt(userId);
                        roomSettings.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(userId));
                    }

                    roomSettings.AppendInt32(room.UsersWithRights.Count);
                    roomSettings.AppendBoolean(room.RoomData.AllowPets);
                    roomSettings.AppendBoolean(room.RoomData.AllowPetsEat);
                    roomSettings.AppendBoolean(room.RoomData.AllowWalkthrough);
                    roomSettings.AppendBoolean(room.RoomData.Hidewalls);
                    roomSettings.AppendInt32(room.RoomData.Wallthick);
                    roomSettings.AppendInt32(room.RoomData.Floorthick);
                    session.SendMessage(roomSettings);
                }
            }
        }
    }
}
