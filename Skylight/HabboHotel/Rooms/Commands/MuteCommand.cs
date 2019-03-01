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
    class MuteCommand : Command
    {
        public override string CommandInfo()
        {
            return ":mute [user] [time/P] - Mutes user for given time";
        }

        public override string RequiredPermission()
        {
            return "cmd_mute";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 3)
            {
                if (session.GetHabbo().HasPermission("cmd_mute"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        if (target.GetHabbo().Rank < session.GetHabbo().Rank)
                        {
                            string lenght = args[2];

                            char muteLenghtChar = lenght.Substring(lenght.Length - 1)[0];
                            int muteLenght = 0;

                            if (Char.IsNumber(muteLenghtChar))
                            {
                                muteLenght = int.Parse(lenght);
                            }
                            else
                            {
                                if (lenght != "P") //not permament
                                {
                                    if (int.TryParse(lenght.Substring(0, lenght.Length - 1), out muteLenght))
                                    {
                                        if (muteLenghtChar == 'm')
                                        {
                                            muteLenght *= 60;
                                        }
                                        else if (muteLenghtChar == 'h')
                                        {
                                            muteLenght *= 3600;
                                        }
                                        else if (muteLenghtChar == 'd')
                                        {
                                            muteLenght *= 86400;
                                        }
                                        else if (muteLenghtChar == 'w')
                                        {
                                            muteLenght *= 604800;
                                        }
                                        else if (muteLenghtChar == 'M')
                                        {
                                            muteLenght *= 2419200;
                                        }
                                        else if (muteLenghtChar == 'y')
                                        {
                                            muteLenght *= 30758400;
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }

                            if (muteLenght >= 0)
                            {
                                double expires = TimeUtilies.GetUnixTimestamp() + muteLenght;

                                if (lenght == "P")
                                {
                                    expires = -1;
                                }

                                target.GetHabbo().MuteExpires = expires;
                                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                                {
                                    dbClient.AddParamWithValue("muteExpires", expires);
                                    dbClient.AddParamWithValue("targetId", target.GetHabbo().ID);
                                    dbClient.ExecuteQuery("UPDATE users SET mute_expires = @muteExpires WHERE id = @targetId LIMIT 1");
                                }

                                target.SendMessage(OutgoingPacketsEnum.Flood, new ValueHolder("TimeLeft", target.GetHabbo().MuteTimeLeft()));
                                target.SendNotif("You have been muted!");
                            }
                            else
                            {
                                session.SendNotif("You can't enter negative mute lenght!");
                            }
                        }
                        else
                        {
                            session.SendNotif("You are not allowed to mute this user.");
                        }
                    }
                    else
                    {
                        session.SendNotif("Unable to find user");
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
