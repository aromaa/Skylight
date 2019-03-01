using SkylightEmulator.HabboHotel.Users.Inventory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class TradeUser
    {
        public ConcurrentDictionary<uint, InventoryItem> OfferedItems { get; }

        public RoomUnitUser RoomUser { get; }
        public TradeConfirmStatus ConfirmStatus { get; set; }

        public TradeUser(RoomUnitUser user)
        {
            this.OfferedItems = new ConcurrentDictionary<uint, InventoryItem>();

            this.RoomUser = user;
            this.ConfirmStatus = TradeConfirmStatus.None;
        }

        public uint UserID => this.RoomUser.UserID;
    }
}
