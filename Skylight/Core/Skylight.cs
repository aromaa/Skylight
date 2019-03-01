using MySql.Data.MySqlClient;
using SkylightEmulator.Communication;
using SkylightEmulator.HabboHotel;
using SkylightEmulator.Net;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Core
{
    public sealed class Skylight
    {
        public static readonly Revision Revision = Revision.RELEASE63_35255_34886_201108111108;
        private static DateTime ServerStarted;
        private static ConfigurationData ConfigurationData;
        private static DatabaseManager DatabaseManager;
        private static Game Game;
        private static PacketManager PacketManager;
        private static SocketsManager SocketsManager;

        public static bool ServerShutdown = false;

        public void Initialize()
        {
            Logging.WriteLine("█▀▀▀█ █ █ █  █ █    ▀  █▀▀▀ █  █ ▀▀█▀▀", ConsoleColor.Yellow);
            Logging.WriteLine("▀▀▀▄▄ █▀▄ █▄▄█ █   ▀█▀ █ ▀█ █▀▀█   █  ", ConsoleColor.Yellow);
            Logging.WriteLine("█▄▄▄█ ▀ ▀ ▄▄▄█ ▀▀▀ ▀▀▀ ▀▀▀▀ ▀  ▀   ▀  ", ConsoleColor.Yellow);
            Logging.WriteBlank();

            try
            {
                Skylight.ServerStarted = DateTime.Now;

                ConfigurationData = new ConfigurationData("config.conf");

                Logging.Write("Connecting to database... ", ConsoleColor.White);
                try
                {
                    DatabaseServer DatabaseServer = new DatabaseServer(Skylight.GetConfig()["db.hostname"], uint.Parse(Skylight.GetConfig()["db.port"]), Skylight.GetConfig()["db.username"], Skylight.GetConfig()["db.password"]);
                    Database Database = new Database(Skylight.GetConfig()["db.name"], uint.Parse(Skylight.GetConfig()["db.pool.minsize"]), uint.Parse(Skylight.GetConfig()["db.pool.maxsize"]));
                    Skylight.DatabaseManager = new DatabaseManager(DatabaseServer, Database);

                    using (DatabaseClient dbClient = Skylight.DatabaseManager.GetClient())
                    {

                    }
                }
                catch(MySqlException ex)
                {
                    Logging.WriteLine("failed!", ConsoleColor.Red);
                    ExceptionShutdown(ex);
                    return;
                }
                Logging.WriteLine("completed!", ConsoleColor.Green);

                Skylight.Game = new Game();
                Skylight.PacketManager = BasicUtilies.GetRevisionPacketManager(Skylight.Revision);
                Skylight.PacketManager.Initialize();
                Skylight.SocketsManager = new SocketsManager(Skylight.GetConfig()["game.tcp.bindip"], int.Parse(Skylight.GetConfig()["game.tcp.port"]), int.Parse(Skylight.GetConfig()["game.tcp.conlimit"]));
                Skylight.SocketsManager.Start();

                TimeSpan bootTime = DateTime.Now - Skylight.ServerStarted;
                Logging.WriteLine("READY! (" + bootTime.Seconds + " s, " + bootTime.Milliseconds + " ms)", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                Logging.WriteLine("FAILED BOOT! ", ConsoleColor.Red);
                ExceptionShutdown(ex);
                return;
            }
        }

        public static void ExceptionShutdown(Exception exception)
        {
            Logging.WriteLine(exception.ToString(), ConsoleColor.Red);
            Logging.WriteBlank();
            Logging.WriteLine("Press any key to close", ConsoleColor.Blue);
            Console.ReadKey();
            Skylight.Destroy();
        }

        public static ConfigurationData GetConfig()
        {
            return ConfigurationData;
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return DatabaseManager;
        }

        public static Game GetGame()
        {
            return Game;
        }

        public static SocketsManager GetSocketsManager()
        {
            return SocketsManager;
        }

        public static Encoding GetDefaultEncoding()
        {
            return Encoding.Default;
        }

        public static PacketManager GetPacketManager()
        {
            return PacketManager;
        }

        public static void Destroy()
        {
            if (Skylight.ServerShutdown)
            {
                return;
            }
            Skylight.ServerShutdown = true;

            Environment.Exit(0);
        }
    }
}
