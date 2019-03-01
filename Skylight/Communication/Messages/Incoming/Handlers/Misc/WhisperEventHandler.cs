using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class WhisperEventHandler : IncomingPacket
    {
        protected string Username;
        protected string Message;
        protected int Bubble;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            Room room = session?.GetHabbo()?.GetRoomSession()?.GetRoom();
            if (room != null)
            {
                if (!room.RoomMute || session.GetHabbo().HasPermission("acc_ignore_roommute"))
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
                        }


                        RoomUnitUser user = room.RoomUserManager.GetUserByName(this.Username);
                        if (user != null)
                        {
                            string filteredMessage = TextUtilies.CheckBlacklistedWords(TextUtilies.FilterString(this.Message));

                            if (!session.GetHabbo().HasPermission("acc_nochatlog_whisper"))
                            {
                                Skylight.GetGame().GetChatlogManager().LogRoomMessage(session, "Whisper" + (char)9 + user.Session.GetHabbo().ID + (char)9 + this.Message);
                            }

                            session.GetHabbo().GetUserSettings().ChatColor = this.Bubble;

                            session.SendMessage(new ReceiveWhisperComposerHandler(session.GetHabbo().GetRoomSession().GetRoomUser().VirtualID, filteredMessage, this.Bubble));
                            user.Session.SendMessage(new ReceiveWhisperComposerHandler(session.GetHabbo().GetRoomSession().GetRoomUser().VirtualID, filteredMessage, this.Bubble));
                        }
                    }
                }
            }
        }
    }
}
