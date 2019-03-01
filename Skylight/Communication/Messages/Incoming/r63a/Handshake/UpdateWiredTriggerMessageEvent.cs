using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class UpdateWiredTriggerMessageEvent : IncomingPacket
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
                    if (item is RoomItemWiredEnterRoom)
                    {
                        RoomItemWiredEnterRoom wired = (RoomItemWiredEnterRoom)item;

                        message.PopWiredInt32(); //extra data count, 0
                        wired.SpecifyUser = message.PopFixedString(); //user id
                    }
                    else if (item is RoomItemWiredAtTime)
                    {
                        RoomItemWiredAtTime wired = (RoomItemWiredAtTime)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.Time = message.PopWiredInt32();
                    
                    }
                    else if (item is RoomItemWiredOffFurni)
                    {
                        RoomItemWiredOffFurni wired = (RoomItemWiredOffFurni)item;

                        message.PopWiredInt32(); //extra data count, 0
                        message.PopFixedString(); //empty

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
                    }
                    else if (item is RoomItemWiredOnSay)
                    {
                        RoomItemWiredOnSay wired = (RoomItemWiredOnSay)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.OnlyOwner = message.PopWiredBoolean();
                        wired.Message = message.PopFixedString();
                    }
                    else if (item is RoomItemWiredAtScore)
                    {
                        RoomItemWiredAtScore wired = (RoomItemWiredAtScore)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.Score = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredTimer)
                    {
                        RoomItemWiredTimer wired = (RoomItemWiredTimer)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.Time = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredFurniState)
                    {
                        RoomItemWiredFurniState wired = (RoomItemWiredFurniState)item;

                        message.PopWiredInt32(); //extra data count, 0
                        message.PopFixedString(); //empty

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
                    }
                    else if (item is RoomItemWiredOnFurni)
                    {
                        RoomItemWiredOnFurni wired = (RoomItemWiredOnFurni)item;

                        message.PopWiredInt32(); //extra data count, 0
                        message.PopFixedString(); //empty

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
                    }

                    //int itemExtraDataCount = message.PopWiredInt32();
                    //read extra data
                    //string data = message.PopFixedString();
                    //int itemCount = message.PopWiredInt32();
                    //read items
                    //int delay = message.PopWiredInt32();

                    //we want to save it too to db :)
                    room.RoomItemManager.ItemDataChanged.AddOrUpdate(item.ID, item, (key, oldValue) => item);
                }
            }
        }
    }
}
