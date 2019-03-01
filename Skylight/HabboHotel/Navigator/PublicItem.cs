using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Navigator
{
    public class PublicItem
    {
        public readonly int ID;
        public PublicItemBannerType BannerType;
        public string Caption;
        public string Image;
        public PublicItemType Type;
        public readonly uint RoomID;
        public int ParentCategoryID;

        public PublicItem(int id, int bannerType, string caption, string image, string type, uint roomId, int parentCategoryId)
        {
            this.ID = id;
            this.BannerType = bannerType == 0 ? PublicItemBannerType.BIG : PublicItemBannerType.SMALL;
            this.Caption = caption;
            this.Image = image;
            this.Type = type == "TAG" ? PublicItemType.TAG : type == "FLAT" ? PublicItemType.FLAT : type == "PUBLIC_FLAT" ? PublicItemType.PUBLIC_FLAT : PublicItemType.CATEGORY;
            this.RoomID = roomId;
            this.ParentCategoryID = parentCategoryId;
        }

        public RoomData RoomData
        {
            get
            {
                if (this.RoomID == 0)
                {
                    return new RoomData(null);
                }
                else
                {
                    return Skylight.GetGame().GetRoomManager().GetRoomData(this.RoomID);
                }
            }
        }

        public void Serialize(ServerMessage message)
        {
            message.AppendInt32(this.ID);
            message.AppendStringWithBreak(this.Type != PublicItemType.PUBLIC_FLAT ? this.Caption : this.RoomData.Name);
            message.AppendStringWithBreak(this.RoomData.Description);
            message.AppendInt32((int)this.BannerType);
            message.AppendStringWithBreak(this.Type == PublicItemType.PUBLIC_FLAT ? this.Caption : "");
            message.AppendStringWithBreak(this.Image);
            message.AppendInt32(this.ParentCategoryID);
            message.AppendInt32(this.RoomData.UsersNow);
            message.AppendInt32((int)this.Type);

            if (this.Type == PublicItemType.TAG)
            {
                message.AppendString("NOT READY YET");
            }
            else if (this.Type == PublicItemType.FLAT)
            {
                this.RoomData.Serialize(message, false);
            }
            else if (this.Type == PublicItemType.PUBLIC_FLAT)
            {
                message.AppendStringWithBreak(this.Image);
                message.AppendUInt(1337u);
                message.AppendInt32(0);
                message.AppendStringWithBreak(""); //CTTs
                message.AppendInt32(100); //users max
                message.AppendUInt(this.RoomID);
            }
            else if (this.Type == PublicItemType.CATEGORY)
            {
                message.AppendBoolean(false); //is open
            }
        }
    }
}
