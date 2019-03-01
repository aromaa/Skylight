using SkylightEmulator.Collections;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomTile
    {
        public readonly Room Room;
        public readonly int X;
        public readonly int Y;
        public readonly int ModelZ;
        public readonly ModelTileState ModelTileState;

        public RoomItem HigestRoomItem;
        public ModelItemState ModelItemState;

        public ConcurrentListDictionary<uint, Type, RoomItem> ItemsOnTile;
        public Dictionary<int, RoomUnit> UsersOnTile; //key = virtual Id, value = roomUser

        public RoomTile(Room room, int x, int y, int modelZ, ModelTileState modelTileState)
        {
            this.Room = room;
            this.X = x;
            this.Y = y;
            this.ModelZ = modelZ;
            this.ModelTileState = modelTileState;

            this.ItemsOnTile = new ConcurrentListDictionary<uint, Type, RoomItem>();
            this.UsersOnTile = new Dictionary<int, RoomUnit>();

            this.ModelItemState = ModelItemState.NONE;
        }

        public bool IsHole
        {
            get
            {
                return this.ModelTileState == ModelTileState.BLOCKED;
            }
        }

        public bool IsDoor
        {
            get
            {
                return this.ModelTileState == ModelTileState.DOOR;
            }
        }

        public bool IsSeat
        {
            get
            {
                return this.ModelItemState == ModelItemState.SEAT || this.ModelTileState == ModelTileState.SEAT;
            }
        }

        public bool IsBed
        {
            get
            {
                return this.ModelItemState == ModelItemState.BED;
            }
        }

        public bool IsWalkable
        {
            get
            {
                return !this.IsHole && this.ModelItemState != ModelItemState.NONE;
            }
        }

        public bool IsStackable
        {
            get
            {
                if (this.HigestRoomItem != null)
                {
                    return !this.IsHole && this.HigestRoomItem.BaseItem.Stackable;
                }
                else
                {
                    return !this.IsHole;
                }
            }
        }

        public bool IsInUse
        {
            get
            {
                return this.UsersOnTile.Count > 0;
            }
        }

        public bool CanUserMoveToTile
        {
            get
            {
                return this.ModelItemState != ModelItemState.LOCKED && !this.IsHole && !this.IsInUse && this.ItemsOnTile.Get(typeof(RoomItemBlackHole)).Count <= 0;
            }
        }

        public double GetZ(bool includeItemHeight)
        {
            return (includeItemHeight ? this.HigestRoomItem?.ActiveHeight : this.HigestRoomItem?.Z) ?? this.ModelZ;
        }

        public void AddItemToTile(RoomItem item)
        {
            this.ItemsOnTile.Add(item.ID, item.GetType(), item);

            if (this.HigestRoomItem == null || item.Z >= this.HigestRoomItem.Z) //there isint any higer items or the placed item can be the new higest item
            {
                this.UpdateTile();
            }
        }

        public void RemoveItemFromTile(RoomItem item)
        {
            this.ItemsOnTile.Remove(item.ID, item.GetType());

            if (this.HigestRoomItem == item) //if removed item is the higest item we should check stuff
            {
                this.UpdateTile();
            }
        }

        public RoomItem GetHigestItem(params RoomItem[] ignore)
        {
            RoomItem tempHigestItem = null;
            foreach (RoomItem item in this.ItemsOnTile.Values.OrderByDescending(i => i.Z))
            {
                if (!ignore.Contains(item))
                {
                    if ((tempHigestItem == null || (tempHigestItem.Z == item.Z && item.ActiveHeight > tempHigestItem.ActiveHeight)))
                    {
                        tempHigestItem = item;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return tempHigestItem;
        }

        public void UpdateTile()
        {
            this.HigestRoomItem = this.GetHigestItem();
            if (this.HigestRoomItem != null)
            {
                if (this.HigestRoomItem is RoomItemGate)
                {
                    this.ModelItemState = this.HigestRoomItem.ExtraData == "1" ? ModelItemState.NONE : ModelItemState.LOCKED;
                }
                else if (this.HigestRoomItem is RoomItemFreezeGateBlue || this.HigestRoomItem is RoomItemFreezeGateGreen || this.HigestRoomItem is RoomItemFreezeGateRed || this.HigestRoomItem is RoomItemFreezeGateYellow)
                {
                    this.ModelItemState = this.Room.RoomFreezeManager.GameStarted ? ModelItemState.LOCKED : ModelItemState.NONE;
                }
                else if (this.HigestRoomItem is RoomItemFreezeIceBlock)
                {
                    this.ModelItemState = (this.HigestRoomItem.ExtraData == "0" || string.IsNullOrEmpty(this.HigestRoomItem.ExtraData)) ? ModelItemState.LOCKED : ModelItemState.NONE;
                }
                else if (this.HigestRoomItem is RoomItemHorseObstacle)
                {
                    if (this.HigestRoomItem.Rot == 0 || this.HigestRoomItem.Rot == 2)
                    {
                        if ((this.HigestRoomItem.X + 1 == this.X && this.HigestRoomItem.Y == this.Y) || (this.HigestRoomItem.X + 1 == this.X && this.HigestRoomItem.Y + 1 == this.Y))
                        {
                            this.ModelItemState = ModelItemState.LOCKED;
                        }
                        else
                        {
                            this.ModelItemState = ModelItemState.NONE;
                        }
                    }
                    else if (this.HigestRoomItem.Rot == 4)
                    {
                        if ((this.HigestRoomItem.X == this.X && this.HigestRoomItem.Y + 1 == this.Y) || (this.HigestRoomItem.X + 1 == this.X && this.HigestRoomItem.Y + 1 == this.Y))
                        {
                            this.ModelItemState = ModelItemState.LOCKED;
                        }
                        else
                        {
                            this.ModelItemState = ModelItemState.NONE;
                        }
                    }
                }
                else
                {
                    this.ModelItemState = this.HigestRoomItem.BaseItem.InteractionType == "bed" ? ModelItemState.BED : this.HigestRoomItem.BaseItem.IsSeat ? ModelItemState.SEAT : this.HigestRoomItem.BaseItem.Walkable ? ModelItemState.NONE : ModelItemState.LOCKED;
                }
            }
            else
            {
                this.ModelItemState = ModelItemState.NONE;
            }
        }

        public void ResetGamemap()
        {
            this.ItemsOnTile.Clear();
            this.UsersOnTile.Clear();
            this.HigestRoomItem = null;

            this.UpdateTile();
        }
    }
}
