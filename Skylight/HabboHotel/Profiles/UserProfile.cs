using SkylightEmulator.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Users.Badges;

namespace SkylightEmulator.HabboHotel.Profiles
{
    public class UserProfile
    {
        private static readonly List<uint> Empty = new List<uint>(0);
        private static readonly List<string> EmptyBadges = new List<string>(0);

        public readonly uint UserID;
        public string Username { get; private set; }
        public string Look { get; private set; }
        public string Motto { get; private set; }
        public double AccountCreated { get; private set; }
        public int AchievementScore { get; private set; }
        public int FriendsCount { get; private set; }
        public bool Online { get; private set; }
        public double LastOnline { get; private set; }
        public List<uint> Lovers { get; private set; }
        public List<uint> Friends { get; private set; }
        public List<uint> Haters { get; private set; }
        public List<string> Badges { get; private set; } //Has right order

        public UserProfile(uint userId)
        {
            this.UserID = userId;
        }

        public void UpdateValues(Habbo habbo)
        {
            this.Username = habbo.Username;
            this.Look = habbo.Look;
            this.Motto = habbo.Motto;
            this.AccountCreated = habbo.AccountCreated;
            this.AchievementScore = habbo.GetUserStats()?.AchievementPoints ?? 0;
            this.FriendsCount = habbo.GetMessenger()?.GetFriends()?.Count ?? 0;
            this.LastOnline = habbo.LastOnline;
            this.Lovers = habbo.GetMessenger()?.GetFriends()?.Where(f => f.Relation == MessengerFriendRelation.Love).Select(f => f.ID).ToList() ?? UserProfile.Empty;
            this.Friends = habbo.GetMessenger()?.GetFriends()?.Where(f => f.Relation == MessengerFriendRelation.Smile).Select(f => f.ID).ToList() ?? UserProfile.Empty;
            this.Haters = habbo.GetMessenger()?.GetFriends()?.Where(f => f.Relation == MessengerFriendRelation.Angry).Select(f => f.ID).ToList() ?? UserProfile.Empty;
            this.Badges = habbo.GetBadgeManager()?.GetActiveBadges().Select(b => b.BadgeID).ToList() ?? UserProfile.EmptyBadges;
        }

        public void UpdateValues(DataRow user)
        {
            this.Username = (string)user["username"];
            this.Look = (string)user["look"];
            this.Motto = (string)user["motto"];
            this.AccountCreated = (double)user["account_created"];
            this.AchievementScore = (user["achievement_points"] as int?) ?? 0;
            this.FriendsCount = (int)((user["friends"] as long?) ?? 0);
            this.LastOnline = (double)user["last_online"];
            this.Lovers = (user["lovers"] as string ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(u => uint.Parse(u)).ToList();
            this.Friends = (user["friends2"] as string ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(u => uint.Parse(u)).ToList();
            this.Haters = (user["haters"] as string ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(u => uint.Parse(u)).ToList();
            this.Badges = (user["badges"] as string ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public void SetOnline(bool online)
        {
            this.Online = online;
        }
    }
}
