<%@ Page Language="C#" MasterPageFile="~/Arachnode.Master" AutoEventWireup="true"
    CodeBehind="Search.aspx.cs" Inherits="Arachnode.Web.Search" Title="arachnode.net | Search"
    EnableEventValidation="false" %>

<%@ Register Src="UserControls/SearchResults.ascx" TagName="SearchResults" TagPrefix="uc1" %>
<%@ Import Namespace="System.ComponentModel" %>
<asp:Content ID="uxUcSearchContent" ContentPlaceHolderID="uxUcContentPlaceHolder"
    runat="server">
    <div id="header_arachnode_net_search">
        <table>
            <tr>
                <td>
                    arachnode.net:&nbsp;
                </td>
                <td>
                    <asp:Panel DefaultButton="uxBtnSearch" ID="uxPnlSearchPanel" runat="server">
                        <asp:TextBox CssClass="searchBox" ID="uxTbQuery" MaxLength="2048" runat="server" /><asp:Button
                            CssClass="searchButton" ID="uxBtnSearch" Text="Search" runat="server" OnClick="uxBtnSearch_Click" />
                        <asp:Label ID="uxLblTotalNumberOfHits" runat="server"></asp:Label>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
        </table>
    </div>
    <asp:Panel ID="uxPnlResultsDetails" runat="server" Visible="true">
        <div id="resultsDetails">
            <div id="discoveryType">
                <asp:RadioButtonList ID="uxRblDiscoveryType" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Value="File">Files</asp:ListItem>
                    <asp:ListItem Value="Image">Images</asp:ListItem>
                    <asp:ListItem Selected="True" Value="WebPage">WebPages</asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <asp:Label ID="uxLblResultsDetails" runat="server">&nbsp;</asp:Label>
        </div>
    </asp:Panel>
    <div id="searchResults">
        <uc1:SearchResults ID="ucUxSearchResults" runat="server" />
    </div>
    <asp:Label CssClass="exception" ID="uxLblException" runat="server" Visible="false"></asp:Label>
</asp:Content>
