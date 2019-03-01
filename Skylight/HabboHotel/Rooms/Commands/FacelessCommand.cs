using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class FacelessCommand : Command
    {
        public override string CommandInfo()
        {
            return ":faceless - Removes your face :O (!PAINFUL!)";
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
            FigureParts parts = new FigureParts(session.GetHabbo().Look);
            Dictionary<string, object> data = parts.GetPart("hd");
            if (data == null)
            {
                data = new Dictionary<string, object>() { { "type", "hd" }, {"setid", "99999"}, { "colorids", new List<string>() { "2" } } };
            }

            if ((data["setid"] as string) != "99999")
            {
                data["setid"] = "99999";

                parts.ReplacePart(data);
                session.GetHabbo().Look = parts.GetPartString();

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                    dbClient.AddParamWithValue("look", session.GetHabbo().Look);

                    dbClient.ExecuteQuery("UPDATE users SET look = @look WHERE id = @userId LIMIT 1");
                }

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.UpdateUser);
                message_.AppendInt32(-1);
                message_.AppendString(session.GetHabbo().Look);
                message_.AppendString(session.GetHabbo().Gender.ToLower());
                message_.AppendString(session.GetHabbo().Motto);
                message_.AppendInt32(session.GetHabbo().GetUserStats().AchievementPoints);
                session.SendMessage(message_);

                ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_2.Init(r63aOutgoing.UpdateUser);
                message_2.AppendInt32(session.GetHabbo().GetRoomSession().GetRoomUser().VirtualID);
                message_2.AppendString(session.GetHabbo().Look);
                message_2.AppendString(session.GetHabbo().Gender.ToLower());
                message_2.AppendString(session.GetHabbo().Motto);
                message_2.AppendInt32(session.GetHabbo().GetUserStats().AchievementPoints);
                session.GetHabbo().GetRoomSession().GetRoom().SendToAll(message_2);

                Skylight.GetGame().GetAchievementManager().AddAchievement(session, "AvatarLook", 1);
            }

            return true;
        }
    }
}
