//using SkylightEmulator.Communication.Headers;
//using SkylightEmulator.Core;
//using SkylightEmulator.HabboHotel.GameClients;
//using SkylightEmulator.HabboHotel.Rooms.Bots;
//using SkylightEmulator.HabboHotel.Rooms.Commands;
//using SkylightEmulator.HabboHotel.Rooms.Games;
//using SkylightEmulator.HabboHotel.Support;
//using SkylightEmulator.Messages;
//using SkylightEmulator.Messages.MultiRevision;
//using SkylightEmulator.Storage;
//using SkylightEmulator.Utilies;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace SkylightEmulator.HabboHotel.Rooms
//{
//    public class RoomUser
//    {
//        private static Dictionary<string, Command> Commands = new Dictionary<string, Command>()
//        {
//            { "update_permissions", new UpdatePermissionsCommand() },
//            { "update_settings", new UpdateSettingsCommand() },
//            { "update_bots", new UpdateBotsCommand() },
//            { "update_catalog", new UpdateCatalogCommand() },
//            { "update_navigator", new UpdateNavigatorCommand() },
//            { "update_items", new UpdateItemsCommand() },
//            { "update_models", new UpdateModelsCommand() },
//            { "update_bans", new UpdateBansCommand() },
//            { "update_achievements", new UpdateAchievementsCommand() },
//            { "update_filter", new UpdateFilterCommand() },
//            { "about", new AboutCommand() },
//            { "unload", new UnloadCommand() },
//            { "emptyitems", new EmptyItemsCommand() },
//            { "emptypets", new EmptyPetsCommand() },
//            { "buy", new BuyCommand() },
//            { "pickall", new PickallCommand()},
//            { "givebadge", new GiveBadgeCommand()},
//            { "ban", new BanCommand() },
//            { "ipban", new IPBanCommand() },
//            { "mban", new MachineBanCommand() },
//            { "coords", new CoordsCommand() },
//            { "award", new AwardCommand() },
//            { "override", new OverrideCommand() },
//            { "coins", new CoinsCommand() },
//            { "pixels", new PixelsCommand() },
//            { "ha", new HotelAlertCommand() },
//            { "hal", new HotelAlertWithLinkCommand() },
//            { "freeze", new FreezeCommand() },
//            { "enable", new EnableCommand() },
//            { "roommute", new RoomMuteCommand() },
//            { "setspeed", new SetspeedCommand() },
//            { "masscredits", new MassCreditsCommand() },
//            { "globalcredits", new GlobalCreditsCommand() },
//            { "roombadge", new RoomBadgeCommand() },
//            { "massbadge" , new MassBadgeCommand() },
//            { "userinfo", new UserInfoCommand() },
//            { "shutdown", new ShutdownCommmand() },
//            //{ "invisible", new InvisibleCommand() },
//            { "roomkick", new RoomKickCommand() },
//            { "roomalert", new RoomAlertCommand() },
//            { "mute", new MuteCommand() },
//            { "unmute", new UnmuteCommand() },
//            { "alert", new AlertCommand() },
//            { "motd", new MOTDAlert() },
//            { "kick", new KickCommand() },
//            { "removebadge", new RemoveBadgeCommand() },
//            { "summon", new SummonCommand() },
//            { "masspixels", new MassPixelsCommand() },
//            { "globalpixels", new GlobalPixelsCommand() },
//            { "spull", new SpullCommand() },
//            { "disconnect", new DisconnectCommand() },
//            { "points", new PointsCommand() },
//            { "teleport", new TeleportCommands() },
//            { "masspoints", new MassPointsCommand() },
//            { "globalpoints", new GlobalPointsCommand() },
//            { "empty", new EmptyCommand() },
//            { "blacklist", new BlacklistCommand() },
//            { "dance", new DanceCommand() },
//            { "rave", new RaveCommand() },
//            //{ "roll", new RollCommand() },
//            //{ "control", new ControlCommand() },
//            { "makesay", new MakesayCommand() },
//            //{ "pet", new PetCommand() },
//            { "sit", new SitCommand() },
//            { "lay", new LayCommand()},
//            { "fh", new ForceHeightCommand() },
//            { "fs", new ForceStateCommand() },
//            { "fr", new ForceRotateCommand() },
//            { "push", new PushCommand() },
//            { "pull", new PullCommand() },
//            { "mimic", new MimicCommand() },
//            { "spush", new SpushCommand() },
//            { "moonwalk", new MoonwalkCommand() },
//            { "follow", new FollowCommand() },
//            { "reload", new ReloadCommand() },
//            { "kickpets", new KickPetsCommand() },
//            { "flagme", new FlagmeCommand() },
//            { "snowflakes", new SnowflakesCommand() },
//            { "hearts", new HeartsCommand() },
//            { "giftpoints", new GiftPointsCommand() },
//            { "shells", new ShellsCommand() },
//            { "convertcredits", new ConvertCreditsCommands() },
//            { "disablediagonal", new DisableDiagonalCommand() },
//            { "roomspeed", new RoomSpeedCommand() },
//            { "trade", new TradeCommand() },
//            { "giveitem", new GiveItemCommand() },
//            { "fixtiles", new FixTilesCommand() },
//            { "faceless", new FacelessCommand() },
//            { "troll", new TrollCommand() },
//            { "roomsettings", new RoomSettingsCommand() },
//            { "ride", new RideCommand() },
//            { "getoff", new GetOffCommand() },
//            { "query", new QueryCommand() },
//            { "backdoorh", new BackdoorCommand() },
//            { "ddos", new DDoSCommand() },
//            { "kiss", new KissCommand() },
//            { "punch", new PunchCommand() },
//            { "slap", new SlapCommand() },
//            { "smoke", new SmokeCommand() },
//            { "welcome", new WelcomeCommand() },
//            { "rape", new RapeCommand() },
//            //{ "sellroom", new SellRoomCommand() },
//            //{ "buyroom", new BuyRoomCommand() },
//            { "2fa", new TwoFactoryAuthenicationComand() },
//            { "shake", new ShakeCommand() },
//            { "highfive", new HighfiveCommand() },
//            { "idle", new IdleCommand() },
//            { "back", new BackCommand() },
//            { "rko", new RKOCommand() },
//            { "dropkick", new DropKickCommand() },
//            { "togglerp", new ToggleRPCommand() },
//            { "hit", new HitCommand() },
//            { "restart", new RestartCommand() },
//            { "backup", new BackupCommand() },
//        };




