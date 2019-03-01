using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Items.Interactors;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomItem
    {
        public uint ID;
        public uint RoomID;
        public Item BaseItem;
        public string ExtraData;
        public int X;
        public int Y;
        public double Z;
        public int Rot;
        public WallCoordinate WallCoordinate;
        public Room Room;
        public Dictionary<int, AffectedTile> AffectedTiles;

        public RoomItem(uint id, uint roomId, uint baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
        {
            this.ID = id;
            this.RoomID = roomId;
            this.BaseItem = Skylight.GetGame().GetItemManager().GetItem(baseItem);
            this.ExtraData = extraData;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Rot = rot;
            this.WallCoordinate = wallCoordinate;
            this.Room = room;

            this.AffectedTiles = ItemUtilies.AffectedTiles(this.BaseItem.Lenght, this.BaseItem.Width, x, y, this.Rot);
        }

        public bool IsFloorItem
        {
            get
            {
                return this.BaseItem.Type == "s";
            }
        }

        public bool IsWallItem
        {
            get
            {
                return this.BaseItem.Type == "i";
            }
        }

        public void SetLocation(int x, int y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.AffectedTiles = ItemUtilies.AffectedTiles(this.BaseItem.Lenght, this.BaseItem.Width, x, y, this.Rot);
        }

        public void Serialize(ServerMessage message)
        {
            if (this.IsFloorItem)
            {
                message.AppendUInt(this.ID);
                message.AppendInt32(this.BaseItem.SpriteID);
                message.AppendInt32(this.X);
                message.AppendInt32(this.Y);
                message.AppendInt32(this.Rot);
                message.AppendStringWithBreak(TextUtilies.DoubleWithDotDecimal(this.Z));
                message.AppendInt32(0);
                message.AppendStringWithBreak(this.ExtraData);
                message.AppendInt32(-1);
                message.AppendBoolean(true); //use button
            }
            else
            {
                if (this.IsWallItem) //just for sure nothing goes wrong
                {
                    message.AppendStringWithBreak(this.ID.ToString());
                    message.AppendInt32(this.BaseItem.SpriteID);
                    message.AppendStringWithBreak(this.WallCoordinate != null ? this.WallCoordinate.ToString() : ""); //sometimes its null ;(

                    if (this.BaseItem.ItemName.StartsWith("poster_"))
                    {
                        message.AppendString(this.BaseItem.ItemName.Split(new char[]
						{
							'_'
						})[1]);
                    }
                    else
                    {
                        string text = this.BaseItem.InteractionType.ToLower();
                        if (text != null && text == "postit")
                        {
                            message.AppendStringWithBreak(this.ExtraData.Split(new char[]
						    {
							    ' '
						    })[0]);
                        }
                        else
                        {
                            message.AppendStringWithBreak(this.ExtraData);
                        }
                    }

                    message.AppendBoolean(true); //use button
                }
            }
        }

        public void UpdateState(bool updateDatabase, bool updateRoom)
        {
            if (updateDatabase)
            {
                this.Room.RoomItemManager.UpdateItemStateToDatabase(this);
            }

            if (updateRoom)
            {
                ServerMessage updateItem = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                if (this.IsWallItem)
                {
                    updateItem.Init(r63aOutgoing.UpdateRoomWallItem);
                    this.Serialize(updateItem);
                }
                else
                {
                    updateItem.Init(r63aOutgoing.UpdateRoomFloorItem);
                    updateItem.AppendStringWithBreak(this.ID.ToString());
                    updateItem.AppendStringWithBreak(this.ExtraData);
                    this.Serialize(updateItem);
                }
                this.Room.SendToAll(updateItem, null);
            }
        }

        public FurniInteractor GetFurniInteractor()
        {
            string interactionType = this.BaseItem.InteractionType;

            switch(interactionType)
            {
                default:
                    return new InteractorDefault(this.BaseItem.InteractionModeCounts);
            }
        }
    }
}
