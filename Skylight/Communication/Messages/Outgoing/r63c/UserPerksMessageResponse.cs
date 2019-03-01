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

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class UserPerksMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            GameClient session = valueHolder.GetValue<GameClient>("Session");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message.Init(r63cOutgoing.UserPerks);
            message.AppendInt32(7);

            message.AppendString("NAVIGATOR_PHASE_ONE_2014");
            message.AppendString("");
            message.AppendBoolean(true);

            message.AppendString("NAVIGATOR_PHASE_TWO_2014");
            message.AppendString("");
            message.AppendBoolean(true);

            message.AppendString("TRADE");
            message.AppendString("");
            message.AppendBoolean(session.GetHabbo().GetPerks().Contains("TRADE"));

            message.AppendString("CITIZEN");
            message.AppendString("");
            message.AppendBoolean(session.GetHabbo().GetPerks().Contains("CITIZEN"));

            message.AppendString("GIVE_GUIDE_TOURS");
            message.AppendString("");
            message.AppendBoolean(session.GetHabbo().CanGiveTour);

            message.AppendString("USE_GUIDE_TOOL");
            message.AppendString("");
            message.AppendBoolean(session.GetHabbo().IsHelper);

            message.AppendString("JUDGE_CHAT_REVIEWS");
            message.AppendString("");
            message.AppendBoolean(session.GetHabbo().IsGuardian);




            //message.AppendInt32(15);

            //message.AppendString("USE_GUIDE_TOOL");
            //message.AppendString("requirement.unfulfilled.helper_level_4");
            //message.AppendBoolean(true);

            //message.AppendString("GIVE_GUIDE_TOURS");
            //message.AppendString("");
            //message.AppendBoolean(true);

            //message.AppendString("JUDGE_CHAT_REVIEWS");
            //message.AppendString("requirement.unfulfilled.helper_level_6");
            //message.AppendBoolean(true);

            //message.AppendString("VOTE_IN_COMPETITIONS");
            //message.AppendString("requirement.unfulfilled.helper_level_2");
            //message.AppendBoolean(true);

            //message.AppendString("CALL_ON_HELPERS");
            //message.AppendString("");
            //message.AppendBoolean(true);

            //message.AppendString("CITIZEN");
            //message.AppendString("");
            //message.AppendBoolean(true);

            //message.AppendString("TRADE");
            //message.AppendString("");
            //message.AppendBoolean(true);

            //message.AppendString("HEIGHTMAP_EDITOR_BETA");
            //message.AppendString("requirement.unfulfilled.feature_disabled");
            //message.AppendBoolean(true);

            //message.AppendString("BUILDER_AT_WORK");
            //message.AppendString("");
            //message.AppendBoolean(true);

            //message.AppendString("NAVIGATOR_PHASE_ONE_2014");
            //message.AppendString("");
            //message.AppendBoolean(true);

            //message.AppendString("CAMERA");
            //message.AppendString("");
            //message.AppendBoolean(true);

            //message.AppendString("NAVIGATOR_PHASE_TWO_2014");
            //message.AppendString("");
            //message.AppendBoolean(true);

            //message.AppendString("MOUSE_ZOOM");
            //message.AppendString("");
            //message.AppendBoolean(true);

            //message.AppendString("NAVIGATOR_ROOM_THUMBNAIL_CAMERA");
            //message.AppendString("");
            //message.AppendBoolean(false);

            //message.AppendString("HABBO_CLUB_OFFER_BETA");
            //message.AppendString("");
            //message.AppendBoolean(true);
            return message;
        }
    }
}
