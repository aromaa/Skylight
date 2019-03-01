using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class WhisperMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                if (!session.GetHabbo().GetRoomSession().GetRoom().RoomMute || session.GetHabbo().HasPermission("acc_ignore_roommute"))
                {
                    if (!session.GetHabbo().IsMuted())
                    {
                        session.GetHabbo().GetRoomSession().GetRoomUser().Unidle();

                        if (session.GetHabbo().GetRoomSession().GetRoomUser().CheckForFloor())
                        {
                            session.GetHabbo().GetRoomSession().GetRoomUser().FloodUser();
                        }
                        else
                        {
                            session.GetHabbo().LastRoomMessage = DateTime.Now;
                            session.GetHabbo().FloodCounter++;

                            string[] data = message.PopFixedString().Split(new char[] { ' ' }, 2);
                            string username = data[0];
                            string message_ = data[1];
                            if (message_.Length > 300)
                            {
                                message_ = message_.Substring(0, 300);
                            }
                            message_ = TextUtilies.FilterString(message_);

                            RoomUnitUser user = room.RoomUserManager.GetUserByName(username);
                            if (user != null)
                            {
                                if (!session.GetHabbo().HasPermission("acc_nochatlog_whisper"))
                                {
                                    Skylight.GetGame().GetChatlogManager().LogRoomMessage(session, "Whisper" + (char)9 + user.Session.GetHabbo().ID + (char)9 + message_);
                                }

                                message_ = TextUtilies.CheckBlacklistedWords(message_);

                                ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                message_2.Init(r63aOutgoing.Whisper);
                                message_2.AppendInt32(session.GetHabbo().GetRoomSession().GetRoomUser().VirtualID);

                                Dictionary<int, string> links = new Dictionary<int, string>();
                                if (message_.Contains("http://") || message_.Contains("www.") || message_.Contains("https://"))
                                {
                                    string[] words = message_.Split(' ');
                                    message_ = "";

                                    foreach (string word in words)
                                    {
                                        if (TextUtilies.ValidURL(word))
                                        {
                                            int index = links.Count;
                                            links.Add(index, word);

                                            message_ += " {" + index + "}";
                                        }
                                        else
                                        {
                                            message_ += " " + word;
                                        }
                                    }
                                }
                                message_2.AppendString(message_);
                                message_2.AppendInt32(RoomUnit.GetGesture(message_.ToLower())); //gesture
                                message_2.AppendInt32(links.Count); //links count
                                foreach (KeyValuePair<int, string> link in links)
                                {
                                    message_2.AppendString("/redirect.php?url=" + link.Value);
                                    message_2.AppendString(link.Value);
                                    message_2.AppendBoolean(true); //trushed, can link be opened
                                }
                                user.Session.SendMessage(message_2);
                                session.SendMessage(message_2);
                            }
                        }
                    }
                }
            }
        }
    }
}
