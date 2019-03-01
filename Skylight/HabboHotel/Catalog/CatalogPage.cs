using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Catalog
{
    public class CatalogPage
    {
        public int PageID;
        public int ParentID;
        public string Caption;
        public bool Visible;
        public bool Enabled;
        public int MinRank;
        public bool ClubOnly;
        public int IconColor;
        public int IconImage;
        public string PageLayout;
        public string PageHeadline;
        public string PageTeaser;
        public string PageSpecial;
        public string PageText1;
        public string PageText2;
        public string PageTextDetails;
        public string PageTextTeaser;
        public string PageLinkDescription;
        public string PageLinkPagename;
        public List<CatalogItem> Items;

        private ServerMessage ServerMessagePageInfo;

        public CatalogPage(int pageId, int parentId, string caption, bool visible, bool enabled, int minRank, bool clubOnly, int iconColor, int iconImage, string pageLayout, string pageHeadline, string pageTeaser, string pageSpecial, string pageText1, string pageText2, string pageTextDetails, string pageTextTeaser, string pageLinkDescription, string pageLinkPagename, List<CatalogItem> items)
        {
            this.PageID = pageId;
            this.ParentID = parentId;
            this.Caption = caption;
            this.Visible = visible;
            this.Enabled = enabled;
            this.MinRank = minRank;
            this.ClubOnly = clubOnly;
            this.IconColor = iconColor;
            this.IconImage = iconImage;

            this.PageLayout = pageLayout; //1
            this.PageHeadline = pageHeadline; //2
            this.PageTeaser = pageTeaser; //3
            this.PageSpecial = pageSpecial; //4
            this.PageText1 = pageText1; //5
            this.PageText2 = pageText2; //6
            this.PageTextDetails = pageTextDetails; //7
            this.PageTextTeaser = pageTextTeaser; //8
            this.PageLinkDescription = pageLinkDescription; //9
            this.PageLinkPagename = pageLinkPagename; //10
            this.Items = items;
        }

        public ServerMessage PageData
        {
            get
            {
                return this.ServerMessagePageInfo;
            }
        }

        public void Serialize(ServerMessage message)
        {
            message.AppendBoolean(this.Visible);
            message.AppendInt32(this.IconColor); //color
            message.AppendInt32(this.IconImage); //image
            message.AppendInt32(this.PageID); //page id
            message.AppendStringWithBreak(this.Caption); //name
            message.AppendInt32(Skylight.GetGame().GetCatalogManager().GetParentPages(this.PageID).Count); //count sub indexes
        }

        public void SerializePageInfo()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            message.Init(r63aOutgoing.CatalogPage);
            message.AppendInt32(this.PageID);

            switch (this.PageLayout)
            {
                case "frontpage":
                    {
                        message.AppendStringWithBreak("frontpage3");
                        message.AppendInt32(3);
                        message.AppendStringWithBreak(this.PageHeadline);
                        message.AppendStringWithBreak(this.PageTeaser);
                        message.AppendStringWithBreak("");
                        message.AppendInt32(11);
                        message.AppendStringWithBreak(this.PageText1);
                        message.AppendStringWithBreak(this.PageLinkDescription);
                        message.AppendStringWithBreak(this.PageText2);
                        message.AppendStringWithBreak(this.PageTextDetails);
                        message.AppendStringWithBreak(this.PageLinkPagename);
                        message.AppendStringWithBreak("#FAF8CC");
                        message.AppendStringWithBreak("#FAF8CC");
                        message.AppendStringWithBreak("Read More >");
                        message.AppendStringWithBreak("magic.credits");
                    }
                    break;

                default:
                    {
                        message.AppendStringWithBreak(this.PageLayout);
                        message.AppendInt32(3);
                        message.AppendStringWithBreak(this.PageHeadline);
                        message.AppendStringWithBreak(this.PageTeaser);
                        message.AppendStringWithBreak(this.PageSpecial);
                        message.AppendInt32(3);
                        message.AppendStringWithBreak(this.PageText1);
                        message.AppendStringWithBreak(this.PageTextDetails);
                        message.AppendStringWithBreak(this.PageTextTeaser);
                    }
                    break;
            }

            message.AppendInt32(this.Items.Count); //items count
            foreach(CatalogItem item in this.Items)
            {
                item.Serialize(message);
            }
            this.ServerMessagePageInfo = message;
        }

        public CatalogItem GetItem(uint id)
        {
            foreach(CatalogItem item in this.Items)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
