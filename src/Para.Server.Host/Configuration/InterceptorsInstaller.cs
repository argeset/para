using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using Para.Server.Host.Configuration.Helpers;

namespace Para.Server.Host.Configuration
{
    public class InterceptorsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ExceptionInterceptor>());
        }
    }
}