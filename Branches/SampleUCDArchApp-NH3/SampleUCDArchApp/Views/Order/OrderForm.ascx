<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SampleUCDArchApp.Controllers.OrderViewModel>" %>
<%@ Import Namespace="SampleUCDArchApp.Core.Domain"%>

<%= Html.ClientSideValidation<Order>("Order") %>
    
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>
        <%= Html.HiddenFor(o=>o.Order.Id) %>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <%= this.TextBox("Order.OrderDate").Format("{0:d}").Value(Model.Order.OrderDate).Label("Order Date:") %>
                <%= Html.ValidationMessageFor(x=>x.Order.OrderDate) %>
            </p>
            <p>
                <%= this.TextBox("Order.ShipAddress").Value(Model.Order.ShipAddress).Label("Ship Address:") %>
                <%= Html.ValidationMessageFor(x=>x.Order.ShipAddress) %>
            </p>
            <p>
                <%= this.Select("Order.OrderedBy")
                        .Options(Model.Customers, x=>x.Id, x=>x.CompanyName)
                        .Selected(Model.Order.OrderedBy != null ? Model.Order.OrderedBy.Id : "")
                        .FirstOption(null, "--Select A Company--")
                        .HideFirstOptionWhen(Model.Order.OrderedBy != null)
                        .Label("Ordered By:")%>
                        
                <%= Html.ValidationMessageFor(x=>x.Order.OrderedBy) %>

            </p>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>
