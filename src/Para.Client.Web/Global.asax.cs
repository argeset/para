using System;
using System.Configuration;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

using Para.Server.Contract;

namespace Para.Client.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            HtmlHelper.ClientValidationEnabled =
            HtmlHelper.UnobtrusiveJavaScriptEnabled = false;
            MvcHandler.DisableMvcResponseHeader = true;

            GlobalFilters.Filters.Add(new HandleErrorAttribute());

            RegisterRoutes(RouteTable.Routes);

            PrepareIocContainer();
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("SadeceDeger", "Deger", new { controller = "Data", action = "SadeceDeger" }, new[] { "Para.Client.Web.Controllers" });
            routes.MapRoute("Cevrim", "Cevrim", new { controller = "Data", action = "Cevrim" }, new[] { "Para.Client.Web.Controllers" });
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Data", action = "Index", id = UrlParameter.Optional }, new[] { "Para.Client.Web.Controllers" });
        }

        private void PrepareIocContainer()
        {
            var container = new WindsorContainer();
            container.AddFacility<WcfFacility>();

            var netTcpBinding = new NetTcpBinding
            {
                PortSharingEnabled = true,
                Security = new NetTcpSecurity { Mode = SecurityMode.None },
                MaxBufferSize = 67108864,
                MaxReceivedMessageSize = 67108864,
                TransferMode = TransferMode.Streamed,
                ReceiveTimeout = new TimeSpan(0, 30, 0),
                SendTimeout = new TimeSpan(0, 30, 0)
            };

            container.Register(Component.For(typeof(IParaService))
                                        .AsWcfClient(new DefaultClientModel
                                        {
                                            Endpoint = WcfEndpoint.BoundTo(netTcpBinding)
                                                                  .At(string.Format("net.tcp://localhost:{0}/ParaService", ConfigurationManager.AppSettings["TcpPort"]))
                                        }).LifestylePerWebRequest());

            container.Register(Classes.FromThisAssembly()
                                      .BasedOn<IController>()
                                      .LifestyleTransient());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container.Kernel));
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("X-Powered-By");
            HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
            HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");

            HttpContext.Current.Response.Headers.Set("Server", "Para Server");
        }
    }
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            _kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }

            return (IController)_kernel.Resolve(controllerType);
        }
    }
}