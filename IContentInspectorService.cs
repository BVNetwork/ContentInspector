using System.Collections.Generic;
using System.Reflection;
using EPiCode.ContentInspector.Models;
using EPiServer.Core;
using EPiServer.DataAbstraction;

namespace EPiCode.ContentInspector
{
    public interface IContentInspectorService
    {
        ContentInspectorViewModel CreateModel(ContentReference contentReference, List<string> visitorGroupNames,string contentGroup,int level, List<ContentReference> parentIds);
        List<string> GetVisitorGroupNames(string internalFormat);
        ContentInspectorViewModel.InspectorContentViewModel CreateInspectorContentViewModel(IContent content);
        List<PropertyInfo> GetInspectableProperties(ContentType contentType);
        List<ContentInspectorViewModel.ContentAreaItemViewModel> GetContentAreaItems(int level,List<ContentReference> parentIds, ContentType contentType, IContent content);
    }
}