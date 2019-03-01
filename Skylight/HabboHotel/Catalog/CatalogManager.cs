using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Catalog
{
    public class CatalogManager
    {
        private Dictionary<int, CatalogPage> CatalogPages;
        private Dictionary<uint, CatalogItem> CatalogItems;

        public CatalogManager()
        {
            this.CatalogPages = new Dictionary<int, CatalogPage>();
            this.CatalogItems = new Dictionary<uint, CatalogItem>();
        }

        public void LoadCatalogPages(DatabaseClient dbClient)
        {
            Logging.Write("Loading catalog pages... ");
            this.CatalogPages.Clear();

            DataTable pages = dbClient.ReadDataTable("SELECT * FROM catalog_pages ORDER BY order_num ASC");
            if (pages != null)
            {
                foreach(DataRow dataRow in pages.Rows)
                {
                    int id = (int)dataRow["id"];
                    this.CatalogPages.Add(id, new CatalogPage((int)dataRow["Id"], (int)dataRow["parent_id"], (string)dataRow["caption"], TextUtilies.StringToBool(dataRow["visible"].ToString()), TextUtilies.StringToBool(dataRow["enabled"].ToString()), (int)dataRow["min_rank"], TextUtilies.StringToBool(dataRow["club_only"].ToString()), (int)dataRow["icon_color"], (int)dataRow["icon_image"], (string)dataRow["page_layout"], (string)dataRow["page_headline"], (string)dataRow["page_teaser"], (string)dataRow["page_special"], (string)dataRow["page_text1"], (string)dataRow["page_text2"], (string)dataRow["page_text_details"], (string)dataRow["page_text_teaser"], (string)dataRow["page_link_description"], (string)dataRow["page_link_pagename"], this.CatalogItems.Values.Where(i => i.PageID == id).ToList()));
                }
            }
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void LoadCatalogItems(DatabaseClient dbClient)
        {
            Logging.Write("Loading catalog items... ");
            this.CatalogItems.Clear();

            DataTable items = dbClient.ReadDataTable("SELECT * FROM catalog_items");
            if (items != null)
            {
                foreach(DataRow dataRow in items.Rows)
                {
                    uint id = (uint)dataRow["id"];
                    this.CatalogItems.Add(id, new CatalogItem(id, (string)dataRow["catalog_name"], (string)dataRow["item_ids"], (int)dataRow["cost_credits"], (int)dataRow["cost_pixels"], (int)dataRow["cost_snow"], (int)dataRow["amount"], (int)dataRow["page_id"]));
                }
            }
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public ServerMessage GetIndexes(int rank)
        {
            List<CatalogPage> mainPages = this.GetParentPagesWithRankedSystem(-1, rank);

            ServerMessage indexes = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            indexes.Init(r63aOutgoing.CatalogIndexes);
            indexes.AppendBoolean(true); //visible
            indexes.AppendInt32(0); //color
            indexes.AppendInt32(0); //image
            indexes.AppendInt32(-1); //page id
            indexes.AppendStringWithBreak(""); //name
            indexes.AppendInt32(mainPages.Count); //count sub indexes

            foreach(CatalogPage page in mainPages)
            {
                if (page.ParentID == -1)
                {
                    page.Serialize(indexes);
                    page.SerializePageInfo();

                    foreach (CatalogPage page2 in this.GetParentPagesWithRankedSystem(page.PageID, rank))
                    {
                        page2.Serialize(indexes);
                        page2.SerializePageInfo();
                    }
                }
            }

            indexes.AppendBoolean(false); //new
            return indexes;
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

        public void BuyItem(GameClient session, int pageId, uint itemId, string extraData)
        {
            CatalogPage page = this.GetCatalogPage(pageId);
            if (page != null && page.Visible & page.Enabled && page.MinRank <= session.GetHabbo().Rank)
            {
                CatalogItem item = page.GetItem(itemId);
                if (item != null)
                {
                    bool noCredits = false;
                    bool noActivityPoints = false;

                    if (session.GetHabbo().Credits < item.CostCredits)
                    {
                        noCredits = true;
                    }

                    if (session.GetHabbo().ActivityPoints[item.ActivityPointsType] < item.CostActivityPoints)
                    {
                        noActivityPoints = true;
                    }

                    if (noCredits || noActivityPoints)
                    {
                        ServerMessage noEnoughtCash = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        noEnoughtCash.Init(r63aOutgoing.NoEnoughtCash);
                        noEnoughtCash.AppendBoolean(noCredits);
                        noEnoughtCash.AppendBoolean(noActivityPoints);
                        session.SendMessage(noEnoughtCash);
                    }
                    else
                    {
                        if (item.CostCredits > 0)
                        {
                            session.GetHabbo().Credits -= item.CostCredits;
                            session.GetHabbo().UpdateCredits(true);
                        }

                        if (item.CostActivityPoints > 0)
                        {
                            session.GetHabbo().ActivityPoints[item.ActivityPointsType] -= item.CostActivityPoints;
                            session.GetHabbo().UpdateActivityPoints(item.ActivityPointsType, true);
                        }

                        ServerMessage BuyInfo = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        BuyInfo.Init(r63aOutgoing.BuyInfo);
                        BuyInfo.AppendUInt(item.GetItem().ID);
                        BuyInfo.AppendStringWithBreak(item.GetItem().PublicName);
                        BuyInfo.AppendInt32(item.CostCredits);
                        BuyInfo.AppendInt32(item.CostActivityPoints);
                        BuyInfo.AppendInt32(item.ActivityPointsType);
                        BuyInfo.AppendInt32(1);
                        BuyInfo.AppendStringWithBreak(item.GetItem().Type.ToString());
                        BuyInfo.AppendInt32(item.GetItem().SpriteID);
                        BuyInfo.AppendStringWithBreak("");
                        BuyInfo.AppendInt32(1);
                        BuyInfo.AppendInt32(-1);
                        BuyInfo.AppendStringWithBreak("");
                        session.SendMessage(BuyInfo);

                        this.AddItem(session, item.GetItem(), item.Amount, extraData);
                    }
                }
            }
        }

        public void AddItem(GameClient session, Item item, int amount, string extraData)
        {
            switch (item.Type)
            {
                case "i":
                case "s":
                    int i = 0;
                    ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    Message.Init(r63aOutgoing.NewItemAdded);
                    Message.AppendInt32(1);
                    Message.AppendInt32(item.Type == "s" ? 1 : 2);
                    Message.AppendInt32(amount);

                    while (i < amount)
                    {
                        i++;

                        uint itemId = session.GetHabbo().GetInventoryManager().AddItem(0u, item.ID, extraData, true).ID;
                        Message.AppendUInt(itemId);
                    }

                    session.GetHabbo().GetInventoryManager().UpdateInventory(true);
                    session.SendMessage(Message);
                    break;
            }
        }
    }
}
