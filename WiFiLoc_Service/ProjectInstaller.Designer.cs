namespace WiFiLoc_Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Liberare le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.WiFiServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.WiFiServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // WiFiServiceProcessInstaller
            // 
            this.WiFiServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.WiFiServiceProcessInstaller.Password = null;
            this.WiFiServiceProcessInstaller.Username = null;
            // 
            // WiFiServiceInstaller
            // 
            this.WiFiServiceInstaller.ServiceName = "WiFiLoc_Service";
            this.WiFiServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.WiFiServiceProcessInstaller,
            this.WiFiServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller WiFiServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller WiFiServiceInstaller;
    }
}