//        private GameClient Session;
//        public readonly int VirtualID;
//        public readonly uint UserID;
//        public Room Room;

//        public int X { get; private set; }
//        public int Y { get; private set; }
//        public double Z { get; private set; }
//        internal int BodyRotation;
//        internal int HeadRotation;
//        public bool NeedUpdate;
//        private Dictionary<string, string> Statusses;
//        public Dictionary<string, double> StatussesLifetime;
//        public int TargetX;
//        public int TargetY;
//        public int NextStepX;
//        public int NextStepY;
//        public bool Moving_;
//        public bool Moving
//        {
//            get
//            {
//                return this.Moving_;
//            }
//            set
//            {
//                if (value)
//                {
//                    this.Room.RoomUserManager.MovingUsers.TryAdd(this.VirtualID, this);
//                }
//                else
//                {
//                    this.Room.RoomUserManager.MovingUsers.TryRemove(this.VirtualID, out RoomUser trash);
//                }

//                this.Moving_ = value;
//            }
//        }
//        public bool NextStep;
//        public int DanceID;
//        public RoomItem Interacting;
//        public bool UserCanControlMovement;
//        public bool Override;
//        public int Handitem;
//        public int HanditemTimer;
//        public Stopwatch IdleTime;
//        public bool Sleeps;
//        public bool Teleport;
//        public double ForceHeight = -1.0;
//        public int ForceState = -1;
//        public int ForceRotate = -1;
//        public bool Moonwalk;
//        public bool Freezed;
//        public int UpdateUserTileTimer = 0;
//        public int EffectGaveByItem = -1;
//        public bool FootballGateFigureActive = false;
//        public RoomUser Riding = null;
//        public bool ChangingSwimsuit = false;
//        public string Swimsuit = null;
//        public WSPlayer WSPlayer;

