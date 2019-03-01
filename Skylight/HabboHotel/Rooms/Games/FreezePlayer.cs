using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Games
{
    public class FreezePlayer
    {
        public readonly Room Room;
        public readonly RoomUnitUser User;
        public readonly GameTeam Team;
        public FreezeBallType BallType;
        public int Lives;
        public int Balls;
        public int Range;
        public int ShiedTimer;
        public int FreezeTimer;
        public bool Freezed;
        public bool Shield;

        public FreezePlayer(Room room, RoomUnitUser user, GameTeam team)
        {
            this.Room = room;
            this.User = user;
            this.Team = team;
            this.Reset();
        }

        public void Reset()
        {
            this.BallType = FreezeBallType.Normal;
            this.Lives = 3;
            this.Balls = 1;
            this.Range = 2;
            this.Freezed = false;
            this.Shield = false;
            this.ShiedTimer = 0;
            this.FreezeTimer = 0;
        }

        public void ActiveShield()
        {
            this.Shield = true;
            this.User.ApplyEffect(48 + (int)this.Team);
            this.ShiedTimer += 50;
        }

        public void GiveHelmet()
        {
            this.User.ApplyEffect(39 + (int)this.Team);
        }

        public void ShowLives()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.FreezeLives);
            message.AppendInt32(this.User.VirtualID);
            message.AppendInt32(this.Lives);
            this.Room.SendToAll(message);
        }

        public void Freeze()
        {
            this.Freezed = true;
            this.FreezeTimer = 50;
            this.User.RestrictMovementType |= RestrictMovementType.Server;
            this.Lives--;
            this.ShowLives();
            this.User.ApplyEffect(12);
            this.Room.RoomFreezeManager.UpdateScoreboards();
        }

        public void Cycle()
        {
            if (this.Freezed)
            {
                this.FreezeTimer--;
                if (this.FreezeTimer <= 0)
                {
                    this.Freezed = false;
                    this.User.RestrictMovementType &= ~RestrictMovementType.Server;

                    if (this.Lives <= 0)
                    {
                        this.Room.RoomFreezeManager.LeaveGame(this, true);
                    }
                    else
                    {
                        this.ActiveShield();
                    }
                }
            }

            if (this.Shield)
            {
                this.ShiedTimer--;
                if (this.ShiedTimer <= 0)
                {
                    this.Shield = false;
                    this.GiveHelmet();
                }
            }
        }
    }
}
