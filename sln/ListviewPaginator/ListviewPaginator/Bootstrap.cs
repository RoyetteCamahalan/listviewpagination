using ListviewPaginator.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ListviewPaginator
{
    public class Bootstrap : System.Web.UI.UserControl
    {
        #region "Properties"
        private static string upanellistview_name = "upanellistview";
        private static string simple_plst_page_name = "simple_plst_page";
        private static string cmbpagelength_name = "cmbpagelength";
        private static string panelheadertools_name = "panelheadertools";
        private static string panellengthchange_name = "panellengthchange";
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
        //public bool allowSearch
        //{
        //    get { return Convert.ToBoolean(ViewState["allowSearch"] ?? false); }
        //    set { ViewState["allowSearch"] = value; }
        //}
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
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                HtmlGenericControl licontrol = (HtmlGenericControl)e.Item.FindControl("pageli");
                if (licontrol != null)
                {
                    LinkButton btn = (LinkButton)e.Item.FindControl("btnsimplepage");
                    btn.Text = DataBinder.Eval(e.Item.DataItem, "data").ToString();
                    btn.CommandArgument = DataBinder.Eval(e.Item.DataItem, "pageNumber").ToString();
                    if (Convert.ToBoolean(DataBinder.Eval(e.Item.DataItem, "isactive")))
                    {
                        licontrol.Attributes["class"] = licontrol.Attributes["class"] + " active";
                    }
                }
            }
        }
        public event EventHandler LayoutCreated;
        protected void lst_LayoutCreated(object sender, EventArgs e)
        {
            LayoutCreated(sender, e);
        }
        #endregion
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            CreateLayout();
        }
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

            this.renderPages(dataSize);

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
            UpdatePanel upanellistview = (UpdatePanel)this.FindControl(upanellistview_name);
            ListView lstSimpleList = (ListView)this.FindControl("lstSimpleList");

            lstSimpleList.DataSource = this.Paginate();
            lstSimpleList.DataBind();
            upanellistview.Update();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                DropDownList cmbpagesize = (DropDownList)this.FindControl(cmbpagelength_name);
                if (cmbpagesize != null)
                {
                    cmbpagesize.SelectedValue = this.pageSize.ToString();
                    Panel panelheadertools = (Panel)this.FindControl(panelheadertools_name);
                    panelheadertools.Visible = this.lengthChange; //|| this.allowSearch;
                    Panel panellengthchange = (Panel)this.FindControl(panellengthchange_name);
                    panellengthchange.Visible = this.lengthChange;
                }
                //panelsearch.Visible = this.allowSearch;
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
            UpdatePanel upanelpages = (UpdatePanel)this.FindControl("upanelpages");
            if (upanelpages == null)
            {
                this.CreateLayout();
                upanelpages = (UpdatePanel)this.FindControl("upanelpages");
            }
            if (upanelpages != null)
            {
                Literal lblpaginationinfo = (Literal)this.FindControl("lblpaginationinfo");
                if (dataSize > 0)
                {
                    int lastrecord = (this.pageSize * this.pageIndex) + this.pageSize;
                    lblpaginationinfo.Text = String.Format("Showing {0} to {1} of {2} entries", (this.pageSize * this.pageIndex) + 1, lastrecord > dataSize ? dataSize : lastrecord, dataSize);
                }

                HtmlGenericControl paging_simple_next = (HtmlGenericControl)this.FindControl("paging_simple_next");
                HtmlGenericControl paging_simple_previous = (HtmlGenericControl)this.FindControl("paging_simple_previous");
                ListView lstpages = (ListView)this.FindControl(simple_plst_page_name);

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

                lstpages.DataSource = pagenumbers;
                lstpages.DataBind();
                upanelpages.Update();
            }
        }

        protected void btnprevious_Click(object sender, EventArgs e)
        {
            if (pageIndex >= 1)
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
            this.pageSize = Convert.ToInt32(((DropDownList)sender).SelectedValue);
            this.Bind();
        }
        //protected void btnsearch_Click(object sender, EventArgs e)
        //{
        //    pageIndex = 0;
        //    searchKey = txtsearch.Text;
        //    Bind();
        //    searchKey = "";
        //}


        public void CreateLayout()
        {
            UpdatePanel upanelpages = (UpdatePanel)this.FindControl("upanelpages");
            if (upanelpages == null)
            {
                UpdatePanel upanellvheaderrow = new UpdatePanel { ID = "upanellvheaderrow", UpdateMode = UpdatePanelUpdateMode.Conditional };
                var panelheadertools = new Panel() { ID = "panelheadertools", CssClass = "row" };
                var panellengthchange = new Panel() { ID = "panellengthchange", CssClass = "col-sm-12 col-md-6" };
                var divlengthchange = new HtmlGenericControl("div");
                divlengthchange.Attributes["class"] = "dataTables_length";
                var lbllengthchange = new HtmlGenericControl("label") { InnerText = "Show " };
                lbllengthchange.Attributes["style"] = "font-weight: normal; text-align: left; white-space: nowrap;";

                var cmbpagechange = new DropDownList { ID = "cmbpagelength", AutoPostBack = true, CssClass = "custom-select custom-select-sm form-control form-control-sm" };
                cmbpagechange.Attributes["style"] = "width: auto; display: inline-block;";
                cmbpagechange.Items.Add(new ListItem { Value = "10", Text = "10" });
                cmbpagechange.Items.Add(new ListItem { Value = "25", Text = "25" });
                cmbpagechange.Items.Add(new ListItem { Value = "50", Text = "50" });
                cmbpagechange.Items.Add(new ListItem { Value = "100", Text = "100" });
                cmbpagechange.SelectedIndexChanged += new EventHandler(cmbpagesize_SelectedIndexChanged);

                lbllengthchange.Controls.Add(cmbpagechange);
                lbllengthchange.Controls.Add(new HtmlGenericControl("span") { InnerText = " entries" });
                //lbllengthchange.InnerText +="entries";
                divlengthchange.Controls.Add(lbllengthchange);
                panellengthchange.Controls.Add(divlengthchange);
                panelheadertools.Controls.Add(panellengthchange);
                upanellvheaderrow.ContentTemplateContainer.Controls.Add(panelheadertools);

                UpdatePanel upanellistview = new UpdatePanel { ID = "upanellistview", UpdateMode = UpdatePanelUpdateMode.Conditional };
                var divlvmain = new HtmlGenericControl("div");
                divlvmain.Attributes["class"] = "row";
                var divlvmaininner = new HtmlGenericControl("div");
                divlvmaininner.Attributes["class"] = "col-md-12";

                var lstSimpleList = new ListView() { ID = "lstSimpleList" };
                if (this.ItemPlaceholderID != "")
                    lstSimpleList.ItemPlaceholderID = this.ItemPlaceholderID;

                if (this.LayoutTemplate != null)
                    lstSimpleList.LayoutTemplate = this.LayoutTemplate;

                if (this.ItemTemplate != null)
                    lstSimpleList.ItemTemplate = this.ItemTemplate;

                if (ItemDataBound != null)
                    lstSimpleList.ItemDataBound += ItemDataBound;

                if (LayoutCreated != null)
                    lstSimpleList.LayoutCreated += LayoutCreated;

                divlvmaininner.Controls.Add(lstSimpleList);
                divlvmain.Controls.Add(divlvmaininner);
                upanellistview.ContentTemplateContainer.Controls.Add(divlvmain);


                upanelpages = new UpdatePanel { ID = "upanelpages", UpdateMode = UpdatePanelUpdateMode.Conditional };
                var maindiv = new HtmlGenericControl("div");
                maindiv.Attributes["class"] = "row";

                var divpaginationinfo = new HtmlGenericControl("div");
                divpaginationinfo.Attributes["class"] = "col-sm-12 col-md-5";

                var divpaginationinfoinner = new HtmlGenericControl("div");
                divpaginationinfoinner.Attributes["class"] = "dataTables_info";
                divpaginationinfoinner.Attributes["role"] = "status";

                var lblpaginationinfo = new Literal { ID = "lblpaginationinfo" };
                divpaginationinfoinner.Controls.Add(lblpaginationinfo);
                divpaginationinfo.Controls.Add(divpaginationinfoinner);

                var divpagescontainer = new HtmlGenericControl("div");
                divpagescontainer.Attributes["class"] = "col-sm-12 col-md-7";

                var divpagescontainerinner = new HtmlGenericControl("div");
                divpagescontainerinner.Attributes["class"] = "dataTables_paginate paging_simple_numbers";

                var ulpagescontainer = new HtmlGenericControl("ul");
                ulpagescontainer.Attributes["class"] = "pagination";

                var liprevious = new HtmlGenericControl("li") { ID = "paging_simple_previous" };
                liprevious.Attributes["class"] = "paginate_button page-item previous disabled";
                LinkButton btnprevious = new LinkButton() { Text = "Previous", CssClass = "page-link" };
                btnprevious.Click += new EventHandler(btnprevious_Click);

                var lstpages = new ListView() { ID = "simple_plst_page" };
                lstpages.ItemTemplate = new IPageTemplate(ListItemType.Item, btnsimplepagenumber_Click);
                lstpages.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lst_ItemDataBound);

                var linext = new HtmlGenericControl("li") { ID = "paging_simple_next" };
                linext.Attributes["class"] = "paginate_button page-item next disabled";
                LinkButton btnnext = new LinkButton() { ID = "btn_paging_simple_next", Text = "Next", CssClass = "page-link" };
                btnnext.Click += new EventHandler(btnnext_Click);

                liprevious.Controls.Add(btnprevious);
                linext.Controls.Add(btnnext);
                //linext.Controls.Add(new LiteralControl("<asp:LinkButton ID='btnnext' runat='server' CssClass='page-link' OnClick='btnnext_Click' CommandArgument='-2'>Next</asp:LinkButton>"));
                ulpagescontainer.Controls.Add(liprevious);
                ulpagescontainer.Controls.Add(lstpages);
                ulpagescontainer.Controls.Add(linext);
                divpagescontainerinner.Controls.Add(ulpagescontainer);
                divpagescontainer.Controls.Add(divpagescontainerinner);

                maindiv.Controls.Add(divpaginationinfo);
                maindiv.Controls.Add(divpagescontainer);
                upanelpages.ContentTemplateContainer.Controls.Add(maindiv);

                var rootdiv = new HtmlGenericControl("div");
                rootdiv.Attributes["class"] = "dataTables_wrapper dt-bootstrap4 no-footer";
                this.Controls.Add(upanellvheaderrow);
                this.Controls.Add(upanellistview);
                this.Controls.Add(upanelpages);
                this.SaveControlState();
            }
        }
    }
}