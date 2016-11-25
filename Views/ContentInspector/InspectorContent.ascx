<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EPiCode.ContentInspector.Models.ContentInspectorViewModel>" %>
<%@ Import Namespace="EPiCode.ContentInspector" %>
<%@ Import Namespace="EPiServer.Shell" %>

<% var previousContentGroup = ViewBag.group;

if (!string.IsNullOrEmpty(previousContentGroup) && string.IsNullOrEmpty(Model.ContentGroup)|| (!string.IsNullOrEmpty(previousContentGroup) && !string.IsNullOrEmpty(Model.ContentGroup) && Model.ContentGroup != previousContentGroup ) )
{
     %>
         </div>
<%} 
if (string.IsNullOrEmpty(previousContentGroup) && !string.IsNullOrEmpty(Model.ContentGroup) || (!string.IsNullOrEmpty(previousContentGroup) && !string.IsNullOrEmpty(Model.ContentGroup) && Model.ContentGroup != previousContentGroup ) )
{
    %>

         <div class="inspector_group">
    <%} %> 
            
            <% ViewBag.group = ""; %>
    <ul class="inspector inspectorcontent">
        <li class="inspectoritem">
            <span>
                <ul>
                    <% if (!string.IsNullOrEmpty(Model.ContentGroup))
                    {%>
                        <li class="inspector_green">
                            <i>
                                <span class="epi-iconUsers dijitFolderOpened inspector_visitor_group"></span>
                                <span class="inspector_black">Visible for:</span>
                                <% if (Model.VisitorGroupsNames != null && Model.VisitorGroupsNames.Any())
                                {

                                    for (int i = 0; i < Model.VisitorGroupsNames.Count; i++)
                                    {
                                        var visitorGroup = Model.VisitorGroupsNames[i];%>
                                        <%: visitorGroup%>
                                       <% if (i != Model.VisitorGroupsNames.Count - 1)
                                        { %>
                                            <span class="inspector_black">and</span>
                                       <% }
                                    }
                                }
                                else
                                { %>
                                    <text>Everyone else </text>
                               <% } %>
                            </i>
                        </li>
                   <% } %>
                    <%if (Model.Content.MainType == MainContentType.Image)
                    { %>
                        <img width="48" height="48" src="<%:Model.Content.ThumbnailUrl %>" />
                   <% } 
                    else if (Model.Content.MainType == MainContentType.Block)
                    {%>
                        <span class="epi-iconObjectSharedBlock epi-icon--large inspector_block"></span>
                   <%  }
                    else if (Model.Content.MainType == MainContentType.Page)
                    {%>
                        <span class="epi-iconObjectPage epi-icon--large inspector_block"></span>
                   <% }%>
                    <li>
                        <b><%:Model.Content.Name %></b> <a target="_blank" class="epi-visibleLink" href="<%:Model.Content.EditUrl %>">edit </a>

                        <span data-type="<%:Model.Content.MainType %>" data-previewUrl="<%: Model.Content.PreviewUrl%>" class="inspector_preview_button dijitReset dijitInline dijitIcon epi-icon--medium epi-iconPreview">

                        </span>
                    </li>
                    <li>Type: <%:Model.Content.Type %></li>
           <% Html.RenderPartial(Paths.ToResource("EPiCode.ContentInspector","Views/ContentInspector/InspectorStatus.ascx"), Model.Content);%> 
        
                                  <%   foreach (var additionalProperty in Model.Content.AdditionalProperties)
    {%>
        <li><%:additionalProperty.Key %>: <i><%:additionalProperty.Value %></i></li>
    <%} %>
                    <%if (Model.Content.IsMaxLevel)
                    { %>
                        <li><i>Content has sub items, but the inspector will only show 10 sub item levels. Please edit and inspect a sub item to see additional levels</i></li>
                   <% } 
                    if (Model.Content.HasDuplicateParent)
                    {%>
                        <li><i>Content has already been expanded by a parent item. Further sub items will not be displayed</i></li>
                   <%  }%>
                </ul>
            </span>
        </li>

        <% Html.RenderPartial(Paths.ToResource("EPiCode.ContentInspector","Views/ContentInspector/InspectorContentArea.ascx"), Model.ContentAreaItems);%> 
        <%if (Model.ContentReferenceItems != null && Model.ContentReferenceItems.Any())
        {
            foreach (var contentReferenceViewModel in Model.ContentReferenceItems)
            { %>
                <li>
                    <div class="inspector_property">
                        [Content reference]<i> <%:contentReferenceViewModel.Name %></i> value:
                         <% Html.RenderPartial(Paths.ToResource("EPiCode.ContentInspector", "Views/ContentInspector/InspectorContent.ascx"), contentReferenceViewModel.ContentReferenceItem); %> 

                    </div>
                </li>
           <% } %>
    <%    } %>
    </ul>
