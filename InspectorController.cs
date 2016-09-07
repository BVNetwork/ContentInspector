using System.Collections.Generic;
using System.Security;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Shell;

namespace BVNetwork.ContentAreaInspector
{
    [Authorize]
    public class ContentAreaInspectorController : Controller
    {
        private readonly IContentAreaInspectorService _contentAreaInspectorService;
        public ContentAreaInspectorController(IContentAreaInspectorService contentAreaInspectorService)
        {
            _contentAreaInspectorService = contentAreaInspectorService;
        }

        public ActionResult Index(int id)
        {
            if (!PrincipalInfo.HasEditAccess)
            {
                throw new SecurityException("Access denied");
            }
            var model = _contentAreaInspectorService.CreateModel(new ContentReference(id), null, null, 0, new List<ContentReference>());
            return View(Paths.ToResource(this.GetType(),
                "Views/ContentAreaInspector/Index.ascx"), model);
        }
    }
}