using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class EntryRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession().RequestedRoomID > 0 && session.GetHabbo().GetRoomSession().LoadingRoom)
            {
                Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().RequestedRoomID);

                session.GetHabbo().GetRoomSession().ResetRequestedRoom();
                if (room != null)
                {
                    room.RoomUserManager.EnterRoom(session);

                    MultiRevisionServerMessage usersStatuses = room.RoomUserManager.GetUserStatus(true);
                    if (usersStatuses != null)
                    {
                        session.SendData(usersStatuses.GetBytes(session.Revision));
                    }

                    room.RoomWiredManager.UserEnterRoom(session.GetHabbo().GetRoomSession().GetRoomUser());
                }
            }
        }
    }
}
