<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<ContentInspectorViewModel.ContentAreaItemViewModel>>" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="EPiCode.ContentInspector.Models" %>
<%@ Import Namespace="EPiServer.Shell" %>

<%
var previousContentGroup = "";
foreach (var item in Model)
{ %>
<li class="inspector_area">
    <div class="inspector_property">
        [Content area]<i> <%: item.Name%></i>
        <text> value:</text>
    </div>
    <div class="inspector_content_area">
        <div>
            <% for (int i = 0; i < item.ContentAreaItems.Count(); i++)
                       {%>
            <% Html.RenderPartial(Paths.ToResource("EPiCode.ContentInspector","Views/ContentInspector/InspectorContent.ascx"), item.ContentAreaItems[i], new ViewDataDictionary{{"group",previousContentGroup}});%>
            <% {  
                    previousContentGroup = item.ContentAreaItems[i].ContentGroup;
               }
               if (item.ContentAreaItems.Count() == i && !string.IsNullOrEmpty(item.ContentAreaItems[i].ContentGroup))
               {%>
                    </div>
            <% } %>
        <%  } %>
        </div>
    </div>
</li>
<%
    previousContentGroup = null;
} %>
