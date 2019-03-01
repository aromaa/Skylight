using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class RespectPetMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                uint petId = message.PopWiredUInt();
                RoomPet pet = room.RoomUserManager.GetPetByID(petId);
                if (pet != null && pet.PetData != null && session.GetHabbo().GetUserStats().DailyPetRespectPoints > 0)
                {
                    session.GetHabbo().GetUserStats().DailyPetRespectPoints--;
                    session.GetHabbo().GetUserStats().PetRespectGiven++;
                    pet.OnRespect();

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                        dbClient.AddParamWithValue("petRespect", session.GetHabbo().GetUserStats().DailyPetRespectPoints);
                        dbClient.AddParamWithValue("petRespectGiven", session.GetHabbo().GetUserStats().PetRespectGiven);
                        dbClient.ExecuteQuery("UPDATE user_stats SET daily_pet_respect_points = @petRespect, pet_respect_given = @petRespectGiven WHERE user_id = @userId LIMIT 1");
                    }

                    session.GetHabbo().GetUserAchievements().CheckAchievement("PetRespectGiven");
                }
            }
        }
    }
}
