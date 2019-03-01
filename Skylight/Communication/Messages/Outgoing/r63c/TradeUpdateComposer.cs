using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Users.Inventory;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class TradeUpdateComposer<T> : OutgoingHandlerPacket where T : TradeUpdateComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.TradeUpdate);
            foreach(TradeUser trader in handler.Traders)
            {
                message.AppendUInt(trader.UserID);

                message.AppendInt32(trader.OfferedItems.Count);
                foreach (InventoryItem item in trader.OfferedItems.Values)
                {
                    message.AppendUInt(item.ID);
                    message.AppendString(item.GetItem().Type.ToLower());
                    message.AppendUInt(item.ID);
                    message.AppendInt32(item.GetItem().SpriteID);
                    message.AppendInt32(0);
                    message.AppendBoolean(item.GetItem().AllowInventoryStack);
                    message.AppendInt32(0);
                    message.AppendString(item.ExtraData);
                    message.AppendInt32(0); //year
                    message.AppendInt32(0); //month
                    message.AppendInt32(0); //day
                    if (item.GetItem().Type == "s")
                    {
                        if (item.GetItem().ItemName.StartsWith("present_"))
                        {
                            string[] data = item.ExtraData.Split((char)9);
                            message.AppendInt32(int.Parse(data[2]) * 1000 + int.Parse(data[3])); //gift style
                        }
                        else
                        {
                            message.AppendInt32(0); //song id
                        }
                    }
                }
            }
            return message;
        }
    }
}
