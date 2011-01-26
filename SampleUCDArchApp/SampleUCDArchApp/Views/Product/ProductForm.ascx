<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SampleUCDArchApp.Controllers.ProductViewModel>" %>
<%@ Import Namespace="SampleUCDArchApp.Core.Domain"%>


<%= Html.ClientSideValidation<Order>("Order") %>
    
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>
    
        <fieldset>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>