using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Inventory
{
    public class InventoryManager
    {
        private GameClient Session;
        private Dictionary<uint, InventoryItem> FloorItems;
        private Dictionary<uint, InventoryItem> WallItems;

        public InventoryManager(GameClient session, UserDataFactory dataFactory)
        {
            this.Session = session;

            this.FloorItems = new Dictionary<uint, InventoryItem>();
            this.WallItems = new Dictionary<uint, InventoryItem>();

            this.FloorItems.Clear();
            this.WallItems.Clear();

            DataTable inventoryItems = dataFactory.GetInventoryItems();
            if (inventoryItems != null)
            {
                foreach (DataRow dataRow in inventoryItems.Rows)
                {
                    InventoryItem item = new InventoryItem((uint)dataRow["id"], (uint)dataRow["base_item"], (string)dataRow["extra_data"]);
                    this.FloorItems.Add(item.ID, item);
                }
            }
        }

        public void LoadItems()
        {
            DataTable inventoryItems = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userid", this.Session.GetHabbo().ID);
                inventoryItems = dbClient.ReadDataTable("SELECT id, base_item, extra_data FROM items WHERE room_id = 0 AND user_id = @userid");
            }

            this.FloorItems.Clear();
            this.WallItems.Clear();
            if (inventoryItems != null)
            {
                foreach (DataRow dataRow in inventoryItems.Rows)
                {
                    InventoryItem item = new InventoryItem((uint)dataRow["id"], (uint)dataRow["base_item"], (string)dataRow["extra_data"]);
                    if (item.GetItem().IsWallItem)
                    {
                        this.WallItems.Add(item.ID, item);
                    }
                    else
                    {
                        this.FloorItems.Add(item.ID, item);
                    }
                }
            }
        }

        public void UpdateInventory(bool loadDataFromDB)
        {
            if (loadDataFromDB)
            {
                this.LoadItems();
            }

            if (this.Session != null)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                message.Init(r63aOutgoing.InventoryUpdate);
                this.Session.SendMessage(message);
            }
        }

        public InventoryItem AddItem(uint id, uint baseItem, string extraData, bool newFurni)
        {
            if (newFurni)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userid", this.Session.GetHabbo().ID);
                    dbClient.AddParamWithValue("baseitem", baseItem);
                    dbClient.AddParamWithValue("extradata", extraData);
                    dbClient.ExecuteQuery("INSERT INTO items(user_id, base_item, extra_data) VALUES (@userid, @baseitem, @extradata)");

                    id = (uint)dbClient.GetID();
                }
            }
            else
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userid", this.Session.GetHabbo().ID);
                    dbClient.AddParamWithValue("itemid", id);
                    dbClient.ExecuteQuery("UPDATE items SET room_id = 0 WHERE user_id = @userid AND id = @itemid LIMIT 1");
                }
            }

            InventoryItem item = new InventoryItem(id, baseItem, extraData);
            if (item.GetItem().IsWallItem)
            {
                this.WallItems.Add(item.ID, item);
            }
            else
            {
                this.FloorItems.Add(item.ID, item);
            }

            return item;
        }

        public ServerMessage SerializeAllItems()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            message.Init(r63aOutgoing.InventoryItems);
            message.AppendStringWithBreak("S");
            message.AppendInt32(1);
            message.AppendInt32(1);
            message.AppendInt32(this.FloorItems.Count + this.WallItems.Count);
            foreach (InventoryItem item in this.FloorItems.Values.ToList())
            {
                item.Serialize(message);
            }

            foreach (InventoryItem item in this.WallItems.Values.ToList())
            {
                item.Serialize(message);
            }
            return message;
        }

        public ServerMessage SerializeFloorItems()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            message.Init(r63aOutgoing.InventoryItems);
            message.AppendStringWithBreak("S");
            message.AppendInt32(1);
            message.AppendInt32(1);
            message.AppendInt32(this.FloorItems.Count);
            foreach(InventoryItem item in this.FloorItems.Values.ToList())
            {
                item.Serialize(message);
            }
            return message;
        }

        public ServerMessage SerializeWallItems()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            message.Init(r63aOutgoing.InventoryItems);
            message.AppendStringWithBreak("I");
            message.AppendInt32(1);
            message.AppendInt32(1);
            message.AppendInt32(this.WallItems.Count);
            foreach (InventoryItem item in this.WallItems.Values.ToList())
            {
                item.Serialize(message);
            }
            return message;
        }

        public InventoryItem GetItem(uint id)
        {
            if (this.FloorItems.ContainsKey(id))
            {
                return this.FloorItems[id];
            }
            else if (this.WallItems.ContainsKey(id))
            {
                return this.WallItems[id];
            }
            else
            {
                return null;
            }
        }

        public void RemoveItemFromHand(uint id)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.RemoveItemFromHand);
            Message.AppendUInt(id);
            this.Session.SendMessage(Message);

            this.FloorItems.Remove(id);
            this.WallItems.Remove(id);
        }

        public void AddItemToHand(RoomItem item)
        {
            InventoryItem inventoryItem = new InventoryItem(item.ID, item.BaseItem.ID, item.ExtraData);
            if (item.IsWallItem)
            {
                this.WallItems.Add(inventoryItem.ID, inventoryItem);
            }
            else
            {
                this.FloorItems.Add(inventoryItem.ID, inventoryItem);
            }
        }

        public void DeleteAllItems()
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userid", this.Session.GetHabbo().ID);
                dbClient.ExecuteQuery("DELETE FROM items WHERE room_id = 0 AND user_id = @userid");
            }

            this.UpdateInventory(true); //lets just update inventory for sure :)
        }
    }
}
