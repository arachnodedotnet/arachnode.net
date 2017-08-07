<%@ Page Language="C#" MasterPageFile="~/Arachnode.Master" AutoEventWireup="true"
    CodeBehind="Explanation.aspx.cs" Title="arachnode.net | Search" Inherits="Arachnode.Web.Explanation" %>

<asp:Content ID="uxUcExplanation" ContentPlaceHolderID="uxUcContentPlaceHolder" runat="server">
    <div id="header_arachnode_net_search">
        <table>
            <tr>
                <td>
                    arachnode.net
                </td>
            </tr>
        </table>
    </div>
    <div id="explanationDetails">
        <asp:HyperLink ID="uxHlAbsoluteUri" runat="server"></asp:HyperLink>
    </div>
    <asp:Label ID="uxLblExplanation" runat="server"></asp:Label>
    <asp:Label ID="uxLblStrength" runat="server"></asp:Label>
</asp:Content>
