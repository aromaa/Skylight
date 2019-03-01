using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Rooms.Games;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class UpdateWiredActionMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.GaveRoomRights(session))
            {
                uint itemId = message.PopWiredUInt();
                RoomItem item = room.RoomItemManager.TryGetRoomItem(itemId);
                if (item != null)
                {
                    if (item is RoomItemWiredSayMessage)
                    {
                        message.PopWiredInt32(); //extra data count
                        string message_ = message.PopFixedString();
                        if (message_.Length > 100)
                        {
                            message_ = message_.Substring(0, 100);
                        }

                        ((RoomItemWiredSayMessage)item).Message = message_;
                    }
                    else if (item is RoomItemWiredMoveRotate)
                    {
                        RoomItemWiredMoveRotate wired = (RoomItemWiredMoveRotate)item;

                        message.PopWiredInt32(); //extra data count, 2
                        wired.Movement = message.PopWiredInt32();
                        wired.Rotation = message.PopWiredInt32();

                        message.PopFixedString(); //data

                        List<RoomItem> items = new List<RoomItem>();
                        int itemCount = message.PopWiredInt32();
                        for (int i = 0; i < itemCount; i++)
                        {
                            uint itemId_ = message.PopWiredUInt();
                            RoomItem item_ = room.RoomItemManager.TryGetRoomItem(itemId_);
                            if (item_ != null)
                            {
                                items.Add(item_);
                            }
                        }
                        wired.SelectedItems = items;

                        wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredGivePoints)
                    {
                        RoomItemWiredGivePoints wired = (RoomItemWiredGivePoints)item;
                        message.PopWiredInt32(); //extra data count, 2
                        wired.Points = message.PopWiredInt32();
                        wired.PointsAmount = message.PopWiredInt32();

                        message.PopFixedString(); //data
                        message.PopWiredInt32(); //items count

                        wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredResetTimers)
                    {
                        RoomItemWiredResetTimers wired = (RoomItemWiredResetTimers)item;
                        message.PopWiredInt32(); //extra data count, 0
                        message.PopFixedString(); //data
                        message.PopWiredInt32(); //items count

                        wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredToggleFurni)
                    {
                        RoomItemWiredToggleFurni wired = (RoomItemWiredToggleFurni)item;

                        message.PopWiredInt32(); //extra data count, 0
                        message.PopFixedString(); //data

                        List<RoomItem> items = new List<RoomItem>();
                        int itemCount = message.PopWiredInt32();
                        for (int i = 0; i < itemCount; i++)
                        {
                            uint itemId_ = message.PopWiredUInt();
                            RoomItem item_ = room.RoomItemManager.TryGetRoomItem(itemId_);
                            if (item_ != null)
                            {
                                items.Add(item_);
                            }
                        }
                        wired.SelectedItems = items;

                        wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredMoveUser)
                    {
                        RoomItemWiredMoveUser wired = (RoomItemWiredMoveUser)item;

                        message.PopWiredInt32(); //extra data count, 0
                        message.PopFixedString(); //data

                        List<RoomItem> items = new List<RoomItem>();
                        int itemCount = message.PopWiredInt32();
                        for (int i = 0; i < itemCount; i++)
                        {
                            uint itemId_ = message.PopWiredUInt();
                            RoomItem item_ = room.RoomItemManager.TryGetRoomItem(itemId_);
                            if (item_ != null)
                            {
                                items.Add(item_);
                            }
                        }
                        wired.SelectedItems = items;

                        wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredMatchFurni)
                    {
                        RoomItemWiredMatchFurni wired = (RoomItemWiredMatchFurni)item;

                        message.PopWiredInt32(); //extra data count, 3
                        wired.FurniState = message.PopWiredBoolean();
                        wired.Direction = message.PopWiredBoolean();
                        wired.Position = message.PopWiredBoolean();
                        message.PopFixedString(); //data

                        List<RoomItem> items = new List<RoomItem>();
                        Dictionary<uint, MatchFurniData> data = new Dictionary<uint, MatchFurniData>();
                        int itemCount = message.PopWiredInt32();
                        for (int i = 0; i < itemCount; i++)
                        {
                            uint itemId_ = message.PopWiredUInt();
                            RoomItem item_ = room.RoomItemManager.TryGetRoomItem(itemId_);
                            if (item_ != null)
                            {
                                items.Add(item_);
                                data.Add(item_.ID, new MatchFurniData(item_.WiredIgnoreExtraData() ? "" : item_.ExtraData, item_.Rot, item_.X, item_.Y, item_.Z));
                            }
                        }
                        wired.SelectedItems = items;
                        wired.MatchFurniData = data;

                        wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredActionChase)
                    {
                        RoomItemWiredActionChase wired = (RoomItemWiredActionChase)item;

                        message.PopWiredInt32(); //extra data count, 0
                        message.PopFixedString(); //data

                        List<RoomItem> items = new List<RoomItem>();
                        int itemCount = message.PopWiredInt32();
                        for (int i = 0; i < itemCount; i++)
                        {
                            uint itemId_ = message.PopWiredUInt();
                            RoomItem item_ = room.RoomItemManager.TryGetRoomItem(itemId_);
                            if (item_ != null)
                            {
                                items.Add(item_);
                            }
                        }
                        wired.SelectedItems = items;

                        wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredCallStack)
                    {
                        RoomItemWiredCallStack wired = (RoomItemWiredCallStack)item;

                        message.PopWiredInt32(); //extra data count, 0
                        message.PopFixedString(); //data

                        List<RoomItem> items = new List<RoomItem>();
                        int itemCount = message.PopWiredInt32();
                        for (int i = 0; i < itemCount; i++)
                        {
                            uint itemId_ = message.PopWiredUInt();
                            RoomItem item_ = room.RoomItemManager.TryGetRoomItem(itemId_);
                            if (item_ != null)
                            {
                                items.Add(item_);
                            }
                        }
                        wired.SelectedItems = items;

                        wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredJoinTeam)
                    {
                        RoomItemWiredJoinTeam wired = (RoomItemWiredJoinTeam)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.Team = (GameTeam)message.PopWiredInt32(); //team
                    }
                    else if (item is RoomItemWiredLeaveTeam)
                    {
                        RoomItemWiredLeaveTeam wired = (RoomItemWiredLeaveTeam)item;

                        message.PopWiredInt32(); //extra data count, 0
                        message.PopFixedString(); //data
                        message.PopWiredInt32(); //items
                        wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredActionSkylight)
                    {
                        RoomItemWiredActionSkylight wired = (RoomItemWiredActionSkylight)item;

                        message.PopWiredInt32(); //extra data count
                        string message_ = message.PopFixedString();

                        string[] data = message_.Split(new char[] { ':' }, 2);
                        string correctFormat;
                        if (wired.IsValidFormat(data[0], data.Length > 1 ? data[1] : "", out correctFormat))
                        {
                            if (session.GetHabbo().HasPermission("wired_action_" + data[0]))
                            {
                                wired.Message = message_;
                            }
                            else
                            {
                                session.SendNotif("You are not allowed to use this super wired action!");
                            }
                        }
                        else
                        {
                            session.SendNotif("Super wired format error!\n" + correctFormat);
                        }
                    }
                    else if (item is RoomItemActionTriggerStacks)
                    {
                        RoomItemActionTriggerStacks wired = (RoomItemActionTriggerStacks)item;

                        message.PopWiredInt32(); //extra data count, 0
                        message.PopFixedString(); //data

                        List<RoomItem> items = new List<RoomItem>();
                        int itemCount = message.PopWiredInt32();
                        for (int i = 0; i < itemCount; i++)
                        {
                            uint itemId_ = message.PopWiredUInt();
                            RoomItem item_ = room.RoomItemManager.TryGetRoomItem(itemId_);
                            if (item_ != null)
                            {
                                items.Add(item_);
                            }
                        }
                        wired.SelectedItems = items;

                        wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredGiveScoreTeam)
                    {
                        RoomItemWiredGiveScoreTeam wired = (RoomItemWiredGiveScoreTeam)item;
                        message.PopWiredInt32(); //extra data count, 3
                        wired.Points = message.PopWiredInt32();
                        wired.PointsAmount = message.PopWiredInt32();
                        wired.PointsTeam = (GameTeam)message.PopWiredInt32();

                        //message.PopFixedString(); //data
                        //message.PopWiredInt32(); //items count

                        //wired.Delay = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredMoveToDir)
                    {
                        RoomItemWiredMoveToDir wired = (RoomItemWiredMoveToDir)item;

                        message.PopWiredInt32(); //extra data count, 2
                        wired.Direction = message.PopWiredInt32();
                        wired.Action = message.PopWiredInt32();

                        message.PopFixedString(); //data

                        List<RoomItem> items = new List<RoomItem>();
                        int itemCount = message.PopWiredInt32();
                        for (int i = 0; i < itemCount; i++)
                        {
                            uint itemId_ = message.PopWiredUInt();
                            RoomItem item_ = room.RoomItemManager.TryGetRoomItem(itemId_);
                            if (item_ != null)
                            {
                                items.Add(item_);
                            }
                        }
                        wired.SelectedItems = items;
                        wired.ActiveDirections.Clear();
                    }
                    //int itemExtraDataCount = message.PopWiredInt32();
                    ////read extra data
                    //string data = message.PopFixedString();
                    //int itemCount = message.PopWiredInt32();
                    ////read items
                    //int delay = message.PopWiredInt32();
                    //int idk = message.PopWiredInt32();

                    room.RoomItemManager.ItemDataChanged.AddOrUpdate(item.ID, item, (key, oldValue) => item);
                }
            }
        }
    }
}
