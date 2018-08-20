namespace TVSService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TVSServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.TVSServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // TVSServiceProcessInstaller
            // 
            this.TVSServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.TVSServiceProcessInstaller.Password = null;
            this.TVSServiceProcessInstaller.Username = null;
            // 
            // TVSServiceInstaller
            // 
            this.TVSServiceInstaller.Description = "TVS Service owned by RingRing";
            this.TVSServiceInstaller.DisplayName = "TVS Service";
            this.TVSServiceInstaller.ServiceName = "TVS Service";
            this.TVSServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.TVSServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.TVSServiceInstaller_AfterInstall);
            this.TVSServiceInstaller.BeforeUninstall += new System.Configuration.Install.InstallEventHandler(this.TVSServiceInstaller_BeforeUninstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.TVSServiceProcessInstaller,
            this.TVSServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller TVSServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller TVSServiceInstaller;
    }
}