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
    class LetUserInMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = Skylight.GetGame().GetRoomManager().GetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
                if (room != null && room.HaveRights(session))
                {
                    string username = message.PopFixedString();
                    GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(username);
                    if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetRoomSession() != null && gameClient.GetHabbo().GetRoomSession().WaitingForDoorbellAnswer && gameClient.GetHabbo().GetRoomSession().RequestedRoomID == room.ID)
                    {
                        bool letIn = message.PopBase64Boolean();
                        if (letIn)
                        {
                            gameClient.GetHabbo().GetRoomSession().LoadingRoom = true;

                            ServerMessage doorBellAnswer = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                            doorBellAnswer.Init(r63aOutgoing.DoorBellAnswer);
                            gameClient.SendMessage(doorBellAnswer);
                        }
                        else
                        {
                            ServerMessage doorBellNoAnswer = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                            doorBellNoAnswer.Init(r63aOutgoing.DoorBellNoAnswer);
                            gameClient.SendMessage(doorBellNoAnswer);
                        }
                    }
                }
            }
        }
    }
}
