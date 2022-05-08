<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Bootstrap.ascx.cs" Inherits="ListviewPaginator.Bootstrap" %>

<!--
    Created By: Royette Camahalan
    Beta: 2022-05-06
-->
<div class="dataTables_wrapper dt-bootstrap4 no-footer">
    <asp:UpdatePanel ID="upanelfilters" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="panelheadertools" runat="server" CssClass="row">
                <asp:Panel ID="panelpagesize" runat="server" CssClass="col-sm-12 col-md-6">
                    <div class="dataTables_length">
                        <label>Show 
                            <asp:DropDownList ID="cmbpagesize" runat="server" CssClass="custom-select custom-select-sm form-control form-control-sm" AutoPostBack="true" OnSelectedIndexChanged="cmbpagesize_SelectedIndexChanged">
                                <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                <asp:ListItem Value="25" Text="25"></asp:ListItem>
                                <asp:ListItem Value="50" Text="50"></asp:ListItem>
                                <asp:ListItem Value="100" Text="100"></asp:ListItem>
                            </asp:DropDownList>entries
                        </label>
                    </div>
                </asp:Panel>
                <asp:Panel ID="panelsearch" runat="server" CssClass="col-sm-12 col-md-6" DefaultButton="btnsearch">
                    <div class="dataTables_filter">
                        <label>Search:
                            <asp:TextBox ID="txtsearch" runat="server" CssClass="form-control form-control-sm" autocomplete="disabled" placeholder=""></asp:TextBox>
                        </label>
                        <asp:Button ID="btnsearch" runat="server" style="display:none" OnClick="btnsearch_Click"/>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upanellist" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="row">
                <div class="col-sm-12">
                    <asp:PlaceHolder ID="listplaceholder" runat="server"></asp:PlaceHolder>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upanelpagination" runat="server" UpdateMode="Conditional" Visible="false">
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
</div>
