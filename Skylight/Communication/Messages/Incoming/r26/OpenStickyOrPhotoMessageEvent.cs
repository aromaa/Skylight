using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class OpenStickyOrPhotoMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint itemId = uint.Parse(message.PopStringUntilBreak(null));

            RoomItemPhoto item = (RoomItemPhoto)session.GetHabbo().GetRoomSession().GetRoom().RoomItemManager.TryGetWallItem(itemId);
            if (item != null)
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                message_.Init(r26Outgoing.OpenStickyOrPhoto);
                message_.AppendString(item.ID.ToString(), 9);
                message_.AppendString(item.ExtraData + " x " + TimeUtilies.UnixTimestampToDateTime(item.Photo.Time) + "\r" + item.Photo.Text, null);
                session.SendMessage(message_);
            }
        }
    }
}
