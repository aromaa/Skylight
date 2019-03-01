using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Subscription
{
    public class SubscriptionManager
    {
        private uint ID;
        private Habbo Habbo;
        private Dictionary<string, List<Subscription>> Subscriptions;

        public SubscriptionManager(uint id, Habbo habbo, UserDataFactory factory)
        {
            this.Subscriptions = new Dictionary<string, List<Subscription>>();

            this.ID = id;
            this.Habbo = habbo;
            
            foreach(DataRow dataRow in factory.GetSubscriptions()?.Rows)
            {
                string subscription = (string)dataRow["subscription_name"];

                Subscription sub = new Subscription((int)dataRow["id"], subscription, (double)dataRow["subscription_started"], (double)dataRow["subscription_expires"]);
                if (!this.Subscriptions.ContainsKey(subscription))
                {
                    this.Subscriptions.Add(subscription, new List<Subscription> { sub });
                }
                else
                {
                    this.Subscriptions[subscription].Add(sub);
                }
            }
        }

        public bool HasSubscription(string subscription)
        {
            if (this.Subscriptions.ContainsKey(subscription))
            {
                foreach (Subscription subscription_ in this.Subscriptions[subscription])
                {
                    if (subscription_.IsActive())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Subscription TryGetSubscription(string subscription, bool needToBeActive, bool dontReturnNull)
        {
            Subscription subscription_ = null;

            if (this.Subscriptions.ContainsKey(subscription))
            {
                foreach (Subscription subscription_2 in this.Subscriptions[subscription])
                {
                    if (subscription_2.IsActive())
                    {
                        subscription_ = subscription_2;
                    }
                    else
                    {
                        if (!needToBeActive)
                        {
                            if (subscription_ == null || !subscription_.IsActive())
                            {
                                subscription_ = subscription_2;
                            }
                        }
                    }
                }
            }

            if (subscription_ == null && dontReturnNull)
            {
                subscription_ = new Subscription(0, subscription, 0, 0);
            }

            return subscription_;
        }

        public void AddSubscription(string subscription, double secounds)
        {
            //pretty sure we don't want do that on here
            if (secounds <= 0)
            {
                return;
            }

            if (this.Subscriptions.ContainsKey(subscription))
            {
                foreach (Subscription subscription_ in this.Subscriptions[subscription])
                {
                    if (subscription_.IsActive())
                    {
                        subscription_.Expand(secounds);

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("id", subscription_.ID);
                            dbClient.AddParamWithValue("expires", subscription_.GetExpires());

                            dbClient.ExecuteQuery("UPDATE user_subscriptions SET subscription_expires = @expires WHERE id = @id LIMIT 1;");
                        }

                        return;
                    }
                }
            }

            //it we didint found active sub

            int id = 0;
            double started = TimeUtilies.GetUnixTimestamp();
            double expires = started + secounds;

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userId", this.ID);
                dbClient.AddParamWithValue("subscription", subscription);
                dbClient.AddParamWithValue("started", started);
                dbClient.AddParamWithValue("expires", expires);

                id = (int)dbClient.ExecuteQuery("INSERT INTO user_subscriptions(user_id, subscription_name, subscription_started, subscription_expires) VALUES(@userId, @subscription, @started, @expires)");
            }

            if (id > 0) //lets do it for sure :/
            {
                Subscription newSub = new Subscription(id, subscription, started, expires);
                if (!this.Subscriptions.ContainsKey(subscription))
                {
                    this.Subscriptions.Add(subscription, new List<Subscription> { newSub });
                }
                else
                {
                    this.Subscriptions[subscription].Add(newSub);
                }
            }
        }

        public void EndSubscription(string subscription)
        {
            if (this.Subscriptions.ContainsKey(subscription))
            {
                foreach (Subscription subscription_ in this.Subscriptions[subscription])
                {
                    if (subscription_.IsActive())
                    {
                        subscription_.End();

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("id", subscription_.ID);
                            dbClient.AddParamWithValue("expires", subscription_.GetExpires());

                            dbClient.ExecuteQuery("UPDATE user_subscriptions SET subscription_expires = @expires WHERE id = @id LIMIT 1;");
                        }

                        return;
                    }
                }
            }
        }

        public double SubscriptionTime(string type)
        {
            double time = 0;
            if (this.Subscriptions.ContainsKey(type))
            {
                foreach (Subscription subscription_ in this.Subscriptions[type])
                {
                    if (subscription_.IsActive())
                    {
                        time += (TimeUtilies.GetUnixTimestamp() - subscription_.GetStarted());
                    }
                    else
                    {
                        time += (subscription_.GetExpires() - subscription_.GetStarted());
                    }
                }
            }
            return time;
        }
    }
}
