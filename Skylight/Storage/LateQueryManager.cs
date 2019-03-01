using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkylightEmulator.Storage
{
    public class LateQueryManager
    {
        private ConcurrentDictionary<uint, ConcurrentBag<ManualResetEvent>> TempLockedAccounts;

        public LateQueryManager()
        {
            this.TempLockedAccounts = new ConcurrentDictionary<uint, ConcurrentBag<ManualResetEvent>>();
        }

        public void TempLockAccount(uint userId, ManualResetEvent event_)
        {
            ConcurrentBag<ManualResetEvent> events;
            if (!this.TempLockedAccounts.TryGetValue(userId, out events))
            {
                events = new ConcurrentBag<ManualResetEvent>();
            }
            events.Add(event_);
            this.TempLockedAccounts.AddOrUpdate(userId, events, (key, oldValue) => events);
        }

        public ManualResetEvent TryGetTempAccountLock(uint userId)
        {
            ConcurrentBag<ManualResetEvent> events;
            if (this.TempLockedAccounts.TryGetValue(userId, out events))
            {
                ManualResetEvent event_;
                events.TryPeek(out event_);
                return event_;
            }
            else
            {
                return null;
            }
        }
    }
}
