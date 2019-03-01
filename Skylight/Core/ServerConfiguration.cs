using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Core
{
    public class ServerConfiguration
    {
        public static bool EnableSecureSession = false;
        public static bool EveryoneVIP = true;
        public static int MaxRoomsPerUser = 100;
        public static bool UseIPLastForBans = true;
        public static string MOTD = "This emulator is still in HUGE development state :)";

        //marketplace
        public static bool EnableMarketplace = true;
        public static int MarketplaceMinPrice = 1;
        public static int MarketplaceMaxPrice = 10000;
        public static int MarketplaceOffersActiveHours = 48;
        public static int MarketplaceTokensNonPremium = 5;
        public static int MarketplaceTokensPremium = 10;
        public static int MarketplaceTokensPrice = 1;
        public static int MarketplaceCompremission = 1;
        public static int MarketplaceAvaragePriceDays = 7;

        //events
        public static bool EventsEnabled = true;

        //crypto
        public static bool EnableCrypto = false;
        public static bool RequireMachineID = true;
        public static int CryptoType = 0;

        //idle kicks
        public static double IdleTime = 5.0 * 60.0; //min * secs, 5 mins
        public static double IdleKickTime = 15.0 * 60.0; //min * secs, 15 mins

        //activity bonus
        public static double ActivityBonusTime = 15.0 * 60.0; //min * secs, 15 mins
        public static int CreditsBonus = 2000;
        public static int PixelsBonus = 250;
        public static int ActivityPointsBonusType = 4;
        public static int ActivityPointsBonus = 1;

        //blacklist command
        public static List<string> AllowedComamndsToBeBlacklisted = new List<string>();
        public static Dictionary<int, int> BlacklistedEffects = new Dictionary<int, int>();

        //security
        public static int MinRankRequireLogin = 10;

        //staff picks
        public static int StaffPicksCategoryId = 0;

        public static void LoadConfigsFromDB(DatabaseClient dbClient)
        {
            Logging.Write("Loading server settings... ");

            DataRow settings = dbClient.ReadDataRow("SELECT * FROM server_settings");
            ServerConfiguration.EnableSecureSession = TextUtilies.StringToBool((string)settings["enable_secure_session"]);
            ServerConfiguration.EveryoneVIP = TextUtilies.StringToBool((string)settings["everyone_vip"]);
            ServerConfiguration.MaxRoomsPerUser = (int)settings["max_rooms_per_user"];
            ServerConfiguration.UseIPLastForBans = TextUtilies.StringToBool((string)settings["ip_last_for_bans"]);
            ServerConfiguration.MOTD = (string)settings["motd"];
            ServerConfiguration.EnableMarketplace = TextUtilies.StringToBool((string)settings["enable_marketplace"]);
            ServerConfiguration.MarketplaceMinPrice = (int)settings["marketplace_min_price"];
            ServerConfiguration.MarketplaceMaxPrice = (int)settings["marketplace_max_price"];
            ServerConfiguration.MarketplaceOffersActiveHours = (int)settings["marketplace_offers_active_hours"];
            ServerConfiguration.MarketplaceTokensNonPremium = (int)settings["marketplace_tokens_non_premium"];
            ServerConfiguration.MarketplaceTokensPremium = (int)settings["marketplace_tokens_premium"];
            ServerConfiguration.MarketplaceTokensPrice = (int)settings["marketplace_tokens_price"];
            ServerConfiguration.MarketplaceCompremission = (int)settings["marketplace_compremission"];
            ServerConfiguration.MarketplaceAvaragePriceDays = (int)settings["marketplace_avarage_price_days"];
            ServerConfiguration.EventsEnabled = TextUtilies.StringToBool((string)settings["events_enabled"]);
            ServerConfiguration.EnableCrypto = TextUtilies.StringToBool((string)settings["enable_crypto"]);
            ServerConfiguration.RequireMachineID = TextUtilies.StringToBool((string)settings["require_machine_id"]);
            ServerConfiguration.CryptoType = (int)settings["crypto_type"];
            ServerConfiguration.IdleTime = (int)settings["idle_time"];
            ServerConfiguration.IdleKickTime = (int)settings["idle_kick_time"];
            ServerConfiguration.ActivityBonusTime = (int)settings["activity_bonus_time"];
            ServerConfiguration.CreditsBonus = (int)settings["credits_bonus"];
            ServerConfiguration.PixelsBonus = (int)settings["pixels_bonus"];
            ServerConfiguration.ActivityPointsBonusType = (int)settings["activity_points_bonus_type"];
            ServerConfiguration.ActivityPointsBonus = (int)settings["activity_points_bonus"];
            ServerConfiguration.AllowedComamndsToBeBlacklisted = ((string)settings["allowed_commands_to_be_blacklisted"]).ToLower().Split(',').ToList();
            ServerConfiguration.MinRankRequireLogin = (int)settings["min_rank_require_login"];

            ServerConfiguration.BlacklistedEffects.Clear();
            foreach (string string_ in ((string)settings["blacklisted_effects"]).Split(';'))
            {
                string[] string_2 = string_.Split(',');
                if (string_2.Length == 2)
                {
                    ServerConfiguration.BlacklistedEffects.Add(int.Parse(string_2[0]), int.Parse(string_2[1]));
                }
            }

            ServerConfiguration.StaffPicksCategoryId = (int)settings["staff_picks_category_id"];

            Logging.WriteLine("completed!", ConsoleColor.Green);
        }
    }
}
