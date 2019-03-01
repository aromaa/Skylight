using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class PickupPetMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                uint petId = message.PopWiredUInt();
                RoomPet pet = room.RoomUserManager.GetPetByID(petId);
                if (pet != null && pet.PetData.OwnerID == session.GetHabbo().ID)
                {
                    room.RoomUserManager.LeaveRoom(pet);

                    session.GetHabbo().GetInventoryManager().AddPet(pet.PetData);
                    pet.PetData.NeedUpdate = true;
                    session.GetHabbo().GetInventoryManager().SavePetData();
                }
            }
        }
    }
}
