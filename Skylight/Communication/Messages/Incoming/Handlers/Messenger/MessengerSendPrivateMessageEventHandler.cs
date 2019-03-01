using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.Core;
using SkylightEmulator.Utilies;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger
{
    public class MessengerSendPrivateMessageEventHandler : IncomingPacket
    {
        protected uint UserID;
        protected string Message;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetMessenger() != null)
            {
                if (this.UserID > 0) //Real user
                {
                    if (!session.GetHabbo().GetMessenger().IsFriendWith(this.UserID))
                    {
                        session.SendMessage(new MessengerSendPrivateMessageErrorComposerHandler(this.UserID, MessengerSendPrivateMessageErrorCode.NotFriend));
                    }
                    else
                    {
                        if (session.GetHabbo().IsMuted())
                        {
                            session.SendMessage(new MessengerSendPrivateMessageErrorComposerHandler(this.UserID, MessengerSendPrivateMessageErrorCode.SenderMuted));
                        }
                        else
                        {
                            GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.UserID);
                            if (target?.GetHabbo()?.GetMessenger() != null)
                            {
                                if (target.GetHabbo().IsMuted())
                                {
                                    session.SendMessage(new MessengerSendPrivateMessageErrorComposerHandler(this.UserID, MessengerSendPrivateMessageErrorCode.ReceiverMuted));
                                }
                                else
                                {
                                    string filteredMessage = TextUtilies.CheckBlacklistedWords(TextUtilies.FilterString(this.Message));

                                    if (!session.GetHabbo().HasPermission("acc_nochatlog_pm"))
                                    {
                                        Skylight.GetGame().GetChatlogManager().LogPrivateMessage(session, target, this.Message);
                                    }

                                    target.SendMessage(new MessengerReceivePrivateMessageComposerHandler(session.GetHabbo().ID, filteredMessage));
                                }
                            }
                            else
                            {
                                session.SendMessage(new MessengerSendPrivateMessageErrorComposerHandler(this.UserID, MessengerSendPrivateMessageErrorCode.Offline));
                            }
                        }
                    }
                }
                else if (this.UserID == 0) //Staff chat
                {
                    if (session.GetHabbo().HasPermission("acc_staffchat"))
                    {
                        List<uint> receiverIds = new List<uint>();
                        List<string> receiverUsernames = new List<string>();
                        List<int> receiverSessionIds = new List<int>();

                        foreach (GameClient session_ in Skylight.GetGame().GetGameClientManager().GetClients())
                        {
                            if ((session_?.GetHabbo()?.HasPermission("acc_staffchat") ?? false) && session_.GetHabbo().ID != session.GetHabbo().ID)
                            {
                                receiverIds.Add(session_.GetHabbo().ID);
                                receiverUsernames.Add(session_.GetHabbo().Username);
                                receiverSessionIds.Add(session_.SessionID);

                                session_.SendMessage(new MessengerReceivePrivateMessageComposerHandler(this.UserID, this.Message, 0, session.GetHabbo().Username, session.GetHabbo().Look, session.GetHabbo().ID));
                            }
                        }

                        Skylight.GetGame().GetChatlogManager().LogStaffChatMessage(session, receiverIds, receiverUsernames, receiverSessionIds, this.Message);
                    }
                    else
                    {
                        session.SendMessage(new MessengerSendPrivateMessageErrorComposerHandler(this.UserID, MessengerSendPrivateMessageErrorCode.NotFriend));
                    }
                }
                else //Group chats
                {
                    //MUCH TODO

                    session.SendMessage(new MessengerSendPrivateMessageErrorComposerHandler(this.UserID, MessengerSendPrivateMessageErrorCode.Offline));
                }
            }
        }
    }
}
