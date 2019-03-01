using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Roles
{
    public class PermissionRank
    {
        public readonly int ID;
        public readonly string Name;
        public readonly string PublicName;
        public readonly string BadgeID;
        private List<string> Permissions;
        public int Floodtime;
        public int HaInterval;
        public int HalInterval;
        public int WiredTriggerLimit = 5;
        public int WiredActionLimit = 5;
        public int WiredConditionLimit = 5;

        public PermissionRank(int id, string name, string publicName, string badgeId)
        {
            this.Permissions = new List<string>();

            this.ID = id;
            this.Name = name;
            this.PublicName = publicName;
            this.BadgeID = badgeId;
            this.WiredTriggerLimit = 5;
            this.WiredActionLimit = 5;
            this.WiredConditionLimit = 5;
        }

        public void SetPermissions(List<string> permissions)
        {
            this.Permissions = permissions;
        }

        public bool HasPermissions(string permission)
        {
            return this.Permissions.Contains(permission);
        }
    }
}
