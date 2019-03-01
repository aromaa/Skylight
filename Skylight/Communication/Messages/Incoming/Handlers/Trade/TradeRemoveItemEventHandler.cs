using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Trade
{
    public class TradeRemoveItemEventHandler : IncomingPacket
    {
        protected uint ItemID;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            session.GetHabbo().GetRoomSession()?.GetRoom()?.GetTradeByUserId(session.GetHabbo().ID)?.RemoveItem(session, this.ItemID);
        }
    }
}
