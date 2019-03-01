using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.HabboHotel.Pets;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Bots
{
    public class RoomPet : RoomUnit, BotAI
    {
        public Pet PetData;
        private int NextActionTick;
        private int NextEnergyTick;
        private int ActionTick;
        private string Action;
        public HorseJumpStatus JumpStatus;
        public int LastJump;

        public RoomPet(Pet pet, Room room, int virtualId) : base(room, virtualId)
        {
            this.PetData = pet;

            this.NextActionTick = this.GetRandom().Next(10, 60);
            this.NextEnergyTick = this.EnergyTick;
            this.JumpStatus = HorseJumpStatus.NONE;
        }

        public Random GetRandom()
        {
            return new Random((this.VirtualID ^ 2) + DateTime.Now.Millisecond);
        }

        public int EnergyTick
        {
            get
            {
                return 30;
            }
        }

        public override void Serialize(ServerMessage message)
        {
            message.AppendUInt(this.PetData.ID);
            message.AppendString(this.PetData.Name);
            message.AppendString(""); //pets dont have motto xD
            message.AppendString(this.PetData.Look);
            message.AppendInt32(this.VirtualID);
            message.AppendInt32(this.X);
            message.AppendInt32(this.Y);
            message.AppendString(TextUtilies.DoubleWithDotDecimal(this.Z));
            message.AppendInt32(2); //dir
            message.AppendInt32(2);
            message.AppendInt32(int.Parse(this.PetData.Race));
        }

        public void OnSelfEnterRoom()
        {

        }

        public void OnSelfLeaveRoom(bool kicked)
        {

        }

        public void OnUserLeaveRoom(RoomUnitUser user)
        {
            if (!this.Room.HaveOwnerRights(user.Session))
            {
                if (user.UserID == this.PetData.OwnerID)
                {
                    this.Room.RoomUserManager.LeaveRoom(this);
                    user.Session.GetHabbo().GetInventoryManager().AddPet(this.PetData);
                }
            }

            if (this.Rider != null && this.Rider.VirtualID == user.VirtualID)
            {
                this.Rider = null;
            }
        }

        public void EndAction()
        {
            this.ActionTick = 0;

            if (this.Action == "sit")
            {
                this.RemoveStatus("sit");
            }

            this.Action = "";
        }

        public void OnUserSpeak(RoomUnitUser user, string message, bool shout)
        {
            if (!shout)
            {
                if (message == this.PetData.Name)
                {
                    this.SetRotation(WalkRotation.Walk(this.X, this.Y, user.X, user.Y), false);
                }
                else if (message.StartsWith(this.PetData.Name + " ") && user.UserID == this.PetData.OwnerID) //pet commands
                {
                    int random = RandomUtilies.GetRandom(1, 100);
                    if (random <= this.PetData.Happiness)
                    {
                        string command = message.Split(' ')[1];
                        switch (command)
                        {
                            case "free":
                                {
                                    this.EndAction();
                                }
                                break;
                            case "sit":
                                {
                                    if (this.PetData.Energy >= 10)
                                    {
                                        this.AddExpirience(10);
                                        this.PetData.Energy -= 10;
                                        this.PetData.NeedUpdate = true;
                                        this.AddStatus("sit", TextUtilies.DoubleWithDotDecimal(this.Z));

                                        this.Moving = false;
                                        if (this.HasStatus("mv"))
                                        {
                                            this.RemoveStatus("mv");
                                        }

                                        this.ActionTick = 8;
                                        this.Action = "sit";
                                    }
                                    else
                                    {
                                        this.Speak("Too tired! :(", false);
                                    }
                                }
                                break;
                            default:
                                this.Speak("*not yet implemented*", false);
                                break;
                        }
                    }
                    else
                    {
                        this.PetData.Energy -= 10;
                        if (this.PetData.Energy < 0)
                        {
                            this.PetData.Energy = 0;
                        }
                        this.PetData.NeedUpdate = true;

                        this.Speak("NO! >:(", false);
                    }
                }
            }
        }

        public override void Speak(string message, bool shout, int bubble = 0)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            if (shout)
            {
                Message.Init(r63aOutgoing.Shout);
            }
            else
            {
                Message.Init(r63aOutgoing.Say);
            }
            Message.AppendInt32(this.VirtualID);
            Message.AppendString(message);
            Message.AppendInt32(RoomUnit.GetGesture(message));
            Message.AppendInt32(0); //links count
            Message.AppendInt32(0); //unknown
            this.Room.SendToAll(Message);
        }

        public void OnRoomCycle()
        {
            if (this.Rider == null)
            {
                if (this.ActionTick > 0)
                {
                    if (--this.ActionTick <= 0)
                    {
                        this.EndAction();
                    }
                }
                else
                {
                    if (--this.NextActionTick <= 0)
                    {
                        int x = RandomUtilies.GetRandom(0, this.Room.RoomGamemapManager.Model.MaxX);
                        int y = RandomUtilies.GetRandom(0, this.Room.RoomGamemapManager.Model.MaxY);
                        this.MoveTo(x, y);
                        this.PetData.NeedUpdate = true; //bcs we moved
                        
                        this.NextActionTick = this.GetRandom().Next(10, 60);
                    }
                }
            }

            if (--this.NextEnergyTick <= 0)
            {
                if (this.PetData.Energy < this.PetData.MaxEnergy)
                {
                    this.PetData.Energy++;
                }

                this.NextEnergyTick = this.EnergyTick;
            }
        }

        public void OnRespect()
        {
            this.PetData.Respect++;
            this.PetData.Happiness += 10;
            if (this.PetData.Happiness > this.PetData.MaxHappiness)
            {
                this.PetData.Happiness = this.PetData.MaxHappiness;
            }
            this.PetData.NeedUpdate = true;

            this.AddExpirience(10);

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.PetGainRespect);
            message.AppendInt32(this.PetData.Respect);
            message.AppendInt32(0); //not used
            message.AppendUInt(this.PetData.ID);
            message.AppendString(this.PetData.Name);
            message.AppendInt32(this.PetData.Type);
            message.AppendInt32(int.Parse(this.PetData.Race));
            message.AppendString(this.PetData.Color);
            this.Room.SendToAll(message);

            GameClient owner = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.PetData.OwnerID);
            if (owner != null)
            {
                owner.GetHabbo().Pets[this.PetData.ID] = this.PetData;
                owner.GetHabbo().GetUserAchievements().CheckAchievement("PetRespectReceiver");
            }
        }

        public void AddExpirience(int amount, bool showExp = true)
        {
            GameClient owner = null;
            if (this.PetData.Expirience + amount >= this.PetData.ExpirienceGoal && this.PetData.Level != this.PetData.MaxLevel) //level up
            {
                this.Speak("*leveled up to level " + (this.PetData.Level + 1) + "*", false);

                owner = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.PetData.OwnerID);
            }

            this.PetData.Expirience += amount;
            this.PetData.NeedUpdate = true;

            if (owner != null)
            {
                owner.GetHabbo().Pets[this.PetData.ID] = this.PetData;
                owner.GetHabbo().GetUserAchievements().CheckAchievement("PetTrainer");
            }

            if (showExp)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.PetGainExpirience);
                message.AppendUInt(this.PetData.ID);
                message.AppendInt32(this.VirtualID);
                message.AppendInt32(amount);
                this.Room.SendToAll(message);
            }
        }
    }
}
