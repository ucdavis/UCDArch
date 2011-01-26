<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<SampleUCDArchApp.Core.Domain.Order>>" %>
<%@ Import Namespace="SampleUCDArchApp.Controllers"%>
<%@ Import Namespace="SampleUCDArchApp.Helpers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <% Html.Grid(Model)
            .Transactional()
            .Name("Orders")
            .Columns(col =>
                {
                    col.Template(order =>
                        {
                            %>
                            <%=Html.ActionLink<OrderController>(a => a.Edit(order.Id), "Edit")%>
                            <%
                        });
                    col.Bound(order => order.OrderedBy.CompanyName);
                    col.Bound(order => order.ShipAddress);
                    col.Bound(order => order.OrderDate).Format("{0:d}");
                })
            .Pageable(x=>x.PageSize(20))
            .Sortable()
            .Render();
        
        %>

    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

</asp:Content>

