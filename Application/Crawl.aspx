<%@ Page Title="" Language="C#" MasterPageFile="~/Arachnode.Master" AutoEventWireup="true"
    CodeBehind="Crawl.aspx.cs" Inherits="Application.Crawl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script src="Scripts/library.js" type="text/javascript"></script>
    <script src="Scripts/prototype.js" type="text/javascript"></script>
    <script src="Scripts/scriptaculous.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">
    <div id="header_arachnode_net_crawl">
        <table>
            <tr>
                <td>
                    arachnode.net:&nbsp;
                </td>
                <td>
                    <asp:Panel DefaultButton="uxBtnCrawl" ID="uxPnlCrawlPanel" runat="server">
                        <asp:TextBox CssClass="crawlBox" ID="uxTbAbsoluteUri" MaxLength="2048" runat="server" >http://waikiki.com</asp:TextBox>
                        <asp:Button CssClass="crawlButton" ID="uxBtnCrawl" Text="Crawl" runat="server" OnClick="UxBtnCrawl_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="uxPnlResultsDetails" runat="server" Visible="true">
        <div id="resultsDetails">
            &nbsp;
        </div>
    </asp:Panel>
    <div id="canvas">
    </div>

    <script type="text/javascript">
        updatePage(1000, 1000);        
    </script>

</asp:Content>
