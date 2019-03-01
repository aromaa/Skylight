using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Users.Inventory;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class AddDiscToJukeboxMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.HaveOwnerRights(session))
            {
                if (room.RoomItemManager.Jukebox != null && room.RoomItemManager.Jukebox.Playlist.Count < room.RoomItemManager.Jukebox.PlaylistCapacity)
                {
                    uint itemId = message.PopWiredUInt();
                    InventoryItem item = session.GetHabbo().GetInventoryManager().TryGetFloorItem(itemId);
                    if (item != null && item.GetItem() != null && item.GetItem().InteractionType == "musicdisk")
                    {
                        int songId = 0;
                        if (int.TryParse(item.ExtraData, out songId))
                        {
                            session.GetHabbo().GetInventoryManager().RemoveItemFromHand(itemId, true);
                            room.RoomItemManager.Jukebox.AddDisc(new MusicDisc(itemId, item.BaseItem, songId));

                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            message_.Init(r63aOutgoing.JukeboxPlaylist);
                            message_.AppendInt32(room.RoomItemManager.Jukebox.PlaylistCapacity);
                            message_.AppendInt32(room.RoomItemManager.Jukebox.Playlist.Count);
                            foreach (MusicDisc disc in room.RoomItemManager.Jukebox.Playlist)
                            {
                                message_.AppendUInt(disc.ItemId);
                                message_.AppendInt32(disc.SongID);
                            }
                            session.SendMessage(message_);
                        }
                    }
                }
            }
        }
    }
}
