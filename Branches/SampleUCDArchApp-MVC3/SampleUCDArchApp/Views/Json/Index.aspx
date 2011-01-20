<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">

    $(document).ready(function() {

        $("#getOrder").click(function() {
            $.get('<%= Url.Action("Get", "Json") %>', 
                displayResult);
        });

        $("#getOrders").click(function() {
            $.get('<%= Url.Action("List", "Json") %>',
                displayResult);
        });
    });

    function displayResult(result) {
        $("#RawResults").html(result);
    }

</script>

    <h2>Index</h2>
    
    <h4>You can change which properties are serialized by going to Order.cs and adding the JsonProperty attribute to the desired properties</h4>
    
    <p>
        <a href="javascript:;" id="getOrder">Click Here to Retrieve An Order Via Json</a>
    </p>
    <p>
        <a href="javascript:;" id="getOrders">Click Here to Retrieve 100 Orders Via Json</a>
    </p>
    <p>
        <label>Raw Results: </label>
        <span id="RawResults"></span>
    </p>

</asp:Content>
