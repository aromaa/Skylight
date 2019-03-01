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
    class GetPetInfoMessageEvent : IncomingPacket
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
                    message_.Init(r63aOutgoing.PetInfo);
                    message_.AppendUInt(pet.PetData.ID);
                    message_.AppendString(pet.PetData.Name);
                    message_.AppendInt32(pet.PetData.Level);
                    message_.AppendInt32(pet.PetData.MaxLevel);
                    message_.AppendInt32(pet.PetData.Expirience);
                    message_.AppendInt32(pet.PetData.ExpirienceGoal);
                    message_.AppendInt32(pet.PetData.Energy);
                    message_.AppendInt32(pet.PetData.MaxEnergy);
                    message_.AppendInt32(pet.PetData.Happiness);
                    message_.AppendInt32(pet.PetData.MaxHappiness);
                    message_.AppendString(pet.PetData.Look.ToLower());
                    message_.AppendInt32(pet.PetData.Respect);
                    message_.AppendUInt(pet.PetData.OwnerID);
                    message_.AppendInt32(pet.PetData.Age);
                    message_.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(pet.PetData.OwnerID));
                    session.SendMessage(message_);
                }
            }
        }
    }
}
