using System;
using System.ServiceProcess;

using NLog;

using Para.Server.Host.Configuration;

namespace Para.Server.Host
{
    partial class ParaWindowsService : ServiceBase
    {
        public ParaWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var logger = LogManager.GetLogger(ServiceName);
            try
            {
                logger.Info("Trying starting");

                Bootstrapper.Initialize();
            }
            catch (Exception ex)
            {
                logger.Error("Error starting: ", ex);
                throw;
            }
            finally
            {
                logger.Info("Starting ended");
            }
        }

        protected override void OnStop()
        {
            var logger = LogManager.GetLogger(ServiceName);
            try
            {
                logger.Info("Trying stopping");

                var container = Bootstrapper.Container;
                if (container != null)
                {
                    container.Dispose();
                }
            }
            catch (Exception e)
            {
                logger.Error("Error stopping: ", e);
                throw;
            }
            finally
            {
                logger.Info("Stopping ended");
            }
        }
    }
}
