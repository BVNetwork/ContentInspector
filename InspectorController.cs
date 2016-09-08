using System.Collections.Generic;
using System.Security;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Shell;

namespace BVNetwork.ContentInspector
{
    [Authorize]
    public class ContentInspectorController : Controller
    {
        private readonly IContentInspectorService _ContentInspectorService;
        public ContentInspectorController(IContentInspectorService ContentInspectorService)
        {
            _ContentInspectorService = ContentInspectorService;
        }

        public ActionResult Index(string id)
        {
            
            if (!PrincipalInfo.HasEditAccess)
            {
                throw new SecurityException("Access denied");
            }
            var model = _ContentInspectorService.CreateModel(new ContentReference(id), null, null, 0, new List<ContentReference>());
            return View(Paths.ToResource(this.GetType(),
                "Views/ContentInspector/Index.ascx"), model);
        }
    }
}