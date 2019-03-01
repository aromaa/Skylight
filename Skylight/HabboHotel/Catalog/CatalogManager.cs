using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Catalog.Marketplace;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Pets;
using SkylightEmulator.HabboHotel.Users.Inventory;
using SkylightEmulator.HabboHotel.Users.Subscription;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Catalog
{
    public class CatalogManager
    {
        private MarketplaceManager MarketplaceManager;
        private Dictionary<int, CatalogPage> CatalogPages;
        private Dictionary<uint, CatalogItem> CatalogItems;
        private Dictionary<int, List<PetRace>> PetRaces;
        private Dictionary<int, Item> OldPresents; //key = sprite id
        private Dictionary<int, Item> NewPresents; //key = sprite id

        public CatalogManager()
        {
            this.MarketplaceManager = new MarketplaceManager();
            this.CatalogPages = new Dictionary<int, CatalogPage>();
            this.CatalogItems = new Dictionary<uint, CatalogItem>();
            this.PetRaces = new Dictionary<int, List<PetRace>>();
            this.OldPresents = new Dictionary<int, Item>();
            this.NewPresents = new Dictionary<int, Item>();
        }

        public void LoadCatalogPages(DatabaseClient dbClient)
        {
            Logging.Write("Loading catalog pages... ");
            Dictionary<int, CatalogPage> newPages = new Dictionary<int, CatalogPage>();

            DataTable pages = dbClient.ReadDataTable("SELECT * FROM catalog_pages ORDER BY IF(order_num != -1, order_num, caption) ASC");
            if (pages != null)
            {
                foreach(DataRow dataRow in pages.Rows)
                {
                    int id = (int)dataRow["id"];
                    newPages.Add(id, new CatalogPage((int)dataRow["Id"], (int)dataRow["parent_id"], (string)dataRow["caption"], TextUtilies.StringToBool(dataRow["visible"].ToString()), TextUtilies.StringToBool(dataRow["enabled"].ToString()), (int)dataRow["min_rank"], TextUtilies.StringToBool(dataRow["club_only"].ToString()), (int)dataRow["icon_color"], (int)dataRow["icon_image"], (string)dataRow["page_layout"], (string)dataRow["page_headline"], (string)dataRow["page_teaser"], (string)dataRow["page_special"], (string)dataRow["page_text1"], (string)dataRow["page_text2"], (string)dataRow["page_text_details"], (string)dataRow["page_text_teaser"], (string)dataRow["page_link_description"], (string)dataRow["page_link_pagename"], this.CatalogItems.Values.Where(i => i.PageID == id)));
                }
            }

            this.CatalogPages = newPages;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void LoadCatalogItems(DatabaseClient dbClient)
        {
            Logging.Write("Loading catalog items... ");
            Dictionary<uint, CatalogItem> newItems = new Dictionary<uint, CatalogItem>();

            DataTable items = dbClient.ReadDataTable("SELECT * FROM catalog_items");
            if (items != null)
            {
                foreach(DataRow dataRow in items.Rows)
                {
                    uint id = (uint)dataRow["id"];
                    newItems.Add(id, new CatalogItem(id, (string)dataRow["catalog_name"], (string)dataRow["item_ids"], (int)dataRow["cost_credits"], (int)dataRow["cost_pixels"], (int)dataRow["cost_snow"], (string)dataRow["amounts"], (int)dataRow["page_id"]));
                }
            }

            this.CatalogItems = newItems;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void LoadPetRaces(DatabaseClient dbClient)
        {
            Logging.Write("Loading pet races... ");
            Dictionary<int, List<PetRace>> newPetRaces = new Dictionary<int, List<PetRace>>();

            DataTable races = dbClient.ReadDataTable("SELECT * FROM catalog_pet_races");
            if (races != null && races.Rows.Count > 0)
            {
                foreach(DataRow dataRow in races.Rows)
                {
                    int id = (int)dataRow["race_id"];
                    PetRace race = new PetRace(id, (int)dataRow["color1"]);

                    if (!newPetRaces.ContainsKey(id))
                    {
                        newPetRaces.Add(id, new List<PetRace>());
                    }

                    newPetRaces[id].Add(race);
                }
            }

            this.PetRaces = newPetRaces;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void LoadPresents(DatabaseClient dbClient)
        {
            Logging.Write("Loading presents... ");
            Dictionary<int, Item> oldPresents = new Dictionary<int, Item>();
            Dictionary<int, Item> newPresents = new Dictionary<int, Item>();

            DataTable presents = dbClient.ReadDataTable("SELECT * FROM catalog_presents");
            if (presents != null && presents.Rows.Count > 0)
            {
                foreach(DataRow dataRow in presents.Rows)
                {
                    string type = (string)dataRow["type"];
                    Item item = Skylight.GetGame().GetItemManager().TryGetItem((uint)dataRow["item_id"]);

                    if (type == "new")
                    {
                        newPresents.Add(item.SpriteID, item);
                    }
                    else if (type == "old")
                    {
                        oldPresents.Add(item.SpriteID, item);
                    }
                }
            }

            this.OldPresents = oldPresents;
            this.NewPresents = newPresents;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public List<CatalogPage> GetParentPages(int pageId)
        {
            return this.CatalogPages.Values.Where(p => p.ParentID == pageId).ToList();
        }

        public List<CatalogPage> GetParentPagesWithRankedSystem(int pageId, int rank)
        {
            return this.CatalogPages.Values.Where(p => p.ParentID == pageId && p.MinRank <= rank).ToList();
        }

        public CatalogPage GetCatalogPage(int pageId)
        {
            if (this.CatalogPages.ContainsKey(pageId))
            {
                return this.CatalogPages[pageId];
            }
            else
            {
                return null;
            }
        }

        public List<CatalogPage> GetCatalogPages()
        {
            return this.CatalogPages.Values.ToList();
        }

        public void BuyItem(GameClient session, int pageId, uint itemId, string extraData, int amountMultiplayer, bool asGift, string receiverUsername = "", string giftMessage = "", int giftSpriteId = 0, int giftBoxId = 0, int giftRibbonId = 0)
        {
            CatalogPage page = this.GetCatalogPage(pageId);
            if (page != null && page.Visible & page.Enabled && page.MinRank <= session.GetHabbo().Rank)
            {
                CatalogItem item = page.GetItem(itemId);
                if (item != null)
                {
                    if (item.IsDeal)
                    {
                        if (amountMultiplayer > 1)
                        {
                            session.SendNotif("You can't buy multiple deals at once!");
                            return;
                        }
                    }

                    Tuple<Item, int>[] products = item.GetItems();
                    uint giftReceiverId = 0;
                    if (asGift)
                    {
                        foreach (Tuple<Item, int> data in products)
                        {
                            if (!data.Item1.AllowGift)
                            {
                                return;
                            }
                        }

                        giftReceiverId = Skylight.GetGame().GetGameClientManager().GetIDByUsername(receiverUsername);
                        if (giftReceiverId == 0) //not valid user
                        {
                            ServerMessage receiverNotFound = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            receiverNotFound.Init(r63aOutgoing.GiftReceiverNotFound);
                            session.SendMessage(receiverNotFound);
                            return;
                        }
                    }

                    if (amountMultiplayer > 1 && products[0].Item2 * amountMultiplayer > 100)
                    {
                        session.SendNotif("You can't buy more then 100 at once with the buy command");
                        return;
                    }

                    int finalCredits = item.CostCredits * amountMultiplayer;
                    int finalPixels = item.CostActivityPoints * amountMultiplayer;

                    if (asGift && giftSpriteId > 0) //special gifts costs one credit more
                    {
                        finalCredits++;
                    }

                    bool noCredits = false;
                    bool noActivityPoints = false;

                    if (session.GetHabbo().Credits < finalCredits)
                    {
                        noCredits = true;
                    }

                    if (session.GetHabbo().TryGetActivityPoints(item.ActivityPointsType) < finalPixels)
                    {
                        noActivityPoints = true;
                    }

                    if (noCredits || noActivityPoints)
                    {
                        ServerMessage noEnoughtCash = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        noEnoughtCash.Init(r63aOutgoing.NoEnoughtCash);
                        noEnoughtCash.AppendBoolean(noCredits);
                        noEnoughtCash.AppendBoolean(noActivityPoints);
                        session.SendMessage(noEnoughtCash);
                    }
                    else
                    {
                        if (asGift)
                        {
                            foreach (Tuple<Item, int> data in products)
                            {
                                if (data.Item1.Type == "e")
                                {
                                    session.SendNotif("You can not send this item as a gift.");
                                    return;
                                }
                            }
                        }

                        if (!item.IsDeal)
                        {
                            switch (products[0].Item1.InteractionType.ToLower())
                            {
                                case "pet":
                                    {
                                        string[] data = extraData.Split('\n');
                                        string petName = data[0];
                                        int petRace = int.Parse(data[1]);
                                        string color = data[2];

                                        Regex regex = new Regex(@"^[A-Z0-9_-]+$", RegexOptions.IgnoreCase);
                                        if (petName.Length >= 2 && petName.Length <= 16 && regex.IsMatch(petName) && !TextUtilies.HaveBlacklistedWords(petName))
                                        {
                                            //buy
                                        }
                                        else
                                        {
                                            return;
                                        }

                                        if (color.Length != 6)
                                        {
                                            return;
                                        }
                                        break;
                                    }
                                case "roomeffect":
                                    {
                                        extraData = TextUtilies.DoubleWithDotDecimal(double.Parse(extraData));
                                    }
                                    break;
                                case "postit":
                                    {
                                        extraData = "FFFF33"; //if you leave extra data empty same result, but for sure
                                    }
                                    break;
                                case "dimmer":
                                    {
                                        extraData = "1,1,1,#000000,255"; //current mode
                                    }
                                    break;
                                case "trophy":
                                    {
                                        extraData = session.GetHabbo().ID.ToString() + (char)9 + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + (char)9 + TextUtilies.FilterString(extraData, false, true);
                                    }
                                    break;
                                default:
                                    {
                                        extraData = "";
                                        break;
                                    }
                            }
                        }

                        if (finalCredits > 0)
                        {
                            session.GetHabbo().Credits -= finalCredits;
                            session.GetHabbo().UpdateCredits(true);
                        }

                        if (finalPixels > 0)
                        {
                            session.GetHabbo().RemoveActivityPoints(item.ActivityPointsType, finalPixels);
                            session.GetHabbo().UpdateActivityPoints(item.ActivityPointsType, true);
                        }

                        session.SendMessage(BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.BuyInfo).Handle(new ValueHolder("Item", item, "Products", products, "Credits", finalCredits, "Pixels", finalPixels)));

                        if (asGift)
                        {
                            Item gift = this.GetGiftLook(giftSpriteId);

                            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                            {
                                string giftData = TextUtilies.FilterString(giftMessage, false, true) + (char)9 + session.GetHabbo().ID + (char)9 + giftBoxId + (char)9 + giftRibbonId;
                                dbClient.AddParamWithValue("giftData", giftData);
                                dbClient.AddParamWithValue("receiverId", giftReceiverId);
                                dbClient.AddParamWithValue("giftBaseItem", gift.ID);
                                uint giftItemId = (uint)dbClient.ExecuteQuery("INSERT INTO items(user_id, base_item, extra_data) VALUES(@receiverId, @giftBaseItem, @giftData)");

                                string baseItems = "";
                                string amounts = "";
                                foreach (Tuple<Item, int> data in products)
                                {
                                    if (baseItems.Length > 0)
                                    {
                                        baseItems += ",";
                                        amounts += ",";
                                    }

                                    baseItems += data.Item1.ID;
                                    amounts += data.Item2 * amountMultiplayer;
                                }

                                dbClient.AddParamWithValue("itemId", giftItemId);
                                dbClient.AddParamWithValue("baseItems", baseItems);
                                dbClient.AddParamWithValue("amounts", amounts);
                                dbClient.AddParamWithValue("extraData", extraData);
                                dbClient.ExecuteQuery("INSERT INTO items_presents(item_id, base_ids, amounts, extra_data) VALUES(@itemId, @baseItems, @amounts, @extraData)");

                                GameClient receiver = Skylight.GetGame().GetGameClientManager().GetGameClientById(giftReceiverId);
                                if (receiver != null)
                                {
                                    if (giftReceiverId != session.GetHabbo().ID) //do achievement progress
                                    {
                                        receiver.GetHabbo().GetUserStats().GiftsReceived++;
                                        receiver.GetHabbo().GetUserAchievements().CheckAchievement("GiftReceiver");
                                    }

                                    receiver.GetHabbo().GetInventoryManager().AddItem(giftItemId, gift.ID, giftData, false); //add it to inventory first

                                    session.SendMessage(BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.UnseenItem).Handle(new ValueHolder("Type", 1, "ID", giftItemId)));//receiver one item in this case the gift

                                    //receiver.GetHabbo().GetInventoryManager().UpdateInventoryItems(true); //update inv
                                }
                                else
                                {
                                    if (giftReceiverId != session.GetHabbo().ID) //do achievement progress
                                    {
                                        dbClient.ExecuteQuery("UPDATE user_stats SET gifts_received = gifts_received + 1 WHERE user_id = @receiverId LIMIT 1");
                                    }
                                }
                            }

                            if (giftReceiverId != session.GetHabbo().ID) //do achievement progress
                            {
                                session.GetHabbo().GetUserStats().GiftsGived++;
                                session.GetHabbo().GetUserAchievements().CheckAchievement("GiftGiver");
                            }
                        }
                        else
                        {
                            session.GetHabbo().GetInventoryManager().SetQueueBytes(true);
                            foreach (Tuple<Item, int> data in products)
                            {
                                this.AddItem(session, data.Item1, data.Item2 * amountMultiplayer, extraData, true, false);
                            }
                            session.GetHabbo().GetInventoryManager().SetQueueBytes(false);
                        }
                    }
                }
            }
        }

        public Item GetGiftLook(int spriteId)
        {
            Item item = null;
            if (spriteId == 0) //use free gift
            {
                int random = RandomUtilies.GetRandom(0, this.OldPresents.Count - 1);
                item = this.OldPresents.ElementAt(random).Value;
            }
            else
            {
                this.NewPresents.TryGetValue(spriteId, out item);
            }
            return item;
        }

        public void AddItem(GameClient session, Item item, int amount, string extraData, bool newFurni, bool queueBytes = false)
        {
            if (queueBytes)
            {
                session.GetHabbo().GetInventoryManager().SetQueueBytes(true);
            }

            List<uint> newFloorItems = new List<uint>();
            List<uint> newWallItems = new List<uint>();
            List<uint> newPets = new List<uint>();

            switch (item.Type)
            {
                case "i": //wall item
                    {
                        switch (item.InteractionType.ToLower())
                        {
                            case "dimmer":
                            default:
                                {
                                    foreach (uint itemId_ in DatabaseUtils.CreateItems(session.GetHabbo().ID, item, extraData, amount))
                                    {
                                        newWallItems.Add(itemId_);
                                        session.GetHabbo().GetInventoryManager().AddInventoryItemToHand(new InventoryItem(itemId_, item.ID, extraData));
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "s": //floor item
                    {
                        switch(item.InteractionType.ToLower())
                        {
                            case "pet":
                                {
                                    string[] data = extraData.Split('\n');
                                    string petName = data[0];
                                    string petRace = data[1];
                                    string petColor = data[2];
                                    double timestamp = TimeUtilies.GetUnixTimestamp();
                                    foreach(uint petId in DatabaseUtils.CreatePets(session.GetHabbo().ID, petName, petRace, petColor, int.Parse(item.ItemName.Split('t')[1]), timestamp, amount))
                                    {
                                        newPets.Add(petId);

                                        Pet pet = new Pet(petId, session.GetHabbo().ID, int.Parse(item.ItemName.Split('t')[1]), petName, petRace, petColor, 0, 120, 100, 0, timestamp);
                                        session.GetHabbo().GetInventoryManager().AddPet(pet);
                                        session.GetHabbo().Pets.Add(petId, pet);
                                    }
                                }
                                break;
                            case "teleport":
                            default:
                                {
                                    foreach(uint itemId_ in DatabaseUtils.CreateItems(session.GetHabbo().ID, item, extraData, amount))
                                    {
                                        newFloorItems.Add(itemId_);
                                        session.GetHabbo().GetInventoryManager().AddInventoryItemToHand(new InventoryItem(itemId_, item.ID, extraData));
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "h":
                    {
                        double hcLenghtInSecounds = (2678400.0 * amount);

                        string clubType = "habbo_club";
                        if (item.ItemName.StartsWith("DEAL_VIP")) //bought vip days
                        {
                            clubType = "habbo_vip";

                            if (!session.GetHabbo().IsVIP() && session.GetHabbo().IsHC()) //is upgrade
                            {
                                hcLenghtInSecounds += session.GetHabbo().GetSubscriptionManager().TryGetSubscription("habbo_club", false, true).SecoundsLeft() / 1.67;

                                session.GetHabbo().GetSubscriptionManager().EndSubscription("habbo_club"); //R.I.P. HC
                            }

                            Skylight.GetGame().GetAchievementManager().AddAchievement(session, "HCMember", 1);
                            Skylight.GetGame().GetAchievementManager().AddAchievement(session, "VIPMember", 1);

                            session.GetHabbo().GetUserAchievements().CheckAchievement("HCMember");
                            session.GetHabbo().GetUserAchievements().CheckAchievement("VIPMember");
                        }
                        else //bought hc days
                        {
                            if (session.GetHabbo().IsVIP() && !session.GetHabbo().IsHC()) //is downgrade
                            {
                                hcLenghtInSecounds += session.GetHabbo().GetSubscriptionManager().TryGetSubscription("habbo_vip", false, true).SecoundsLeft() * 1.67;

                                session.GetHabbo().GetSubscriptionManager().EndSubscription("habbo_vip"); //R.I.P. VIP
                            }

                            Skylight.GetGame().GetAchievementManager().AddAchievement(session, "HCMember", 1);

                            session.GetHabbo().GetUserAchievements().CheckAchievement("HCMember");
                        }

                        session.GetHabbo().GetSubscriptionManager().AddSubscription(clubType, hcLenghtInSecounds);

                        session.SendMessage(OutgoingPacketsEnum.ClubMembership, new ValueHolder("Session", session, "ClubType", clubType));
                        session.SendMessage(BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.Fuserights).Handle(new ValueHolder().AddValue("Session", session)));
                        break;
                    }
                default:
                    {
                        session.SendNotif("Invalid item type... (" + item.Type + ") Please don't try buy this item again!");
                        break;
                    }
            }

            if (newPets.Count > 0)
            {
                session.GetHabbo().GetUserAchievements().CheckAchievement("PetOwner");
            }

            session.SendMessage(BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.NewItems).Handle(new ValueHolder("Floors", newFloorItems, "Walls", newWallItems, "Pets", newPets)));
            if (queueBytes)
            {
                session.GetHabbo().GetInventoryManager().SetQueueBytes(false);
            }
        }

        //public void AddItem(GameClient session, Item item, int amount, string extraData, bool newFurni, uint itemId = 0)
        //{
        //    switch (item.Type)
        //    {
        //        case "i":
        //        case "s":
        //            {
        //                session.GetHabbo().GetInventoryManager().SetQueueBytes(true);

        //                int i = 0;
        //                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
        //                Message.Init(r63aOutgoing.NewItemAdded);
        //                Message.AppendInt32(1);
        //                if (item.InteractionType.ToLower() == "pet")
        //                {
        //                    Message.AppendInt32(3);
        //                }
        //                else
        //                {
        //                    Message.AppendInt32(item.Type == "s" ? 1 : 2);
        //                }
        //                Message.AppendInt32(item.InteractionType.ToLower() == "teleport" ? amount * 2 : amount);

        //                while (i < amount)
        //                {
        //                    i++;

        //                    switch(item.InteractionType.ToLower())
        //                    {
        //                        case "dimmer":
        //                            {
        //                                uint itemId_ = session.GetHabbo().GetInventoryManager().AddItem(itemId, item.ID, extraData, newFurni).ID;
        //                                Message.AppendUInt(itemId_);

        //                                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
        //                                {
        //                                    dbClient.AddParamWithValue("itemId", itemId_);
        //                                    dbClient.ExecuteQuery("INSERT INTO items_moodlight(item_id, enabled, current_preset, preset_one, preset_two, preset_three) VALUES(@itemId, '0', '1', '#000000,255,0', '#000000,255,0', '#000000,255,0')");
        //                                }
        //                            }
        //                            break;
        //                        case "pet":
        //                            {
        //                                string[] data = extraData.Split('\n');
        //                                string petName = data[0];
        //                                string petRace = data[1];
        //                                string petColor = data[2];
        //                                double timestamp = TimeUtilies.GetUnixTimestamp();
        //                                uint petId = 0;

        //                                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
        //                                {
        //                                    dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
        //                                    dbClient.AddParamWithValue("name", petName);
        //                                    dbClient.AddParamWithValue("race", petRace);
        //                                    dbClient.AddParamWithValue("color", petColor);
        //                                    dbClient.AddParamWithValue("type", int.Parse(item.ItemName.Split('t')[1]));
        //                                    dbClient.AddParamWithValue("timestamp", timestamp);
        //                                    petId = (uint)dbClient.ExecuteQuery("INSERT INTO user_pets(user_id, name, race, color, type, create_timestamp, expirience, energy, happiness, respect) VALUES(@userId, @name, @race, @color, @type, @timestamp, 0, 120, 100, 0)");
        //                                }

        //                                if (petId > 0)
        //                                {
        //                                    Pet pet = new Pet(petId, session.GetHabbo().ID, int.Parse(item.ItemName.Split('t')[1]), petName, petRace, petColor, 0, 120, 100, 0, timestamp);
        //                                    session.GetHabbo().GetInventoryManager().AddPet(pet);
        //                                }

        //                                Message.AppendUInt(petId);
        //                            }
        //                            break;
        //                        case "teleport":
        //                            {
        //                                uint itemId_ = session.GetHabbo().GetInventoryManager().AddItem(itemId, item.ID, extraData, newFurni).ID;
        //                                Message.AppendUInt(itemId_);

        //                                uint itemId_2 = session.GetHabbo().GetInventoryManager().AddItem(0, item.ID, extraData, newFurni).ID; //teleport pair, id is unknown allways
        //                                Message.AppendUInt(itemId_2);

        //                                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
        //                                {
        //                                    dbClient.AddParamWithValue("tele1", itemId_);
        //                                    dbClient.AddParamWithValue("tele2", itemId_2);
        //                                    dbClient.ExecuteQuery("INSERT INTO items_teleports_links(tele_one_id, tele_two_id) VALUES(@tele1, @tele2)");
        //                                }
        //                                break;
        //                            }
        //                        default:
        //                            {
        //                                uint itemId_ = session.GetHabbo().GetInventoryManager().AddItem(itemId, item.ID, extraData, newFurni).ID;
        //                                Message.AppendUInt(itemId_);
        //                            }
        //                            break;
        //                    }
        //                }

        //                session.SendMessage(Message);
        //                //session.GetHabbo().GetInventoryManager().UpdateInventoryItems(true);
        //                session.GetHabbo().GetInventoryManager().SetQueueBytes(false);
        //                break;
        //            }
        //        case "h":
        //            {
        //                double hcLenghtInSecounds = (2678400.0 * amount);

        //                string clubType = "habbo_club";
        //                if (item.ItemName.StartsWith("DEAL_VIP")) //bought vip days
        //                {
        //                    clubType = "habbo_vip";

        //                    if (!session.GetHabbo().IsVIP() && session.GetHabbo().IsHC()) //is upgrade
        //                    {
        //                        hcLenghtInSecounds += session.GetHabbo().GetSubscriptionManager().TryGetSubscription("habbo_club", false, true).SecoundsLeft() / 1.67;

        //                        session.GetHabbo().GetSubscriptionManager().EndSubscription("habbo_club"); //R.I.P. HC
        //                    }

        //                    Skylight.GetGame().GetAchievementManager().AddAchievement(session, "HCMember", 1);
        //                    Skylight.GetGame().GetAchievementManager().AddAchievement(session, "VIPMember", 1);
        //                    session.GetHabbo().GetUserAchievements().CheckAchievement("HCMember");
        //                    session.GetHabbo().GetUserAchievements().CheckAchievement("VIPMember");
        //                }
        //                else //bought hc days
        //                {
        //                    if (session.GetHabbo().IsVIP() && !session.GetHabbo().IsHC()) //is downgrade
        //                    {
        //                        hcLenghtInSecounds += session.GetHabbo().GetSubscriptionManager().TryGetSubscription("habbo_vip", false, true).SecoundsLeft() * 1.67;

        //                        session.GetHabbo().GetSubscriptionManager().EndSubscription("habbo_vip"); //R.I.P. VIP
        //                    }

        //                    Skylight.GetGame().GetAchievementManager().AddAchievement(session, "HCMember", 1);
        //                    session.GetHabbo().GetUserAchievements().CheckAchievement("HCMember");
        //                }
                        
        //                session.GetHabbo().GetSubscriptionManager().AddSubscription(clubType, hcLenghtInSecounds);

        //                Subscription subscription = session.GetHabbo().GetSubscriptionManager().TryGetSubscription(clubType, false, true);
        //                int daysLeft = subscription.DaysLeft();
        //                int monthsLeft = daysLeft / 31;
        //                if (monthsLeft >= 1)
        //                {
        //                    monthsLeft--;
        //                }

        //                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
        //                Message.Init(r63aOutgoing.SendClubMembership);
        //                Message.AppendString(clubType == "habbo_vip" ? "habbo_club" : clubType);
        //                Message.AppendInt32(daysLeft - (monthsLeft * 31)); //club days left
        //                Message.AppendInt32(0); //un used
        //                Message.AppendInt32(monthsLeft); //club months left
        //                Message.AppendInt32(0); //response type
        //                Message.AppendBoolean(false); //unknown bool
        //                Message.AppendBoolean(session.GetHabbo().IsVIP()); //is vip
        //                Message.AppendInt32(0); //hc club gifts(?)
        //                Message.AppendInt32(0); //vip club gifts(?)
        //                Message.AppendBoolean(false); //show promo
        //                Message.AppendInt32(10); //normal price
        //                Message.AppendInt32(0); //promo price
        //                session.SendMessage(Message);

        //                session.SendMessage(Skylight.GetPacketManager().GetOutgouings().Fuserights(session));
        //                break;
        //            }
        //        default:
        //            {
        //                session.SendNotif("Invalid item type... (" + item.Type + ") Please don't try buy this item again!");
        //                break;
        //            }
        //    }
        //}

        public List<PetRace> GetPetRaces(int raceId)
        {
            List<PetRace> races = null;
            this.PetRaces.TryGetValue(raceId, out races);
            if (races == null)
            {
                races = new List<PetRace>();
            }
            return races;
        }

        public MarketplaceManager GetMarketplaceManager()
        {
            return this.MarketplaceManager;
        }

        public CatalogItem GetItemByID(uint itemId)
        {
            CatalogItem item = null;
            this.CatalogItems.TryGetValue(itemId, out item);
            return item;
        }

        public List<Item> GetNewGifts()
        {
            return this.NewPresents.Values.ToList();
        }

        public void Shutdown()
        {
            if (this.CatalogPages != null)
            {
                this.CatalogPages.Clear();
            }
            this.CatalogPages = null;

            if (this.CatalogItems != null)
            {
                this.CatalogItems.Clear();
            }
            this.CatalogItems = null;

            if (this.PetRaces != null)
            {
                this.PetRaces.Clear();
            }
            this.PetRaces = null;

            if (this.OldPresents != null)
            {
                this.OldPresents.Clear();
            }
            this.OldPresents = null;

            if (this.NewPresents != null)
            {
                this.NewPresents.Clear();
            }
            this.NewPresents = null;

            if (this.MarketplaceManager != null)
            {
                this.MarketplaceManager.Shutdown();
            }
            this.MarketplaceManager = null;
        }
    }
}
