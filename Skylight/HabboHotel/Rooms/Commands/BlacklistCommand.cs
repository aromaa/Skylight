using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class BlacklistCommand : Command
    {
        public override string CommandInfo()
        {
            return ":blacklist [type] [action] <extra param>";
        }

        public override string RequiredPermission()
        {
            return "cmd_blacklist";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().GetRoomSession().GetRoom().HaveOwnerRights(session))
            {
                if (args.Length >= 3)
                {
                    string type = args[1];
                    string action = args[2];
                    switch (type)
                    {
                        case "cmd":
                            {
                                switch (action)
                                {
                                    case "add":
                                        {
                                            if (args.Length >= 4)
                                            {
                                                string command = args[3].ToLower();
                                                if (ServerConfiguration.AllowedComamndsToBeBlacklisted.Contains(command))
                                                {
                                                    Room room = session.GetHabbo().GetRoomSession().GetRoom();
                                                    if (room != null)
                                                    {
                                                        if (!room.RoomData.ExtraData.BlacklistedCmds.Contains(command))
                                                        {
                                                            room.RoomData.ExtraData.BlacklistedCmds.Add(command);
                                                        }

                                                        this.ShowBlacklistedCommands(session);
                                                    }
                                                }
                                                else
                                                {
                                                    session.SendNotif("You are not allowed to blacklist this command!");
                                                }
                                            }
                                            else
                                            {
                                                return false;
                                            }
                                        }
                                        return true;
                                    case "remove":
                                        {
                                            if (args.Length >= 4)
                                            {
                                                string command = args[3].ToLower();
                                                Room room = session.GetHabbo().GetRoomSession().GetRoom();
                                                if (room != null)
                                                {
                                                    room.RoomData.ExtraData.BlacklistedCmds.Remove(command);

                                                    this.ShowBlacklistedCommands(session);
                                                }
                                            }
                                            else
                                            {
                                                return false;
                                            }
                                        }
                                        return true;
                                    case "list":
                                        {
                                            this.ShowBlacklistedCommands(session);
                                        }
                                        return true;
                                    default:
                                        {
                                            session.SendNotif("Unknown action! Valid actions:\r\nadd\r\nremove\r\nlist");
                                            return true;
                                        }
                                }
                            }
                        default:
                            {
                                session.SendNotif("Unknown type! Valid types:\r\ncmd");
                                return true;
                            }
                    }
                }
            }

            return false;
        }

        public void ShowBlacklistedCommands(GameClient session)
        {
            string list = "Blacklisted commands in this room:\n";          
            foreach(string cmd in session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.BlacklistedCmds)
            {
                list += cmd + "\n";
            }
            session.SendNotif(list, 2);
        }
    }
}
