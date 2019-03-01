using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemWiredMoveToDir : RoomItemWiredAction
    {
        public List<RoomItem> SelectedItems;
        public int Direction;
        public int Action;
        public Dictionary<uint, int> ActiveDirections;

        public RoomItemWiredMoveToDir(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.SelectedItems = new List<RoomItem>();
            this.ActiveDirections = new Dictionary<uint, int>();
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (userHasRights)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.WiredAction);
                message.AppendBoolean(false); //check box toggling
                message.AppendInt32(session.GetHabbo().GetWiredActionLimit()); //furni limit
                message.AppendInt32(this.SelectedItems.Count); //furni count
                foreach (RoomItem item_ in this.SelectedItems)
                {
                    message.AppendUInt(item_.ID);
                }
                message.AppendInt32(this.GetBaseItem().SpriteID); //sprite id, show the help thing
                message.AppendUInt(this.ID); //item id
                message.AppendString(""); //data
                message.AppendInt32(2); //extra data count
                message.AppendInt32(this.Direction);
                message.AppendInt32(this.Action);
                message.AppendInt32(0); //confligts count

                message.AppendInt32(13); //type
                message.AppendInt32(this.Delay); //delay
                message.AppendInt32(0); //conflicts count
                session.SendMessage(message);
            }
        }

        public override string GetItemData()
        {
            return string.Join(",", this.SelectedItems.Select(i => i.ID)) + (char)9 + this.Direction + (char)9 + this.Action;
        }

        public override void LoadItemData(string data)
        {
            string[] splitData = data.Split((char)9);

            foreach (string sItemId in splitData[0].Split(','))
            {
                if (!string.IsNullOrEmpty(sItemId))
                {
                    RoomItem item = this.Room.RoomItemManager.TryGetRoomItem(uint.Parse(sItemId));
                    if (item != null)
                    {
                        this.SelectedItems.Add(item);
                    }
                }
            }

            this.Direction = int.Parse(splitData[1]);
            this.Action = int.Parse(splitData[2]);
        }

        public override void OnLoad()
        {
            this.ExtraData = "0";
        }

        public override void OnPickup(GameClient session)
        {
            this.ExtraData = "0";
        }

        public override void OnPlace(GameClient session)
        {
            this.ExtraData = "0";
        }

        public override void OnCycle()
        {
            if (this.UpdateNeeded)
            {
                this.UpdateTimer--;
                if (this.UpdateTimer <= 0)
                {
                    this.UpdateNeeded = false;

                    this.ExtraData = "0";
                    this.UpdateState(false, true);
                }
            }
        }

        public override void DoWiredAction(RoomUnitUser user, HashSet<uint> used)
        {
            Random random = RandomUtilies.GetRandom();

            foreach (RoomItem item_ in this.SelectedItems)
            {
                if (!this.ActiveDirections.TryGetValue(item_.ID, out int direction))
                {
                    direction = this.Direction;
                }

                for (int i = 0; i < this.TriesCount(); i++)
                {
                    int x = item_.X;
                    int y = item_.Y;

                    if (direction == 0)
                    {
                        y--;
                    }
                    else if (direction == 1)
                    {
                        y--;
                        x++;
                    }
                    else if (direction == 2)
                    {
                        x++;
                    }
                    else if (direction == 3)
                    {
                        x++;
                        y++;
                    }
                    else if (direction == 4)
                    {
                        y++;
                    }
                    else if (direction == 5)
                    {
                        y++;
                        x--;
                    }
                    else if (direction == 6)
                    {
                        x--;
                    }
                    else if (direction == 7)
                    {
                        x--;
                        y--;
                    }

                    if (this.TryMoveItem(item_, x, y))
                    {
                        if (this.ActiveDirections.ContainsKey(item_.ID))
                        {
                            this.ActiveDirections[item_.ID] = direction;
                        }
                        else
                        {
                            this.ActiveDirections.Add(item_.ID, direction);
                        }

                        break;
                    }
                    else
                    {
                        if (this.Action > 0)
                        {
                            this.FindNewDirection(item_, ref direction);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        public int TriesCount()
        {
            if (this.Action == 1) //turn right 45 degress
            {
                return 5;
            }
            else if (this.Action == 2) //turn right 90 degress
            {
                return 3;
            }
            else if (this.Action == 3) //turn left 45 degress
            {
                return 5;
            }
            else if (this.Action == 4) //turn left 90 degress
            {
                return 3;
            }
            else if (this.Action == 5) //turn back
            {
                return 2;
            }
            else if (this.Action == 6) //turn to random direction
            {
                return 8;
            }
            else
            {
                return 1;
            }
        }

        public void FindNewDirection(RoomItem item, ref int direction)
        {
            if (this.Action == 1) //turn right 45 degress
            {
                direction++;
            }
            else if (this.Action == 2) //turn right 90 degress
            {
                direction += 2;
            }
            else if (this.Action == 3) //turn left 45 degress
            {
                direction--;
            }
            else if (this.Action == 4) //turn left 90 degress
            {
                direction -= 2;
            }
            else if (this.Action == 5) //turn back
            {
                direction += 4;
            }
            else if (this.Action == 6) //turn to random direction
            {
                direction++;
            }

            if (direction > 7)
            {
                direction -= 8;
            }
            else if (direction < 0)
            {
                direction += 8;
            }
        }

        private bool TryMoveItem(RoomItem item, int x, int y)
        {
            int oldX = item.X;
            int oldY = item.Y;
            double oldZ = item.Z;

            if (this.Room.RoomItemManager.MoveFloorItemOnRoom(null, item, x, y, item.Rot, -1.0, false))
            {
                this.Room.RoomItemManager.MoveAnimation[item.ID] = new RoomItemRollerMovement(item.ID, item.X, item.Y, item.Z, 0, oldX, oldY, oldZ);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
