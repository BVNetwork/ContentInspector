using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using EPiCode.ContentInspector.Models;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Cms.Shell.UI.Editor.TinyMCE.Plugins;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;

namespace EPiCode.ContentInspector
{
    [ServiceConfiguration(typeof(IContentInspectorService))]
    public class ContentInspectorService : IContentInspectorService
    {
        private readonly int _maxLevel;
        private readonly IContentLoader _contentLoader;

        public ContentInspectorService()
        {
            var maxLevel = ConfigurationManager.AppSettings["ContentInspector.MaxLevel"];
            _maxLevel = Convert.ToInt32(!string.IsNullOrEmpty(maxLevel) ? maxLevel : "10");
            _contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
        }

        public ContentInspectorViewModel CreateModel(ContentReference contentReference, List<string> visitorGroupNames, string contentGroup,
            int level, List<ContentReference> parentIds)
        {
            level++;
            var content = _contentLoader.Get<IContent>(contentReference);
            var currentItem = CreateInspectorContentViewModel(content);
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

            var model = new ContentInspectorViewModel()
            {
                Content = currentItem,
                VisitorGroupsNames = visitorGroupNames,
                ContentAreaItems = new List<ContentInspectorViewModel.ContentAreaItemViewModel>(),
                ContentGroup = contentGroup,
                ContentReferenceItems = new List<ContentInspectorViewModel.ContentReferenceViewModel>(),
                XhtmlStringItems = new List<ContentInspectorViewModel.XhtmlStringViewModel>()
            };

            var contentType = ServiceLocator.Current.GetInstance<IContentTypeRepository>().Load(content.ContentTypeID);

            if (level >= _maxLevel)
            {
                model.Content.IsMaxLevel = true;
            }
            else if (!currentItem.HasDuplicateParent)
            {
                model.ContentAreaItems = GetContentAreaItems(level, parentIds, contentType, content); ;
            }
            var inspectablePropertes = GetInspectableProperties(contentType);
            foreach (var propertyInfo in inspectablePropertes)
            {
                if (propertyInfo.PropertyType == typeof(ContentReference) ||
                    propertyInfo.PropertyType == typeof(PageReference))
                {
                    GetContentReferenceProperty(level, parentIds, content, propertyInfo, model);
                }
                else if (propertyInfo.PropertyType == typeof(XhtmlString))
                {
                    var property = content.Property[propertyInfo.Name] as PropertyXhtmlString;
                    var fragments = property?.XhtmlString?.Fragments;
                    var xhtmlStringViewModel = new ContentInspectorViewModel.XhtmlStringViewModel();
                    xhtmlStringViewModel.Name = property.TranslateDisplayName();
                    //xhtmlStringViewModel.Fragments = fragments?.ToList();
                    if (fragments != null)
                    {
                        foreach (var fragment in fragments)
                        {
                            CreateFragment(level, parentIds, fragment, xhtmlStringViewModel, null);
                        }
                    }
                    model.XhtmlStringItems.Add(xhtmlStringViewModel);

                }
                else
                {
                    var property = content.Property[propertyInfo.Name];
                    if (property != null)
                    {
                        model.Content.AdditionalProperties.Add(property.TranslateDisplayName(), property.Value);

                    }
                    else
                    {
                        model.Content.AdditionalProperties.Add(propertyInfo.Name, propertyInfo.GetValue(content));
                    }
                   
                }
            }
            return model;
        }

        private void GetContentReferenceProperty(int level, List<ContentReference> parentIds, IContent content, PropertyInfo propertyInfo,
            ContentInspectorViewModel model)
        {
            var contentReferenceProperty = content.Property[propertyInfo.Name] as PropertyContentReference;
            var contentReferenceSubItem = contentReferenceProperty?.Value as ContentReference;
            if (contentReferenceSubItem != null)
            {
                if (!model.Content.IsMaxLevel && !model.Content.HasDuplicateParent)
                {
                    var contentReferenceItem = CreateModel(contentReferenceSubItem, null, null, level,
                        new List<ContentReference>(parentIds));
                    var contentReferenceViewModel = new ContentInspectorViewModel.ContentReferenceViewModel()
                    {
                        Name = contentReferenceProperty.TranslateDisplayName() ?? propertyInfo.Name,
                        ContentReferenceItem = contentReferenceItem
                    };
                    model.ContentReferenceItems.Add(contentReferenceViewModel);
                }
            }
        }

