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
            string ipReg = (string)habboData["ip_reg"];
            uint homeRoom = (uint)habboData["home_room"];
            int dailyRespectPoints = (int)habboData["daily_respect_points"];
            int dailyPetRespectPoints = (int)habboData["daily_pet_respect_points"];
            double muteExpires = (double)habboData["mute_expires"];
            bool blockNewFriends = TextUtilies.StringToBool((string)habboData["block_newfriends"]);
            bool hideOnline = TextUtilies.StringToBool((string)habboData["hide_online"]);
            bool hideInRoom = TextUtilies.StringToBool((string)habboData["hide_inroom"]);
            int[] volume = ((string)habboData["volume"]).Split(new char[] { ',' }, 3, StringSplitOptions.RemoveEmptyEntries).Select(v => int.TryParse(v, out int i) ? i : 100).ToArray();
            if (volume.Length == 0)
            {
                volume = new int[] { 100, 100, 100 };
            }
            else if (volume.Length == 1)
            {
                volume = new int[] { volume[0], 100, 100 };
            }
            else if (volume.Length == 2)
            {
                volume = new int[] { volume[0], volume[1], 100 };
            }
            bool acceptTrading = TextUtilies.StringToBool((string)habboData["accept_trading"]);
            int marketplaceTokens = (int)habboData["marketplace_tokens"];
            int newbieStatus = (int)habboData["newbie_status"];
            uint newbieRoom = (uint)habboData["newbie_room"];
            bool friendStream = TextUtilies.StringToBool((string)habboData["friend_stream"]);
            string twoFactoryAuthenicationSecretCode = (string)habboData["two_factory_authenication_secret_code"];
            bool mailConfirmed = TextUtilies.StringToBool((string)habboData["mail_confirmed"]);
            bool preferOldChat = TextUtilies.StringToBool((string)habboData["prefer_old_chat"]);
            bool blockRoomInvites = TextUtilies.StringToBool((string)habboData["block_room_invites"]);
            bool blockCameraFollow = TextUtilies.StringToBool((string)habboData["block_camera_follow"]);
            int chatColor = (int)habboData["chat_color"];
            double guideEnrollmentTimestamp = (double)habboData["guide_enrollment_timestamp"];
            return new Habbo(session, userData, id, username, realName, email, sso, rank, credits, activityPoints, activityPointsLastUpdate, look, gender, motto, accountCreated, lastOnline, ipLast, ipReg, homeRoom, dailyRespectPoints, dailyPetRespectPoints, muteExpires, blockNewFriends, hideOnline, hideInRoom, volume, acceptTrading, marketplaceTokens, newbieStatus, newbieRoom, friendStream, twoFactoryAuthenicationSecretCode, mailConfirmed, preferOldChat, blockRoomInvites, blockCameraFollow, chatColor, guideEnrollmentTimestamp);
        }
    }
}
