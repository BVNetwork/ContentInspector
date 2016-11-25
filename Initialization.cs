using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace EPiCode.ContentInspector
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class Initialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.MapRoute(
                "ContentInspector",
                "ContentInspector/{id}",
                new { controller = "ContentInspector", action = "Index" }  
            );
        }

        public void Uninitialize(InitializationEngine context)
        {

        }
    }
}