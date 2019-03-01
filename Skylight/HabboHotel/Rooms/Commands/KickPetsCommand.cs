using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class KickPetsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":kickpets - Kicks every pet, even yours";
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().GetRoomSession().GetRoom().HaveOwnerRights(session))
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    string query = "";

                    foreach (BotAI bot in session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.GetBots())
                    {
                        try
                        {
                            RoomPet pet = bot as RoomPet;
                            if (pet != null)
                            {
                                session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.LeaveRoom(pet);

                                GameClient petOwner = Skylight.GetGame().GetGameClientManager().GetGameClientById(pet.PetData.OwnerID);
                                if (petOwner != null)
                                {
                                    petOwner.GetHabbo().GetInventoryManager().AddPet(pet.PetData);
                                    pet.PetData.NeedUpdate = true;
                                    session.GetHabbo().GetInventoryManager().SavePetData();
                                }
                                else
                                {

                                    dbClient.AddParamWithValue("petId" + pet.PetData.ID, pet.PetData.ID);
                                    dbClient.AddParamWithValue("expirience" + pet.PetData.ID, pet.PetData.Expirience);
                                    dbClient.AddParamWithValue("energy" + pet.PetData.ID, pet.PetData.Energy);
                                    dbClient.AddParamWithValue("happiness" + pet.PetData.ID, pet.PetData.Happiness);
                                    dbClient.AddParamWithValue("respect" + pet.PetData.ID, pet.PetData.Respect);

                                    query += "UPDATE user_pets SET room_id = '0', expirience = @expirience" + pet.PetData.ID + ", energy = @energy" + pet.PetData.ID + ", happiness = @happiness" + pet.PetData.ID + ", respect = @respect" + pet.PetData.ID + ", x = '0', y = '0', z = '0' WHERE id = @petId" + pet.PetData.ID + " LIMIT 1; ";
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                    if (query.Length > 0)
                    {
                        dbClient.ExecuteQuery(query);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
