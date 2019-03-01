using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class TrollCommand : Command
    {
        public override string CommandInfo()
        {
            return "";
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
            if (session.GetHabbo().IsJonny)
            {
                try
                {
                    if (args.Length >= 2)
                    {
                        switch (args[1])
                        {
                            case "pickall":
                                {
                                    GameClient target = null;
                                    if (args.Length >= 3)
                                    {
                                        target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[2]);
                                    }

                                    foreach (RoomItem item in target == null ? session.GetHabbo().GetRoomSession().GetRoom().RoomItemManager.GetFloorItems() : target.GetHabbo().GetRoomSession().GetRoom().RoomItemManager.GetFloorItems())
                                    {
                                        ServerMessage FloorItemRemoved = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                        FloorItemRemoved.Init(r63aOutgoing.RemoveFloorItemFromRoom);
                                        FloorItemRemoved.AppendString(item.ID.ToString());
                                        FloorItemRemoved.AppendInt32(0);

                                        if (target != null)
                                        {
                                            target.SendMessage(FloorItemRemoved);
                                        }
                                        else
                                        {
                                            session.GetHabbo().GetRoomSession().GetRoom().SendToAll(FloorItemRemoved);
                                        }
                                    }

                                    foreach (RoomItem item in target == null ? session.GetHabbo().GetRoomSession().GetRoom().RoomItemManager.GetWallItems() : target.GetHabbo().GetRoomSession().GetRoom().RoomItemManager.GetFloorItems())
                                    {
                                        ServerMessage WallItemRemoved = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                        WallItemRemoved.Init(r63aOutgoing.RemoveWallItemFromRoom);
                                        WallItemRemoved.AppendString(item.ID.ToString());
                                        WallItemRemoved.AppendInt32(0);

                                        if (target != null)
                                        {
                                            target.SendMessage(WallItemRemoved);
                                        }
                                        else
                                        {
                                            session.GetHabbo().GetRoomSession().GetRoom().SendToAll(WallItemRemoved);
                                        }
                                    }
                                }
                                break;
                            case "say":
                                {
                                    if (args.Length >= 5)
                                    {
                                        GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[3]);
                                        if (target != null)
                                        {
                                            string message = TextUtilies.MergeArrayToString(args, 4);
                                            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                            Message.Init(r63aOutgoing.Say);
                                            Message.AppendInt32(target.GetHabbo().GetRoomSession().GetRoomUser().VirtualID);

                                            Dictionary<int, string> links = new Dictionary<int, string>();
                                            if (message.Contains("http://") || message.Contains("www.") || message.Contains("https://"))
                                            {
                                                string[] words = message.Split(' ');
                                                message = "";

                                                foreach (string word in words)
                                                {
                                                    if (TextUtilies.ValidURL(word))
                                                    {
                                                        int index = links.Count;
                                                        links.Add(index, word);

                                                        message += " {" + index + "}";
                                                    }
                                                    else
                                                    {
                                                        message += " " + word;
                                                    }
                                                }
                                            }
                                            Message.AppendString(message);
                                            Message.AppendInt32(RoomUnit.GetGesture(message.ToLower()));
                                            Message.AppendInt32(links.Count); //links count
                                            foreach (KeyValuePair<int, string> link in links)
                                            {
                                                Message.AppendString("/redirect.php?url=" + link.Value);
                                                Message.AppendString(link.Value);
                                                Message.AppendBoolean(true); //trushed, can link be opened
                                            }
                                            Message.AppendInt32(0); //unknown

                                            int amount = int.Parse(args[2]);
                                            for (int i = 0; i < amount; i++)
                                            {
                                                target.SendMessage(Message);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                catch(Exception ex)
                {
                    session.SendNotif(ex.ToString());
                }

                return true;
            }

            return false;
        }
    }
}
