using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class WindowsUtils
    {
        private readonly static string GuidFWPolicy2 = "{E2B3C97F-6AE1-41AC-817A-F6F92166D7DD}";
        private readonly static string GuidRWRule = "{2C5BC43E-3369-4C33-AB0C-BE9469677AF4}";

        public static void WindowsFirewallBlockIP(string ip)
        {
            Type typeFWPolicy2 = Type.GetTypeFromCLSID(new Guid(WindowsUtils.GuidFWPolicy2));
            Type typeFWRule = Type.GetTypeFromCLSID(new Guid(WindowsUtils.GuidRWRule));
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(typeFWPolicy2);
            INetFwRule newRule = (INetFwRule)Activator.CreateInstance(typeFWRule);
            newRule.Name = "Skylight DDoS Protection - IP " + ip;
            newRule.Description = "Skylight DDoS Protection - IP " + ip;
            newRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_ANY;
            newRule.RemoteAddresses = ip;
            newRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            newRule.Enabled = true;
            newRule.Grouping = "@firewallapi.dll,-23255";
            newRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
            newRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            fwPolicy2.Rules.Add(newRule);
        }
    }
}
