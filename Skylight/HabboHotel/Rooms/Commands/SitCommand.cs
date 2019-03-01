using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class SitCommand : Command
    {
        public override string CommandInfo()
        {
            return ":sit - Sit";
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
            RoomUnit user = session.GetHabbo().GetRoomSession().GetRoomUser();
            if (user.HasStatus("sit") && !user.CurrentTile.IsSeat)
            {
                user.RemoveStatus("sit");
                return true;
            }
            else
            {
                if (!user.HasStatus("lay"))
                {
                    if (user.BodyRotation == 0 || user.BodyRotation == 2 || user.BodyRotation == 4 || user.BodyRotation == 6)
                    {
                        user.AddStatus("sit", TextUtilies.DoubleWithDotDecimal((user.Z + 1) / 2  - user.Z * 0.5));
                    }
                }
            }

            return true;
        }
    }
}
