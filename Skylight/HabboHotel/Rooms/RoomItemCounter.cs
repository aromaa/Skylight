using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemCounter : RoomItem
    {
        public RoomItemCounter(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (userHasRights)
            {
                if (request == 0) //play or stop, called by wired
                {
                    this.Room.RoomGameManager.ToggleGameStatus();
                }
                else if (request == 1) //play or stop, called by user
                {
                    this.Room.RoomGameManager.ToggleGameStatus();
                }
                else if (request == 2) //add time
                {
                    double currentTime = 0;
                    double.TryParse(this.ExtraData, out currentTime);

                    currentTime += 30;
                    if (currentTime >= 660)
                    {
                        currentTime = 0;
                    }

                    this.ExtraData = currentTime.ToString();
                    this.UpdateState(true, true);
                }
            }
        }

        public override void OnCycle()
        {
            if (this.Room.RoomFreezeManager.GameStarted)
            {
                double currentTime = 0;
                double.TryParse(this.ExtraData, out currentTime);

                if (currentTime > 0.5)
                {
                    currentTime -= 0.5;
                }
                else
                {
                    currentTime = 0;
                    this.Room.RoomGameManager.StopGame();
                }

                this.ExtraData = currentTime.ToString();
                this.UpdateState(true, true);
            }
        }
    }
}
