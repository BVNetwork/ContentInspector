<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ContentAreaInspectorViewModel.InspectorContentViewModel>" %>
<%@ Import Namespace="BVNetwork.ContentAreaInspector" %>
<%@ Import Namespace="EPiServer.Core" %>
<%@ Import Namespace="EPiServer.Shell" %>



<% string cssClass = "";
               if (Model.Status == VersionStatus.DelayedPublish)
               {
                   cssClass = "inspector_scheduled";
               }
               else if (Model.Status != VersionStatus.Published)
               {
                   cssClass = "inspector_not_published epi-iconDanger  epi-icon--colored";
               }
               var statusString = EPiServer.Framework.Localization.LocalizationService.Current.GetString("/episerver/cms/versionstatus/" + Model.Status.ToString().ToLower());
     %>
    <li>Status: 
        <span class="<%:cssClass %>"><%: statusString %></span> - 
        <% if (Model.PublishedDate != null)
           { %>
        <%: Model.PublishedDate %>
        <% } %>
    </li>