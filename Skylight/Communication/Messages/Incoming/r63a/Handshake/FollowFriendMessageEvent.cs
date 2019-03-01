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
    class FollowFriendMessageEvent : IncomingPacket
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
                        ServerMessage followFriend = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        followFriend.Init(r63aOutgoing.RoomForward);
                        followFriend.AppendBoolean(room.RoomData.IsPublicRoom);
                        followFriend.AppendUInt(room.ID);
                        session.SendMessage(followFriend);
                    }
                }
            }
        }
    }
}
