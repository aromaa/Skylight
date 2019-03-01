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
    class RoomItemWiredOnSay : RoomItemWiredTrigger
    {
        public string Message;
        public bool OnlyOwner;

        public RoomItemWiredOnSay(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.Message = "";
            this.OnlyOwner = false;
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (userHasRights)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.WiredTrigger);
                message.AppendBoolean(false); //check box toggling
                message.AppendInt32(0); //furni limit
                message.AppendInt32(0); //furni count
                message.AppendInt32(this.GetBaseItem().SpriteID); //sprite id, show the help thing
                message.AppendUInt(this.ID); //item id
                message.AppendString(this.Message); //data
                message.AppendInt32(1); //extra data count
                message.AppendBoolean(this.OnlyOwner); //only owner
                message.AppendInt32(0); //delay, not work with this wired

                message.AppendInt32(0); //type
                message.AppendInt32(0); //conflicts count
                session.SendMessage(message);
            }
        }

        public override string GetItemData()
        {
            return this.Message + (char)9 + TextUtilies.BoolToString(this.OnlyOwner);
        }

        public override void LoadItemData(string data)
        {
            string[] splitData = data.Split((char)9);
            this.Message = splitData[0];
            this.OnlyOwner = TextUtilies.StringToBool(splitData[1]);
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

        public override bool TryTrigger(RoomUnitUser triggerer, object extraData)
        {
            string message = extraData as string;
            if (!string.IsNullOrEmpty(message))
            {
                string keyword = this.Message;
                if (!this.Room.RoomData.ExtraData.RoomSettingsLogic.Contains("wired-onsay-case-sensitive"))
                {
                    message = message.ToLower();
                    keyword = keyword.ToLower();
                }

                if (!this.OnlyOwner || (this.OnlyOwner && triggerer.Session.GetHabbo().ID == this.Room.RoomData.OwnerID))
                {
                    if ((!this.Room.RoomData.ExtraData.RoomSettingsLogic.Contains("wired-onsay-equals") && message.Contains(keyword)) || message == keyword)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
