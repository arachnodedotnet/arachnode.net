<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchResults.ascx.cs"
    Inherits="Arachnode.Web.UserControls.SearchResults" %>
<div class="Results">
    <asp:PlaceHolder ID="uxPhSearchResults" runat="server"></asp:PlaceHolder>
</div>
<div id="toggleClustering">
    <asp:HyperLink ID="uxHlShouldDocumentsBeClustered" runat="server" Visible="false" Text="Toggle Clustering"/>
</div>
<div id="pager">
    <asp:Label ID="uxLblPage" runat="server" Visible="false"><b>Page:</b></asp:Label>
    <asp:HyperLink ID="uxHlPrevious" Visible="false" runat="server" Text="&lt; Previous" />
    <asp:PlaceHolder ID="uxPhPages" runat="server"></asp:PlaceHolder>
    <asp:HyperLink ID="uxHlNext" Visible="false" runat="server" Text="Next &gt;" />
</div>
