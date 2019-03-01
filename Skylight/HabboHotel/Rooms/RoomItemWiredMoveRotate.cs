using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemWiredMoveRotate : RoomItemWiredAction
    {
        public List<RoomItem> SelectedItems;
        public int Movement;
        public int Rotation;

        public RoomItemWiredMoveRotate(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.SelectedItems = new List<RoomItem>();
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
                message.AppendInt32(this.Movement);
                message.AppendInt32(this.Rotation);
                message.AppendInt32(0); //confligts count

                message.AppendInt32(4); //type
                message.AppendInt32(this.Delay); //delay
                message.AppendInt32(0); //conflicts count
                session.SendMessage(message);
            }
        }

        public override string GetItemData()
        {
            return string.Join(",", this.SelectedItems.Select(i => i.ID)) + (char)9 + this.Movement + (char)9 + this.Rotation + (char)9 + this.Delay;
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

            this.Movement = int.Parse(splitData[1]);
            this.Rotation = int.Parse(splitData[2]);
            this.Delay = int.Parse(splitData[3]);
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
            if (this.Rotation != 0 || this.Movement != 0) //we are doing something
            {
                Random random = RandomUtilies.GetRandom();

                Dictionary<RoomItem, Tuple<int, int, int>> items = new Dictionary<RoomItem, Tuple<int, int, int>>(this.SelectedItems.Count);
                foreach (RoomItem item_ in this.SelectedItems)
                {
                    int x = item_.X;
                    int y = item_.Y;

                    if (this.Movement != 0) //no movement
                    {
                        if (this.Movement == 1) //up, down, left or right
                        {
                            int movement = random.Next(0, 4);
                            if (movement == 0) //up
                            {
                                y--;
                            }
                            else if (movement == 1) //down
                            {
                                y++;
                            }
                            else if (movement == 2) //left
                            {
                                x--;
                            }
                            else if (movement == 3) //right
                            {
                                x++;
                            }
                        }
                        else if (this.Movement == 2) //left or right
                        {
                            int movement = random.Next(0, 2);
                            if (movement == 0) //left
                            {
                                x--;
                            }
                            else if (movement == 1) //right
                            {
                                x++;
                            }
                        }
                        else if (this.Movement == 3) //up or down
                        {
                            int movement = random.Next(0, 2);
                            if (movement == 0) //up
                            {
                                y--;
                            }
                            else if (movement == 1) //down
                            {
                                y++;
                            }
                        }
                        else if (this.Movement == 4) //up
                        {
                            y--;
                        }
                        else if (this.Movement == 5) //right
                        {
                            x++;
                        }
                        else if (this.Movement == 6) //down
                        {
                            y++;
                        }
                        else if (this.Movement == 7) //left
                        {
                            x--;
                        }
                    }

                    int rotation = item_.Rot;
                    if (this.Rotation != 0) // no rotation
                    {
                        if (this.Rotation == 1) //clockwise
                        {
                            rotation += 2;
                            if (rotation > 6)
                            {
                                rotation = 0;
                            }
                        }
                        else if (this.Rotation == 2) //counter clockwise
                        {
                            rotation -= 2;
                            if (rotation < 0)
                            {
                                rotation = 6;
                            }
                        }
                        else if (this.Rotation == 3) //clockwise or counter clcokwise
                        {
                            int movement = random.Next(0, 2);
                            if (movement == 0)
                            {
                                rotation += 2;
                                if (rotation > 6)
                                {
                                    rotation = 0;
                                }
                            }
                            else if (movement == 1)
                            {
                                rotation -= 2;
                                if (rotation < 0)
                                {
                                    rotation = 6;
                                }
                            }
                        }
                    }

                    if (this.Room.RoomItemManager.CanPlaceItemAt(item_, x, y))
                    {
                        items.Add(item_, new Tuple<int, int, int>(x, y, rotation));
                    }
                }
                
                List<RoomItem> ignore = new List<RoomItem>(items.Keys);
                foreach (KeyValuePair<RoomItem, Tuple<int, int, int>> item_ in items.OrderBy(i => i.Key.Z))
                {
                    int oldX = item_.Key.X;
                    int oldY = item_.Key.Y;
                    double oldZ = item_.Key.Z;

                    if (this.Room.RoomItemManager.MoveFloorItemOnRoom(null, item_.Key, item_.Value.Item1, item_.Value.Item2, item_.Value.Item3, -1.0, false, ignore.ToArray()))
                    {
                        this.Room.RoomItemManager.MoveAnimation[item_.Key.ID] = new RoomItemRollerMovement(item_.Key.ID, oldX, oldY, oldZ, 0, item_.Key.X, item_.Key.Y, item_.Key.Z);
                    }

                    ignore.Remove(item_.Key);
                }
            }
        }
    }
}
