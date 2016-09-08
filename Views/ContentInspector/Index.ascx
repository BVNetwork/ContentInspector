<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BVNetwork.ContentInspector.Models.ContentInspectorViewModel>" %>
<%@ Import Namespace="BVNetwork.ContentInspector.Models" %>
<%@ Import Namespace="EPiServer.Core" %>
<%@ Import Namespace="EPiServer.Shell" %>

<div class="inspector_container">
    <div class="inspector_main">
        <ul>
            <li class="inspector_main_preview">Type: <%: Model.Content.Type %>   <span data-type="<%:Model.Content.MainType %>" data-previewUrl="<%: Model.Content.PreviewUrl%>" class="inspector_preview_button dijitReset dijitInline dijitIcon epi-icon--medium epi-iconPreview">
                        </span></li>
         
             <% Html.RenderPartial(Paths.ToResource("BVNetwork.ContentInspector","Views/ContentInspector/InspectorStatus.ascx"), Model.Content);%> 
        
              <%   foreach (var additionalProperty in Model.Content.AdditionalProperties)
    {%>
        <li><%:additionalProperty.Key %>: <i><%:additionalProperty.Value %></i></li>
    <%} %>
                 
        </ul>
    </div>
    <ul class="inspector">
        <% Html.RenderPartial(Paths.ToResource("BVNetwork.ContentInspector","Views/ContentInspector/InspectorContentArea.ascx"), Model.ContentAreaItems);%> 
        <%if (Model.ContentReferenceItems != null && Model.ContentReferenceItems.Any())
        {
            foreach (var contentReference in Model.ContentReferenceItems)
            { %>
                <li class="inspector_area">
                    <div class="inspector_property">[Content reference]<i> <%:contentReference.Name %></i>  value:</div>
                    <div class="inspector_content_area">
                        <div>
                              <% Html.RenderPartial(Paths.ToResource("BVNetwork.ContentInspector","Views/ContentInspector/InspectorContent.ascx"), contentReference.ContentReferenceItem); %> 
                        </div>
                    </div>
                </li>
           <% }%>
        <% }%>
    </ul>
</div>
  






