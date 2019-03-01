using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Catalog;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel
{
    public class Game
    {
        private GameClientManager GameClientManager;
        private NavigatorManager NavigatorManager;
        private RoomManager RoomManager;
        private CatalogManager CatalogManager;
        private ItemManager ItemManager;

        private Task GameCycleTask;

        public Game()
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                this.GameClientManager = new GameClientManager();

                this.NavigatorManager = new NavigatorManager();
                this.NavigatorManager.LoadPublicRooms(dbClient);

                this.RoomManager = new RoomManager();
                this.RoomManager.LoadRoomModels(dbClient);

                this.ItemManager = new ItemManager();
                this.ItemManager.LoadItems(dbClient);

                this.CatalogManager = new CatalogManager();
                this.CatalogManager.LoadCatalogItems(dbClient);
                this.CatalogManager.LoadCatalogPages(dbClient);
            }


            this.GameCycleTask = new Task(new Action(this.GameCycle));
            this.GameCycleTask.Start();
        }

        public async void GameCycle()
        {
            while (!Skylight.ServerShutdown)
            {
                try
                {
                    if (this.RoomManager.RoomCycleTask == null ||this.RoomManager.RoomCycleTask.IsCompleted)
                    {
                        this.RoomManager.RoomCycleTask = new Task(new Action(this.RoomManager.OnCycle));
                        this.RoomManager.RoomCycleTask.Start();
                    }
                }
                catch(Exception ex)
                {
                    Logging.LogException("Error in game cycle! " + ex.ToString());
                }

                await Task.Delay(25);
            }
        }

        public GameClientManager GetGameClientManager()
        {
            return this.GameClientManager;
        }

        public NavigatorManager GetNavigatorManager()
        {
            return this.NavigatorManager;
        }

        public RoomManager GetRoomManager()
        {
            return this.RoomManager;
        }

        public CatalogManager GetCatalogManager()
        {
            return this.CatalogManager;
        }

        public ItemManager GetItemManager()
        {
            return this.ItemManager;
        }
    }
}