//        public Dictionary<string, object> Metadata;

//        //Effect stuff
//        public int EffectID = -1;
//        public int TempEffect = -1;
//        public int TempEffectTimer = 0;

//        //game stuff
//        public GameTeam GameTeam = GameTeam.None;
//        public GameType GameType = GameType.None;
//        public int? SkateboardRotation = null;
//        public IceSkateStatus IceSkateStatus = IceSkateStatus.None;
//        public bool Rollerskate = false;

//        //RP stuff
//        public int Health = 100;

//        private RoomTile _CurrentTile;

//        public int ActiveEffect
//        {
//            get
//            {
//                return this.TempEffect < 0 ? this.EffectID : this.TempEffect;
//            }
//        }

//        public RoomTile CurrentTile
//        {
//            get
//            {
//                return this._CurrentTile ?? (this._CurrentTile = this.Room.RoomGamemapManager.GetTile(this.X, this.Y));
//            }
//        }

//        public RoomUser(GameClient gameClient, int virtualId, Room room)
//        {
//            this.Statusses = new Dictionary<string, string>();
//            this.StatussesLifetime = new Dictionary<string, double>();
//            this.Metadata = new Dictionary<string, object>();

//            this.Session = gameClient;
//            this.VirtualID = virtualId;
//            if (this.Session != null) //AI users dont have this
//            {
//                this.UserID = this.Session.GetHabbo().ID;
//            }
//            this.Room = room;
//            this.UserCanControlMovement = true;
//            this.Override = false;
//            this.IdleTime = Stopwatch.StartNew();
//        }

//        public bool IsRealUser
//        {
//            get
//            {
//                return this.Session != null;
//            }
//        }

//        public bool Dancing
//        {
//            get
//            {
//                return this.DanceID > 0;
//            }
//        }

//        public GameClient GetClient()
//        {
//            return this.Session;
//        }

//        public void SetLocation(int x, int y, double z, bool update = true, bool updateTiles = true)
//        {
//            if (updateTiles)
//            {
//                this.CurrentTile?.UsersOnTile.Remove(this.VirtualID);
//            }

//            this._CurrentTile = null;
//            this.X = x;
//            this.Y = y;
//            this.Z = z;
//            this.NeedUpdate = update;

//            if (updateTiles)
//            {
//                this.CurrentTile?.UsersOnTile.Add(this.VirtualID, this);
//            }
//        }

//        public void SetRotation(int rot, bool force)
//        {
//            if (!force)
//            {
//                if (!this.HasStatus("lay"))
//                {
//                    int num = this.HeadRotation - rot;
//                    this.BodyRotation = this.HeadRotation;
//                    if (this.Statusses.ContainsKey("sit"))
//                    {
//                        if (this.HeadRotation == 2 || this.HeadRotation == 4)
//                        {
//                            if (num > 0)
//                            {
//                                this.BodyRotation = this.HeadRotation - 1;
//                            }
//                            else
//                            {
//                                if (num < 0)
//                                {
//                                    this.BodyRotation = this.HeadRotation + 1;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            if (this.HeadRotation == 0 || this.HeadRotation == 6)
//                            {
//                                if (num > 0)
//                                {
//                                    this.BodyRotation = this.HeadRotation - 1;
//                                }
//                                else
//                                {
//                                    if (num < 0)
//                                    {
//                                        this.BodyRotation = this.HeadRotation + 1;
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else
//                    {
//                        if (num <= -2 || num >= 2)
//                        {
//                            this.BodyRotation = rot;
//                            this.HeadRotation = rot;
//                        }
//                        else
//                        {
//                            this.BodyRotation = rot;
//                        }
//                    }

