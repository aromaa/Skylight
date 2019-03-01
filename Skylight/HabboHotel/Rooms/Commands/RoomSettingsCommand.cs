using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RoomSettingsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":roomsettings [type] [setting] <extra params> - Make your room more coolio";
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
            if (session.GetHabbo().GetRoomSession().GetRoom().HaveOwnerRights(session))
            {
                if (args.Length >= 3)
                {
                    string type = args[1];
                    string setting = args[2];
                    switch (type)
                    {
                        case "logic":
                            {
                                switch (setting)
                                {
                                    case "wired-onsay-case-sensitive":
                                    case "wired-onsay-equals":
                                    case "building-users-block-furni-placement":
                                        {
                                            if (args.Length >= 4)
                                            {
                                                bool add = false;
                                                if (bool.TryParse(args[3], out add))
                                                {
                                                    Room room = session.GetHabbo().GetRoomSession().GetRoom();
                                                    if (room != null)
                                                    {
                                                        if (add)
                                                        {
                                                            if (!room.RoomData.ExtraData.RoomSettingsLogic.Contains(setting))
                                                            {
                                                                room.RoomData.ExtraData.RoomSettingsLogic.Add(setting);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            room.RoomData.ExtraData.RoomSettingsLogic.Remove(setting);
                                                        }

                                                        this.ShowRoomLogic(session);
                                                    }
                                                }
                                            }
                                        }
                                        return true;
                                    default:
                                        {
                                            session.SendNotif("Unknown setting! Valid settings:\r\nwired-onsay-case-sensitive\r\nwired-onsay-equals\r\nbuilding-users-block-furni-placement");
                                            return true;
                                        }
                                }
                            }
                        default:
                            {
                                session.SendNotif("Unknown type! Valid types:\r\nlogic");
                                return true;
                            }
                    }
                }
            }

            return false;
        }

        public void ShowRoomLogic(GameClient session)
        {
            string list = "Room logic in this room:\n";
            foreach (string cmd in session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.RoomSettingsLogic)
            {
                list += cmd + "\n";
            }
            session.SendNotif(list, 2);
        }
    }
}
