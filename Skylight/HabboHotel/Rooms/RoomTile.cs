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
        public readonly int X;
        public readonly int Y;
        public readonly int ModelZ;
        public readonly ModelTileState ModelTileState;

        public RoomItem HigestRoomItem;
        public ModelItemState ModelItemState;

        private List<RoomItem> ItemsOnTile;
        public List<RoomUser> UsersOnTile;

        public RoomTile(int x, int y, int modelZ, ModelTileState modelTileState)
        {
            this.X = x;
            this.Y = y;
            this.ModelZ = modelZ;
            this.ModelTileState = modelTileState;

            this.ItemsOnTile = new List<RoomItem>();
            this.UsersOnTile = new List<RoomUser>();

            this.ModelItemState = ModelItemState.NONE;
        }

        public bool IsHole
        {
            get
            {
                return this.ModelTileState == ModelTileState.HOLE;
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
                return this.ModelItemState == ModelItemState.SEAT;
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
                return this.ModelItemState != ModelItemState.LOCKED && !this.IsHole && !this.IsInUse;
            }
        }

        public double GetZ(bool includeItemHeight)
        {
            if (this.HigestRoomItem != null)
            {
                if (includeItemHeight)
                {
                    return this.HigestRoomItem.Z + this.HigestRoomItem.BaseItem.Height;
                }
                else
                {
                    return this.HigestRoomItem.Z;
                }
            }
            else
            {
                return this.ModelZ;
            }
        }

        public void AddItemToTile(RoomItem item)
        {
            this.ItemsOnTile.Add(item);
            
            this.UpdateTile();
        }

        public void RemoveItemFromTile(RoomItem item)
        {
            this.ItemsOnTile.Remove(item);

            this.UpdateTile();
        }

        public void UpdateTile()
        {
            if (this.ItemsOnTile.Count > 0)
            {
                this.HigestRoomItem = this.ItemsOnTile.OrderByDescending(i => i.Z).First();
            }

            if (this.HigestRoomItem != null)
            {
                this.ModelItemState = this.HigestRoomItem.BaseItem.InteractionType == "bed" ? ModelItemState.BED : this.HigestRoomItem.BaseItem.IsSeat ? ModelItemState.SEAT : this.HigestRoomItem.BaseItem.Walkable ? ModelItemState.NONE : ModelItemState.LOCKED;
            }
            else
            {
                this.ModelItemState = ModelItemState.NONE;
            }
        }

        public void ResetGamemap()
        {
            this.ItemsOnTile.Clear();
            this.HigestRoomItem = null;

            this.UpdateTile();
        }
    }
}
