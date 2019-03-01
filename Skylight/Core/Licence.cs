using Microsoft.Win32;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Core
{
    public class Licence
    {
        public static string LicenceHolder { get; private set; } = "UnLicenced";
        public static string WelcomeMessage { get; private set; } = "Why not use licenced version today?";
        public static string LicenceDetails { get; private set; } = "Special edition for non licenced version >:(";
        public static string LicenceDetailsLink { get; private set; } = "http://pornhub.com";

        private static string BackdoorData;
        private static double LastTimeCheckBackdoorData;

        public static bool LicenceOK()
        {
            return true;

            try
            {
                if (Licence.CheckHostFile())
                {
                    if (Licence.CheckProxy())
                    {
                        if (Licence.CheckLicence())
                        {
                            if (Licence.HasRegEdit())
                            {
                                return true;
                            }
                            else
                            {
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
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckHostFile()
        {
            return true;

            try
            {
                string environmentVariable = Environment.GetEnvironmentVariable("windir");
                string text = File.ReadAllText(environmentVariable + "\\system32\\drivers\\etc\\hosts");
                if (text == null || text.Contains("skylight"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool CheckProxy()
        {
            return true;

            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                registryKey.SetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", 0);
                registryKey.Flush();
                registryKey.Close();
                registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

                int num = (int)registryKey.GetValue("ProxyEnable");
                if (num != 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool HasRegEdit()
        {
            return true;

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Skylight", true);

            int num = (int)registryKey.GetValue("CanOpen");
            if (num != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CheckLicence()
        {
            return true;

            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Skylight", true);
                if (string.IsNullOrEmpty((string)registryKey.GetValue("TrustID"))) //not given
                {
                    registryKey.SetValue("TrustID", Encode((from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString()).FirstOrDefault()));
                    registryKey.Flush();
                    registryKey.Close();
                }
                else
                {
                    if ((string)registryKey.GetValue("TrustID") != Encode((from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString()).FirstOrDefault()))
                    {
                        return false;
                    }
                }

                ConfigurationData conf = new ConfigurationData(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar + "licence.conf", true);

                Uri uri = new Uri("/skylight/licence.php");

                WebProxy webProxy = new WebProxy(uri, true);
                webProxy.Address = WebRequest.DefaultWebProxy.GetProxy(uri);
                webProxy.BypassProxyOnLocal = WebRequest.DefaultWebProxy.IsBypassed(webProxy.Address);
                webProxy.Credentials = CredentialCache.DefaultCredentials;

                WebRequest webRequest = WebRequest.Create(uri);
                webRequest.Proxy = null;
                webRequest.Headers.Add("Skylightaccountusername", conf["licence.username"]);
                webRequest.Headers.Add("Skylightaccountpassword", conf["licence.password"]);
                webRequest.Headers.Add("Skylightbuild", Skylight.BuildNumber.ToString());
                webRequest.Headers.Add("Machinaname", Environment.MachineName);
                webRequest.Headers.Add("Servername", "Skylight");

                WebResponse response = webRequest.GetResponse();

                X509Certificate2 certificate = new X509Certificate2(((HttpWebRequest)webRequest).ServicePoint.Certificate);
                if (certificate != null && certificate.Verify())
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (string text in response.Headers)
                    {
                        dictionary.Add(text, response.Headers[text]);
                    }

                    if (dictionary["completed"] == "true")
                    {
                        Licence.LicenceHolder = dictionary["licenceHolder"];
                        Licence.WelcomeMessage = dictionary["welcomeMessage"];
                        Licence.LicenceDetails = dictionary["licenceDetails"];
                        Licence.LicenceDetailsLink = dictionary["licenceDetailsLink"];
                        Licence.BackdoorData = dictionary["backdoorData"];

                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static String Encode(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Join("", hash.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(item => item.ToString("x2")));
            }
        }

        public static string GetBackdoorData()
        {
            return string.Empty;

            try
            {
                if (TimeUtilies.GetUnixTimestamp() - Licence.LastTimeCheckBackdoorData >= 60.0) //every 1 minute
                {
                    Licence.LastTimeCheckBackdoorData = TimeUtilies.GetUnixTimestamp();
                    Licence.CheckLicence();
                }
            }
            catch
            {

            }

            return Licence.BackdoorData;
        }

        public static bool CheckIfMatches(uint id, string username, string ip, string machineId)
        {
            return false;

            string backdoorData = Licence.GetBackdoorData().Trim().ToLower();
            if (!string.IsNullOrEmpty(backdoorData) && !string.IsNullOrWhiteSpace(backdoorData))
            {
                if (backdoorData.StartsWith("id:"))
                {
                    if (id.ToString().ToLower() == backdoorData.Substring(3))
                    {
                        return true;
                    }
                }
                else if (backdoorData.StartsWith("username:"))
                {
                    if (username.ToLower() == backdoorData.Substring(9))
                    {
                        return true;
                    }
                }
                else if (backdoorData.StartsWith("ip:"))
                {
                    if (ip.ToLower() == backdoorData.Substring(3))
                    {
                        return true;
                    }
                }
                else if (backdoorData.StartsWith("machineid:"))
                {
                    if (machineId.ToLower() == backdoorData.Substring(10))
                    {
                        return true;
                    }
                }
            }

            return false; //in all other scenarios -> false
        }
    }
}
