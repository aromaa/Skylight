using SkylightEmulator.Core;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomUnitHuman : RoomUnit
    {
        public int IdleTimer { get; private set; }
        public bool Sleeps { get; private set; }

        public int Handitem { get; private set; }
        public int HanditemTimer { get; private set; }

        public int EffectID { get; private set; } = -1;
        public int TempEffect { get; private set; } = -1;
        public int TempEffectTimer { get; private set; } = 0;

        public int DanceID { get; set; }
        public bool Dancing => this.DanceID > 0;

        public int ActiveEffect
        {
            get
            {
                return this.TempEffect < 0 ? this.EffectID : this.TempEffect;
            }
        }

        public RoomUnitHuman(Room room, int virtualId) : base(room, virtualId)
        {
        }

        public override void Cycle()
        {
            base.Cycle();

            this.IdleTimer++;
            if (!this.Sleeps)
            {
                if (this.IdleTimer >= ServerConfiguration.IdleTime / 0.5)
                {
                    this.Sleeps = true;

                    this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Idle, new ValueHolder("VirtualID", this.VirtualID, "Sleep", true)));
                }
            }

            if (this.Handitem > 0)
            {
                if (--this.HanditemTimer <= 0)
                {
                    this.SetHanditem(0);
                }
            }

            if (this.TempEffectTimer > 0)
            {
                if (--this.TempEffectTimer <= 0)
                {
                    this.ApplyEffect(this.ActiveEffect);
                }
            }
        }

        public void SetHanditem(int handitem)
        {
            this.Handitem = handitem;

            if (handitem > 1000)
            {
                this.HanditemTimer = 5000;
            }
            else if (handitem > 0)
            {
                this.HanditemTimer = 240;
            }
            else
            {
                this.HanditemTimer = 0;
            }

            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Handitem, new ValueHolder("VirtualID", this.VirtualID, "Handitem", this.Handitem)));
        }

        public void ApplyEffect(int effectId, int tempTimer = 0)
        {
            if (tempTimer <= 0)
            {
                this.EffectID = effectId;

                if (this.TempEffect >= 0)
                {
                    this.TempEffect = -1;
                    this.TempEffectTimer = 0;
                }
            }
            else
            {
                this.TempEffect = effectId;
                this.TempEffectTimer = tempTimer;
            }

            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Effect, new ValueHolder("VirtualID", this.VirtualID, "EffectID", effectId)));
        }

        public void Unidle()
        {
            this.IdleTimer = 0;
            if (this.Sleeps)
            {
                this.SetIdleStatus(false);
            }
        }

        public void SetIdleStatus(bool status)
        {
            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Idle, new ValueHolder("VirtualID", this.VirtualID, "Sleep", this.Sleeps = status)));
        }

        public void SetDance(int dance)
        {
            this.DanceID = dance;

            this.Room.SendToAll(OutgoingPacketsEnum.Dance, new ValueHolder("VirtualID", this.VirtualID, "DanceID", this.DanceID));
        }
    }
}
