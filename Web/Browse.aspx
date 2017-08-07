<%@ Page Language="C#" MasterPageFile="~/Arachnode.Master" AutoEventWireup="true"
    Codebehind="Browse.aspx.cs" Inherits="Arachnode.Web.Browse" Title="arachnode.net | Browse" %>

<asp:Content ID="uxUcBrowseContent" ContentPlaceHolderID="uxUcContentPlaceHolder"
    runat="server">
    <div id="header_arachnode_net_browse">
        arachnode.net
    </div>
    <div>
        <asp:Literal ID="uxLWebPage" runat="server"></asp:Literal>
        <asp:Label ID="uxLblException" runat="server" CssClass="exception" 
            Text="uxLblException" Visible="False"></asp:Label>
    </div>
</asp:Content>
