using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Quests
{
    public class QuestManager
    {
        private Dictionary<uint, Quest> Quests;

        public QuestManager()
        {
            this.Quests = new Dictionary<uint, Quest>();
        }

        public void LoadQuests(DatabaseClient dbClient)
        {
            Logging.Write("Loading quests... ");
            this.Quests.Clear();


            Logging.WriteLine("completed!", ConsoleColor.Green);
        }
    }
}