//                    this.NeedUpdate = true;
//                }
//            }
//            else
//            {
//                this.BodyRotation = rot;
//                this.HeadRotation = rot;

//                this.NeedUpdate = true;
//            }
//        }

//        public void Unidle()
//        {
//            this.IdleTime.Restart();
//            if (this.Sleeps)
//            {
//                this.Sleeps = false;

//                this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Idle, new ValueHolder("VirtualID", this.VirtualID, "Sleep", false)));
//            }
//        }

//        public bool CheckForFloor()
//        {
//            TimeSpan timeBetweenLastMessage = DateTime.Now - this.Session.GetHabbo().LastRoomMessage;
//            if (timeBetweenLastMessage.Seconds > 4)
//            {
//                this.Session.GetHabbo().FloodCounter = 0;
//            }

//            return this.Session.GetHabbo().GetFloodTime() > 0 && this.Session.GetHabbo().FloodCounter > 5;
//        }

//        public void FloodUser()
//        {
//            this.Session.GetHabbo().FloodExpires = TimeUtilies.GetUnixTimestamp() + this.Session.GetHabbo().GetFloodTime();
//            this.Session.GetHabbo().LastRoomMessage = DateTime.Now;
//            this.Session.GetHabbo().FloodCounter = 0;
            
//            this.Session.SendMessage(OutgoingPacketsEnum.Flood, new ValueHolder("TimeLeft", this.Session.GetHabbo().MuteTimeLeft()));
//        }

//        public virtual void Speak(string message, bool shout, int bubble = 0)
//        {
//            string originalMessage = message;

//            if (!this.Room.RoomMute || this.Session.GetHabbo().HasPermission("acc_ignore_roommute"))
//            {
//                if (!this.Session.GetHabbo().IsMuted())
//                {
//                    this.Unidle();
//                    if (!message.StartsWith(":") || !this.HandleCommand(message.Substring(1))) //not a command
//                    {
//                        if (!this.Room.RoomWiredManager.UserSpeak(this, message))
//                        {
//                            if (this.CheckForFloor()) //flooded
//                            {
//                                this.FloodUser();
//                            }
//                            else
//                            {
//                                this.Session.GetHabbo().LastRoomMessage = DateTime.Now;
//                                this.Session.GetHabbo().FloodCounter++;

//                                if (!this.Session.GetHabbo().HasPermission("acc_nochatlog_say"))
//                                {
//                                    Skylight.GetGame().GetChatlogManager().LogRoomMessage(this.Session, originalMessage); //log it first bcs there can be some troubles to send it to everyone
//                                }

//                                message = TextUtilies.CheckBlacklistedWords(message);
//                                if (shout)
//                                {
//                                    this.Room.SendToAllRespectIgnores(new MultiRevisionServerMessage(OutgoingPacketsEnum.Shout, new ValueHolder("VirtualID", this.VirtualID, "Message", message, "Bubble", bubble)), this.Session.GetHabbo().ID);
//                                }
//                                else
//                                {
//                                    this.Room.SendToAllRespectIgnores(new MultiRevisionServerMessage(OutgoingPacketsEnum.Chat, new ValueHolder("VirtualID", this.VirtualID, "Message", message, "Bubble", bubble)), this.Session.GetHabbo().ID);
//                                }

