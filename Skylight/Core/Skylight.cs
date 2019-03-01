using FastFoodServerAPI;
using FastFoodServerAPI.Data;
using FastFoodServerAPI.Enums;
using FastFoodServerAPI.Interfaces;
using MySql.Data.MySqlClient;
using SkylightEmulator.Communication;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel;
using SkylightEmulator.HabboHotel.Cypto;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Net;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Core
{
    public sealed class Skylight
    {
        private static Revision TargetRevision = Revision.RELEASE63_35255_34886_201108111108; //most supported
        private static bool MultiRevisionSupport = true;
        private static bool ExternalFlashPolicyFileRequestPort = false;
        private static Stopwatch ServerStarted;
        private static ConfigurationData ConfigurationData;
        private static DatabaseManager DatabaseManager;
        private static Game Game;
        private static PacketManager PacketManager;
        private static SocketsManager SocketsManager;
        private static RCONListener RCONListener;
        private static MUSListener MUSListener;
        private static FlashPolicyFileRequestListener FlashPolicyFileRequestListener;
        public static volatile bool ServerShutdown = false;
        public static readonly int BuildNumber = Assembly.GetEntryAssembly().GetName().Version.Revision;
        public static readonly string Version = "Skylight 2.0.0.0 (Build: " + Skylight.BuildNumber + ")";
        private static string PublicToken;
        private static HabboCrypto HabboCrypto;
        private static LateQueryManager LateQueryManager;

        private static BigInteger n = new BigInteger("86851DD364D5C5CECE3C883171CC6DDC5760779B992482BD1E20DD296888DF91B33B936A7B93F06D29E8870F703A216257DEC7C81DE0058FEA4CC5116F75E6EFC4E9113513E45357DC3FD43D4EFAB5963EF178B78BD61E81A14C603B24C8BCCE0A12230B320045498EDC29282FF0603BC7B7DAE8FC1B05B52B2F301A9DC783B7", 16);
        private static BigInteger e = new BigInteger(3);
        private static BigInteger d = new BigInteger("59AE13E243392E89DED305764BDD9E92E4EAFA67BB6DAC7E1415E8C645B0950BCCD26246FD0D4AF37145AF5FA026C0EC3A94853013EAAE5FF1888360F4F9449EE023762EC195DFF3F30CA0B08B8C947E3859877B5D7DCED5C8715C58B53740B84E11FBC71349A27C31745FCEFEEEA57CFF291099205E230E0C7C27E8E1C0512B", 16);

        public void Initialize()
        {
            Logging.WriteLine("█▀▀▀█ █ █ █  █ █    ▀  █▀▀▀ █  █ ▀▀█▀▀", ConsoleColor.Yellow);
            Logging.WriteLine("▀▀▀▄▄ █▀▄ █▄▄█ █   ▀█▀ █ ▀█ █▀▀█   █  ", ConsoleColor.Yellow);
            Logging.WriteLine("█▄▄▄█ ▀ ▀ ▄▄▄█ ▀▀▀ ▀▀▀ ▀▀▀▀ ▀  ▀   ▀  ", ConsoleColor.Yellow);
            Logging.WriteLine(Skylight.Version, ConsoleColor.Yellow);
            Logging.WriteBlank();

            Logging.WriteLine(Licence.WelcomeMessage, ConsoleColor.Green);
            Logging.WriteBlank();

            try
            {
                Skylight.ServerStarted = Stopwatch.StartNew();
                Skylight.ConfigurationData = new ConfigurationData("config.conf");

                Logging.Write("Connecting to database... ", ConsoleColor.White);
                try
                {
                    DatabaseServer DatabaseServer = new DatabaseServer(Skylight.GetConfig()["db.hostname"], uint.Parse(Skylight.GetConfig()["db.port"]), Skylight.GetConfig()["db.username"], Skylight.GetConfig()["db.password"]);
                    Database Database = new Database(Skylight.GetConfig()["db.name"], uint.Parse(Skylight.GetConfig()["db.pool.minsize"]), uint.Parse(Skylight.GetConfig()["db.pool.maxsize"]));
                    Skylight.DatabaseManager = new DatabaseManager(DatabaseServer, Database);

                    using (DatabaseClient dbClient = Skylight.DatabaseManager.GetClient())
                    {
                        //WHAT AN LOVLY COMMAND WE HAVE OVER HERE! =D
                        dbClient.ExecuteQuery(@"DROP PROCEDURE IF EXISTS parse_activity_points;
CREATE PROCEDURE parse_activity_points(bound VARCHAR(255), bound2 VARCHAR(255))
  BEGIN
    DECLARE id INT DEFAULT 0;
    DECLARE value TEXT;
    DECLARE occurance INT DEFAULT 0;
    DECLARE i INT DEFAULT 0;
    DECLARE splitted_value TEXT;
    DECLARE splitted_value_2 TEXT;
    DECLARE splitted_value_3 TEXT;
    DECLARE done INT DEFAULT 0;
    DECLARE cur1 CURSOR FOR SELECT users.id, users.activity_points FROM users;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    DROP TEMPORARY TABLE IF EXISTS activity_points_parsed_data;
    CREATE TEMPORARY TABLE activity_points_parsed_data(`id` INT NOT NULL,`value` VARCHAR(255) NOT NULL,`value2` VARCHAR(255) NOT NULL, PRIMARY KEY (`id`,`value`)) ENGINE=Memory;
    OPEN cur1;
      read_loop: LOOP
        FETCH cur1 INTO id, value;
        IF done THEN LEAVE read_loop;
        END IF;
        SET occurance = (SELECT LENGTH(value) - LENGTH(REPLACE(value, bound, '')) +1);
        SET i=1;
        WHILE i <= occurance DO
          SET splitted_value = (SELECT REPLACE(SUBSTRING(SUBSTRING_INDEX(value, bound, i), LENGTH(SUBSTRING_INDEX(value, bound, i - 1)) + 1), bound, ''));
					SET splitted_value_2 = (SELECT REPLACE(SUBSTRING(SUBSTRING_INDEX(splitted_value, bound2, 1), LENGTH(SUBSTRING_INDEX(splitted_value, bound2, 1 - 1)) + 1), bound2, ''));
					SET splitted_value_3 = (SELECT REPLACE(SUBSTRING(SUBSTRING_INDEX(splitted_value, bound2, 2), LENGTH(SUBSTRING_INDEX(splitted_value, bound2, 2 - 1)) + 1), bound2, ''));
                    IF splitted_value_3 = '' THEN SET splitted_value_3 = splitted_value_2, splitted_value_2 = '0'; END IF;
          INSERT INTO activity_points_parsed_data VALUES (id, splitted_value_2, splitted_value_3) ON DUPLICATE KEY UPDATE value2 = value2 + splitted_value_3;
          SET i = i + 1;
        END WHILE;
      END LOOP;
    CLOSE cur1;
  END;");
                    }
                }
                catch(MySqlException ex)
                {
                    Logging.WriteLine("failed!", ConsoleColor.Red);

                    Skylight.ExceptionShutdown(ex);
                    return;
                }
                Logging.WriteLine("completed!", ConsoleColor.Green);

                Skylight.LateQueryManager = new LateQueryManager();

                Skylight.PublicToken = new BigInteger(DiffieHellman.GenerateRandomHexString(15), 16).ToString();
                Skylight.HabboCrypto = new HabboCrypto(n, e, d);

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("bannerData", Skylight.HabboCrypto.Prime + ":" + Skylight.HabboCrypto.Generator);
                    dbClient.ExecuteQuery("UPDATE users SET online = '0'; UPDATE rooms SET users_now = '0'; UPDATE server_settings SET banner_data = @bannerData;");
                }

                Skylight.TargetRevision = RevisionUtilies.StringToRevision(Skylight.GetConfig()["game.revision"]);
                Skylight.MultiRevisionSupport = TextUtilies.StringToBool(Skylight.GetConfig()["game.mrs.enabled"]);

                Skylight.PacketManager = BasicUtilies.GetRevisionPacketManager(Skylight.Revision); //needed for stuff
                Skylight.PacketManager.Initialize();

                Skylight.Game = new Game();
                Skylight.Game.Init();

                if (Skylight.GetConfig()["game.efpfr.enabled"] == "1")
                {
                    Skylight.ExternalFlashPolicyFileRequestPort = true;
                    Skylight.FlashPolicyFileRequestListener = new FlashPolicyFileRequestListener(Skylight.GetConfig()["game.efpfr.bindip"], int.Parse(Skylight.GetConfig()["game.efpfr.port"]));
                    Skylight.FlashPolicyFileRequestListener.Start();
                }

                Skylight.SocketsManager = new SocketsManager(Skylight.GetConfig()["game.tcp.bindip"], int.Parse(Skylight.GetConfig()["game.tcp.port"]), int.Parse(Skylight.GetConfig()["game.tcp.conlimit"]));
                foreach(string key in Skylight.ConfigurationData.GetChildKeys("game.tcp.extra"))
                {
                    Skylight.SocketsManager.AddListener(new SocketsListener(Skylight.SocketsManager, Skylight.GetConfig()["game.tcp.extra." + key + ".bindip"], int.Parse(Skylight.GetConfig()["game.tcp.extra." + key + ".port"]), RevisionUtilies.StringToRevision(Skylight.GetConfig()["game.tcp.extra." + key + ".revision"]), RevisionUtilies.StringToCrypto(Skylight.GetConfig().TryGet("game.tcp.extra." + key + ".crypto"))));
                }
                Skylight.SocketsManager.Start();

                if (Skylight.GetConfig()["rcon.tcp.enabled"] == "1")
                {
                    Skylight.RCONListener = new RCONListener(Skylight.GetConfig()["rcon.tcp.bindip"], int.Parse(Skylight.GetConfig()["rcon.tcp.port"]), Skylight.GetConfig()["rcon.tcp.allowedips"]);
                    Skylight.RCONListener.Start();
                }

                if (Skylight.GetConfig()["mus.tcp.enabled"] == "1")
                {
                    Skylight.MUSListener = new MUSListener(Skylight.GetConfig()["mus.tcp.bindip"], int.Parse(Skylight.GetConfig()["mus.tcp.port"]));
                    Skylight.MUSListener.Start();
                }

                TimeSpan bootTime = Skylight.Uptime;
                Logging.WriteLine("READY! (" + bootTime.Seconds + " s, " + bootTime.Milliseconds + " ms)", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                Logging.WriteLine("FAILED TO BOOT! ", ConsoleColor.Red);

                Skylight.ExceptionShutdown(ex);
            }
        }

        public static TimeSpan Uptime
        {
            get
            {
                return Skylight.ServerStarted.Elapsed;
            }
        }

        public static bool MultiRevisionSupportEnabled
        {
            get
            {
                return Skylight.MultiRevisionSupport;
            }
        }

        public static bool ExternalFlashPolicyFileRequestPortEnabled
        {
            get
            {
                return Skylight.ExternalFlashPolicyFileRequestPort;
            }
        }

        public static Revision Revision
        {
            get
            {
                return Skylight.TargetRevision;
            }
        }

        public static void ExceptionShutdown(Exception exception)
        {
            Logging.WriteLine(exception.ToString(), ConsoleColor.Red);
            Logging.WriteBlank();
            Logging.WriteLine("Press any key to close", ConsoleColor.Blue);
            Console.ReadKey();
            Program.Destroy();
        }

        public static HabboCrypto GetHabboCrypto()
        {
            return Skylight.HabboCrypto;
        }

        public static ConfigurationData GetConfig()
        {
            return Skylight.ConfigurationData;
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return Skylight.DatabaseManager;
        }

        public static Game GetGame()
        {
            return Skylight.Game;
        }

        public static SocketsManager GetSocketsManager()
        {
            return Skylight.SocketsManager;
        }

        public static Encoding GetDefaultEncoding()
        {
            return Encoding.Default;
        }

        public static PacketManager GetPacketManager()
        {
            return Skylight.PacketManager;
        }

        public static string GetPublicToken()
        {
            return Skylight.PublicToken;
        }

        public static LateQueryManager GetLateQueryManager()
        {
            return Skylight.LateQueryManager;
        }

        public static void Destroy(bool close, bool takeBackup, bool backupCompress, bool restart)
        {
            if (Skylight.ServerShutdown)
            {
                return;
            }

            Skylight.ServerShutdown = true;

            try //we dont want shutdowns fails bcs of this
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.SendNotifFromAdmin);
                message.AppendString("ATTENTION:\r\nThe server is shutting down. All furniture placed in rooms/traded/bought after this message is on your own responsibillity.");
                byte[] data = message.GetBytes();

                foreach (GameClient client in Skylight.GetGame().GetGameClientManager().GetClients())
                {
                    try //try send
                    {
                        if (client != null)
                        {
                            client.SendData(data);
                        }
                    }
                    catch //ignore error
                    {

                    }
                }
            }
            catch //nothing special really
            {

            }

            if (Skylight.ExternalFlashPolicyFileRequestPort)
            {
                Logging.Write("Disposing efpfr listener... ");
                if (Skylight.FlashPolicyFileRequestListener != null)
                {
                    Skylight.FlashPolicyFileRequestListener.Stop();
                }
                Skylight.FlashPolicyFileRequestListener = null;
                Logging.WriteLine("DONE!", ConsoleColor.Green);
            }

            Logging.Write("Disposing socket manager... ");
            if (Skylight.SocketsManager != null)
            {
                Skylight.SocketsManager.Stop();
            }
            Skylight.SocketsManager = null;
            Logging.WriteLine("DONE!", ConsoleColor.Green);

            Logging.Write("Disposing packet manager... ");
            if (Skylight.PacketManager != null) //release memory
            {
                Skylight.PacketManager.Clear();
            }
            Skylight.PacketManager = null;
            Logging.WriteLine("DONE!", ConsoleColor.Green);

            Logging.Write("Disposing RCON... ");
            if (Skylight.RCONListener != null)
            {
                Skylight.RCONListener.Stop();
            }
            Logging.WriteLine("DONE!", ConsoleColor.Green);

            Logging.Write("Disposing game... ");
            if (Skylight.Game != null)
            {
                Skylight.Game.Shutdown();
            }
            Skylight.Game = null;
            Logging.WriteLine("DONE!", ConsoleColor.Green);

            if (takeBackup)
            {
                Logging.WriteLine("Taking backup... ");
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.TakeBackup(backupCompress);
                }
                Logging.WriteLine("DONE!", ConsoleColor.Green);
            }

            Logging.Write("Disposing database... ");
            if (Skylight.DatabaseManager != null)
            {
                MySqlConnection.ClearAllPools();
            }
            Skylight.DatabaseManager = null;
            Logging.WriteLine("DONE!", ConsoleColor.Green);

            Logging.WriteLine("");
            Logging.WriteLine("Server closed propriety!", ConsoleColor.Green);

            if (restart)
            {
                Process.Start(AppDomain.CurrentDomain.FriendlyName);
            }

            if (close)
            {
                Environment.Exit(0);
            }
        }
    }
}
