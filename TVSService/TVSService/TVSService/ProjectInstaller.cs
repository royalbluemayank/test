using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace TVSService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void TVSServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            Constants.Log("Service Installed", "@" + DateTime.Now.ToString());
            new ServiceController(TVSServiceInstaller.ServiceName).Start();
        }

        int i = 0;
        private void TVSServiceInstaller_BeforeUninstall(object sender, InstallEventArgs e)
        {
            ServiceController controller = new ServiceController(TVSServiceInstaller.ServiceName);
            do
            {
                i++;
                try
                {
                    Constants.Log("Service Uninstalled Attempt" + i, "@" + DateTime.Now.ToString());
                    if (controller.Status == ServiceControllerStatus.Running | controller.Status == ServiceControllerStatus.Paused)
                    {
                        controller.Stop();
                    }
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 15));
                    controller.Close();
                    i = 5;
                }
                catch (Exception ex)
                {
                    Constants.Log("Service Uninstalled Error", "@" + DateTime.Now.ToString() + " Service failed to stop");
                }
            } while (i < 3);
            Constants.Log("Service Uninstalled", "@" + DateTime.Now.ToString());
        }
    }
}
