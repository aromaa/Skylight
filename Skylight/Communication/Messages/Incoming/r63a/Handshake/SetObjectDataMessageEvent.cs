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
    class SetObjectDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                uint itemId = message.PopWiredUInt();
                RoomItem item = room.RoomItemManager.TryGetRoomItem(itemId);
                if (item != null)
                {
                    int amount = message.PopWiredInt32();

                    Dictionary<string, string> data = new Dictionary<string, string>();
                    for (int i = 0; i < amount / 2; i++)
                    {
                        data.Add(message.PopFixedString(), message.PopFixedString());
                    }

                    string extraData = "";
                    foreach (KeyValuePair<string, string> data_ in data)
                    {
                        if (extraData.Length > 0)
                        {
                            extraData += Convert.ToChar(9);
                        }

                        extraData += data_.Key + "=" + data_.Value;
                    }

                    item.ExtraData = extraData;
                    item.UpdateState(true, true);
                }
            }
        }
    }
}
