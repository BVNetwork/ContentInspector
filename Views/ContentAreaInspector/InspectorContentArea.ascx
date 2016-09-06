<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<ContentAreaInspectorViewModel.ContentAreaItemViewModel>>" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="BVNetwork.ContentAreaInspector" %>
<%@ Import Namespace="EPiServer.Shell" %>

<%
var previousContentGroup = "";
foreach (var item in Model)
{ %>
        <li class="inspector_area">
            <div class="inspector_property">[Content area]<i> <%: item.Name%></i> <text> value:</text></div>
            <div class="inspector_content_area">
                <div>
                    
                   <% foreach (var subItem in item.ContentAreaItems)
                    { %>
                
                       <% Html.RenderPartial(Paths.ToResource("ContentAreaInspector","Views/ContentAreaInspector/InspectorContent.ascx"), subItem, new ViewDataDictionary{{"group",previousContentGroup}});%> 
                       <% {
                            previousContentGroup = subItem.ContentGroup;
                        } 
                    }%>
                        

                </div>
            </div>
        </li>
<%} %>
