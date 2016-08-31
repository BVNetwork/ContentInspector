using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.SpecializedProperties;

namespace BVNetwork.ContentAreaInspector
{
    public class ContentAreaInspectorController : Controller
    {
        public ActionResult Index(int id)
        {
            var model = CreateModel(id, null);
            //return View(Paths.ToResource(this.GetType(),
            //    "Views/ContentAreaInspector/Index.cshtml"), model);
            return View(Paths.PublicRootPath + "_ContentAreaInspector/Views/ContentAreaInspector/Index.cshtml", model);
        }

        private ContentAreaInspectorViewModel CreateModel(int id, List<string> visitorGroupNames)
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var content = contentLoader.Get<IContent>(new ContentReference(id));
            var status = EPiServer.Framework.Localization.LocalizationService.Current.GetString("/episerver/cms/versionstatus/" + (content as IVersionable)?.Status.ToString().ToLower());
            var currentItem = new ContentAreaInspectorViewModel.InspectorContentViewModel()
            {
               
                Name = content.Name,
                Status = status,
                Type = content.GetOriginalType().Name
            };                   
            currentItem.EditUrl = PageEditing.GetEditUrl(content.ContentLink);
            if (content is ImageData)
            { 
                currentItem.MainType = MainContentType.Image;
                currentItem.PreviewUrl = content.ThumbnailUrl();
            }
            else if(content is BlockData)
                currentItem.MainType = MainContentType.Block;
            else
                currentItem.MainType = MainContentType.Page;

            var model = new ContentAreaInspectorViewModel()
            {
                Content = currentItem,
                VisitorGroupsNames = visitorGroupNames,
                ContentAreaItems = new List<ContentAreaInspectorViewModel.ContentAreaItemViewModel>()
            };
            var contentType = ServiceLocator.Current.GetInstance<IContentTypeRepository>().Load(content.ContentTypeID);
            foreach (var prop in contentType.PropertyDefinitions.Where(x => x.Type.Name == "ContentArea"))
            {
                var nestedContentArea = content.Property[prop.Name] as PropertyContentArea;
                if (nestedContentArea?.Value is ContentArea && (nestedContentArea.Value as ContentArea).Items != null)
                {
                    var contentAreaViewModel = new ContentAreaInspectorViewModel.ContentAreaItemViewModel
                    {
                        Name = prop.Name,
                        ContentAreaItems = new List<ContentAreaInspectorViewModel>()
                    };
                    var contentArea = (nestedContentArea.Value as ContentArea);
                    for (int i = 0; i < (nestedContentArea.Value as ContentArea).Count; i++)
                    {                       
                        var contentAreaItem = contentArea.Items[i];                        
                        var internalFormat = contentArea.Fragments[i].InternalFormat;
                        var visitorGroups = GetVisitorGroupNames(internalFormat);
                        contentAreaViewModel.ContentAreaItems.Add(CreateModel(contentAreaItem.ContentLink.ID,
                            visitorGroups));
                        
                    }
                    model.ContentAreaItems.Add(contentAreaViewModel);
                }
            }
            return model;
        }

        private List<string> GetVisitorGroupNames(string internalFormat)
        {
            List<string> visitorGroups = new List<string>();
            var indexOfDataGroups = internalFormat.IndexOf("data-groups=\"");
            if (indexOfDataGroups != -1)
            {
                indexOfDataGroups = indexOfDataGroups + 13;
                var endIndexOfDataGroups = internalFormat.IndexOf("\"", indexOfDataGroups);
                var length = endIndexOfDataGroups - indexOfDataGroups;
                var visitorGroupsStrings = internalFormat.Substring(indexOfDataGroups, length);
                var visitorGroupGuids = visitorGroupsStrings.Split(',');

                foreach (var visitorGroupGuid in visitorGroupGuids)
                {
                    var vgr = ServiceLocator.Current.GetInstance<IVisitorGroupRepository>
                        ().Load(new Guid(visitorGroupGuid));
                    visitorGroups.Add(vgr.Name);
                }
            }
            return visitorGroups;
        }
    }

    public enum MainContentType
    {
        Block,
        Image,
        Page
            
    }
}