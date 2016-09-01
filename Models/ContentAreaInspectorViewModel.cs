﻿using System.Collections.Generic;

namespace BVNetwork.ContentAreaInspector
{
    public class ContentAreaInspectorViewModel
    {
        public InspectorContentViewModel Content { get; set; }
        public List<string> VisitorGroupsNames { get; set; }
        public List<ContentAreaItemViewModel> ContentAreaItems { get; set; }

        public class InspectorContentViewModel
        {
            public string Name { get; set; }
            public string Status { get; set; }
            public string Type { get; set; }
            public string EditUrl { get; set; }
            public string PreviewUrl { get; set; }
            public MainContentType MainType { get; set; }

        }

        public class ContentAreaItemViewModel
        {
            public string Name { get; set; }
            public List<ContentAreaInspectorViewModel> ContentAreaItems { get; set; }
        }
    }


}