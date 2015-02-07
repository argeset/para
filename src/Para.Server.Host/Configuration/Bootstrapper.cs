using Castle.Windsor;

using Para.Server.Contract;
using Para.Server.Contract.Argument;

namespace Para.Server.Host.Configuration
{
    public class Bootstrapper
    {
        public static IWindsorContainer Container { get; private set; }

        public static void Initialize()
        {
            Container = new WindsorContainer();
            Container.Install(new FacilitiesInstaller())
                     .Install(new InterceptorsInstaller())
                     .Install(new ManagerInstaller())
                     .Install(new ServiceInstaller());

            var paraService = Container.Resolve<IParaService>();
            paraService.SaveValue(); 
            paraService.StartTimer();
        }
    }
}