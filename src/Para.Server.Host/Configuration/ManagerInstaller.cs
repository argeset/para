using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using Para.Server.Business.Manager;

namespace Para.Server.Host.Configuration
{
    public class ManagerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IParaManager>().ImplementedBy<ParaManager>());
        }
    }
}