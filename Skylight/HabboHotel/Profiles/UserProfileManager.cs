using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Profiles
{
    public class UserProfileManager
    {
        private Dictionary<uint, UserProfile> Profiles;

        public UserProfileManager()
        {
            this.Profiles = new Dictionary<uint, UserProfile>();
        }

        public UserProfile GetProfile(uint userId)
        {
            GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
            if (!this.Profiles.TryGetValue(userId, out UserProfile profile))
            {
                profile = this.Profiles[userId] = new UserProfile(userId);

                if (target?.GetHabbo() == null)
                {
                    DataRow user = null;
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("userId", userId);
                        user = dbClient.ReadDataRow("SELECT u.username, u.look, u.motto, u.account_created, u.online, u.last_online, s.achievement_points, COUNT(f.user_one_id) AS friends, GROUP_CONCAT(IF(IF(f.user_one_id = @userId, f.user_one_relation, f.user_two_relation) = '1', f.user_one_relation, NULL)) AS lovers, GROUP_CONCAT(IF(IF(f.user_one_id = @userId, f.user_one_relation, f.user_two_relation) = '2', f.user_one_relation, NULL)) AS friends2 , GROUP_CONCAT(IF(IF(f.user_one_id = @userId, f.user_one_relation, f.user_two_relation) = '3', f.user_one_relation, NULL)) AS haters, GROUP_CONCAT(b.badge_id ORDER BY b.badge_slot) AS badges FROM users u LEFT JOIN user_stats s ON u.id = s.user_id LEFT JOIN messenger_friends f ON f.user_one_id = @userId OR f.user_two_id = @userId LEFT JOIN user_badges b ON b.user_id = u.id AND b.badge_slot > 0 WHERE u.id = @userId GROUP BY u.id");
                    }

                    if (user != null)
                    {
                        profile.UpdateValues(user);
                        profile.SetOnline(TextUtilies.StringToBool((string)user["online"]));
                    }
                }
            }

            if (target?.GetHabbo() != null)
            {
                profile.UpdateValues(target.GetHabbo());
                profile.SetOnline(true);
            }
            else
            {
                profile.SetOnline(false);
            }

            return profile;
        }
    }
}
