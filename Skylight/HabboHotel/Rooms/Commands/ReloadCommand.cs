using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class ReloadCommand : Command
    {
        public override string CommandInfo()
        {
            return ":reload - Reloads the room";
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
                uint roomId = session.GetHabbo().GetRoomSession().CurrentRoomID;
                List<GameClient> users = session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.GetRealUsers().Select(u => ((RoomUnitUser)u).Session).ToList();

                Skylight.GetGame().GetRoomManager().UnloadRoom(session.GetHabbo().GetRoomSession().GetRoom());
                Room room = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoom(roomId);
                if (room != null)
                {
                    ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message.Init(r63aOutgoing.RoomForward);
                    message.AppendBoolean(room.RoomData.IsPublicRoom);
                    message.AppendUInt(room.ID);

                    byte[] data = message.GetBytes();
                    foreach(GameClient session_ in users)
                    {
                        try
                        {
                            session_.SendData(data);
                            session_.SendNotif("Room reloaded! Sending you back!");
                        }
                        catch
                        {

                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
