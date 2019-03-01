using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Trade
{
    public class TradeStartEventHandler : IncomingPacket
    {
        protected int VirtualID;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            Room room;
            if ((room = session?.GetHabbo()?.GetRoomSession()?.GetRoom()) != null)
            {
                RoomUnitUser user = room.RoomUserManager.GetUserByVirtualID(this.VirtualID);
                if (user != null)
                {
                    RoomAllowTradeType tradeSettings = room.RoomData.AllowTrade;
                    if (tradeSettings == RoomAllowTradeType.ALLOWED || (tradeSettings == RoomAllowTradeType.OWNER_ONLY && room.HaveOwnerRights(session)) || (tradeSettings == RoomAllowTradeType.CATEGORY && (Skylight.GetGame().GetNavigatorManager().GetFlatCat(session.GetHabbo().GetRoomSession().GetRoom().RoomData.Category)?.CanTrade ?? false)))
                    {
                        if (session.GetHabbo().GetUserSettings().AcceptTrading)
                        {
                            if (!session.GetHabbo().GetRoomSession().GetRoomUser().HasStatus("trd"))
                            {
                                if (!user.HasStatus("trd"))
                                {
                                    if (user.Session.GetHabbo().GetUserSettings().AcceptTrading)
                                    {
                                        room.StartTrade(session.GetHabbo().GetRoomSession().GetRoomUser(), user);
                                    }
                                    else
                                    {
                                        session.SendMessage(new TradeStartErrorComposerHandler(TradeStartErrorCode.TargetCannotTrade));
                                    }
                                }
                                else
                                {
                                    session.SendMessage(new TradeStartErrorComposerHandler(TradeStartErrorCode.TargetTrading));
                                }
                            }
                            else
                            {
                                session.SendMessage(new TradeStartErrorComposerHandler(TradeStartErrorCode.TradeOpen));
                            }
                        }
                        else
                        {
                            session.SendMessage(new TradeStartErrorComposerHandler(TradeStartErrorCode.YourTradingDisabled));
                        }
                    }
                    else
                    {
                        session.SendMessage(new TradeStartErrorComposerHandler(TradeStartErrorCode.TradingNotAllowInRoom));
                    }
                }

            }
        }
    }
}
