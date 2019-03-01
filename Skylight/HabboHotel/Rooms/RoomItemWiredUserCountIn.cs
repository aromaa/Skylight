using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemWiredUserCountIn : RoomItemWiredCondition
    {
        public int UsersMin;
        public int UsersMax;

        public RoomItemWiredUserCountIn(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.UsersMin = 1;
            this.UsersMax = 1;
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (userHasRights)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.WiredCondition);
                message.AppendBoolean(false); //check box toggling
                message.AppendInt32(0); //furni limit
                message.AppendInt32(0); //furni count
                message.AppendInt32(this.GetBaseItem().SpriteID); //sprite id, show the help thing
                message.AppendUInt(this.ID); //item id
                message.AppendString(""); //data
                message.AppendInt32(2); //extra data count
                message.AppendInt32(this.UsersMin);
                message.AppendInt32(this.UsersMax);
                message.AppendInt32(0); //delay, not work with this wired

                message.AppendInt32(5); //type
                session.SendMessage(message);
            }
        }

        public override string GetItemData()
        {
            return this.UsersMin.ToString() + (char)9 + this.UsersMax.ToString();
        }

        public override void LoadItemData(string data)
        {
            string[] data_ = data.Split((char)9);
            this.UsersMin = int.Parse(data_[0]);
            this.UsersMax = int.Parse(data_[1]);
        }

        public override void OnLoad()
        {
            this.ExtraData = "0";
        }

        public override void OnPickup(GameClient session)
        {
            this.ExtraData = "0";
        }

        public override void OnPlace(GameClient session)
        {
            this.ExtraData = "0";
        }

        public override void OnCycle()
        {
            if (this.UpdateNeeded)
            {
                this.UpdateTimer--;
                if (this.UpdateTimer <= 0)
                {
                    this.UpdateNeeded = false;

                    this.ExtraData = "0";
                    this.UpdateState(false, true);
                }
            }
        }

        public override bool IsBlocking(RoomUnitUser triggerer)
        {
            if (this.Room.RoomData.UsersNow < this.UsersMin || this.Room.RoomData.UsersNow > this.UsersMax)
            {
                return true;
            }

            return false;
        }
    }
}