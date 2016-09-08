using System.Collections.Generic;
using BVNetwork.ContentInspector.Models;
using EPiServer.Core;

namespace BVNetwork.ContentInspector
{
    public interface IContentInspectorService
    {
        ContentInspectorViewModel CreateModel(ContentReference contentReference, List<string> visitorGroupNames,
            string contentGroup,
            int level, List<ContentReference> parentIds);

        List<string> GetVisitorGroupNames(string internalFormat);
    }
}