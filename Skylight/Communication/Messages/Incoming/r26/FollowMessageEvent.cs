using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class FollowMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                uint friendId = message.PopWiredUInt();

                GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(friendId);
                if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetRoomSession() != null && gameClient.GetHabbo().GetRoomSession().IsInRoom && gameClient.GetHabbo().GetRoomSession().CurrentRoomID != session.GetHabbo().GetRoomSession().CurrentRoomID && gameClient.GetHabbo().GetUserSettings() != null && !gameClient.GetHabbo().GetUserSettings().HideInRoom)
                {
                    Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(gameClient.GetHabbo().GetRoomSession().CurrentRoomID);
                    if (room != null)
                    {
                        ServerMessage followFriend = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                        followFriend.Init(r26Outgoing.RoomForward);
                        followFriend.AppendBoolean(room.RoomData.IsPublicRoom);
                        followFriend.AppendUInt(room.ID);
                        session.SendMessage(followFriend);
                    }
                }
            }
        }
    }
}