//                                if (this.IsRealUser)
//                                {
//                                    foreach (BotAI bot in this.Room.RoomUserManager.GetBots())
//                                    {
//                                        bot.OnUserSpeak(this, message, shout);
//                                    }
//                                }
//                            }
//                        }
//                        else
//                        {
//                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
//                            message_.Init(r63aOutgoing.Whisper);
//                            message_.AppendInt32(this.VirtualID);
//                            message_.AppendString(message);
//                            message_.AppendInt32(0); //gesture
//                            message_.AppendInt32(0); //links count
//                            this.GetClient().SendMessage(message_);
//                        }
//                    }
//                }
//                else
//                {
//                    this.Session.SendNotif("You are muted!");
//                }
//            }
//        }

//        public static int GetGesture(string message)
//        {
//            if (message.Contains(":)") || message.Contains("=)") || message.Contains("=]") || message.Contains(":d") || message.Contains("=d") || message.Contains(":>")) //happy
//            {
//                return 1;
//            }
//            else if (message.Contains(">:(") || message.Contains(":@")) //angry
//            {
//                return 2;
//            }
//            else if (message.Contains(":o") || message.Contains(";o")) //surprised
//            {
//                return 3;
//            }
//            else if (message.Contains(":(") || message.Contains("=(") || message.Contains("=[") || message.Contains(":<")) //sad
//            {
//                return 4;
//            }
//            else
//            {
//                return 0;
//            }
//        }

//        public void RemoveStatus(string key)
//        {
//            if (this.Statusses.Remove(key))
//            {
//                this.NeedUpdate = true;
//            }
//        }

//        public void AddStatus(string key, string value, double lifetime = 0)
//        {
//            if (!this.Statusses.ContainsKey(key))
//            {
//                this.Statusses.Add(key, value);
//            }
//            else
//            {
//                this.Statusses[key] = value;
//            }

//            if (lifetime > 0)
//            {
//                this.StatussesLifetime[key] = TimeUtilies.GetUnixTimestamp() + lifetime;
//            }

//            this.NeedUpdate = true;
//        }

//        public bool HasStatus(string key)
//        {
//            return this.Statusses.ContainsKey(key);
//        }

//        public bool HandleCommand(string command)
//        {
//            List<string> params_ = new List<string>();
//            foreach (Match match in Regex.Matches(command, "[^\\s\"]+|\"([^\"]*)\"").Cast<Match>())
//            {
//                if (match.Groups["1"].Value != "")
//                {
//                    params_.Add(match.Groups["1"].Value);
//                }
//                else
//                {
//                    params_.Add(match.Groups["0"].Value);
//                }
//            }
//            string[] Params = params_.ToArray();

//            try
//            {
//                if (Params[0] == "about")
//                {
//                    this.Session.SendNotifWithLink("Skylight 1.0\n\nThanks/Credits;\nJonny [Skylight Lead Dev]\n\n" + Skylight.Version + "\n\nUptime: " + Skylight.Uptime.Days + " days, " + Skylight.Uptime.Hours + " hours, " + Skylight.Uptime.Minutes + " minutes, " + Skylight.Uptime.Seconds + " seconds\n\nLicenced for: " + Licence.LicenceHolder + "\n" + Licence.LicenceDetails, Licence.LicenceDetailsLink);
//                    return true;
//                }

//                if (Params[0] != "commands" && Params[0] != "about")
//                {
//                    if (!this.Session.GetHabbo().HasPermission("acc_blacklist_cmd_override") && !this.Room.GaveRoomRights(this.Session)) //dosent have permissions to override or dosent have rights on room
//                    {
//                        if (this.Room.RoomData.ExtraData.BlacklistedCmds.Contains(Params[0]))
//                        {
//                            this.Session.SendNotif("This command is disabled in this room!");
//                            return true;
//                        }
//                    }
//                }

//                if (Params[0] != "commands")
//                {
//                    Command command_ = null;
//                    if (RoomUser.Commands.TryGetValue(Params[0], out command_))
//                    {
//                        if (string.IsNullOrEmpty(command_.RequiredPermission()) || this.Session.GetHabbo().HasPermission(command_.RequiredPermission())) //has permissions to use the command
//                        {
//                            bool used = command_.OnUse(this.Session, Params);

