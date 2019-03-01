using Microsoft.Win32;
using SkylightEmulator.Communication.Handlers;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Achievements;
using SkylightEmulator.HabboHotel.Catalog;
using SkylightEmulator.HabboHotel.FastFood;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Guide;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.HabboHotel.Profiles;
using SkylightEmulator.HabboHotel.Quests;
using SkylightEmulator.HabboHotel.Roles;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Support;
using SkylightEmulator.HabboHotel.Talent;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SkylightEmulator.HabboHotel
{
    public class Game
    {
        private GameClientManager GameClientManager;
        private NavigatorManager NavigatorManager;
        private RoomManager RoomManager;
        private CatalogManager CatalogManager;
        private ItemManager ItemManager;
        private PermissionManager PermissionManager;
        private BanManager BanManager;
        private ModerationToolManager ModerationToolManager;
        private CautionManager CautionManager;
        private HelpManager HelpManager;
        private ChatlogManager ChatlogManager;
        private RoomvisitManager RoomvisitManager;
        private AchievementManager AchievementManager;
        private BotManager BotManager;
        private QuestManager QuestManager;
        private FastFoodManager FastFoodManager;
        private TalentManager TalentManager;
        private UserProfileManager UserProfileManager;
        private GuideManager GuideManager;

        private System.Timers.Timer GameCycleTimer;
        private Task UpdateEmulatorStatusTask;
        private Task CheckActivityBonusesTask;
        private Task TimeoutTask;

        private DateTime CurrentDay;
        private Stopwatch LastUpdateEmulatorStatus;
        private Stopwatch LastActivityBonusesCheck;
        private Stopwatch LastTimeoutCheck;

        private bool ClientPingEnabled;

        //auto restart stuff
        private bool AutoRestartEnabled = false;
        private bool AutoRestartBackup = false;
        private bool AutoRestartBackupCompress = false;
        private DateTime AutoRestartTime;
        private bool AutoRestartWarningSend = false;

        public Game()
        {
            this.CurrentDay = DateTime.Today;
        }

        public void Init()
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                ServerConfiguration.LoadConfigsFromDB(dbClient);

                this.GameClientManager = new GameClientManager();

                this.NavigatorManager = new NavigatorManager();
                this.NavigatorManager.LoadPublicRooms(dbClient);
                this.NavigatorManager.LoadFlatCats(dbClient);

                this.RoomManager = new RoomManager();
                this.RoomManager.LoadRoomModels(dbClient);
                this.RoomManager.LoadNewbieRooms(dbClient);

                this.ItemManager = new ItemManager();
                this.ItemManager.LoadItems(dbClient);
                this.ItemManager.LoadSoundtracks(dbClient);
                this.ItemManager.LoadNewbieRoomItems(dbClient);

                this.CatalogManager = new CatalogManager();
                this.CatalogManager.LoadCatalogItems(dbClient);
                this.CatalogManager.LoadCatalogPages(dbClient);
                this.CatalogManager.LoadPetRaces(dbClient);
                this.CatalogManager.LoadPresents(dbClient);

                this.CatalogManager.GetMarketplaceManager().LoadMarketplaceOffers(dbClient);

                this.PermissionManager = new PermissionManager();
                this.PermissionManager.LoadRanks(dbClient);

                this.BanManager = new BanManager();
                this.BanManager.LoadBans(dbClient);

                this.ModerationToolManager = new ModerationToolManager();
                this.ModerationToolManager.LoadIssues(dbClient);
                this.ModerationToolManager.LoadPresents(dbClient);
                this.ModerationToolManager.LoadSupportTickets(dbClient);

                this.CautionManager = new CautionManager();
                this.CautionManager.LoadCauctions(dbClient);

                this.HelpManager = new HelpManager();
                this.HelpManager.LoadFAQs(dbClient);

                this.ChatlogManager = new ChatlogManager();

                this.RoomvisitManager = new RoomvisitManager();

                this.AchievementManager = new AchievementManager();
                this.AchievementManager.LoadAchievements(dbClient);

                this.BotManager = new BotManager();
                this.BotManager.LoadBots(dbClient);
                this.BotManager.LoadNewbieBotActions(dbClient);

                TextUtilies.LoadWordfilter(dbClient);

                this.QuestManager = new QuestManager();
                this.QuestManager.LoadQuests(dbClient);

                this.TalentManager = new TalentManager();
                this.TalentManager.LoadTalents(dbClient);

                this.FastFoodManager = new FastFoodManager();
                this.FastFoodManager.CreateNewConnection();

                this.UserProfileManager = new UserProfileManager();

                this.GuideManager = new GuideManager();
            }

            this.ClientPingEnabled = TextUtilies.StringToBool(Skylight.GetConfig()["client.ping.enabled"]);

            this.AutoRestartEnabled = TextUtilies.StringToBool(Skylight.GetConfig()["auto.restart.enabled"]);
            if (this.AutoRestartEnabled)
            {
                this.AutoRestartBackup = TextUtilies.StringToBool(Skylight.GetConfig()["auto.restart.backup"]);
                this.AutoRestartBackupCompress = TextUtilies.StringToBool(Skylight.GetConfig()["auto.restart.backup.compress"]);
                this.AutoRestartTime = DateTime.ParseExact(Skylight.GetConfig()["auto.restart.time"], "HH:mm", CultureInfo.InvariantCulture);
            }

            this.LastUpdateEmulatorStatus = Stopwatch.StartNew();
            this.LastActivityBonusesCheck = Stopwatch.StartNew();
            this.LastTimeoutCheck = Stopwatch.StartNew();

            this.GameCycleTimer = new System.Timers.Timer();
            this.GameCycleTimer.Elapsed += this.GameCycle;
            this.GameCycleTimer.AutoReset = true;
            this.GameCycleTimer.Interval = 1; //moved from 25ms, 40 times in a second to 1ms, 1000 times in second to help keep everything be in sync
            this.GameCycleTimer.Start();
            GC.KeepAlive(this.GameCycleTimer); //IK timer adds itself to the gc already, but just for sure ;P
        }

        public void GameCycle(object sender, ElapsedEventArgs e)
        {
            if (!Skylight.ServerShutdown)
            {
                try
                {
                    if (this.RoomManager.RoomCycleTask?.IsCompleted ?? true)
                    {
                        (this.RoomManager.RoomCycleTask = new Task(new Action(this.RoomManager.OnCycle))).Start();
                    }

                    if (this.UpdateEmulatorStatusTask?.IsCompleted ?? true)
                    {
                        (this.UpdateEmulatorStatusTask = new Task(new Action(this.UpdateEmulatorStatus))).Start();
                    }

                    if (this.CheckActivityBonusesTask?.IsCompleted ?? true)
                    {
                        (this.CheckActivityBonusesTask = new Task(new Action(this.CheckActivityBonuses))).Start();
                    }

                    if (this.GuideManager.CycleTask?.IsCompleted ?? true)
                    {
                        (this.GuideManager.CycleTask = new Task(new Action(this.GuideManager.OnCycle))).Start();
                    }

                    if (this.ClientPingEnabled)
                    {
                        if (this.TimeoutTask?.IsCompleted ?? true)
                        {
                            (this.TimeoutTask = new Task(new Action(this.CheckTimeout))).Start();
                        }
                    }

                    try
                    {
                        if (this.AutoRestartEnabled)
                        {
                            if (Skylight.Uptime.TotalMinutes >= 2) //restart
                            {
                                if (!this.AutoRestartWarningSend)
                                {
                                    if (DateTime.Now.ToString("HH:mm") == this.AutoRestartTime.AddMinutes(-1).ToString("HH:mm"))
                                    {
                                        this.AutoRestartWarningSend = true;

                                        ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                        message.Init(r63aOutgoing.SendNotifFromAdmin);
                                        message.AppendString(Skylight.GetConfig()["auto.restart.warning.message"]);
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
                                }
                                else
                                {
                                    if (DateTime.Now.ToString("HH:mm") == this.AutoRestartTime.ToString("HH:mm"))
                                    {
                                        Program.Destroy(true, this.AutoRestartBackup, this.AutoRestartBackupCompress, true);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.LogException("Error in auto restart check! " + ex.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Logging.LogException("Error in game cycle! " + ex.ToString());
                }
            }
        }

        public void CheckActivityBonuses()
        {
            try
            {
                if (this.LastActivityBonusesCheck.Elapsed.TotalSeconds > 15)
                {
                    this.LastActivityBonusesCheck.Restart();

                    foreach (GameClient session in this.GameClientManager.GetClients())
                    {
                        if (session?.GetHabbo() != null)
                        {
                            session.GetHabbo().SendOnlineUsersCount();

                            if (session.GetHabbo().GetRoomSession() != null)
                            {
                                if (session.GetHabbo().GetRoomSession().IsInRoom && !session.GetHabbo().GetRoomSession().GetRoomUser().Sleeps)
                                {
                                    if (session.GetHabbo().CanReceiveActivityBonus())
                                    {
                                        session.GetHabbo().ReceiveActivityBonus();
                                    }
                                }
                            }

                            session.GetHabbo().GetUserStats().UpdateOnlineTime();
                            session.GetHabbo().GetUserAchievements().CheckAchievement("OnlineTime");

                            session.GetHabbo().GetUserStats().UpdateGuideOnDutyPresence();
                            session.GetHabbo().GetUserAchievements().CheckAchievement("GuideOnDutyPresence");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logging.LogException("Error in check activity bonuses task! " + ex.ToString());
            }
        }

        public void UpdateEmulatorStatus()
        {
            try
            {
                if (this.LastUpdateEmulatorStatus.Elapsed.TotalSeconds > 5)
                {
                    this.LastUpdateEmulatorStatus.Restart();

                    TimeSpan uptime = Skylight.Uptime;
                    int usersOnline = Skylight.GetGame().GetGameClientManager().OnlineCount;
                    int loadedRooms = Skylight.GetGame().GetRoomManager().LoadedRoomsCount;

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("usersOnline", usersOnline);
                        dbClient.AddParamWithValue("loadedRooms", loadedRooms);

                        dbClient.ExecuteQuery("UPDATE server_status SET users_online = @usersOnline, rooms_loaded = @loadedRooms LIMIT 1; UPDATE server_status SET users_online_peak = @usersOnline WHERE users_online_peak < @usersOnline LIMIT 1; UPDATE server_status SET rooms_loaded_peak = @loadedRooms WHERE rooms_loaded_peak < @loadedRooms LIMIT 1;");
                    }

                    Skylight.GetGame().GetChatlogManager().PushRoomChatlogToDB();
                    Skylight.GetGame().GetChatlogManager().PushPrivateChatlogToDB();
                    Skylight.GetGame().GetRoomvisitManager().PushRoomvisitsToDB();

                    if (this.CurrentDay != DateTime.Today)
                    {
                        this.CurrentDay = DateTime.Today;
                        foreach (GameClient session in this.GetGameClientManager().GetClients())
                        {
                            if (session != null && session.GetHabbo() != null)
                            {
                                try
                                {
                                    session.GetHabbo().CheckDailyStuff(true);

                                    ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                    Message.Init(r63aOutgoing.SendUserInfo);
                                    Message.AppendString(session.GetHabbo().ID.ToString());
                                    Message.AppendString(session.GetHabbo().Username);
                                    Message.AppendString(session.GetHabbo().Look);
                                    Message.AppendString(session.GetHabbo().Gender.ToUpper());
                                    Message.AppendString(session.GetHabbo().Motto);
                                    Message.AppendString(session.GetHabbo().RealName);
                                    Message.AppendInt32(0); //unknown
                                    Message.AppendInt32(session.GetHabbo().GetUserStats().RespectReceived);
                                    Message.AppendInt32(session.GetHabbo().GetUserStats().DailyRespectPoints);
                                    Message.AppendInt32(session.GetHabbo().GetUserStats().DailyPetRespectPoints);
                                    Message.AppendBoolean(false); //friend stream enabled
                                    session.SendMessage(Message);
                                }
                                catch
                                {

                                }
                            }
                        }
                    }

                    Console.Title = "Skylight | Users online: " + usersOnline + " | Rooms loaded: " + loadedRooms + " | Uptime: " + uptime.Days + " days, " + uptime.Hours + " hours, " + uptime.Minutes + " minutes | Memory: " + GC.GetTotalMemory(false) / 1024 + " KB";
                }
            }

            catch (Exception ex)
            {
                Logging.LogException("Error in update emulator status task! " + ex.ToString());
            }
        }

        public void CheckTimeout()
        {
            try
            {
                int pingInterval = int.Parse(Skylight.GetConfig()["client.ping.interval"]);

                if (this.LastTimeoutCheck.ElapsedMilliseconds >= pingInterval)
                {
                    this.LastTimeoutCheck.Restart();

                    MultiRevisionServerMessage message = new MultiRevisionServerMessage(OutgoingPacketsEnum.Ping);
                    foreach (GameClient session in this.GameClientManager.GetClients())
                    {
                        if (session != null)
                        {
                            TwoFactorAuthenticationHandler twofa = session.GetMessageHandler<TwoFactorAuthenticationHandler>(TwoFactorAuthenticationHandler.DefaultIdentifier);
                            if (twofa != null)
                            {
                                if (twofa.Started.ElapsedMilliseconds > 60000) //1 minute
                                {
                                    session.Stop("Two factory authenication timeout");
                                }
                            }
                            else
                            {
                                if (session.LastPong.ElapsedMilliseconds > pingInterval * 2) //timeout
                                {
                                    session.Stop("Timeout");
                                }
                                else
                                {
                                    byte[] bytes = message.GetBytes(session.Revision);
                                    if (bytes != null)
                                    {
                                        session.SendData(bytes);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogException("Error in check timeout task! " + ex.ToString());
            }
        }

        public GameClientManager GetGameClientManager()
        {
            return this.GameClientManager;
        }

        public NavigatorManager GetNavigatorManager()
        {
            return this.NavigatorManager;
        }

        public RoomManager GetRoomManager()
        {
            return this.RoomManager;
        }

        public CatalogManager GetCatalogManager()
        {
            return this.CatalogManager;
        }

        public ItemManager GetItemManager()
        {
            return this.ItemManager;
        }

        public PermissionManager GetPermissionManager()
        {
            return this.PermissionManager;
        }

        public BanManager GetBanManager()
        {
            return this.BanManager;
        }

        public ModerationToolManager GetModerationToolManager()
        {
            return this.ModerationToolManager;
        }

        public CautionManager GetCautionManager()
        {
            return this.CautionManager;
        }

        public HelpManager GetHelpManager()
        {
            return this.HelpManager;
        }

        public ChatlogManager GetChatlogManager()
        {
            return this.ChatlogManager;
        }

        public RoomvisitManager GetRoomvisitManager()
        {
            return this.RoomvisitManager;
        }

        public AchievementManager GetAchievementManager()
        {
            return this.AchievementManager;
        }

        public BotManager GetBotManager()
        {
            return this.BotManager;
        }

        public QuestManager GetQuestManager()
        {
            return this.QuestManager;
        }

        public FastFoodManager GetFastFoodManager()
        {
            return this.FastFoodManager;
        }

        public TalentManager GetTalentManager()
        {
            return this.TalentManager;
        }

        public UserProfileManager GetUserProfileManager()
        {
            return this.UserProfileManager;
        }

        public GuideManager GetGuideManager()
        {
            return this.GuideManager;
        }

        public void Shutdown()
        {
            //we dont wnat dispose gameclientmanager

            if (this.GameCycleTimer != null)
            {
                this.GameCycleTimer.Stop();
            }
            this.GameCycleTimer = null;

            if (this.NavigatorManager != null)
            {
                this.NavigatorManager.Shutdown();
            }
            this.NavigatorManager = null;

            if (this.RoomManager != null)
            {
                this.RoomManager.Shutdown();
            }
            this.RoomManager = null;

            if (this.ItemManager != null)
            {
                this.ItemManager.Shutdown();
            }
            this.ItemManager = null;

            if (this.CatalogManager != null)
            {
                this.CatalogManager.Shutdown();
            }
            this.CatalogManager = null;

            if (this.PermissionManager != null)
            {
                this.PermissionManager.Shutdown();
            }
            this.PermissionManager = null;

            if (this.BanManager != null)
            {
                this.BanManager.Shutdown();
            }
            this.BanManager = null;

            if (this.ModerationToolManager != null)
            {
                this.ModerationToolManager.Shutdown();
            }
            this.ModerationToolManager = null;

            if (this.CautionManager != null)
            {
                this.CautionManager.Shutdown();
            }
            this.CautionManager = null;

            if (this.HelpManager != null)
            {
                this.HelpManager.Shutdown();
            }
            this.HelpManager = null;

            if (this.ChatlogManager != null)
            {
                this.ChatlogManager.Shutdown();
            }
            this.ChatlogManager = null;

            if (this.RoomvisitManager != null)
            {
                this.RoomvisitManager.Shutdown();
            }
            this.RoomvisitManager = null;

            if (this.AchievementManager != null)
            {
                this.AchievementManager.Shutdown();
            }
            this.AchievementManager = null;
        }
    }
}
