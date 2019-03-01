using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Bots
{
    public interface BotAI
    {
        void OnSelfEnterRoom();
        void OnSelfLeaveRoom(bool kicked);
        void OnUserLeaveRoom(RoomUnitUser user);
        void OnUserSpeak(RoomUnitUser user, string message, bool shout);
        void OnRoomCycle();
    }
}
