using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Para.Server.Host
{
    [RunInstaller(true)]
    public partial class EasyInstaller : Installer
    {
        public EasyInstaller()
        {
            InitializeComponent();

            var serviceProcess = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };

            const string serviceName = "Argeset SetCrm Para Service";
            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = serviceName.Replace(" ", ""),
                DisplayName = serviceName,
                Description = "This service manages the currency rates business",
                StartType = ServiceStartMode.Automatic
            };

            Installers.Add(serviceProcess);
            Installers.Add(serviceInstaller);
        }
    }
}
