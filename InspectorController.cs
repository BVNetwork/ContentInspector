﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            var model = CreateModel(id, null, null, 0, new List<int>());
            //return View(Paths.ToResource(this.GetType(),
            //    "Views/ContentAreaInspector/Index.cshtml"), model);
            return View(Paths.PublicRootPath + "_ContentAreaInspector/Views/ContentAreaInspector/Index.cshtml", model);
        }

        private ContentAreaInspectorViewModel CreateModel(int id, List<string> visitorGroupNames, string contentGroup, int level, List<int> parentIds)
        {
            level++;
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var content = contentLoader.Get<IContent>(new ContentReference(id));
            var status = (content as IVersionable).Status;
            var currentItem = new ContentAreaInspectorViewModel.InspectorContentViewModel()
            {
                Name = content.Name,
                Id = content.ContentLink.ID.ToString(),
                Status = status,
                Type = content.GetOriginalType().Name,
                PreviewUrl = content.PreviewUrl()
            };
            if (parentIds.Contains(id))
            {
                currentItem.HasDuplicateParent = true;
            }
            else
            {
                parentIds.Add(id);
            }

            currentItem.EditUrl = PageEditing.GetEditUrl(content.ContentLink);
            if (content is ImageData)
            {
                currentItem.MainType = MainContentType.Image;
                currentItem.ThumbnailUrl = content.ThumbnailUrl();
            }
            else if (content is BlockData)
                currentItem.MainType = MainContentType.Block;
            else
                currentItem.MainType = MainContentType.Page;

            var model = new ContentAreaInspectorViewModel()
            {
                Content = currentItem,
                VisitorGroupsNames = visitorGroupNames,
                ContentAreaItems = new List<ContentAreaInspectorViewModel.ContentAreaItemViewModel>(),
                ContentGroup = contentGroup,
                ContentReferenceItems = new List<ContentAreaInspectorViewModel.ContentReferenceViewModel>()
            };
            if (level >= 10)
            {
                model.Content.IsMaxLevel = true;
            }
            var contentType = ServiceLocator.Current.GetInstance<IContentTypeRepository>().Load(content.ContentTypeID);

            // Get content area properties
            foreach (var prop in contentType.PropertyDefinitions.Where(x => x.Type.Name == "ContentArea"))
            {
                var nestedContentArea = content.Property[prop.Name] as PropertyContentArea;
                if (nestedContentArea?.Value is ContentArea && (nestedContentArea.Value as ContentArea).Items != null)
                {
                    if (model.Content.IsMaxLevel || currentItem.HasDuplicateParent)
                    {
                        return model;
                    }
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
                            visitorGroups, contentAreaItem.ContentGroup, level, new List<int> (parentIds)));

                    }
                    model.ContentAreaItems.Add(contentAreaViewModel);
                }
            }

            // Get content reference properties
            foreach (var prop in contentType.PropertyDefinitions.Where(x => x.Type.Name == "ContentReference" || x.Type.Name == "PageReference"))
            {
                var contentReferenceProperty = content.Property[prop.Name] as PropertyContentReference;
                var contentReference = contentReferenceProperty?.Value as ContentReference;
                if (contentReference != null)
                {
                    if (level >= 10 || model.Content.IsMaxLevel)
                    {
                        return model;
                    }
                    if (currentItem.HasDuplicateParent)
                        return model;
                    var contentReferenceItem = CreateModel(contentReference.ID, null, null, level, new List<int>(parentIds));

                    var contentReferenceViewModel = new ContentAreaInspectorViewModel.ContentReferenceViewModel()
                    {
                        Name = prop.TranslateDisplayName() ?? prop.Name,
                        ContentReferenceItem = contentReferenceItem
                    };
                    model.ContentReferenceItems.Add(contentReferenceViewModel);
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