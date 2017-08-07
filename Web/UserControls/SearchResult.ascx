<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchResult.ascx.cs"
    Inherits="Arachnode.Web.UserControls.SearchResult" %>
<div class="searchResult">
    <div class="title">
        <asp:HyperLink ID="uxHlTitle" runat="server"></asp:HyperLink></div>
    <div class="summary">
        <asp:Label ID="uxLblSummary" runat="server"></asp:Label></div>
        <asp:Image ID="uxImgImage" CssClass="image" runat="server"></asp:Image>
    <div class="absoluteUri">
        <asp:Label ID="uxLblAbsoluteUri" runat="server"></asp:Label>&nbsp;-
        <asp:HyperLink ID="uxHlBrowse" Text="Browse / " runat="server" Visible="false"></asp:HyperLink><asp:HyperLink ID="uxHlCached" Text="Cached" runat="server"></asp:HyperLink>&nbsp(<asp:Label ID="uxLblScoreAndStrength" runat="server"></asp:Label>)&nbsp;-&nbsp;<asp:HyperLink ID="uxHlExplain" Text="Explain" runat="server"></asp:HyperLink></div>
</div>
