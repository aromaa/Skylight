using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Catalog.Marketplace
{
    public class MarketplaceOffer
    {
        public readonly uint ID;
        public readonly uint UserID;
        public readonly uint ItemID;
        public readonly int Price;
        public readonly double Timestamp;
        public bool Sold;
        public double SoldTimestamp;
        public bool Redeem;
        public bool Cancalled;
        public readonly uint FurniID;
        public readonly string FurniExtraData;

        public MarketplaceOffer(uint id, uint userId, uint itemId, int price, double timestamp, bool sold, double soldTimestamp, bool redeem, uint furniId, string furniExtraData)
        {
            this.ID = id;
            this.UserID = userId;
            this.ItemID = itemId;
            this.Price = price;
            this.Timestamp = timestamp;
            this.Sold = sold;
            this.SoldTimestamp = soldTimestamp;
            this.Redeem = redeem;
            this.FurniID = furniId;
            this.FurniExtraData = furniExtraData;
            this.Cancalled = false;
        }

        public double Timeleft
        {
            get
            {
                return (this.Timestamp + ServerConfiguration.MarketplaceOffersActiveHours * 3600.0) - TimeUtilies.GetUnixTimestamp();
            }
        }

        public bool Expired
        {
            get
            {
                return this.Timeleft <= 0;
            }
        }
    }
}
