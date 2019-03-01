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
    class RoomItemActionTriggerStacks : RoomItemWiredAction
    {
        public List<RoomItem> SelectedItems;

        public RoomItemActionTriggerStacks(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.SelectedItems = new List<RoomItem>();
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (userHasRights)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.WiredAction);
                message.AppendBoolean(false); //check box toggling
                message.AppendInt32(session.GetHabbo().GetWiredActionLimit()); //furni limit
                message.AppendInt32(this.SelectedItems.Count); //furni count
                foreach (RoomItem item_ in this.SelectedItems)
                {
                    message.AppendUInt(item_.ID);
                }

                message.AppendInt32(this.GetBaseItem().SpriteID); //sprite id, show the help thing
                message.AppendUInt(this.ID); //item id
                message.AppendString(""); //data
                message.AppendInt32(0); //extra data count
                message.AppendInt32(0); //delay, not work with this wired

                message.AppendInt32(0); //type
                message.AppendInt32(this.Delay); //delay
                message.AppendInt32(0); //conflicts count
                session.SendMessage(message);
            }
        }

        public override string GetItemData()
        {
            return string.Join(",", this.SelectedItems.Select(i => i.ID)) + (char)9 + this.Delay;
        }

        public override void LoadItemData(string data)
        {
            string[] dataSpit = data.Split((char)9);

            foreach (string sItemId in dataSpit[0].Split(','))
            {
                if (!string.IsNullOrEmpty(sItemId))
                {
                    RoomItem item = this.Room.RoomItemManager.TryGetRoomItem(uint.Parse(sItemId));
                    if (item != null)
                    {
                        this.SelectedItems.Add(item);
                    }
                }
            }

            this.Delay = int.Parse(dataSpit[1]);
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

        public override void DoWiredAction(RoomUnitUser triggerer, HashSet<uint> used)
        {
            foreach (RoomItem item in this.SelectedItems.ToList())
            {
                this.Room.RoomWiredManager.TriggerWiredStacks(triggerer, item, this, used);
            }
        }
    }
}
