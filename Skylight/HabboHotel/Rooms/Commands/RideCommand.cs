using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Pets;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RideCommand : Command
    {
        public override string CommandInfo()
        {
            return ":ride [horse] - Ride on horse, wohoo!";
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
            if (args.Length >= 2)
            {
                RoomPet pet = session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.GetPetByName(args[1]);
                if (pet != null)
                {
                    if (pet.Rider == null)
                    {
                        if (pet.PetData.Type == 13)
                        {
                            RoomUnitUser me = session.GetHabbo().GetRoomSession().GetRoomUser();
                            bool doit = true;
                            if ((me.X + 1 != pet.X || me.Y != pet.Y) && (me.X - 1 != pet.X || me.Y != pet.Y) && (me.Y + 1 != pet.Y || me.X != pet.X))
                            {
                                bool skip = false;
                                if (me.X - 1 == pet.X)
                                {
                                    if (me.Y == pet.Y)
                                    {
                                        skip = true;
                                    }
                                }

                                if (!skip)
                                {
                                    doit = me.X == pet.X || me.Y == pet.Y;
                                }
                            }

                            if (doit)
                            {
                                pet.Rider = me;

                                me.Riding = pet;
                                me.StopMoving();
                                me.SetLocation(pet.X, pet.Y, pet.Z + 1);
                                me.ApplyEffect(77);

                                session.SendNotif("If you want get off use command :getoff");

                                GameClient owner = Skylight.GetGame().GetGameClientManager().GetGameClientById(pet.PetData.OwnerID);
                                if (owner != null)
                                {
                                    owner.GetHabbo().GetUserStats().StableOwner++;
                                    owner.GetHabbo().GetUserAchievements().CheckAchievement("StableOwner");
                                }
                                else
                                {
                                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                                    {
                                        dbClient.AddParamWithValue("ownerId", pet.PetData.OwnerID);
                                        dbClient.ExecuteQuery("UPDATE user_stats SET stable_owner = stable_owner + 1 WHERE user-id = @ownerId LIMIT 1");
                                    }
                                }

                                me.Session.GetHabbo().GetUserStats().Equestrian = 0;
                            }
                            else
                            {
                                session.SendNotif("Ah.. you try to access the horse from long range");
                            }
                        }
                        else
                        {
                            session.SendNotif("You blind or something m8? You can clearly see that pet is not a horse!");
                        }
                    }
                    else
                    {
                        session.SendNotif("I think someone is riding that horse already...");
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
