<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<ContentInspectorViewModel.XhtmlStringViewModel>>" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="EPiCode.ContentInspector.Models" %>
<%@ Import Namespace="EPiServer.Shell" %>

<%if(Model != null){ %>
<%    foreach (var xhtmlStringViewModel in Model)
            { %>
<li class="inspector_area">
    <div class="inspector_property">[XhtmlString]<i> <%:xhtmlStringViewModel.Name %></i>  value:</div>

    <% foreach (var fragment in xhtmlStringViewModel.Fragments)
                        { %>

    <div class="inspector_content_area">
        <% if (fragment is ContentInspectorViewModel)
                                        { %>
        <% Html.RenderPartial(Paths.ToResource("EPiCode.ContentInspector", "Views/ContentInspector/InspectorContent.ascx"), fragment); %>
        <% }
                                        else
                                        { %>
        <%:(fragment as ContentInspectorViewModel.XhtmlStringContent).Value %>
        <% } %>
    </div>


    <%} %>

               
</li>
<% }}%>