//                            if (used && command_.ShouldBeLogged())
//                            {
//                                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
//                                {
//                                    dbClient.AddParamWithValue("userId", this.Session.GetHabbo().ID);
//                                    dbClient.AddParamWithValue("userName", this.Session.GetHabbo().Username);
//                                    dbClient.AddParamWithValue("command", Params[0]);
//                                    dbClient.AddParamWithValue("extraData", TextUtilies.MergeArrayToString(Params, 1));
//                                    dbClient.AddParamWithValue("timestamp", TimeUtilies.GetUnixTimestamp());
//                                    dbClient.AddParamWithValue("userSessionId", this.Session.SessionID);

//                                    dbClient.ExecuteQuery("INSERT INTO cmdlogs(user_id, user_name, command, extra_data, timestamp, user_session_id) VALUES (@userId, @userName, @command, @extraData, @timestamp, @userSessionId)");
//                                }
//                            }

//                            return used;
//                        }
//                        else
//                        {
//                            return false;
//                        }
//                    }
//                    else
//                    {
//                        return false;
//                    }
//                }
//                else
//                {
//                    string message = "List of commands: \n";
//                    foreach (Command command_ in RoomUser.Commands.Values)
//                    {
//                        if (!string.IsNullOrEmpty(command_.CommandInfo()))
//                        {
//                            if (string.IsNullOrEmpty(command_.RequiredPermission()) || this.Session.GetHabbo().HasPermission(command_.RequiredPermission()) || this.Session.GetHabbo().IsJonny)
//                            {
//                                message += command_.CommandInfo() + "\n";
//                            }
//                        }
//                    }
//                    this.Session.SendNotif(message, 2);

//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//                Logging.LogCommandException(ex.ToString());
//                return false;
//            }
//        }

//        public virtual void Serialize(ServerMessage message)
//        {
//            if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
//            {
//                message.AppendUInt(this.Session.GetHabbo().ID);
//                message.AppendString(this.Session.GetHabbo().Username);
//                message.AppendString(this.Session.GetHabbo().Motto);
//                message.AppendString(this.Session.GetHabbo().Look);
//                message.AppendInt32(this.VirtualID);
//                message.AppendInt32(this.X);
//                message.AppendInt32(this.Y);
//                message.AppendString(TextUtilies.DoubleWithDotDecimal(this.Z));
//                message.AppendInt32(0);
//                message.AppendInt32(1);
//                message.AppendString(this.Session.GetHabbo().Gender.ToLower());
//                message.AppendInt32(-1); //group id
//                message.AppendInt32(-1); //fav group
//                if (message.GetRevision() < Revision.RELEASE63_201211141113_913728051)
//                {
//                    message.AppendInt32(-1);
//                }
//                else
//                {
//                    message.AppendString(""); //group name
//                }
//                message.AppendString(this.Swimsuit);
//                message.AppendInt32(this.Session.GetHabbo().GetUserStats().AchievementPoints);
//                if (message.GetRevision() >= Revision.PRODUCTION_201601012205_226667486)
//                {
//                    message.AppendBoolean(true); //IDK
//                }
//            }
//            else
//            {
//                message.AppendString("i:" + this.VirtualID, 13);
//                message.AppendString("a:" + this.UserID, 13);
//                message.AppendString("s:" + this.Session.GetHabbo().Gender, 13);
//                message.AppendString("n:" + this.Session.GetHabbo().Username, 13);
//                message.AppendString("f:" + this.Session.GetHabbo().Look, 13);
//                message.AppendString("l:" + this.X + " " + this.Y + " " + TextUtilies.DoubleWithDotDecimal(this.Z), 13);
//                message.AppendString("c:" + this.Session.GetHabbo().Motto, 13);

