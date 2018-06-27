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
            this.TVSServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.TVSServiceProcessInstaller.Password = null;
            this.TVSServiceProcessInstaller.Username = null;
            // 
            // TVSServiceInstaller
            // 
            this.TVSServiceInstaller.ServiceName = "TVSServiceInstaller Test";
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