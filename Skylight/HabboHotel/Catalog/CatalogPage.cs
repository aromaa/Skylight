using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
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
        public Dictionary<uint, CatalogItem> Items;

        private MultiRevisionServerMessage ServerMessagePageInfo;

        public CatalogPage(int pageId, int parentId, string caption, bool visible, bool enabled, int minRank, bool clubOnly, int iconColor, int iconImage, string pageLayout, string pageHeadline, string pageTeaser, string pageSpecial, string pageText1, string pageText2, string pageTextDetails, string pageTextTeaser, string pageLinkDescription, string pageLinkPagename, IEnumerable<CatalogItem> items)
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
            this.Items = items.ToDictionary(i => i.Id, i => i);

            this.ServerMessagePageInfo = new MultiRevisionServerMessage(OutgoingPacketsEnum.CatalogPage, new ValueHolder("CatalogPage", this));
        }

        public byte[] GetBytes(Revision revision)
        {
            return this.ServerMessagePageInfo.GetBytes(revision);
        }

        public void Serialize(ServerMessage message)
        {
            if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                if (message.GetRevision() >= Revision.PRODUCTION_201601012205_226667486)
                {
                    message.AppendBoolean(this.Visible);
                    message.AppendInt32(this.IconImage); //image
                    message.AppendInt32(this.PageID); //page id
                    message.AppendString(this.Caption); //page name
                    message.AppendString(this.Caption); //localization
                    message.AppendInt32(0); //offers count
                    
                    List<CatalogPage> childs = Skylight.GetGame().GetCatalogManager().GetParentPagesWithRankedSystem(this.PageID, 1);

                    message.AppendInt32(childs.Count);
                    foreach (CatalogPage page2 in childs)
                    {
                        page2.Serialize(message);
                    }
                }
                else if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                {
                    message.AppendBoolean(this.Visible);
                    message.AppendInt32(this.IconColor); //color
                    message.AppendInt32(this.IconImage); //image
                    message.AppendInt32(this.PageID); //page id
                    message.AppendString(this.Caption); //name
                    message.AppendString(this.Caption); //name
                }
                else
                {
                    message.AppendBoolean(this.Visible);
                    message.AppendInt32(this.IconColor); //color
                    message.AppendInt32(this.IconImage); //image
                    message.AppendInt32(this.PageID); //page id
                    message.AppendString(this.Caption); //name
                }
            }
            else
            {
                message.AppendString(this.PageID.ToString(), 9);
                message.AppendString(this.Caption, 13);
            }
        }

        public CatalogItem GetItem(uint id)
        {
            CatalogItem item;
            this.Items.TryGetValue(id, out item);
            return item;
        }
    }
}
