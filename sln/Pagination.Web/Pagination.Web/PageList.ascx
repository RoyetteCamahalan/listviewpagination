<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageList.ascx.cs" Inherits="Pagination.Web.PageList" %>

<!--
    Created By: Royette Camahalan
    Beta: 2022-05-06
-->

<asp:UpdatePanel ID="upanellist" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="row">
            <div class="col-sm-12">
                <asp:PlaceHolder ID="listplaceholder" runat="server"></asp:PlaceHolder>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel ID="upanelpagination" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="row">
            <div class="col-sm-12 col-md-5">
                <div class="dataTables_info" id="simple_tbl_info" role="status" aria-live="polite">
                    <asp:Literal ID="lblpaginationinfo" runat="server"></asp:Literal>
                </div>
            </div>
            <asp:Panel ID="panelpagecontainer" runat="server" CssClass="col-sm-12 col-md-7">
                <div class="dataTables_paginate paging_simple_numbers" id="MainContent_tbl_suppliers_paginate">
                    <ul class="pagination">
                        <li class="paginate_button page-item previous disabled" id="paging_simple_previous" runat="server">
                            <asp:LinkButton ID="btnprevious" runat="server" CssClass="page-link" OnClick="btnprevious_Click" CommandArgument="-1">Previous</asp:LinkButton>
                            <%--<a href="#" aria-controls="MainContent_tbl_suppliers" data-dt-idx="0" tabindex="0" class="page-link">Previous</a>--%>
                        </li>
                        <asp:ListView ID="lstsimplepagination" runat="server">
                            <ItemTemplate>
                                <li class='<%# "paginate_button page-item" + (Convert.ToBoolean(Eval("isactive")) ? " active" : "") %>'>
                                    <asp:LinkButton ID="btnsimplepagenumber" runat="server" CssClass="page-link" CausesValidation="false" OnClick="btnsimplepagenumber_Click" CommandArgument='<%# Eval("pageNumber") %>'><%# Eval("data") %></asp:LinkButton>
                                </li>
                            </ItemTemplate>
                        </asp:ListView>
                        <li class="paginate_button page-item next disabled" id="paging_simple_next" runat="server">
                            <asp:LinkButton ID="btnnext" runat="server" CssClass="page-link" OnClick="btnnext_Click" CommandArgument="-2">Next</asp:LinkButton>
                        </li>
                    </ul>
                </div>
            </asp:Panel>                
        </div>
    </ContentTemplate>
</asp:UpdatePanel>