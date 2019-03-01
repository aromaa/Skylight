using SkylightEmulator.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemFootballGate : RoomItem
    {
        public string[] Data; //0 = M, 1 = F

        public RoomItemFootballGate(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.Data = new string[] { "lg-270-62", "ch-630-62.lg-695-62" };
        }

        public void SetFigure(string gender, string look)
        {
            look = look.Replace("hd-99999-99999.", "");
            if (gender == "M")
            {
                this.Data[0] = look;
            }
            else
            {
                this.Data[1] = look;
            }
            this.Room.RoomItemManager.ItemDataChanged.AddOrUpdate(this.ID, this, (key, oldValue) => this);

            this.ExtraData = "hd-99999-99999." + this.Data[0] + "," + "hd-99999-99999." + this.Data[1];
            this.UpdateState(true, true);
        }

        public override string GetItemData()
        {
            return this.Data[0] + (char)9 + this.Data[1];
        }

        public override void LoadItemData(string data)
        {
            string[] data_ = data.Split((char)9);
            this.Data[0] = data_[0];
            this.Data[1] = data_[1];
        }
    }
}
