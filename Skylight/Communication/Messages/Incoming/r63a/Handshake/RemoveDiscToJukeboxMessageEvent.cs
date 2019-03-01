using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class RemoveDiscToJukeboxMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.HaveOwnerRights(session))
            {
                if (room.RoomItemManager.Jukebox != null && room.RoomItemManager.Jukebox.Playlist.Count < room.RoomItemManager.Jukebox.PlaylistCapacity)
                {
                    MusicDisc disc = room.RoomItemManager.Jukebox.RemoveDisc(message.PopWiredInt32());
                    if (disc != null)
                    {
                        session.GetHabbo().GetInventoryManager().AddItem(disc.ItemId, disc.BaseItem, disc.SongID.ToString(), true);

                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.JukeboxPlaylist);
                        message_.AppendInt32(room.RoomItemManager.Jukebox.PlaylistCapacity);
                        message_.AppendInt32(room.RoomItemManager.Jukebox.Playlist.Count);
                        foreach (MusicDisc disc_ in room.RoomItemManager.Jukebox.Playlist)
                        {
                            message_.AppendUInt(disc_.ItemId);
                            message_.AppendInt32(disc_.SongID);
                        }
                        session.SendMessage(message_);
                    }
                }
            }
        }
    }
}
