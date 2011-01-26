<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<SampleUCDArchApp.Core.Domain.Customer>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Customers</h2>

    <table>
        <tr>
            <th>
                CompanyName
            </th>
            <th>
                ContactName
            </th>
            <th>
                Country
            </th>
            <th>
                Fax
            </th>
            <th>
                Id
            </th>
            <th>
                Orders
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.Encode(item.CompanyName) %>
            </td>
            <td>
                <%= Html.Encode(item.ContactName) %>
            </td>
            <td>
                <%= Html.Encode(item.Country) %>
            </td>
            <td>
                <%= Html.Encode(item.Fax) %>
            </td>
            <td>
                <%= Html.Encode(item.Id) %>
            </td>
            <td>
                <%= Html.Encode(item.Orders.Count()) %>
            </td>
        </tr>
    
    <% } %>

    </table>


</asp:Content>

