using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Extanssions;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class SaveLookEventHandler : IncomingPacket
    {
        protected string Gender;
        protected string Look;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (this.Gender.EqualsIgnoreCase("M") || this.Gender.EqualsIgnoreCase("F"))
            {
                session.GetHabbo().Gender = this.Gender;
                session.GetHabbo().Look = this.Look;

                session.SendMessage(new UpdateUserLookComposerHandler(this.Look, this.Gender));

                RoomUnitUser roomUser = session.GetHabbo().GetRoomSession().GetRoomUser();
                if (roomUser != null)
                {
                    roomUser.FootballGateFigureActive = false;

                    roomUser.Room.SendToAll(new UpdateRoomUserComposerHandler(roomUser.VirtualID, this.Look, this.Gender, session.GetHabbo().Motto, session.GetHabbo().GetUserStats().AchievementPoints));
                }

                Skylight.GetGame().GetAchievementManager().AddAchievement(session, "AvatarLook", 1);
            }
            else
            {
                session.SendLeetScripter();
            }
        }
    }
}
