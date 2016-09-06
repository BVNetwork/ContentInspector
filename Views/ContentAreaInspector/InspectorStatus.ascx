<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<VersionStatus>" %>
<%@ Import Namespace="BVNetwork.ContentAreaInspector" %>
<%@ Import Namespace="EPiServer.Core" %>
<%@ Import Namespace="EPiServer.Shell" %>



<% string cssClass = "";
               if (Model == VersionStatus.DelayedPublish)
               {
                   cssClass = "inspector_scheduled";
               }
               else if (Model != VersionStatus.Published)
               {
                   cssClass = "inspector_not_published epi-iconDanger  epi-icon--colored";
               }
               var statusString = EPiServer.Framework.Localization.LocalizationService.Current.GetString("/episerver/cms/versionstatus/" + Model.ToString().ToLower());
     %>
    <li>Status: 
        <span class="<%:cssClass %>"><%: statusString %></span>

    </li>