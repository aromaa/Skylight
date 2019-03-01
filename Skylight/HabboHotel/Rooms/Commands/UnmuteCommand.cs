using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class UnmuteCommand : Command
    {
        public override string CommandInfo()
        {
            return ":unmute [name] - Unmutes the user";
        }

        public override string RequiredPermission()
        {
            return "cmd_unmute";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_unmute"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        target.GetHabbo().MuteExpires = 0;
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("muteExpires", 0);
                            dbClient.AddParamWithValue("targetId", target.GetHabbo().ID);
                            dbClient.ExecuteQuery("UPDATE users SET mute_expires = @muteExpires WHERE id = @targetId LIMIT 1");
                        }
                        
                        target.SendMessage(OutgoingPacketsEnum.Flood);
                        target.SendNotif("You have been unmuted! Reload room to remove your flood time");
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
