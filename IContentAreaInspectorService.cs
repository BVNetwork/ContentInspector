using System.Collections.Generic;
using EPiServer.Core;

namespace BVNetwork.ContentAreaInspector
{
    public interface IContentAreaInspectorService
    {
        ContentAreaInspectorViewModel CreateModel(ContentReference contentReference, List<string> visitorGroupNames,
            string contentGroup,
            int level, List<ContentReference> parentIds);

        List<string> GetVisitorGroupNames(string internalFormat);
    }
}