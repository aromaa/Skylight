using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Messenger
{
    public class MessengerCategory
    {
        public int Id;
        public string Name;

        public MessengerCategory(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
