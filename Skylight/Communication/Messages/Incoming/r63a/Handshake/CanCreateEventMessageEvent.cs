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
    class CanCreateEventMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
            if (room != null)
            {
                if (room.HaveOwnerRights(session))
                {
                    if (room.RoomData.State == RoomStateType.OPEN)
                    {
                        if (ServerConfiguration.EventsEnabled)
                        {
                            if (room.RoomEvent == null)
                            {
                                foreach(uint roomId in session.GetHabbo().UserRooms)
                                {
                                    Room room_ = Skylight.GetGame().GetRoomManager().TryGetRoom(roomId);
                                    if (room_ != null && room_.RoomEvent != null) //if any room events is already going
                                    {
                                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                        message_.Init(r63aOutgoing.CanCreateEventResult);
                                        message_.AppendBoolean(false); //can create
                                        message_.AppendInt32(6); //error code
                                        session.SendMessage(message_);

                                        return;
                                    }
                                }

                                ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                message_2.Init(r63aOutgoing.CanCreateEventResult);
                                message_2.AppendBoolean(true); //can create
                                message_2.AppendInt32(0); //error code
                                session.SendMessage(message_2);
                            }
                            else
                            {
                                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                message_.Init(r63aOutgoing.CanCreateEventResult);
                                message_.AppendBoolean(false); //can create
                                message_.AppendInt32(5); //error code
                                session.SendMessage(message_);
                            }
                        }
                        else
                        {
                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            message_.Init(r63aOutgoing.CanCreateEventResult);
                            message_.AppendBoolean(false); //can create
                            message_.AppendInt32(4); //error code
                            session.SendMessage(message_);
                        }
                    }
                    else
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.CanCreateEventResult);
                        message_.AppendBoolean(false); //can create
                        message_.AppendInt32(3); //error code
                        session.SendMessage(message_);
                    }
                }
                else
                {
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.CanCreateEventResult);
                    message_.AppendBoolean(false); //can create
                    message_.AppendInt32(2); //error code
                    session.SendMessage(message_);
                }
            }
            else
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.CanCreateEventResult);
                message_.AppendBoolean(false); //can create
                message_.AppendInt32(1); //error code
                session.SendMessage(message_);
            }
        }
    }
}
