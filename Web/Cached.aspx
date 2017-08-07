<%@ Page Language="C#" MasterPageFile="~/Arachnode.Master" AutoEventWireup="true"
    Codebehind="Cached.aspx.cs" Inherits="Arachnode.Web.Cached" Title="arachnode.net | Cached" %>

<asp:Content ID="uxUcCachedContent" ContentPlaceHolderID="uxUcContentPlaceHolder"
    runat="server">
    <div id="header_arachnode_net_cached">
        arachnode.net
    </div>
    <div>
        <asp:Literal ID="uxLWebPage" runat="server"></asp:Literal>
    </div>
</asp:Content>
