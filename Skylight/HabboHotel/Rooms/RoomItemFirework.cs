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

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemFirework : RoomItem
    {
        public int Charges;
        private readonly int ChargingCostCredits = 0;
        private readonly int ChargingCostActivityPoints = 20;
        private readonly int ChargingCostActivityPointsType = 0;
        private readonly int ChargingAtOnce = 10;

        public RoomItemFirework(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (request == 0)
            {
                if (this.Charges > 0)
                {
                    item.ExtraData = "2";
                    item.UpdateState(false, true);
                    item.DoUpdate(14);

                    this.Charges--;
                    this.Room.RoomItemManager.ItemDataChanged.AddOrUpdate(this.ID, this, (key, oldValue) => this);
                }
            }
            else if (request == 1)
            {
                this.ShowGUI(session);
            }
            else if (request == 2)
            {
                if (session.GetHabbo().TryGetActivityPoints(this.ChargingCostActivityPointsType) >= this.ChargingCostActivityPoints)
                {
                    session.GetHabbo().RemoveActivityPoints(this.ChargingCostActivityPointsType, this.ChargingCostActivityPoints);
                    session.GetHabbo().UpdateActivityPoints(this.ChargingCostActivityPointsType, true);
                    if (this.ChargingCostActivityPointsType == 0)
                    {
                        session.GetHabbo().GetUserStats().FireworksCharger += this.ChargingCostActivityPoints;
                        session.GetHabbo().GetUserAchievements().CheckAchievement("FireworksCharger");
                    }

                    this.Charges += this.ChargingAtOnce;
                    this.Room.RoomItemManager.ItemDataChanged.AddOrUpdate(this.ID, this, (key, oldValue) => this);

                    if (this.ExtraData == "0")
                    {
                        this.ExtraData = "1";
                        this.UpdateState(false, true);
                    }

                    this.ShowGUI(session);
                }
            }
        }

        public void ShowGUI(GameClient session)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.FireworkGUI);
            message.AppendUInt(this.ID);
            message.AppendInt32(this.Charges);
            message.AppendInt32(this.ChargingCostCredits);
            message.AppendInt32(this.ChargingCostActivityPoints);
            message.AppendInt32(this.ChargingCostActivityPointsType);
            message.AppendInt32(this.ChargingAtOnce);
            session.SendMessage(message);
        }

        public override void OnLoad()
        {
            if (this.Charges > 0)
            {
                this.ExtraData = "1";
            }
            else
            {
                this.ExtraData = "0";
            }
        }

        public override void OnPickup(GameClient session)
        {
            if (this.Room.RoomItemManager.ItemDataChanged.ContainsKey(this.ID))
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("itemId", this.ID);
                    dbClient.AddParamWithValue("data", this.Charges);
                    dbClient.ExecuteQuery("INSERT INTO items_data(item_id, data) VALUES(@itemId, @data) ON DUPLICATE KEY UPDATE data = VALUES(data);");
                }
            }

            if (this.Charges > 0)
            {
                this.ExtraData = "1";
            }
            else
            {
                this.ExtraData = "0";
            }
        }

        public override void OnPlace(GameClient session)
        {
            DataRow dataRow = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("itemId", this.ID);
                dataRow = dbClient.ReadDataRow("SELECT data FROM items_data WHERE item_id = @itemId LIMIT 1");
            }

            if (dataRow != null)
            {
                if (!int.TryParse((string)dataRow["data"], out this.Charges))
                {
                    this.Charges = 0;
                }
            }

            if (this.Charges > 0)
            {
                this.ExtraData = "1";
            }
            else
            {
                this.ExtraData = "0";
            }
        }

        public override void LoadItemData(string data)
        {
            if (!int.TryParse(data, out this.Charges))
            {
                this.Charges = 0;
                this.ExtraData = "0";
            }
            else
            {
                if (this.Charges > 0)
                {
                    this.ExtraData = "1";
                }
                else
                {
                    this.ExtraData = "0";
                }
            }

            this.UpdateState(false, true);
        }

        public override string GetItemData()
        {
            return this.Charges.ToString();
        }

        public override void OnCycle()
        {
            if (this.UpdateNeeded)
            {
                this.UpdateTimer--;
                if (this.UpdateTimer <= 0)
                {
                    this.UpdateNeeded = false;

                    if (this.Charges > 0)
                    {
                        this.ExtraData = "1";
                    }
                    else
                    {
                        this.ExtraData = "0";
                    }
                    this.UpdateState(false, true);
                }
            }
        }
    }
}
