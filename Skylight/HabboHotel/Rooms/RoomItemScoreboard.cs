using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemScoreboard : RoomItem
    {
        public RoomItemScoreboard(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            this.EditScore(request);
        }

        public void EditScore(int type)
        {
            int score = 0;
            int.TryParse(this.ExtraData, out score);

            switch (type)
            {
                case 1: //add
                    {
                        if (score < 99)
                        {
                            score++;
                        }
                        else
                        {
                            score = 0;
                        }
                    }
                    break;
                case 2: //remove
                    {
                        if (score > 0)
                        {
                            score--;
                        }
                    }
                    break;
                case 3: //reset
                    {
                        score = 0;
                    }
                    break;
            }

            this.ExtraData = score.ToString();
            this.UpdateState(true, true);
        }
    }
}
