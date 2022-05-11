using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ListviewPaginator
{
    public partial class BootstrapPage : System.Web.UI.UserControl
    {
        #region "Properties"
        private int pageIndex
        {
            get { return Convert.ToInt32(ViewState["pageIndex"] ?? 0); }
            set { ViewState["pageIndex"] = value; }
        }
        private int dataSize
        {
            get { return Convert.ToInt32(ViewState["dataSize"] ?? 0); }
            set { ViewState["dataSize"] = value; }
        }
        private int totalPageCount
        {
            get { return Convert.ToInt32(ViewState["totalPageCount"] ?? 0); }
            set { ViewState["totalPageCount"] = value; }
        }
        private string searchKey
        {
            get { return Convert.ToString(ViewState["searchKey"] ?? ""); }
            set { ViewState["searchKey"] = value; }
        }
        #endregion
        #region "ListProperties"
        public string ItemPlaceholderID
        {
            get { return Convert.ToString(ViewState["ItemPlaceholderID"] ?? ""); }
            set { ViewState["ItemPlaceholderID"] = value; }
        }
        public int pageSize
        {
            get { return Convert.ToInt32(ViewState["pageSize"] ?? 10); }
            set { ViewState["pageSize"] = value; }
        }
        public bool lengthChange
        {
            get { return Convert.ToBoolean(ViewState["lengthChange"] ?? true); }
            set { ViewState["lengthChange"] = value; }
        }
        public bool allowSearch
        {
            get { return Convert.ToBoolean(ViewState["allowSearch"] ?? false); }
            set { ViewState["allowSearch"] = value; }
        }
        public bool showInfo
        {
            get { return Convert.ToBoolean(ViewState["showInfo"] ?? true); }
            set { ViewState["showInfo"] = value; }
        }
        public string[] searchFields
        {
            get { return (string[])(ViewState["searchFields"]); }
            set { ViewState["searchFields"] = value; }
        }
        private object DataSource
        {
            get => ViewState["DataSource"];
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

        #region "Events"
        public event EventHandler<ListViewItemEventArgs> ItemDataBound;
        protected void lst_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            ItemDataBound(sender, e);
        }
        public event EventHandler LayoutCreated;
        protected void lst_LayoutCreated(object sender, EventArgs e)
        {
            LayoutCreated(sender, e);
        }
        #endregion
        public object Paginate()
        {
            int dataSize = this.dataSize;
            object bindable = this.DataSource;
            //if (allowSearch && searchFields != null && txtsearch.Text != "")
            //{
            //    bindable = this.search();
            //}
            if (bindable.GetType() == typeof(DataTable))
                dataSize = ((DataTable)bindable).Rows.Count;
            else if (bindable.GetType().IsGenericType && bindable is IEnumerable)
            {
                dataSize = ((IEnumerable)bindable).Cast<object>().Count();
            }
            this.totalPageCount = (dataSize / this.pageSize) + ((dataSize % this.pageSize) > 0 ? 1 : 0);
            panelpagecontainer.Visible = this.totalPageCount > 1;
            this.renderPages(dataSize);
            upanelpagination.Visible = true;
            upanelpagination.Update();

            if (dataSize < this.pageSize)
                return bindable;

            if (bindable.GetType() == typeof(DataTable))
                return ((DataTable)bindable).AsEnumerable().Skip(this.pageSize * this.pageIndex).Take(this.pageSize).CopyToDataTable();
            else if (bindable.GetType().IsGenericType && bindable is IEnumerable)
                return ((IEnumerable)bindable).Cast<object>().Skip(this.pageSize * this.pageIndex).Take(this.pageSize).ToList();

            return bindable;
        }
        public void Bind(object bindable)
        {
            this.DataSource = bindable;
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

            if (ItemDataBound != null)
                lstSimpleList.ItemDataBound += ItemDataBound;
            if (LayoutCreated != null)
                lstSimpleList.LayoutCreated += LayoutCreated;

            lstSimpleList.DataBind();
            upanellist.Update();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                cmbpagesize.SelectedValue = this.pageSize.ToString();
                panelheadertools.Visible = this.lengthChange || this.allowSearch;
                panelpagesize.Visible = this.lengthChange;
                panelsearch.Visible = this.allowSearch;
            }
        }
        private object search()
        {
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (this.DataSource.GetType() == typeof(DataTable))
            {
                var bindable = ((DataTable)this.DataSource).AsEnumerable().Where(x => x.ItemArray.Any(c => c.ToString().IndexOf(searchKey, comparison) >= 0));
                if (bindable.Count() > 0)
                    return bindable.CopyToDataTable();
                return new DataTable();
            }
            //else if (this.DataSource.GetType() == typeof(List<object>))
            //    return ((List<object>)this.DataSource).Skip(this.pageSize * this.pageIndex).Take(this.pageSize).ToList();

            //else if (this.DataSource.GetType() == typeof(IQueryable<object>))
            //    return ((IQueryable<object>)this.DataSource).Skip(this.pageSize * this.pageIndex).Take(this.pageSize).ToList();
            return this.DataSource;
        }
        private void renderPages(int dataSize)
        {
            if (dataSize > 0)
            {
                int lastrecord = (this.pageSize * this.pageIndex) + this.pageSize;
                lblpaginationinfo.Text = String.Format("Showing {0} to {1} of {2} entries", (this.pageSize * this.pageIndex) + 1, lastrecord > dataSize ? dataSize : lastrecord, dataSize);
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

        protected void cmbpagesize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pageIndex = 0;
            this.pageSize = Convert.ToInt32(cmbpagesize.SelectedValue);
            this.Bind();
        }
        protected void btnsearch_Click(object sender, EventArgs e)
        {
            pageIndex = 0;
            searchKey = txtsearch.Text;
            Bind();
            searchKey = "";
        }
    }
}