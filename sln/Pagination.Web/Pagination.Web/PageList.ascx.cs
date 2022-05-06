using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pagination.Web
{
    public partial class PageList : System.Web.UI.UserControl
    {
        #region "Properties"
        public int pageIndex
        {
            get { return Convert.ToInt32(ViewState["pageIndex"] ?? 0); }
            set { ViewState["pageIndex"] = value; }
        }
        public int pageSize
        {
            get { return Convert.ToInt32(ViewState["pageSize"] ?? 10); }
            set { ViewState["pageSize"] = value; }
        }
        public int dataSize
        {
            get { return Convert.ToInt32(ViewState["dataSize"] ?? 0); }
            set { ViewState["dataSize"] = value; }
        }
        private int totalPageCount
        {
            get { return Convert.ToInt32(ViewState["totalPageCount"] ?? 0); }
            set { ViewState["totalPageCount"] = value; }
        }
        #endregion
        #region "ListProperties"
        public string ItemPlaceholderID
        {
            get { return Convert.ToString(ViewState["ItemPlaceholderID"] ?? ""); }
            set { ViewState["ItemPlaceholderID"] = value; }
        }
        public DataTable DataSource
        {
            get => (DataTable)ViewState["DataSource"];
            set => ViewState["DataSource"] = value;
        }
        #endregion

        private ITemplate mLayoutTemplate;
        private ITemplate mItemTemplate;

        [Browsable(false)]
        [DefaultValue(null)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(ListViewDataItem), BindingDirection.TwoWay)]
        public ITemplate ItemTemplate
        {
            get => this.mItemTemplate;
            set => this.mItemTemplate = value;
        }

        [Browsable(false)]
        [DefaultValue(null)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(ListViewDataItem), BindingDirection.TwoWay)]
        public ITemplate LayoutTemplate
        {
            get => this.mLayoutTemplate;
            set => this.mLayoutTemplate = value;
        }
        public DataTable Paginate()
        {
            this.dataSize = this.DataSource != null ? this.DataSource.Rows.Count : 0;

            if (dataSize == 0)
                this.totalPageCount = 0;

            this.totalPageCount = (this.dataSize / this.pageSize) + ((this.dataSize % this.pageSize) > 0 ? 1 : 0);

            panelpagecontainer.Visible = this.totalPageCount > 1;
            this.renderPages();
            upanelpagination.Update();

            if (dataSize <= pageSize)
                return this.DataSource;
            return this.DataSource.AsEnumerable().Skip(this.pageSize * this.pageIndex).Take(this.pageSize).CopyToDataTable();
        }
        public void Bind(DataTable raw)
        {
            this.DataSource = raw;
            this.Bind();
        }
        private void Bind()
        {
            listplaceholder.Controls.Clear();
            ListView lstSimpleList = new ListView();
            if (this.ItemPlaceholderID != "")
                lstSimpleList.ItemPlaceholderID = this.ItemPlaceholderID;

            if (this.LayoutTemplate != null)
                lstSimpleList.LayoutTemplate = this.LayoutTemplate;

            if (this.ItemTemplate != null)
                lstSimpleList.ItemTemplate = this.ItemTemplate;

            listplaceholder.Controls.Add(lstSimpleList);
            lstSimpleList.DataSource = this.Paginate();
            lstSimpleList.DataBind();
            upanellist.Update();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        private void renderPages()
        {
            if (dataSize > 0)
            {
                int lastrecord = (this.pageSize * this.pageIndex) + this.pageSize;
                lblpaginationinfo.Text = String.Format("Showing {0} to {1} of {2} entries", (this.pageSize * this.pageIndex) + 1, lastrecord > dataSize ? dataSize : lastrecord, this.dataSize);
            }
            List<object> pagenumbers = new List<object>();

            int pageNumber = 1;

            // Get Left Pointer to generate ...
            int leftPointer = this.pageIndex - 1;
            if (this.pageIndex == (this.totalPageCount - 1))
                leftPointer = this.totalPageCount - 3;

            // Get Right Pointer to generate ...
            int rightPointer = this.pageIndex + 3;
            if (this.pageIndex == 0)
                rightPointer = 4;

            if (rightPointer == (this.totalPageCount - 1))
                rightPointer = this.totalPageCount;

            while (pageNumber <= totalPageCount)
            {
                if (pageNumber == 1 ||
                    (pageNumber > leftPointer && pageNumber < rightPointer) ||
                    (pageNumber == 3 && this.pageIndex == 0) ||
                    pageNumber == this.totalPageCount)
                {
                    pagenumbers.Add(new { pageNumber = pageNumber - 1, isactive = pageIndex == (pageNumber - 1), data = pageNumber.ToString() });
                }
                else if ((pageNumber == leftPointer && pageNumber != 2) ||
                         (pageNumber == rightPointer && pageNumber != (this.totalPageCount - 1)))
                {
                    pagenumbers.Add(new { pageNumber = -3, isactive = false, data = "..." });
                }
                pageNumber++;
            }
            if (this.totalPageCount != (this.pageIndex + 1))
                paging_simple_next.Attributes["class"] = paging_simple_next.Attributes["class"].Replace(" disabled", "");
            else
                paging_simple_next.Attributes["class"] = paging_simple_next.Attributes["class"] + " disabled";

            if (this.pageIndex != 0)
                paging_simple_previous.Attributes["class"] = paging_simple_previous.Attributes["class"].Replace(" disabled", "");
            else
                paging_simple_previous.Attributes["class"] = paging_simple_previous.Attributes["class"] + " disabled";

            lstsimplepagination.DataSource = pagenumbers;
            lstsimplepagination.DataBind();


        }

        protected void btnprevious_Click(object sender, EventArgs e)
        {
            if (pageIndex > 1)
            {
                pageIndex--;
                this.Bind();
            }
        }

        protected void btnsimplepagenumber_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            if (btn.CommandArgument != "-3")
            {
                this.pageIndex = Convert.ToInt32(btn.CommandArgument);
                this.Bind();
            }
        }

        protected void btnnext_Click(object sender, EventArgs e)
        {
            if (pageIndex < totalPageCount)
            {
                pageIndex++;
                this.Bind();
            }
        }
    }
}