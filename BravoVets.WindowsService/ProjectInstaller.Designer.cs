namespace BravoVets.WindowsService
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
            this.PublishQueueServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.PublishQueueServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // PublishQueueServiceProcessInstaller
            // 
            this.PublishQueueServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.PublishQueueServiceProcessInstaller.Password = null;
            this.PublishQueueServiceProcessInstaller.Username = null;
            this.PublishQueueServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller1_AfterInstall);
            // 
            // PublishQueueServiceInstaller
            // 
            this.PublishQueueServiceInstaller.Description = "Sends Social Data queued from the Social Calendar of the BV system";
            this.PublishQueueServiceInstaller.DisplayName = "BravoVets Publish Queue";
            this.PublishQueueServiceInstaller.ServiceName = "PublishQueue";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.PublishQueueServiceProcessInstaller,
            this.PublishQueueServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller PublishQueueServiceInstaller;
        internal System.ServiceProcess.ServiceProcessInstaller PublishQueueServiceProcessInstaller;
    }
}