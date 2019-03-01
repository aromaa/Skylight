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
    public class TradeOfferItemEventHandler : IncomingPacket
    {
        protected uint ItemID;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            InventoryItem item = session?.GetHabbo()?.GetInventoryManager()?.TryGetItem(this.ItemID);
            if (item != null)
            {
                session.GetHabbo().GetRoomSession()?.GetRoom()?.GetTradeByUserId(session.GetHabbo().ID)?.OfferItem(session, item, session.GetHabbo().GetCommandCache().TradeXCommandValue);
                session.GetHabbo().GetCommandCache().TradeXCommandValue = 1;
            }
        }
    }
}
