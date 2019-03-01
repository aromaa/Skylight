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
    class RoomItemWiredNotMatchSnapshop : RoomItemWiredCondition
    {
        public List<RoomItem> SelectedItems;
        public Dictionary<uint, MatchFurniData> MatchFurniData;
        public bool FurniState;
        public bool Direction;
        public bool Position;

        public RoomItemWiredNotMatchSnapshop(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.SelectedItems = new List<RoomItem>();
            this.MatchFurniData = new Dictionary<uint, MatchFurniData>();
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (userHasRights)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.WiredCondition);
                message.AppendBoolean(false); //check box toggling
                message.AppendInt32(session.GetHabbo().GetWiredConditionLimit()); //furni limit
                message.AppendInt32(this.SelectedItems.Count); //furni count
                foreach(RoomItem item_ in this.SelectedItems.ToList())
                {
                    message.AppendUInt(item_.ID);
                }
                message.AppendInt32(this.GetBaseItem().SpriteID); //sprite id, show the help thing
                message.AppendUInt(this.ID); //item id
                message.AppendString(""); //data
                message.AppendInt32(3); //extra data count
                message.AppendBoolean(this.FurniState);
                message.AppendBoolean(this.Direction);
                message.AppendBoolean(this.Position);
                message.AppendInt32(0); //delay, not work with this wired

                message.AppendInt32(0); //type
                session.SendMessage(message);
            }
        }

        public override string GetItemData()
        {
            string itemString = "";
            foreach (KeyValuePair<uint, MatchFurniData> data in this.MatchFurniData)
            {
                if (itemString.Length > 0)
                {
                    itemString += "|";
                }

                itemString += data.Key + "," + data.Value.ExtraData + "," + data.Value.Rot + "," + data.Value.X + "," + data.Value.Y + "," + TextUtilies.DoubleWithDotDecimal(data.Value.Z);
            }

            return itemString + (char)9 + TextUtilies.BoolToString(this.FurniState) + (char)9 + TextUtilies.BoolToString(this.Direction) + (char)9 + TextUtilies.BoolToString(this.Position);
        }

        public override void LoadItemData(string data)
        {
            string[] dataSpit = data.Split((char)9);

            if (!string.IsNullOrEmpty(dataSpit[0]))
            {
                foreach (string itemData in dataSpit[0].Split('|'))
                {
                    string[] itemDataSlit = itemData.Split(',');
                    uint itemId = uint.Parse(itemDataSlit[0]);

                    RoomItem item = this.Room.RoomItemManager.TryGetRoomItem(itemId);
                    if (item != null)
                    {
                        string extraData = itemDataSlit[1];
                        int rot = int.Parse(itemDataSlit[2]);
                        int x = int.Parse(itemDataSlit[3]);
                        int y = int.Parse(itemDataSlit[4]);
                        double z = double.Parse(itemDataSlit[5]);

                        this.SelectedItems.Add(item);
                        this.MatchFurniData.Add(item.ID, new MatchFurniData(extraData, rot, x, y, z));
                    }
                }
            }

            this.FurniState = TextUtilies.StringToBool(dataSpit[1]);
            this.Direction = TextUtilies.StringToBool(dataSpit[2]);
            this.Position = TextUtilies.StringToBool(dataSpit[3]);
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
            foreach (RoomItem item_ in this.SelectedItems)
            {
                MatchFurniData data = this.MatchFurniData[item_.ID];
                if (this.FurniState && !item_.WiredIgnoreExtraData() && item_.ExtraData == data.ExtraData)
                {
                    return true;
                }

                if (this.Direction && item_.Rot == data.Rot)
                {
                    return true;
                }

                if (this.Position && item_.X == data.X && item_.Y == data.Y)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
