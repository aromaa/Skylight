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
    class GetRoomSettingsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
                if (room != null && room.HaveOwnerRights(session))
                {
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
                    message_.Init(r63cOutgoing.RoomSettings);
                    message_.AppendUInt(room.ID);
                    message_.AppendString(room.RoomData.Name);
                    message_.AppendString(room.RoomData.Description);
                    message_.AppendInt32((int)room.RoomData.State);
                    message_.AppendInt32(room.RoomData.Category);
                    message_.AppendInt32(room.RoomData.UsersMax);
                    message_.AppendInt32(0);
                    message_.AppendInt32(room.RoomData.Tags.Count);
                    foreach(string tag in room.RoomData.Tags)
                    {
                        message_.AppendString(tag);
                    }
                    message_.AppendInt32(0); //Trade
                    message_.AppendInt32(room.RoomData.AllowPets ? 1 : 0);
                    message_.AppendInt32(room.RoomData.AllowPetsEat ? 1 : 0);
                    message_.AppendInt32(room.RoomData.AllowWalkthrough ? 1 : 0);
                    message_.AppendInt32(room.RoomData.Hidewalls ? 1 : 0);
                    message_.AppendInt32(room.RoomData.Wallthick);
                    message_.AppendInt32(room.RoomData.Floorthick);

                    message_.AppendInt32((int)room.RoomData.ChatMode); //Chat mode
                    message_.AppendInt32((int)room.RoomData.ChatWeight); //Chat weight
                    message_.AppendInt32((int)room.RoomData.ChatSpeed); //Chat speed
                    message_.AppendInt32(room.RoomData.ChatDistance); //Chat distance
                    message_.AppendInt32((int)room.RoomData.ChatProtection); //Chat protection

                    message_.AppendBoolean(false); //Mute all button

                    message_.AppendInt32((int)room.RoomData.MuteOption); //Mute option
                    message_.AppendInt32((int)room.RoomData.KickOption); //Kick option
                    message_.AppendInt32((int)room.RoomData.BanOption); //Ban option
                    session.SendMessage(message_);
                }
            }
        }
    }
}
