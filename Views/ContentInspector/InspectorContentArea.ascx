<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<ContentInspectorViewModel.ContentAreaItemViewModel>>" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="EPiCode.ContentInspector.Models" %>
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
                
                       <% Html.RenderPartial(Paths.ToResource("EPiCode.ContentInspector","Views/ContentInspector/InspectorContent.ascx"), subItem, new ViewDataDictionary{{"group",previousContentGroup}});%> 
                       <% {
                            previousContentGroup = subItem.ContentGroup;
                        } 
                    }%>
                        

                </div>
            </div>
        </li>
<%} %>
