<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SampleUCDArchApp.Controllers.OrderViewModel>" %>
<%@ Import Namespace="SampleUCDArchApp.Helpers" %>
    
    <%: Html.ValidationSummary() %>

    <% using (Html.BeginForm()) {%>

        <%: Html.AntiForgeryToken() %>
        <%: Html.HiddenFor(o=>o.Order.Id) %>

        <fieldset>
            <legend>Fields</legend>

            <p>
                <%: Html.LabelFor(x=>x.Order.OrderDate) %>
                <%: Html.EditorFor(x=>x.Order.OrderDate) %>
                <%: Html.ValidationMessageFor(x=>x.Order.OrderDate) %>
            </p>
            <p>
                <%: Html.LabelFor(x=>x.Order.ShipAddress) %>
                <%: Html.EditorFor(x=>x.Order.ShipAddress) %>
                <%: Html.ValidationMessageFor(x=>x.Order.ShipAddress) %>
            </p>
            <p>

                <%: this.Select("Order.OrderedBy")
                        .Options(Model.Customers, x=>x.Id, x=>x.CompanyName)
                        .Selected(Model.Order.OrderedBy != null ? Model.Order.OrderedBy.Id : "")
                        .FirstOption(null, "--Select A Company--")
                        .HideFirstOptionWhen(Model.Order.OrderedBy != null)
                        .Label("Ordered By:")
                        .IncludeUnobtrusiveValidationAttributes(Html)%>
                        
                <%: Html.ValidationMessageFor(x=>x.Order.OrderedBy) %>

            </p>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>
