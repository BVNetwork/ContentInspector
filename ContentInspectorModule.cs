using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using EPiServer.DataAbstraction;
using EPiServer.Framework.TypeScanner;
using EPiServer.Framework.Web.Resources;
using EPiServer.Shell.Modules;

namespace BVNetwork.ContentInspector
{

    public class ContentInspectorModule : EPiServer.Shell.Modules.ShellModule
    {
        public ContentInspectorModule(string name, string routeBasePath, string resourceBasePath)
            : base(name, routeBasePath, resourceBasePath)
        {
        }

        public ContentInspectorModule(string name, string routeBasePath, string resourceBasePath,
            ITypeScannerLookup typeScannerLookup, VirtualPathProvider virtualPathProvider)
            : base(name, routeBasePath, resourceBasePath, typeScannerLookup, virtualPathProvider)
        {
        }

        public override ModuleViewModel CreateViewModel(ModuleTable moduleTable,
            IClientResourceService clientResourceService)
        {
            var contentInspectorViewModel = new ContentInspectorModelViewModel(this, clientResourceService);
            contentInspectorViewModel.rasterizeBaseUrl = ConfigurationManager.AppSettings["ContentInspector.RasterizeBaseUrl"];  // "/static/css/";
            return contentInspectorViewModel;
        }
    }



    public class ContentInspectorModelViewModel : ModuleViewModel
    {
        public ContentInspectorModelViewModel(ShellModule module, IClientResourceService clientResourceService)
            : base(module, clientResourceService)
        {
        }

        public string rasterizeBaseUrl { get; set; }
    }

}
