using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using Para.Server.Business;
using Para.Server.Contract;
using Para.Server.Host.Configuration.Helpers;

namespace Para.Server.Host.Configuration
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IParaService>().ImplementedBy<ParaService>()
                               .Interceptors<ExceptionInterceptor>().LifeStyle.Singleton
                               .AsWcfService(new DefaultServiceModel().AddEndpoints(WcfEndpoint.BoundTo(ConfigurationHelper.NetTcpBinding)
                                                                      .At(string.Format("{0}{1}", ConfigurationHelper.BaseAddress, "ParaService")))
                                                                      .PublishMetadata()));
        }
    }
}