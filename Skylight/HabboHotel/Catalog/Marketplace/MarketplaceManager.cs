using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Users.Inventory;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Catalog.Marketplace
{
    public class MarketplaceManager
    {
        private Dictionary<uint, MarketplaceOffer> Offers;
        private Dictionary<uint, MarketplaceOffer> Sold;
        private Dictionary<uint, MarketplaceOffer> Expired;

        public MarketplaceManager()
        {
            this.Offers = new Dictionary<uint, MarketplaceOffer>();
            this.Sold = new Dictionary<uint, MarketplaceOffer>();
            this.Expired = new Dictionary<uint, MarketplaceOffer>();
        }

        public void LoadMarketplaceOffers(DatabaseClient dbClient)
        {
            Logging.Write("Loading marketplace offers... ");
            this.Offers.Clear();
            this.Sold.Clear();

            DataTable offers = dbClient.ReadDataTable("SELECT * FROM catalog_marketplace_offers");
            if (offers != null && offers.Rows.Count > 0)
            {
                foreach(DataRow dataRow in offers.Rows)
                {
                    uint id = (uint)dataRow["id"];
                    MarketplaceOffer offer = new MarketplaceOffer(id, (uint)dataRow["user_id"], (uint)dataRow["item_id"], (int)dataRow["price"], (double)dataRow["timestamp"], TextUtilies.StringToBool((string)dataRow["sold"]), (double)dataRow["sold_timestamp"], TextUtilies.StringToBool((string)dataRow["redeem"]), (uint)dataRow["furni_id"], (string)dataRow["furni_extra_data"]);

                    if (!offer.Sold) //not sold
                    {
                        if (!offer.Expired) //not expired
                        {
                            this.Offers.Add(id, offer);
                        }
                        else //expired
                        {
                            this.Expired.Add(id, offer);
                        }
                    }
                    else //sold
                    {
                        this.Sold.Add(id, offer);
                    }
                }
            }
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void MakeOffer(GameClient session, InventoryItem item, int price)
        {
            double timestmap = TimeUtilies.GetUnixTimestamp();

            uint id = 0;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                dbClient.AddParamWithValue("itemId", item.GetItem().ID);
                dbClient.AddParamWithValue("price", price);
                dbClient.AddParamWithValue("timestamp", timestmap);
                dbClient.AddParamWithValue("furniId", item.ID);
                dbClient.AddParamWithValue("furniExtraData", item.ExtraData);

                id = (uint)dbClient.ExecuteQuery("INSERT INTO catalog_marketplace_offers(user_id, item_id, price, timestamp, sold, redeem, furni_id, furni_extra_data) VALUES (@userId, @itemId, @price, @timestamp, '0', '0', @furniId, @furniExtraData)");
            }

            if (id > 0)
            {
                this.Offers.Add(id, new MarketplaceOffer(id, session.GetHabbo().ID, item.GetItem().ID, price, timestmap, false, 0, false, item.ID, item.ExtraData));
                session.GetHabbo().GetInventoryManager().RemoveItemFromHand(item.ID, true);
            }
        }

        public int CalcCompremission(float price)
        {
            double num = price / 100.0;
            return (int)Math.Ceiling(num * ServerConfiguration.MarketplaceCompremission);
        }

        public int GetOffersCountByItemID(uint itemId)
        {
            return this.Offers.Values.Count(o => o.ItemID == itemId);
        }

        public List<MarketplaceOffer> GetSoldsByItemID(uint itemId)
        {
            return this.Sold.Values.Where(s => s.ItemID == itemId).ToList();
        }

        public ServerMessage GetOffers(int minPrice, int maxPrice, string search, int order)
        {
            this.Reorganize(); //do it before shiet

            IEnumerable<MarketplaceOffer> rawList = null;
            if (minPrice >= 0 && maxPrice >= 0)
            {
                rawList = this.Offers.Values.Where(o => o.Price > minPrice && o.Price < maxPrice && Skylight.GetGame().GetItemManager().TryGetItem(o.ItemID).PublicName.Contains(search));
            }
            else if (minPrice >= 0)
            {
                rawList = this.Offers.Values.Where(o => o.Price > minPrice && Skylight.GetGame().GetItemManager().TryGetItem(o.ItemID).PublicName.Contains(search));
            }
            else if (maxPrice >= 0)
            {
                rawList = this.Offers.Values.Where(o => o.Price < maxPrice && Skylight.GetGame().GetItemManager().TryGetItem(o.ItemID).PublicName.Contains(search));
            }
            else
            {
                rawList = this.Offers.Values.Where(o => Skylight.GetGame().GetItemManager().TryGetItem(o.ItemID).PublicName.Contains(search));
            }
        
            List<IGrouping<uint, MarketplaceOffer>> offers = null;
            switch (order)
            {
                case 1: //most expensive first
                    {
                        offers = rawList.OrderByDescending(o => o.Price).GroupBy(o => o.ItemID).ToList();
                        break;
                    }
                case 2: //most cheap first
                    {
                        offers = rawList.OrderBy(o => o.Price).GroupBy(o => o.ItemID).ToList();
                        break;
                    }
                case 3: //most traded today
                    {
                        offers = new List<IGrouping<uint, MarketplaceOffer>>();

                        foreach(IGrouping<uint, MarketplaceOffer> sold in this.Sold.Values.Where(s => DateTime.Today.Date == TimeUtilies.UnixTimestampToDateTime(s.SoldTimestamp).Date).GroupBy(o => o.ItemID).OrderByDescending(g => g.Count()))
                        {
                            offers.AddRange(rawList.Where(o => o.ItemID == sold.Key).GroupBy(o => o.ItemID).ToList());
                        }
                        break;
                    }
                case 4: //lest traded today
                    {
                        offers = new List<IGrouping<uint, MarketplaceOffer>>();

                        foreach (IGrouping<uint, MarketplaceOffer> sold in this.Sold.Values.Where(s => DateTime.Today.Date == TimeUtilies.UnixTimestampToDateTime(s.SoldTimestamp).Date).GroupBy(o => o.ItemID).OrderBy(g => g.Count()))
                        {
                            offers.AddRange(rawList.Where(o => o.ItemID == sold.Take(1).ToList()[0].ItemID).GroupBy(o => o.ItemID).ToList());
                        }
                        break;
                    }
                case 5: //most offers avaible
                    {
                        offers = rawList.GroupBy(o => o.ItemID).OrderByDescending(g => g.Count()).ToList();
                        break;
                    }
                case 6: //leasts offers avaible
                    {
                        offers = rawList.GroupBy(o => o.ItemID).OrderBy(g => g.Count()).ToList();
                        break;
                    }
                default:
                    {
                        offers = new List<IGrouping<uint, MarketplaceOffer>>();
                        break;
                    }
            }

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.MarketplaceOffers);
            message.AppendInt32(offers.Count > 500 ? 500 : offers.Count); //how many we send
            foreach(IGrouping<uint, MarketplaceOffer> offerGroup in offers.Take(500))
            {
                foreach (MarketplaceOffer offer in offerGroup.OrderBy(o => o.Price).Take(1))
                {
                    Item item = Skylight.GetGame().GetItemManager().TryGetItem(offer.ItemID);

                    message.AppendUInt(offer.ID);
                    message.AppendInt32(1); //state
                    message.AppendInt32(item.IsFloorItem ? 1 : 2);
                    message.AppendInt32(item.SpriteID);
                    message.AppendString("");
                    message.AppendInt32(this.CalcCompremission(offer.Price) + offer.Price); //compremission + original price
                    message.AppendInt32(item.SpriteID); //unknown but phx says this.. so idk :s

                    int arvgPrice = this.Sold.Values.Where(s => s.ItemID == offer.ItemID && TimeUtilies.GetUnixTimestamp() - s.SoldTimestamp <= ServerConfiguration.MarketplaceAvaragePriceDays * 86400.0).Sum(s => this.CalcCompremission(s.Price) + s.Price);
                    int soldCount = this.Sold.Values.Count(s => s.ItemID == offer.ItemID && TimeUtilies.GetUnixTimestamp() - s.SoldTimestamp <= ServerConfiguration.MarketplaceAvaragePriceDays * 86400.0);
                    if (soldCount > 0)
                    {
                        message.AppendInt32(arvgPrice / soldCount); //arvgprice / sold
                    }
                    else
                    {
                        message.AppendInt32(0); //arvgprice / sold
                    }

                    message.AppendInt32(offerGroup.Count(g => g.ItemID == offer.ItemID)); //amount in sale
                }
            }
            message.AppendInt32(offers.Count); //total count

            return message;
        }

        public void Reorganize()
        {
            foreach(MarketplaceOffer offer in this.Offers.Values.ToList()) //we only want reorganice active offers
            {
                if (offer.Expired)
                {
                    this.Offers.Remove(offer.ID);
                    this.Expired.Add(offer.ID, offer);
                }

                if (offer.Cancalled)
                {
                    this.Offers.Remove(offer.ID);
                }
            }
        }

        public MarketplaceOffer TryGetActiveOffer(uint id)
        {
            MarketplaceOffer offer;
            this.Offers.TryGetValue(id, out offer);
            return offer;
        }

        public MarketplaceOffer TryGetInctiveOffer(uint id)
        {
            MarketplaceOffer offer;
            this.Sold.TryGetValue(id, out offer);
            if (offer == null)
            {
                this.Expired.TryGetValue(id, out offer);
            }
            return offer;
        }

        public MarketplaceOffer TryGetOffer(uint id)
        {
            MarketplaceOffer offer;
            offer = this.TryGetActiveOffer(id);
            if (offer == null)
            {
                offer = this.TryGetInctiveOffer(id);
            }
            return offer;
        }

        public void CancelOffer(GameClient session, uint id)
        {
            MarketplaceOffer offer = this.TryGetOffer(id);
            if (offer != null)
            {
                if (!offer.Sold && !offer.Redeem) //for sure both
                {
                    offer.Cancalled = true;

                    Item item = Skylight.GetGame().GetItemManager().TryGetItem(offer.ItemID);
                    if (item != null)
                    {
                        this.Offers.Remove(offer.ID);
                        this.Sold.Remove(offer.ID); //for sure :D
                        this.Expired.Remove(offer.ID);
                        session.GetHabbo().GetInventoryManager().AddItem(offer.FurniID, item.ID, offer.FurniExtraData, true);

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("offerId", offer.ID);
                            dbClient.ExecuteQuery("DELETE FROM catalog_marketplace_offers WHERE id = @offerId LIMIT 1"); //useless data so we can delete it
                        }

                        ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message.Init(r63aOutgoing.CancelOfferResult);
                        message.AppendUInt(offer.ID);
                        message.AppendInt32(1); //result, 0 = failed, 1 = success
                        session.SendMessage(message);
                    }
                    else
                    {
                        ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message.Init(r63aOutgoing.CancelOfferResult);
                        message.AppendUInt(offer.ID);
                        message.AppendInt32(0); //result, 0 = failed, 1 = success
                        session.SendMessage(message);
                    }
                }
                else
                {
                    ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message.Init(r63aOutgoing.CancelOfferResult);
                    message.AppendUInt(offer.ID);
                    message.AppendInt32(0); //result, 0 = failed, 1 = success
                    session.SendMessage(message);
                }
            }
            else
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.CancelOfferResult);
                message.AppendUInt(id);
                message.AppendInt32(0); //result, 0 = failed, 1 = success
                session.SendMessage(message);
            }
        }

        public void BuyOffer(GameClient session, uint id)
        {
            MarketplaceOffer offer = this.TryGetOffer(id);
            if (offer != null)
            {
                if (!offer.Expired)
                {
                    if (!offer.Sold && !offer.Cancalled)
                    {
                        offer.Sold = true;
                        offer.SoldTimestamp = TimeUtilies.GetUnixTimestamp();

                        this.Offers.Remove(id); //remove it, not anymore active
                        this.Sold.Add(id, offer); //add it to solds :)

                        Item item = Skylight.GetGame().GetItemManager().TryGetItem(offer.ItemID);
                        if (item != null)
                        {
                            int totalPrice = this.CalcCompremission(offer.Price) + offer.Price;
                            if (session.GetHabbo().Credits < totalPrice)
                            {
                                ServerMessage result = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                result.Init(r63aOutgoing.MarketplaceBuyOfferResult);
                                result.AppendInt32(4); //result, 1 = success, 2 = all sold out, 3 = update item and show confirmation, 4 = no credits
                                result.AppendInt32(0);
                                result.AppendInt32(0);
                                result.AppendInt32(0);
                                session.SendMessage(result);
                            }
                            else
                            {
                                session.GetHabbo().Credits -= totalPrice;
                                session.GetHabbo().UpdateCredits(true);
                                session.GetHabbo().GetInventoryManager().AddItem(offer.FurniID, item.ID, offer.FurniExtraData, true);

                                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                                {
                                    dbClient.AddParamWithValue("offerId", offer.ID);
                                    dbClient.AddParamWithValue("timestamp", offer.SoldTimestamp);

                                    dbClient.ExecuteQuery("UPDATE catalog_marketplace_offers SET sold = '1', sold_timestamp = @timestamp WHERE id = @offerId LIMIT 1");
                                }

                                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                message.Init(r63aOutgoing.BuyInfo);
                                message.AppendUInt(item.ID);
                                message.AppendString(item.PublicName);
                                message.AppendInt32(totalPrice);
                                message.AppendInt32(0);
                                message.AppendInt32(0);
                                message.AppendInt32(1);
                                message.AppendString(item.Type.ToString());
                                message.AppendInt32(item.SpriteID);
                                message.AppendString("");
                                message.AppendInt32(1);
                                message.AppendInt32(-1);
                                message.AppendString("");
                                session.SendMessage(message);

                                ServerMessage result_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                result_.Init(r63aOutgoing.MarketplaceBuyOfferResult);
                                result_.AppendInt32(1); //result, 1 = success, 2 = all sold out, 3 = update item and show confirmation, 4 = no credits
                                result_.AppendInt32(0);
                                result_.AppendInt32(0);
                                result_.AppendInt32(0);
                                session.SendMessage(result_);
                            }
                        }
                        else
                        {
                            session.SendNotif("Unable to find item base item!");
                        }
                    }
                    else
                    {
                        MarketplaceOffer newOffer = this.TryGetMarketplaceOfferByItemID(offer.ItemID);
                        if (newOffer != null)
                        {
                            if (newOffer.Price != offer.Price)
                            {
                                ServerMessage result = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                result.Init(r63aOutgoing.MarketplaceBuyOfferResult);
                                result.AppendInt32(3); //result, 1 = success, 2 = all sold out, 3 = update item and show confirmation, 4 = no credits
                                result.AppendUInt(newOffer.ID); //new id
                                result.AppendInt32(this.CalcCompremission(newOffer.Price) + newOffer.Price); //new price
                                result.AppendUInt(offer.ID); //old id
                                session.SendMessage(result);
                            }
                            else
                            {
                                this.BuyOffer(session, newOffer.ID);
                            }
                        }
                        else
                        {
                            ServerMessage result = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            result.Init(r63aOutgoing.MarketplaceBuyOfferResult);
                            result.AppendInt32(2); //result, 1 = success, 2 = all sold out, 3 = update item and show confirmation, 4 = no credits
                            result.AppendUInt(0); //new id
                            result.AppendInt32(0); //price
                            result.AppendUInt(0); //old id
                            session.SendMessage(result);
                        }
                    }
                }
                else
                {
                    session.SendNotif("Offer have expired! Sorry! :(");
                }
            }
            else
            {
                session.SendNotif("Unable to find offer! Sorry! :(");
            }
        }

        public MarketplaceOffer TryGetMarketplaceOfferByItemID(uint id)
        {
            return this.Offers.Values.Where(o => o.ItemID == id && o.Sold == false && o.Cancalled == false).OrderBy(o => o.Price).FirstOrDefault();
        }

        public List<MarketplaceOffer> GetOffersByUserID(uint id)
        {
            List<MarketplaceOffer> offers = this.Offers.Values.Where(o => o.UserID == id).ToList();
            offers.AddRange(this.Sold.Values.Where(o => o.UserID == id).ToList());
            offers.AddRange(this.Expired.Values.Where(o => o.UserID == id).ToList());
            return offers;
        }

        public void Shutdown()
        {
            if (this.Offers != null)
            {
                this.Offers.Clear();
            }
            this.Offers = null;

            if (this.Sold != null)
            {
                this.Sold.Clear();
            }
            this.Sold = null;

            if (this.Expired != null)
            {
                this.Expired.Clear();
            }
            this.Expired = null;
        }
    }
}
