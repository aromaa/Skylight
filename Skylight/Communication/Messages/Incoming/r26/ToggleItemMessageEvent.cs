using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class ToggleItemMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            try
            {
                if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
                {
                    Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
                    if (room != null)
                    {
                        RoomItem item = room.RoomItemManager.TryGetRoomItem(uint.Parse(message.PopFixedString()));
                        if (item != null)
                        {
                            string state = message.PopFixedString();

                            int state_ = 0;
                            if (!int.TryParse(state, out state_))
                            {
                            }

                            item.OnUse(session, item, state_, room.GaveRoomRights(session));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when trying use item! " + ex.ToString());
            }
        }
    }
}
