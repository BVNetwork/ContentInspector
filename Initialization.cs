using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace BVNetwork.ContentAreaInspector
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class Initialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.MapRoute(
                "ContentAreaInspector",
                "ContentAreaInspector/{id}",
                new { controller = "ContentAreaInspector", action = "Index" }  
            );
        }

        public void Uninitialize(InitializationEngine context)
        {

        }
    }
}