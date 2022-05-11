using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ListviewPaginator.Interface
{
    public class IPageTemplate : ITemplate
    {

        ListItemType templateType;
        EventHandler handler;
        public IPageTemplate(ListItemType type, EventHandler handler)
        {
            templateType = type;
            this.handler = handler;
        }
        public void InstantiateIn(Control container)
        {
            switch (templateType)
            {
                case ListItemType.Item:
                    var licontrol = new HtmlGenericControl("li") { ID = "pageli" };
                    licontrol.Attributes["class"] = "paginate_button page-item";
                    container.Controls.Add(licontrol);
                    LinkButton btn = new LinkButton() { ID = "btnsimplepage", CssClass = "page-link" };
                    btn.Click += new EventHandler(handler);
                    licontrol.Controls.Add(btn);
                    break;
            }

        }
    }
}