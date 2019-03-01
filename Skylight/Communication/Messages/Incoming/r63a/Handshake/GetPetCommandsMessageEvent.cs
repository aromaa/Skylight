using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetPetCommandsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                uint petId = message.PopWiredUInt();
                RoomPet pet = room.RoomUserManager.GetPetByID(petId);
                if (pet != null)
                {
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.PetCommands);
                    message_.AppendUInt(petId);
                    message_.AppendInt32(18); //commands count
                    message_.AppendInt32(0); //free
                    message_.AppendInt32(1); //sit
                    message_.AppendInt32(2); //down
                    message_.AppendInt32(3); //here
                    message_.AppendInt32(4); //beg
                    message_.AppendInt32(17); //play football
                    message_.AppendInt32(5); //play dead
                    message_.AppendInt32(6); //stay
                    message_.AppendInt32(7); //follow
                    message_.AppendInt32(8); //stand
                    message_.AppendInt32(9); //jump
                    message_.AppendInt32(10); //speak
                    message_.AppendInt32(11); //play
                    message_.AppendInt32(12); //silent
                    message_.AppendInt32(13); //nest
                    message_.AppendInt32(14); //drink
                    message_.AppendInt32(15); //follow left
                    message_.AppendInt32(16); //follow right

                    message_.AppendInt32(18); //commands avaible
                    message_.AppendInt32(0); //free
                    message_.AppendInt32(1); //sit
                    message_.AppendInt32(2); //down
                    message_.AppendInt32(3); //here
                    message_.AppendInt32(4); //beg
                    message_.AppendInt32(17); //play football
                    message_.AppendInt32(5); //play dead
                    message_.AppendInt32(6); //stay
                    message_.AppendInt32(7); //follow
                    message_.AppendInt32(8); //stand
                    message_.AppendInt32(9); //jump
                    message_.AppendInt32(10); //speak
                    message_.AppendInt32(11); //play
                    message_.AppendInt32(12); //silent
                    message_.AppendInt32(13); //nest
                    message_.AppendInt32(14); //drink
                    message_.AppendInt32(15); //follow left
                    message_.AppendInt32(16); //follow right
                    session.SendMessage(message_);
                }
            }
        }
    }
}
