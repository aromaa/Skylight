using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class GetOffCommand : Command
    {
        public override string CommandInfo()
        {
            return ":getoff - Gets off from horse";
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            RoomUnitUser me = session.GetHabbo().GetRoomSession().GetRoomUser();
            if (me.Riding is RoomPet pet)
            {
                pet.Rider = null;

                me.Riding = null;
                me.StopMoving();
                me.UpdateState();
                me.ApplyEffect(0);
                me.Session.GetHabbo().GetUserStats().Equestrian = 0;
            }

            return true;
        }
    }
}
