using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomItemHorseObstacle : RoomItem
    {
        public RoomItemHorseObstacle(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnWalkOn(RoomUnit user)
        {
            RoomPet pet = user as RoomPet;
            if (pet != null && pet.Rider != null)
            {
                if (this.IsMiddlePart(pet.X, pet.Y))
                {
                    pet.JumpStatus = HorseJumpStatus.JUMPING;
                    pet.LastJump = RandomUtilies.GetRandom(1, 4);

                    this.ExtraData = pet.LastJump.ToString();
                    this.UpdateState(false, true);
                    this.DoUpdate(2);
                }
                else
                {
                    pet.JumpStatus = HorseJumpStatus.ABOUT_TO_JUMP;

                    this.ExtraData = "5";
                    this.UpdateState(false, true);
                }
            }
        }

        public override void OnWalkOff(RoomUnit user)
        {
            if (user is RoomPet pet && pet.Rider is RoomUnitUser user_)
            {
                if (this.IsMiddlePart(pet.X, pet.Y))
                {
                    if (pet.JumpStatus == HorseJumpStatus.JUMPING)
                    {
                        if (pet.PetData.Energy >= 3)
                        {
                            this.Room.EquestrianTrackHost(1);

                            user_.Session.GetHabbo().GetUserStats().HorseJumper++;
                            user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("HorseJumper");

                            if (pet.LastJump == 4)
                            {
                                user_.Session.GetHabbo().GetUserStats().Equestrian++;
                            }
                            else
                            {
                                user_.Session.GetHabbo().GetUserStats().Equestrian = 0;
                            }
                            user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("Equestrian");

                            pet.PetData.Energy -= 3;
                            pet.AddExpirience(3, false);
                        }
                    }
                }
            }
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
        }

        public bool IsMiddlePart(int x, int y)
        {
            if (this.Rot == 0 || this.Rot == 2)
            {
                if ((this.X + 1 == x && this.Y == y) || (this.X + 1 == x && this.Y + 1 == y))
                {
                    return true;
                }
            }
            else if (this.Rot == 4)
            {
                if ((this.X == x && this.Y + 1 == y) || (this.X + 1 == x && this.Y + 1 == y))
                {
                    return true;
                }
            }

            return false;
        }

        public override void OnLoad()
        {
            this.ExtraData = "5";
        }

        public override void OnPickup(GameClient session)
        {
            this.ExtraData = "5";
        }

        public override void OnPlace(GameClient session)
        {
            this.ExtraData = "5";
        }

        public override void OnCycle()
        {
            if (this.UpdateNeeded)
            {
                this.UpdateTimer--;
                if (this.UpdateTimer <= 0)
                {
                    this.UpdateNeeded = false;

                    this.ExtraData = "5";
                    this.UpdateState(false, true);
                }
            }
        }
    }
}
