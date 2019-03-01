using SkylightEmulator.Communication;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Pets;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Inventory
{
    public class InventoryManager
    {
        public static readonly double SEND_ITEMS_PER_PACKETS = 1000;

        private GameClient Session;
        private Dictionary<uint, InventoryItem> FloorItems;
        private Dictionary<uint, InventoryItem> WallItems;
        private Dictionary<uint, Pet> Pets;
        private List<ArraySegment<byte>> BytesWaitingToBeSend;
        private bool QueueBytes;
        public int OldSchoolHandPage;

        public InventoryManager(GameClient session, UserDataFactory dataFactory)
        {
            this.Session = session;

            this.FloorItems = new Dictionary<uint, InventoryItem>();
            this.WallItems = new Dictionary<uint, InventoryItem>();
            this.Pets = new Dictionary<uint, Pet>();
            this.BytesWaitingToBeSend = new List<ArraySegment<byte>>();
            this.QueueBytes = false;

            this.FloorItems.Clear();
            this.WallItems.Clear();

            DataTable inventoryItems = dataFactory.GetInventoryItems();
            if (inventoryItems != null && inventoryItems.Rows.Count > 0)
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

            DataTable pets = dataFactory.GetPets();
            if (pets != null && pets.Rows.Count > 0)
            {
                foreach(DataRow dataRow in pets.Rows)
                {
                    if ((uint)dataRow["room_id"] == 0)
                    {
                        Pet pet = new Pet((uint)dataRow["id"], (uint)dataRow["user_id"], (int)dataRow["type"], (string)dataRow["name"], (string)dataRow["race"], (string)dataRow["color"], (int)dataRow["expirience"], (int)dataRow["energy"], (int)dataRow["happiness"], (int)dataRow["respect"], (double)dataRow["create_timestamp"]);
                        this.Pets.Add(pet.ID, pet);
                    }
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
            if (inventoryItems != null && inventoryItems.Rows.Count > 0)
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

        public void UpdateInventoryItems(bool loadDataFromDB)
        {
            if (loadDataFromDB)
            {
                this.LoadItems();
            }

            if (this.Session != null)
            {
                this.SendData(new InventoryRefreshComposerHandler());
            }
        }

        public void UpdateInventryPets(bool loadDataFromDB)
        {
            if (loadDataFromDB)
            {

            }
        }

        public InventoryItem AddItem(uint id, uint baseItem, string extraData, bool newFurni)
        {
            if (newFurni)
            {
                if (id == 0)
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
                        dbClient.AddParamWithValue("baseitem", baseItem);
                        dbClient.AddParamWithValue("extradata", extraData);
                        dbClient.AddParamWithValue("itemId", id);
                        dbClient.ExecuteQuery("INSERT INTO items(id, user_id, base_item, extra_data) VALUES (@itemId, @userid, @baseitem, @extradata)");
                    }
                }
            }
            else
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userid", this.Session.GetHabbo().ID);
                    dbClient.AddParamWithValue("itemid", id);
                    dbClient.ExecuteQuery("UPDATE items SET room_id = 0, user_id = @userid WHERE id = @itemid LIMIT 1");
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

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.AddItemToHand);
            item.Serialize(Message);
            this.SendData(Message);

            return item;
        }

        public void SendData(OutgoingHandler message)
        {
            this.SendData(this.Session.GetPacketManager().GetNewOutgoing(message));
        }

        public void SendData(ServerMessage message)
        {
            if (message.GetRevision() == this.Session.Revision)
            {
                if (this.QueueBytes)
                {
                    this.BytesWaitingToBeSend.Add(new ArraySegment<byte>(message.GetBytes()));
                }
                else
                {
                    this.Session.SendMessage(message);
                }
            }
            else
            {
                Logging.WriteLine("INVALID PACKET! Packet id: " + message.GetID() + ", Packet Revision: " + message.GetRevision() + ", Destiny Revision: " + this.Session.Revision, ConsoleColor.Red);
            }
        }

        public void SetQueueBytes(bool queue)
        {
            if (queue)
            {
                this.QueueBytes = true;
            }
            else
            {
                this.QueueBytes = false;

                if (this.BytesWaitingToBeSend.Count > 0)
                {
                    this.Session.SendData(this.BytesWaitingToBeSend);
                    this.BytesWaitingToBeSend.Clear();
                }
            }
        }

        public ServerMessage SerializeAllItems()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(this.Session.Revision);
            if (message.GetRevision() == Revision.PRODUCTION_201601012205_226667486)
            {
                message.Init(r63cOutgoing.InventoryItems);
            }
            else
            {
                message.Init(r63aOutgoing.InventoryItems);
                message.AppendString("S");
            }
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

        public void SerializeAllItemsSplit()
        {
            this.SetQueueBytes(true);

            Queue<InventoryItem> items = new Queue<InventoryItem>(this.FloorItems.Values.Concat(this.WallItems.Values));
            int total = Math.Max(1, (int)Math.Ceiling(items.Count / InventoryManager.SEND_ITEMS_PER_PACKETS));
            for (int i = 0; i < total; i++)
            {
                int next = (int)Math.Min(InventoryManager.SEND_ITEMS_PER_PACKETS, items.Count);

                ServerMessage message = BasicUtilies.GetRevisionServerMessage(this.Session.Revision);
                if (message.GetRevision() == Revision.PRODUCTION_201601012205_226667486)
                {
                    message.Init(r63cOutgoing.InventoryItems);
                }
                else
                {
                    message.Init(message.GetRevision() == Revision.RELEASE63_35255_34886_201108111108 ? r63aOutgoing.InventoryItems : r63bOutgoing.InventoryItems);
                    message.AppendString("S");
                }
                message.AppendInt32(total);
                message.AppendInt32(i);
                message.AppendInt32(next);

                for (int j = 0; j < next; j++)
                {
                    items.Dequeue().Serialize(message);
                }
                
                this.SendData(message);
            }

            this.SetQueueBytes(false);
        }

        //public ServerMessage SerializeFloorItems()
        //{
        //    ServerMessage message = BasicUtilies.GetRevisionServerMessage(this.Session.Revision);
        //    if (message.GetRevision() == Revision.PRODUCTION_201601012205_226667486)
        //    {
        //        message.Init(r63cOutgoing.InventoryItems);
        //    }
        //    else
        //    {
        //        message.Init(r63aOutgoing.InventoryItems);
        //        message.AppendString("S");
        //    }
        //    message.AppendInt32(1);
        //    message.AppendInt32(1);
        //    message.AppendInt32(this.FloorItems.Count);
        //    foreach(InventoryItem item in this.FloorItems.Values.ToList())
        //    {
        //        item.Serialize(message);
        //    }
        //    return message;
        //}

        public void SerializeFloorItemsSplitd()
        {
            this.SetQueueBytes(true);

            Queue<InventoryItem> items = new Queue<InventoryItem>(this.FloorItems.Values);
            int total = Math.Max(1, (int)Math.Ceiling(items.Count / InventoryManager.SEND_ITEMS_PER_PACKETS));
            for (int i = 1; i <= total; i++)
            {
                int next = (int)Math.Min(InventoryManager.SEND_ITEMS_PER_PACKETS, items.Count);

                ServerMessage message = BasicUtilies.GetRevisionServerMessage(this.Session.Revision);
                if (message.GetRevision() == Revision.PRODUCTION_201601012205_226667486)
                {
                    message.Init(r63cOutgoing.InventoryItems);
                }
                else
                {
                    message.Init(message.GetRevision() == Revision.RELEASE63_35255_34886_201108111108 ? r63aOutgoing.InventoryItems : r63bOutgoing.InventoryItems);
                    message.AppendString("S");
                }
                message.AppendInt32(total);
                message.AppendInt32(i);
                message.AppendInt32(next);

                for (int j = 0; j < next; j++)
                {
                    items.Dequeue().Serialize(message);
                }

                this.SendData(message);
            }

            this.SetQueueBytes(false);
        }

        //public ServerMessage SerializeWallItems()
        //{
        //    ServerMessage message = BasicUtilies.GetRevisionServerMessage(this.Session.Revision);
        //    if (message.GetRevision() >= Revision.PRODUCTION_201601012205_226667486)
        //    {
        //        message.Init(r63cOutgoing.InventoryItems);
        //    }
        //    else
        //    {
        //        message.Init(r63aOutgoing.InventoryItems);
        //        message.AppendString("I");
        //    }
        //    message.AppendInt32(1);
        //    message.AppendInt32(1);
        //    message.AppendInt32(this.WallItems.Count);
        //    foreach (InventoryItem item in this.WallItems.Values.ToList())
        //    {
        //        item.Serialize(message);
        //    }
        //    return message;
        //}

        public void SerializeWallItemsSplitd()
        {
            this.SetQueueBytes(true);

            Queue<InventoryItem> items = new Queue<InventoryItem>(this.WallItems.Values);
            int total = Math.Max(1, (int)Math.Ceiling(items.Count / InventoryManager.SEND_ITEMS_PER_PACKETS));
            for (int i = 1; i <= total; i++)
            {
                int next = (int)Math.Min(InventoryManager.SEND_ITEMS_PER_PACKETS, items.Count);

                ServerMessage message = BasicUtilies.GetRevisionServerMessage(this.Session.Revision);
                if (message.GetRevision() == Revision.PRODUCTION_201601012205_226667486)
                {
                    message.Init(r63cOutgoing.InventoryItems);
                }
                else
                {
                    message.Init(message.GetRevision() == Revision.RELEASE63_35255_34886_201108111108 ? r63aOutgoing.InventoryItems : r63bOutgoing.InventoryItems);
                    message.AppendString("I");
                }
                message.AppendInt32(total);
                message.AppendInt32(i);
                message.AppendInt32(next);

                for (int j = 0; j < next; j++)
                {
                    items.Dequeue().Serialize(message);
                }

                this.SendData(message);
            }

            this.SetQueueBytes(false);
        }

        public InventoryItem TryGetFloorItem(uint id)
        {
            InventoryItem item;
            this.FloorItems.TryGetValue(id, out item);
            return item;
        }

        public InventoryItem TryGetWallItem(uint id)
        {
            InventoryItem item;
            this.WallItems.TryGetValue(id, out item);
            return item;
        }

        public InventoryItem TryGetItem(uint id)
        {
            InventoryItem item = this.TryGetFloorItem(id);
            if (item == null)
            {
                item = this.TryGetWallItem(id);
            }
            return item;
        }

        public List<InventoryItem> GetWallItemsByBaseID(uint baseId)
        {
            return this.WallItems.Values.Where(i => i.BaseItem == baseId).ToList();
        }
        public List<InventoryItem> GetFloorItemsByBaseID(uint baseId)
        {
            return this.FloorItems.Values.Where(i => i.BaseItem == baseId).ToList();
        }

        public List<InventoryItem> GetFloodItemsByInteractionType(string interactionType)
        {
            return this.FloorItems.Values.Where(i => i.GetItem() != null && i.GetItem().InteractionType == interactionType).ToList();
        }

        public void RemoveItemFromHand(uint id, bool delete)
        {
            if (delete)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("itemId", id);
                    dbClient.ExecuteQuery("DELETE FROM items WHERE id = @itemId LIMIT 1;");
                }
            }

            this.SendData(BasicUtilies.GetRevisionPacketManager(this.Session.Revision).GetOutgoing(OutgoingPacketsEnum.RemoveItemFromHand).Handle(new ValueHolder("InventoryManager", this, "ID", id)));

            this.FloorItems.Remove(id);
            this.WallItems.Remove(id);
        }

        public void AddRoomItemToHand(RoomItem item)
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

            this.SendData(BasicUtilies.GetRevisionPacketManager(this.Session.Revision).GetOutgoing(OutgoingPacketsEnum.AddItemToInventory).Handle(new ValueHolder("InventoryManager", this, "Item", inventoryItem)));
        }


        public void AddInventoryItemToHand(InventoryItem inventoryItem)
        {
            if (inventoryItem.GetItem().IsWallItem)
            {
                this.WallItems.Add(inventoryItem.ID, inventoryItem);
            }
            else
            {
                this.FloorItems.Add(inventoryItem.ID, inventoryItem);
            }

            this.SendData(BasicUtilies.GetRevisionPacketManager(this.Session.Revision).GetOutgoing(OutgoingPacketsEnum.AddItemToInventory).Handle(new ValueHolder("InventoryManager", this, "Item", inventoryItem)));
        }

        public void DeleteAllItems()
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userid", this.Session.GetHabbo().ID);
                dbClient.ExecuteQuery("DELETE FROM items WHERE room_id = 0 AND user_id = @userid");
            }

            this.UpdateInventoryItems(true); //lets just update inventory for sure :)
        }

        public void DeleteAllPets()
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userid", this.Session.GetHabbo().ID);
                dbClient.ExecuteQuery("DELETE FROM user_pets WHERE room_id = 0 AND user_id = @userid");
            }

            this.SendData(this.SerializePets());
        }

        public ServerMessage SerializePets()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.InventoryPets);
            message.AppendInt32(this.Pets.Count);
            foreach(Pet pet in this.Pets.Values.ToList())
            {
                message.AppendUInt(pet.ID);
                message.AppendString(pet.Name);
                message.AppendInt32(pet.Type);
                message.AppendInt32(int.Parse(pet.Race));
                message.AppendString(pet.Color);
            }
            return message;
        }

        public void AddPet(Pet pet)
        {
            this.Pets.Add(pet.ID, pet);

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.AddPet);
            message.AppendUInt(pet.ID);
            message.AppendString(pet.Name);
            message.AppendInt32(pet.Type);
            message.AppendInt32(int.Parse(pet.Race));
            message.AppendString(pet.Color);
            this.SendData(message);
        }

        public Pet TryGetPet(uint petId)
        {
            Pet pet = null;
            this.Pets.TryGetValue(petId, out pet);
            return pet;
        }

        public void RemovePet(Pet pet)
        {
            this.Pets.Remove(pet.ID);

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.RemovePet);
            message.AppendUInt(pet.ID);
            this.SendData(message);
        }

        public void SavePetData()
        {
            string query = "";
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                foreach (Pet pet in this.Pets.Values.ToList())
                {
                    if (pet.NeedUpdate)
                    {
                        dbClient.AddParamWithValue("petId" + pet.ID, pet.ID);
                        dbClient.AddParamWithValue("expirience" + pet.ID, pet.Expirience);
                        dbClient.AddParamWithValue("energy" + pet.ID, pet.Energy);
                        dbClient.AddParamWithValue("happiness" + pet.ID, pet.Happiness);
                        dbClient.AddParamWithValue("respect" + pet.ID, pet.Respect);

                        query += "UPDATE user_pets SET room_id = '0', expirience = @expirience" + pet.ID + ", energy = @energy" + pet.ID + ", happiness = @happiness" + pet.ID + ", respect = @respect" + pet.ID + ", x = '0', y = '0', z = '0' WHERE id = @petId" + pet.ID + " LIMIT 1; ";
                    }

                    if (query.Length > 0)
                    {
                        dbClient.ExecuteQuery(query);
                    }
                }
            }
        }

        public void ConvertCredits()
        {
            this.SetQueueBytes(true);

            int credits = 0;
            List<uint> credits_ = new List<uint>();
            foreach(InventoryItem item in this.FloorItems.Values.ToList())
            {
                if (item != null && (item.GetItem().ItemName.StartsWith("CF_") || item.GetItem().ItemName.StartsWith("CFC_")))
                {
                    if (!credits_.Contains(item.ID))
                    {
                        credits += int.Parse(item.GetItem().ItemName.Split('_')[1]);
                        credits_.Add(item.ID);

                        this.RemoveItemFromHand(item.ID, false); //delete false bcs we use our own stuff to delete them
                    }
                }
            }

            if (credits_.Count > 0)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", this.Session.GetHabbo().ID);
                    dbClient.ExecuteQuery("DELETE FROM items WHERE id IN(" + string.Join(",", credits_) + ") LIMIT " + credits_.Count);
                }

                this.Session.GetHabbo().Credits += credits;
                this.Session.GetHabbo().UpdateCredits(true);
                this.Session.SendNotif("All coins in your inventory have been converted back into " + credits + " credits!");
            }

            this.SetQueueBytes(false);
        }

        public int TotalSize
        {
            get
            {
                return this.WallItems.Count + this.FloorItems.Count;
            }
        }

        public IEnumerable<InventoryItem> OldSchoolGetCurrentHand()
        {
            return this.FloorItems.Values.Concat(this.WallItems.Values).OrderBy(i => i.ID).Skip(this.OldSchoolHandPage * 9).Take(9);
        }

        public ServerMessage OldSchoolGetHand(string mode)
        {
            switch(mode)
            {
                case "next":
                    {
                        this.OldSchoolHandPage = Math.Min((this.TotalSize - 1) / 9, this.OldSchoolHandPage + 1);
                    }
                    break;
                case "prev":
                    {
                        this.OldSchoolHandPage = Math.Max(0, this.OldSchoolHandPage - 1);
                    }
                    break;
                case "last":
                    {
                        this.OldSchoolHandPage = (this.TotalSize - 1) / 9;
                    }
                    break;
                case "update":
                    {
                        this.OldSchoolHandPage = Math.Min((this.TotalSize - 1) / 9, this.OldSchoolHandPage);
                    }
                    break;
                case "new":
                    {
                        this.OldSchoolHandPage = 0;
                    }
                    break;
            }

            int loller = this.OldSchoolHandPage * 9;
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message_.Init(r26Outgoing.CurrenttHand);
            foreach(InventoryItem item in this.OldSchoolGetCurrentHand())
            {
                message_.AppendString("SI", 30);
                message_.AppendString(item.ID.ToString(), 30);
                message_.AppendString(loller++.ToString(), 30);
                message_.AppendString(item.GetItem().IsFloorItem ? "S" : "I", 30);
                message_.AppendString(item.ID.ToString(), 30);
                message_.AppendString(item.GetItem().ItemName, 30);

                if (item.GetItem().IsFloorItem)
                {
                    message_.AppendString(item.GetItem().Lenght.ToString(), 30);
                    message_.AppendString(item.GetItem().Width.ToString(), 30);
                    message_.AppendString("", 30);
                    message_.AppendString("0,0,0", 30);
                    message_.AppendString(item.GetItem().ItemName, 30);
                }
                else
                {
                    message_.AppendString("0,0,0", 30);
                    message_.AppendString(item.GetItem().AllowRecycle ? "1" : "0", 30);
                }

                message_.AppendString("/", null);
            }
            message_.AppendString(Convert.ToChar(13) + this.TotalSize.ToString(), null);
            return message_;
        }
    }
}
