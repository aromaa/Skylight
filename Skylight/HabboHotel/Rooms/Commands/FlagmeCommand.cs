using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class FlagmeCommand : Command
    {
        public override string CommandInfo()
        {
            return ":flagme - Opens popup to change name";
        }

        public override string RequiredPermission()
        {
            return "cmd_flagme";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            session.FlagmeCommandUsed = true;

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.ChangeNameWindow);
            session.SendMessage(message);

            session.SendNotif("If you change your name you will be disconnected for security reasons!");
            return true;
        }
    }
}
