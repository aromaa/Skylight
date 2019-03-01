using FastFoodServerAPI.Interfaces;
using SkylightEmulator.Communication;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Users.Inventory;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    /// <summary>
    /// Totally thread safe class
    /// </summary>
    public class Trade
    {
        public Room Room;
        public TradeUser[] Traders;
        public volatile bool Started;
        public TradeConfirmStatus Status;

        public Trade(Room room, RoomUnitUser user, RoomUnitUser target)
        {
            this.Room = room;
            this.Traders = new TradeUser[] { new TradeUser(user), new TradeUser(target) };
            this.Started = false;
            this.Status = TradeConfirmStatus.None;

            user.AddStatus("trd", "");
            target.AddStatus("trd", "");
        }

        public void Start()
        {
            if (!this.Started)
            {
                this.Started = true;

                this.SendToBoth(new TradeStartComposerHandler(this.Traders));
            }
        }

        public void SendToBoth(OutgoingHandler message)
        {
            foreach(TradeUser user in this.Traders)
            {
                user?.RoomUser?.Session?.SendMessage(message);
            }
        }

        public void Cancel(GameClient session)
        {
            if (this.Started && this.Status != TradeConfirmStatus.Confirmed)
            {
                foreach (TradeUser user in this.Traders)
                {
                    user.RoomUser.RemoveStatus("trd");
                    
                    this.Room.Trades.TryRemove(user.UserID, out Trade trash_);
                }

                this.SendToBoth(new TradeCancelComposerHandler(session.GetHabbo().ID));
            }
        }

        public TradeUser GetUser(uint userId)
        {
            return this.Traders.FirstOrDefault(t => t.RoomUser.UserID == userId);
        }

        public void OfferItem(GameClient session, InventoryItem item, int amount = 1)
        {
            if (this.Started && this.Status == TradeConfirmStatus.None)
            {
                TradeUser user = this.GetUser(session.GetHabbo().ID);
                if (user.ConfirmStatus == TradeConfirmStatus.None)
                {
                    foreach (TradeUser user_ in this.Traders)
                    {
                        user_.ConfirmStatus = TradeConfirmStatus.None;
                    }

                    bool update = false;
                    if (amount == 1)
                    {
                        update = user.OfferedItems.TryAdd(item.ID, item);
                    }
                    else
                    {
                        update = this.OfferManyItems(user, item, amount);
                    }

                    if (update)
                    {
                        this.SendToBoth(new TradeUpdateComposerHandler(this.Traders));
                    }
                }
            }
        }


        public void OfferItem(GameClient session, List<InventoryItem> items)
        {
            if (this.Started && this.Status == TradeConfirmStatus.None)
            {
                TradeUser user = this.GetUser(session.GetHabbo().ID);
                if (user.ConfirmStatus == TradeConfirmStatus.None)
                {
                    foreach (TradeUser user_ in this.Traders)
                    {
                        user_.ConfirmStatus = TradeConfirmStatus.None;
                    }

                    foreach(InventoryItem item in items)
                    {
                        user.OfferedItems.TryAdd(item.ID, item);
                    }

                    this.SendToBoth(new TradeUpdateComposerHandler(this.Traders));
                }
            }
        }

        public bool OfferManyItems(TradeUser user, InventoryItem item, int amount)
        {
            IEnumerable<InventoryItem> items = null;
            if (item.GetItem().IsFloorItem)
            {
                items = user.RoomUser.Session.GetHabbo().GetInventoryManager().GetFloorItemsByBaseID(item.BaseItem).Take(amount);
            }
            else
            {
                items = user.RoomUser.Session.GetHabbo().GetInventoryManager().GetWallItemsByBaseID(item.BaseItem).Take(amount);
            }

            foreach (InventoryItem item_ in items)
            {
                user.OfferedItems.TryAdd(item_.ID, item_);
            }

            return true;
        }

        public void RemoveItem(GameClient session, uint itemId)
        {
            if (this.Started && this.Status == TradeConfirmStatus.None)
            {
                TradeUser user = this.GetUser(session.GetHabbo().ID);
                if (user.ConfirmStatus == TradeConfirmStatus.None)
                {
                    user.OfferedItems.TryRemove(itemId, out InventoryItem item);

                    foreach (TradeUser user_ in this.Traders)
                    {
                        user_.ConfirmStatus = TradeConfirmStatus.None;
                    }

                    this.SendToBoth(new TradeUpdateComposerHandler(this.Traders));
                }
            }
        }

        public void AcceptTrade(GameClient session)
        {
            if (this.Started && this.Status == TradeConfirmStatus.None)
            {
                TradeUser user = this.GetUser(session.GetHabbo().ID);
                if (user.ConfirmStatus == TradeConfirmStatus.None)
                {
                    user.ConfirmStatus = TradeConfirmStatus.Accepted;

                    if (this.BothAccepted) //we can progress
                    {
                        this.Status = TradeConfirmStatus.Confirming;
                        foreach (TradeUser user_ in this.Traders)
                        {
                            user_.ConfirmStatus = TradeConfirmStatus.Confirming;
                        }
                    }
                    
                    this.SendToBoth(new TradeAcceptedComposerHandler(session.GetHabbo().ID, true));

                    if (this.Status == TradeConfirmStatus.Confirming)
                    {
                        this.SendToBoth(new TradeRequireConfirmComposerHandler());
                    }
                }
            }
        }

        public bool BothAccepted => this.Traders.All(t => t.ConfirmStatus == TradeConfirmStatus.Accepted);
        public bool BothConfirmed => this.Traders.All(t => t.ConfirmStatus == TradeConfirmStatus.Confirmed);

        public void ModifyTrade(GameClient session)
        {
            if (this.Started && this.Status == TradeConfirmStatus.None)
            {
                TradeUser user = this.GetUser(session.GetHabbo().ID);
                if (user.ConfirmStatus == TradeConfirmStatus.Accepted && !this.BothAccepted)
                {
                    user.ConfirmStatus = TradeConfirmStatus.None;

                    this.SendToBoth(new TradeAcceptedComposerHandler(session.GetHabbo().ID, false));
                }
            }
        }

        public void ConfirmAcceptTrade(GameClient session)
        {
            if (this.Started && this.Status == TradeConfirmStatus.Confirming)
            {
                TradeUser user = this.GetUser(session.GetHabbo().ID);
                if (user.ConfirmStatus == TradeConfirmStatus.Confirming)
                {
                    user.ConfirmStatus = TradeConfirmStatus.Confirmed;

                    if (this.BothConfirmed)
                    {
                        this.Status = TradeConfirmStatus.Confirmed;

                        foreach (TradeUser trader in this.Traders)
                        {
                            trader.RoomUser.RemoveStatus("trd");

                            this.Room.Trades.TryRemove(trader.UserID, out Trade trash);
                        }
                    }

                    this.SendToBoth(new TradeAcceptedComposerHandler(session.GetHabbo().ID, true));

                    if (this.Status == TradeConfirmStatus.Confirmed)
                    {
                        this.SendToBoth(new TradeWindowCloseComposerHandler());

                        new Task(new Action(this.TradeItems)).Start();
                    }
                }
            }
        }

        public void TradeItems()
        {
            TradeUser userOne = this.Traders[0];
            TradeUser userTwo = this.Traders[1];

            try
            {
                List<uint> tradeOffer1 = userOne.OfferedItems.Keys.ToList();
                List<uint> tradeOffer2 = userTwo.OfferedItems.Keys.ToList();

                //foreach (uint itemId in tradeOffer1)
                //{
                //    if (userOne.RoomUser.GetClient().GetHabbo().GetInventoryManager().TryGetItem(itemId) == null)
                //    {
                //        this.SendToBoth(new TradeCancelComposerHandler(userOne.UserID, TradeCancelErrorCode.ItemsUnavaible));
                //        return;
                //    }
                //}

                //foreach (uint itemId in tradeOffer2)
                //{
                //    if (userTwo.RoomUser.GetClient().GetHabbo().GetInventoryManager().TryGetItem(itemId) == null)
                //    {
                //        this.SendToBoth(new TradeCancelComposerHandler(userTwo.UserID, TradeCancelErrorCode.ItemsUnavaible));
                //        return;
                //    }
                //}

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId1", userOne.UserID);
                    dbClient.AddParamWithValue("userId2", userTwo.UserID);

                    dbClient.StartTransaction();

                    try
                    {
                        if (tradeOffer1.Count > 0)
                        {
                            if (dbClient.ExecuteNonQuery("UPDATE items SET room_id = '0', user_id = @userId2 WHERE id IN(" + string.Join(",", tradeOffer1) + ") LIMIT " + tradeOffer1.Count) != tradeOffer1.Count)
                            {
                                dbClient.Rollback();

                                this.SendToBoth(new TradeCancelComposerHandler(userOne.UserID, TradeCancelErrorCode.ItemsUnavaible));
                                return;
                            }
                        }

                        if (tradeOffer2.Count > 0)
                        {
                            if (dbClient.ExecuteNonQuery("UPDATE items SET room_id = '0', user_id = @userId1 WHERE id IN(" + string.Join(",", tradeOffer2) + ") LIMIT " + tradeOffer2.Count) != tradeOffer2.Count)
                            {
                                dbClient.Rollback();

                                this.SendToBoth(new TradeCancelComposerHandler(userTwo.UserID, TradeCancelErrorCode.ItemsUnavaible));
                                return;
                            }
                        }

                        dbClient.Commit();
                    }
                    catch
                    {
                        dbClient.Rollback();

                        throw;
                    }
                }

                try
                {
                    userOne?.RoomUser?.Session?.GetHabbo()?.GetInventoryManager()?.UpdateInventoryItems(true);
                }
                catch
                {
                }

                try
                {
                    userTwo?.RoomUser?.Session?.GetHabbo()?.GetInventoryManager()?.UpdateInventoryItems(true);
                }
                catch
                {
                }
            }
            catch(Exception ex)
            {
                Logging.LogException("Error in TradeItems task! " + ex.ToString());

                try
                {
                    userOne?.RoomUser?.Session?.SendNotif("Trade failed due to critical error!");
                }
                catch
                {
                }

                try
                {
                    userTwo?.RoomUser?.Session?.SendNotif("Trade failed due to critical error!");
                }
                catch
                {
                }
            }
        }
    }
}
