using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class LayCommand : Command
    {
        public override string CommandInfo()
        {
            return ":lay - Lay";
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            RoomUnit user = session.GetHabbo().GetRoomSession().GetRoomUser();
            if (user.HasStatus("lay") && !user.CurrentTile.IsBed)
            {
                user.RemoveStatus("lay");
                return true;
            }
            else
            {
                if (!user.HasStatus("sit"))
                {
                    if (user.BodyRotation == 0 || user.BodyRotation == 2 || user.BodyRotation == 4 || user.BodyRotation == 6)
                    {
                        user.AddStatus("lay", TextUtilies.DoubleWithDotDecimal(user.Z + 0.55));
                    }
                }
            }

            return true;
        }
    }
}
