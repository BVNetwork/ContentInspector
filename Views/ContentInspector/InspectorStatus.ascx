<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ContentInspectorViewModel.InspectorContentViewModel>" %>
<%@ Import Namespace="EPiCode.ContentInspector.Models" %>
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
        <span class="<%:cssClass %>"><%: statusString %></span>
        <% if ((Model.Status == VersionStatus.Published || Model.Status == VersionStatus.DelayedPublish) && Model.PublishedDate != null)
           { %>
        - 
        <%: Model.PublishedDate %>
        <% } %>
    </li>