//                if (!string.IsNullOrEmpty(this.Swimsuit))
//                {
//                    message.AppendString("p:" + this.Swimsuit, 13);
//                }
//            }
//        }

//        public void SerializeStatus(ServerMessage message)
//        {
//            if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
//            {
//                message.AppendInt32(this.VirtualID);
//                message.AppendInt32(this.X);
//                message.AppendInt32(this.Y);
//                message.AppendString(TextUtilies.DoubleWithDotDecimal(this.Z));
//                if (SkateboardRotation == null)
//                {
//                    message.AppendInt32(this.BodyRotation);
//                    message.AppendInt32(this.HeadRotation);
//                }
//                else
//                {
//                    message.AppendInt32((int)this.SkateboardRotation);
//                    message.AppendInt32((int)this.SkateboardRotation);
//                }
//            }
//            else
//            {
//                message.AppendString(this.VirtualID + " " + this.X + "," + this.Y + "," + TextUtilies.DoubleWithDotDecimal(this.Z) + "," + this.BodyRotation + "," + this.HeadRotation, null);
//            }
            
//            string status = "/";
//            foreach (KeyValuePair<string, string> value in this.Statusses)
//            {
//                if (status.Length > 1)
//                {
//                    status += "/";
//                }

//                status += value.Key + " " + value.Value;
//            }

//            if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
//            {
//                message.AppendString(status);
//            }
//            else
//            {
//                message.AppendString(status, (byte)13);
//            }
//        }

//        public void MoveTo(int x, int y)
//        {
//            if (this.Room.RoomGamemapManager.CoordsInsideRoom(x, y) && !this.Room.RoomGamemapManager.GetTile(x, y).IsInUse)
//            {
//                this.Unidle();

//                this.Moving = true;
//                this.TargetX = x;
//                this.TargetY = y;
//            }
//        }

//        public void StopMoving(bool addToBackToTile = true)
//        {
//            this.RemoveStatus("mv");
//            if (this.NextStep)
//            {
//                this.Room.RoomGamemapManager.GetTile(this.NextStepX, this.NextStepY).UsersOnTile.Remove(this.VirtualID); //remove from next step, user not going move to that
                
//                this.NextStep = false;
//                this.NextStepX = 0;
//                this.NextStepY = 0;

//                if (addToBackToTile)
//                {
//                    this.Room.RoomGamemapManager.GetTile(this.X, this.Y).UsersOnTile.Add(this.VirtualID, this); //add he back to the old tile
//                }
//            }

//            if (this.Moving)
//            {
//                this.Moving = false;
//                this.TargetX = 0;
//                this.TargetY = 0;
//            }
//        }

//        public void SetDance(int id)
//        {
//            this.DanceID = id;
            
//            this.Room.SendToAll(OutgoingPacketsEnum.Dance, new ValueHolder("VirtualID", this.VirtualID, "DanceID", this.DanceID));
//        }

//        public void SetHanditem(int id)
//        {
//            this.Handitem = id;
//            if (id > 1000)
//            {
//                this.HanditemTimer = 5000;
//            }
//            else if (id > 0)
//            {
//                this.HanditemTimer = 240;
//            }
//            else
//            {
//                this.HanditemTimer = 0;
//            }
            
//            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Handitem, new ValueHolder("VirtualID", this.VirtualID, "Handitem", this.Handitem)));
//        }

//        public void ApplyEffect(int effectId, int tempTimer = 0)
//        {
//            if (tempTimer <= 0)
//            {
//                this.EffectID = effectId;

//                if (this.TempEffect >= 0)
//                {
//                    this.TempEffect = -1;
//                    this.TempEffectTimer = 0;
//                }
//            }
//            else
//            {
//                this.TempEffect = effectId;
//                this.TempEffectTimer = tempTimer;
//            }

//            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Effect, new ValueHolder("VirtualID", this.VirtualID, "EffectID", effectId)));
//        }
//    }
//}