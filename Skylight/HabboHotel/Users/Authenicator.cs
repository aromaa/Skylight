using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users
{
    public class Authenicator
    {
        public static Habbo LoadHabbo(UserDataFactory userData, GameClient session)
        {
            DataRow habboData = userData.GetUserData();

            uint id = (uint)habboData["id"];
            string username = (string)habboData["username"];
            string realName = (string)habboData["real_name"];
            string email = (string)habboData["mail"];
            string sso = (string)habboData["auth_ticket"];
            int rank = (int)habboData["rank"];
            int credits = (int)habboData["credits"];
            string activityPoints = (string)habboData["activity_points"];
            double activityPointsLastUpdate = (double)habboData["activity_points_lastupdate"];
            string look = (string)habboData["look"];
            string gender = (string)habboData["gender"];
            string motto = (string)habboData["motto"];
            double accountCreated = (double)habboData["account_created"];
            double lastOnline = (double)habboData["last_online"];
            string ipLast = (string)habboData["ip_last"];
            uint homeRoom = (uint)habboData["home_room"];
            int dailyRespectPoints = (int)habboData["daily_respect_points"];
            int dailyPetRespectPoints = (int)habboData["daily_pet_respect_points"];
            double muteExpires = (double)habboData["mute_expires"];
            bool blockNewFriends = TextUtilies.StringToBool((string)habboData["block_newfriends"]);
            bool hideOnline = TextUtilies.StringToBool((string)habboData["hide_online"]);
            bool hideInRoom = TextUtilies.StringToBool((string)habboData["hide_inroom"]);
            int volume = (int)habboData["volume"];
            bool acceptTrading = TextUtilies.StringToBool((string)habboData["accept_trading"]);
            return new Habbo(session, userData, id, username, realName, email, sso, rank, credits, activityPoints, activityPointsLastUpdate, look, gender, motto, accountCreated, lastOnline, ipLast, homeRoom, dailyRespectPoints, dailyPetRespectPoints, muteExpires, blockNewFriends, hideOnline, hideInRoom, volume, acceptTrading);
        }
    }
}
