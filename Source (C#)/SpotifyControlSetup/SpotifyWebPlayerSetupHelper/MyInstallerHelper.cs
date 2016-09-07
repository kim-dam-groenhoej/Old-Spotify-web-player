using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpotifyWebPlayerSetupHelper
{
    [RunInstaller(true)]
    public partial class MyInstallerHelper : System.Configuration.Install.Installer
    {
        public MyInstallerHelper()
        {
            InitializeComponent();
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Install(IDictionary stateSaver)
        {

            base.Install(stateSaver);

        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            try
            {
                // run spotify web player console app
                FileInfo fileInfo = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //Take custom action data values
                string sProgram = "SpotifyConsoleControl.exe";
                sProgram = Path.Combine(fileInfo.DirectoryName, sProgram);
                Trace.WriteLine("open program= " + sProgram);

                Process.Start(sProgram);
            }
            catch (Exception exc)
            {
                Context.LogMessage(exc.ToString());
                throw;
            }
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        protected override void OnAfterRollback(IDictionary savedState)
        {
            base.OnAfterRollback(savedState);
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            base.OnAfterUninstall(savedState);
        }

        protected override void OnBeforeRollback(IDictionary savedState)
        {
            base.OnBeforeRollback(savedState);
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);
        }

        protected override void OnCommitted(IDictionary savedState)
        {
            base.OnCommitted(savedState);

            try
            {
                FileInfo fileInfo = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //Take custom action data values
                string sProgram = "SpotifyWebPlayerInstaller.msi";
                sProgram = Path.Combine(fileInfo.DirectoryName, sProgram);
                Trace.WriteLine("Install sProgram= " + sProgram);

                Process p = new Process();
                p.StartInfo.FileName = sProgram;
                p.Start();

            }
            catch (Exception exc)
            {
                Context.LogMessage(exc.ToString());
                throw;
            }
        }

        protected override void OnCommitting(IDictionary savedState)
        {
            base.OnCommitting(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
        }
    }
}
