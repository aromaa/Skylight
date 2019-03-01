using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Users.Inventory;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Trade
{
    public class TradeOfferMultipleItemsEventHandler : IncomingPacket
    {
        protected uint[] ItemIds;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            List<InventoryItem> items = new List<InventoryItem>(this.ItemIds.Length);
            foreach (uint itemId in this.ItemIds)
            {
                InventoryItem item = session?.GetHabbo()?.GetInventoryManager()?.TryGetItem(itemId);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            session.GetHabbo().GetRoomSession()?.GetRoom()?.GetTradeByUserId(session.GetHabbo().ID)?.OfferItem(session, items);
        }
    }
}
