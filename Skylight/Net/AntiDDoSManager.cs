using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Net
{
    public class AntiDDoSManager
    {
        //settings
        private static bool DDoSProtectionEnabled = Skylight.GetConfig()["ddos.protection.enabled"] == "1";
        private static int ConnectioLimitByIP = int.Parse(Skylight.GetConfig()["ddos.protection.connections.per.ip"]);
        private static int DDoSProtectionBlockType = int.Parse(Skylight.GetConfig()["ddos.protection.block.type"]);

        //memory cache
        private static MemoryCache ConnectionLimitByIPViolationCounter = new MemoryCache("ConnectionsFailureCounter");
        private static MemoryCache TempBlockedIPs = new MemoryCache("TempBlockedIPs");

        //lists
        private static ConcurrentDictionary<string, ConcurrentDictionary<long, SocketsConnection>> ConnectionsOpenByIP = new ConcurrentDictionary<string, ConcurrentDictionary<long, SocketsConnection>>();

        //return false if the connection shoudn't be accepted
        public static bool OnConnection(SocketsConnection connection)
        {
            if (AntiDDoSManager.DDoSProtectionEnabled)
            {
                string ip = connection.GetIP();
                if (!AntiDDoSManager.IsIPTempBlock(ip)) //not blocked
                {
                    if (!AntiDDoSManager.IsViolatingConnectionLimit(connection))
                    {
                        return true;
                    }
                    else
                    {
                        CacheItem failureCount = AntiDDoSManager.ConnectionLimitByIPViolationCounter.GetCacheItem(ip);
                        if (failureCount == null) //havent violated in 5s
                        {
                            AntiDDoSManager.ConnectionLimitByIPViolationCounter.Set(ip, 1, DateTimeOffset.Now.AddSeconds(5)); //violating expires after 5s
                        }
                        else
                        {
                             AntiDDoSManager.ConnectionLimitByIPViolationCounter.Set(ip, ((int)failureCount.Value) + 1, DateTimeOffset.Now.AddSeconds(5));

                             if (((int)failureCount.Value) >= 5) //5 failures in 5s?!? I call DDoS
                             {
                                 AntiDDoSManager.BlockForDDoS(ip);
                             }
                        }
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public static void BlockForDDoS(string ip)
        {
            AntiDDoSManager.TempBlockedIPs.AddOrGetExisting(ip, true, DateTimeOffset.Now.AddMinutes(5)); //block for five minute from emu

            ConcurrentDictionary<long, SocketsConnection> connectionsOpen = null;
            if (AntiDDoSManager.ConnectionsOpenByIP.TryGetValue(ip, out connectionsOpen)) //there is connections
            {
                foreach(SocketsConnection connection in connectionsOpen.Values)
                {
                    connection.Disconnect("DDoS Protection");
                }
            }

            switch (AntiDDoSManager.DDoSProtectionBlockType)
            {
                case 0: //just block from emu for 5mins
                    {
                        
                    }
                    break;
                case 1: //block from windows firewall
                    {
                        try
                        {
                            WindowsUtils.WindowsFirewallBlockIP(ip);
                        }
                        catch(UnauthorizedAccessException)
                        {
                            Logging.WriteLine("You are using DDoS protection type 1 but you are not running emulator as an administrator!", ConsoleColor.Red);
                        }
                    }
                    break;
                case 2: //x4b ACL using API
                    {

                    }
                    break;
            }

            Logging.LogDDoS(ip); //this to end so if it fails it dosent stop blocking the ddos
        }

        //return true if the connection is violationg connection limti
        public static bool IsViolatingConnectionLimit(SocketsConnection connection)
        {
            ConcurrentDictionary<long, SocketsConnection> connectionsOpen = null;
            if (AntiDDoSManager.ConnectionsOpenByIP.TryGetValue(connection.GetIP(), out connectionsOpen)) //not first connection
            {
                if (connectionsOpen.Count >= AntiDDoSManager.ConnectioLimitByIP)
                {
                    return true;
                }
            }
            else
            {
                connectionsOpen = new ConcurrentDictionary<long, SocketsConnection>();
                AntiDDoSManager.ConnectionsOpenByIP.TryAdd(connection.GetIP(), connectionsOpen);
            }
            connectionsOpen.TryAdd(connection.GetID(), connection);

            return false;
        }

        public static void OnDisconnect(SocketsConnection connection)
        {
            ConcurrentDictionary<long, SocketsConnection> connectionsOpen = null;
            if (AntiDDoSManager.ConnectionsOpenByIP.TryGetValue(connection.GetIP(), out connectionsOpen)) //in list
            {
                SocketsConnection connection_;
                connectionsOpen.TryRemove(connection.GetID(), out connection_);
            }
        }

        public static bool IsIPTempBlock(string ip)
        {
            return AntiDDoSManager.TempBlockedIPs.Contains(ip);
        }
    }
}
