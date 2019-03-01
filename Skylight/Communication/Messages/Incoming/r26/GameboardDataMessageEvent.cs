using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class GameboardDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string[] data = message.PopStringUntilBreak(null).Split(new char[] { ' ' });

            RoomGameboard board = session.GetHabbo().GetRoomSession().GetRoom().RoomGameManager.Gameboards.Values.FirstOrDefault(b => b.ID == int.Parse(data[0]));
            if (board != null && board.InGame(session.GetHabbo().GetRoomSession().GetRoomUser()))
            {
                board.DoAction(session.GetHabbo().GetRoomSession().GetRoomUser(), data.Skip(1).ToArray());
            }
        }
    }
}
