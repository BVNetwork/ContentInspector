using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;

namespace BVNetwork.ContentAreaInspector
{
    [ServiceConfiguration(typeof(IContentAreaInspectorService))]
    public class ContentAreaInspectorService : IContentAreaInspectorService
    {
        public ContentAreaInspectorViewModel CreateModel(ContentReference contentReference, List<string> visitorGroupNames, string contentGroup,
            int level, List<ContentReference> parentIds)
        {
            level++;
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var content = contentLoader.Get<IContent>(contentReference);

            var versionAble = content as IVersionable;
            var status = versionAble?.Status ?? VersionStatus.Published;
            var publishedDate = versionAble?.StartPublish?.ToString("g",
                  DateTimeFormatInfo.InvariantInfo);
            var currentItem = new ContentAreaInspectorViewModel.InspectorContentViewModel()
            {
                Name = content.Name,
                Id = content.ContentLink.ID.ToString(),
                Status = status,
                Type = content.GetOriginalType().Name,
                PreviewUrl = content.PreviewUrl(),
                AdditionalProperties = new Dictionary<string, object>(),
                PublishedDate = publishedDate

            };
           
            if (parentIds.Contains(contentReference))
            {
                currentItem.HasDuplicateParent = true;
            }
            else
            {
                parentIds.Add(contentReference);
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
                        contentAreaViewModel.ContentAreaItems.Add(CreateModel(contentAreaItem.ContentLink,
                            visitorGroups, contentAreaItem.ContentGroup, level, new List<ContentReference>(parentIds)));
                    }
                    model.ContentAreaItems.Add(contentAreaViewModel);
                }
            }
            var properties = contentType.ModelType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var inspectableAttribute =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(InspectableAttribute), true) as
                        InspectableAttribute;
                if (inspectableAttribute != null)
                {
                    if (propertyInfo.PropertyType == typeof(ContentReference) ||
                        propertyInfo.PropertyType == typeof(PageReference))
                    {
                        var contentReferenceProperty = content.Property[propertyInfo.Name] as PropertyContentReference;
                        var contentReferenceSubItem = contentReferenceProperty?.Value as ContentReference;
                        if (contentReferenceSubItem != null)
                        {
                            if (level >= 10 || model.Content.IsMaxLevel)
                            {
                                return model;
                            }
                            if (currentItem.HasDuplicateParent)
                                return model;
                            var contentReferenceItem = CreateModel(contentReferenceSubItem, null, null, level,
                                new List<ContentReference>(parentIds));

                            var contentReferenceViewModel = new ContentAreaInspectorViewModel.ContentReferenceViewModel()
                            {

                                Name = contentReferenceProperty.TranslateDisplayName() ?? propertyInfo.Name,
                                ContentReferenceItem = contentReferenceItem
                            };
                            model.ContentReferenceItems.Add(contentReferenceViewModel);
                        }
                    }
                    else
                    {
                        var property = content.Property[propertyInfo.Name];
                        model.Content.AdditionalProperties.Add(property.TranslateDisplayName(), property.Value);
                    }
                }
            }
            return model;
        }

        public List<string> GetVisitorGroupNames(string internalFormat)
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
}