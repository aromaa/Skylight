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
    class UpdateWiredConditionMessageEvent : IncomingPacket
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
                    if (item is RoomItemWiredTimeMoreThan)
                    {
                        RoomItemWiredTimeMoreThan wired = (RoomItemWiredTimeMoreThan)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.Time = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredTimeLessThan)
                    {
                        RoomItemWiredTimeLessThan wired = (RoomItemWiredTimeLessThan)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.Time = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredMatchSnapshop)
                    {
                        RoomItemWiredMatchSnapshop wired = (RoomItemWiredMatchSnapshop)item;

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
                    }
                    else if (item is RoomItemWiredTriggererOnFurni)
                    {
                        RoomItemWiredTriggererOnFurni wired = (RoomItemWiredTriggererOnFurni)item;

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
                    }
                    else if (item is RoomItemWiredUsersStandOnFurni)
                    {
                        RoomItemWiredUsersStandOnFurni wired = (RoomItemWiredUsersStandOnFurni)item;

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
                    }
                    else if (item is RoomItemWiredHasFurniOn)
                    {
                        RoomItemWiredHasFurniOn wired = (RoomItemWiredHasFurniOn)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.AllFurnis = message.PopWiredBoolean();
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
                    }
                    else if (item is RoomItemWiredInTeam)
                    {
                        RoomItemWiredInTeam wired = (RoomItemWiredInTeam)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.Team = (GameTeam)message.PopWiredInt32(); //team
                    }
                    else if (item is RoomItemWiredUserCountIn)
                    {
                        RoomItemWiredUserCountIn wired = (RoomItemWiredUserCountIn)item;

                        message.PopWiredInt32(); //extra data count, 2
                        wired.UsersMin = message.PopWiredInt32();
                        wired.UsersMax = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredWearingEffect)
                    {
                        RoomItemWiredWearingEffect wired = (RoomItemWiredWearingEffect)item;

                        message.PopWiredInt32(); //extra data count, 0

                        string effect = message.PopFixedString();
                        if (effect.Length > 12)
                        {
                            effect = effect.Substring(0, 12);
                        }
                        wired.EffectID = effect;
                    }
                    else if (item is RoomItemWiredWearingBadge)
                    {
                        RoomItemWiredWearingBadge wired = (RoomItemWiredWearingBadge)item;

                        message.PopWiredInt32(); //extra data count, 0

                        string effect = message.PopFixedString();
                        if (effect.Length > 100)
                        {
                            effect = effect.Substring(0, 12);
                        }
                        wired.BadgeID = effect;
                    }
                    else if (item is RoomItemWiredNotWearingEffect)
                    {
                        RoomItemWiredNotWearingEffect wired = (RoomItemWiredNotWearingEffect)item;

                        message.PopWiredInt32(); //extra data count, 0

                        string effect = message.PopFixedString();
                        if (effect.Length > 12)
                        {
                            effect = effect.Substring(0, 12);
                        }
                        wired.EffectID = effect;
                    }
                    else if (item is RoomItemWiredNotWearingBadge)
                    {
                        RoomItemWiredNotWearingBadge wired = (RoomItemWiredNotWearingBadge)item;

                        message.PopWiredInt32(); //extra data count, 0

                        string effect = message.PopFixedString();
                        if (effect.Length > 100)
                        {
                            effect = effect.Substring(0, 12);
                        }
                        wired.BadgeID = effect;
                    }
                    else if (item is RoomItemWiredNotUserCountIn)
                    {
                        RoomItemWiredNotUserCountIn wired = (RoomItemWiredNotUserCountIn)item;

                        message.PopWiredInt32(); //extra data count, 2
                        wired.UsersMin = message.PopWiredInt32();
                        wired.UsersMax = message.PopWiredInt32();
                    }
                    else if (item is RoomItemWiredUsersNotStandOnFurni)
                    {
                        RoomItemWiredUsersNotStandOnFurni wired = (RoomItemWiredUsersNotStandOnFurni)item;

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
                    }
                    else if (item is RoomItemWiredTriggererNotOnFurni)
                    {
                        RoomItemWiredTriggererNotOnFurni wired = (RoomItemWiredTriggererNotOnFurni)item;

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
                    }
                    else if (item is RoomItemWiredNotMatchSnapshop)
                    {
                        RoomItemWiredNotMatchSnapshop wired = (RoomItemWiredNotMatchSnapshop)item;

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
                    }
                    else if (item is RoomItemWiredTriggered)
                    {
                        RoomItemWiredTriggered wired = (RoomItemWiredTriggered)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.AllFurnis = message.PopWiredBoolean();
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
                    }
                    else if (item is RoomItemWiredNotTriggered)
                    {
                        RoomItemWiredNotTriggered wired = (RoomItemWiredNotTriggered)item;

                        message.PopWiredInt32(); //extra data count, 1
                        wired.AllFurnis = message.PopWiredBoolean();
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