        private void CreateFragment(int level, List<ContentReference> parentIds, IStringFragment fragment,
            ContentInspectorViewModel.XhtmlStringViewModel xhtmlStringViewModel, List<string> visitorGroups)
        {
            if (fragment is PersonalizedContentFragment)
            {
                foreach (var personalizedFragment in (fragment as PersonalizedContentFragment).Fragments)
                {
                    var internalFormat = fragment.InternalFormat;
                    visitorGroups = GetVisitorGroupNames(internalFormat);
                    CreateFragment(level, parentIds, personalizedFragment, xhtmlStringViewModel, visitorGroups);
                }
            }
            else
            {
                var contentFragment = fragment as EPiServer.Core.Html.StringParsing.ContentFragment;
                if (contentFragment != null)
                {
                    var contentItemModel = CreateModel(contentFragment.ContentLink,
                        visitorGroups, contentFragment.ContentGroup, level, new List<ContentReference>(parentIds));

                    xhtmlStringViewModel.Fragments.Add(contentItemModel);
                }
                else
                {
                    xhtmlStringViewModel.Fragments.Add(new ContentInspectorViewModel.XhtmlStringContent()
                    {
                        Value = fragment.GetViewFormat()
                    });
                }
            }
        }

        public virtual ContentInspectorViewModel.InspectorContentViewModel CreateInspectorContentViewModel(IContent content)
        {
            var versionAble = content as IVersionable;
            var currentItem = new ContentInspectorViewModel.InspectorContentViewModel()
            {
                Name = content.Name,
                Id = content.ContentLink.ID.ToString(),
                Status = versionAble?.Status ?? VersionStatus.Published,
                Type = content.GetOriginalType().Name,
                PreviewUrl = content.PreviewUrl(),
                AdditionalProperties = new Dictionary<string, object>(),
                PublishedDate = versionAble?.StartPublish?.ToString("g",
                    DateTimeFormatInfo.InvariantInfo)
            };
            return currentItem;
        }

        public virtual List<PropertyInfo> GetInspectableProperties(ContentType contentType)
        {
            var properties = contentType.ModelType.GetProperties();
            List<PropertyInfo> inspectableProperties = new List<PropertyInfo>();
            foreach (var propertyInfo in properties)
            {
                var inspectableAttribute =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(InspectableAttribute), true) as
                        InspectableAttribute;
                if (inspectableAttribute != null)
                {
                    inspectableProperties.Add(propertyInfo);
                }
            }
            return inspectableProperties;
        }

        public virtual List<ContentInspectorViewModel.ContentAreaItemViewModel> GetContentAreaItems(int level, List<ContentReference> parentIds, ContentType contentType, IContent content)
        {
            var contentAreaItemViewModels = new List<ContentInspectorViewModel.ContentAreaItemViewModel>();
            foreach (var prop in contentType.PropertyDefinitions.Where(x => x.Type.Name == "ContentArea"))
            {
                var nestedContentArea = content.Property[prop.Name] as PropertyContentArea;
                if (nestedContentArea?.Value is ContentArea && (nestedContentArea.Value as ContentArea).Items != null)
                {
                    var contentAreaViewModel = new ContentInspectorViewModel.ContentAreaItemViewModel
                    {
                        Name = prop.Name,
                        ContentAreaItems = new List<ContentInspectorViewModel>()
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
                    contentAreaItemViewModels.Add(contentAreaViewModel);
                }

            }
            return contentAreaItemViewModels;
        }

        public virtual List<string> GetVisitorGroupNames(string internalFormat)
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