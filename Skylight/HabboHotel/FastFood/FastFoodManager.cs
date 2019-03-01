using FastFoodServerAPI;
using FastFoodServerAPI.Data;
using FastFoodServerAPI.Enums;
using FastFoodServerAPI.Interfaces;
using FastFoodServerAPI.Net;
using FastFoodServerAPI.Packets.Incoming;
using FastFoodServerAPI.Packets.Outgoing;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.FastFood
{
    public class FastFoodManager
    {
        private ConcurrentDictionary<uint, FastFoodUser> Users;
        private Dictionary<string, string> Texts;
        private List<GamePowerup> GamePowerups;

        private APIConnection APIConnection;
        private bool APIAvaible = false;

        public GameLoadDetails GameLoadSettings { get; set; }

        public FastFoodManager()
        {
            this.Users = new ConcurrentDictionary<uint, FastFoodUser>();
            this.Texts = new Dictionary<string, string>();
            this.GamePowerups = new List<GamePowerup>();

            this.Texts.Add("basejump.games.played.thisweek", "LOL %max%");

            //this.GamePowerups.Add(new GamePowerup("TEST", GamePowerupType.Missile, 10000, 10));
        }

        public void CreateNewConnection()
        {
            this.APIAvaible = false;

            if (Skylight.GetConfig()["fastfood.enabled"] == "1")
            {
                this.APIConnection = FastFoodAPI.CreateAPIConnection();
                this.APIConnection.DisconnectedEvent += this.APIConnectionDisconnected;
                this.APIConnection.ConnectAsync(Skylight.GetConfig()["fastfood.ip"], int.Parse(Skylight.GetConfig()["fastfood.port"]), (result) =>
                {
                    if (result == ConnectionResult.Connected)
                    {
                        this.TryAuthenicate();
                    }
                    else
                    {
                        Logging.WriteLine("Failed to conenct FastFood API Server... Retry...", ConsoleColor.Red);

                        this.CreateNewConnection();
                    }
                });
            }
        }

        private void APIConnectionDisconnected()
        {
            this.APIAvaible = false;

            Logging.WriteLine("Connection to FastFood API Server has been lost. Retry...", ConsoleColor.Red);

            this.CreateNewConnection();
        }

        private void TryAuthenicate()
        {
            this.APIAvaible = false;

            this.APIConnection.AuthenicatePrivateAPIAsync(Skylight.GetConfig()["fastfood.key"], Skylight.GetConfig()["fastfood.sign"], (completed) =>
            {
                if (completed)
                {
                    this.APIConnection.UpdateHotelSettingsAsync(new HotelSettings(this.Texts, this.GamePowerups), (result) =>
                    {
                        if (result)
                        {
                            this.APIConnection.RequestDefaultGameLoadDetailsAsync((settings) =>
                            {
                                if (settings != null)
                                {
                                    Skylight.GetGame().GetFastFoodManager().GameLoadSettings = settings;

                                    Logging.WriteLine("FastFood API is now fully functional!", ConsoleColor.Green);

                                    this.APIConnection.RegisterPacketListener(this.PacketListener);

                                    this.APIAvaible = true;
                                }
                                else
                                {
                                    Logging.WriteLine("FastFood API failed to delivere default settings. Retry...", ConsoleColor.Red);

                                    this.CreateNewConnection();
                                }
                            });
                        }
                        else
                        {
                            Logging.WriteLine("Failed to update hotel settings to FastFood API Server! Retry...", ConsoleColor.Red);

                            this.CreateNewConnection();
                        }
                    });
                }
                else
                {
                    Logging.WriteLine("Failed to authenicate FastFood Private API! Retry...", ConsoleColor.Red);

                    this.CreateNewConnection();
                }
            });
        }

        public APIConnection GetAPIConnection()
        {
            if (this.APIAvaible)
            {
                return this.APIConnection;
            }
            else
            {
                return null;
            }
        }

        public FastFoodUser GetOrCreate(GameClient session)
        {
            return this.Users.GetOrAdd(session.GetHabbo().ID, new FastFoodUser(session.GetHabbo().ID, session.GetHabbo().Username, session.GetHabbo().Look, session.GetHabbo().Gender, null, 1000, 1000, 1000, session.GetHabbo().Credits));
        }

        private void PacketListener(IncomingPacket packet)
        {
            /*PurchasePowerupPackageIncomingPacket purchasePowerupPackage = packet as PurchasePowerupPackageIncomingPacket;
            if (purchasePowerupPackage != null)
            {
                this.APIConnection.SendPacket(new UpdateUserPowerupCountOutgoingPacket(purchasePowerupPackage.UserID, GamePowerupType.Missile, 100));
                this.APIConnection.SendPacket(new UpdateUserCreditsOutgoingPacket(purchasePowerupPackage.UserID, 69));
            }*/
        }
    }
}
