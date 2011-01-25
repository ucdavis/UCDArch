<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<SampleUCDArchApp.Core.Domain.Product>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

 <h2>Products</h2>
     <table>
        <tr>
            <th>
                ProductName
            </th>
            <th>
                QuantityPerUnit
            </th>
            <th>
                UnitPrice
            </th>
            <th>
                UnitsInStock
            </th>
            <th>
                UnitsOnOrder
            </th>
            <th>
                ReorderLevel
            </th>
            <th>
                Discontinued
            </th>
        </tr>
        
           <% foreach (var item in Model) { %>
    
        <tr>
            <td><%= Html.Encode(item.ProductName)%></td>
            
            <td><%= Html.Encode(item.QuantityPerUnit)%></td>
            
            <td><%= Html.Encode(item.UnitPrice)%></td>
            
            <td><%= Html.Encode(item.UnitsInStock)%></td>
            
            <td><%= Html.Encode(item.UnitsOnOrder)%></td>
            
            <td><%= Html.Encode(item.ReorderLevel)%></td>
            
            <td><%= Html.Encode(item.Discontinued)%></td>
            
        </tr>
    
    <% } %>

    </table>



</asp:Content>
