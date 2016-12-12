using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;

namespace EPiCode.ContentInspector.Models
{
    public class ContentInspectorViewModel : IXhtmlStringItem
    {
        public InspectorContentViewModel Content { get; set; }
        public List<string> VisitorGroupsNames { get; set; }
        public string ContentGroup { get; set; }
        public List<ContentAreaItemViewModel> ContentAreaItems { get; set; }
        public List<ContentReferenceViewModel> ContentReferenceItems { get; set; }
        public List<ContentInspectorViewModel.XhtmlStringViewModel> XhtmlStringItems { get; set; }

        public class InspectorContentViewModel
        {
            public string Name { get; set; }
            public string PublishedDate { get; set; }
            public string Id { get; set; }
            public VersionStatus Status { get; set; }
            public string Type { get; set; }
            public string EditUrl { get; set; }
            public string PreviewUrl { get; set; }
            public string ThumbnailUrl { get; set; }
            public MainContentType MainType { get; set; }
            public bool IsMaxLevel { get; set; }
            public bool HasDuplicateParent { get; set; }
            public Dictionary<string,object> AdditionalProperties { get; set; }


        }

        public class ContentAreaItemViewModel
        {
            public string Name { get; set; }
            public List<ContentInspectorViewModel> ContentAreaItems { get; set; }
        }


        public class ContentReferenceViewModel
        {
            public string Name { get; set; }
            public ContentInspectorViewModel ContentReferenceItem { get; set; }
        }
        public class XhtmlStringViewModel
        {
            public XhtmlStringViewModel()
            {
                Fragments = new List<IXhtmlStringItem>();
            }

            public string Name { get; set; }
            public List<IXhtmlStringItem> Fragments { get; set; }
        }

        public class XhtmlStringContent : IXhtmlStringItem
        {
            public string Value { get; set; }
        }
    }

    public interface IXhtmlStringItem
    {

    }
}