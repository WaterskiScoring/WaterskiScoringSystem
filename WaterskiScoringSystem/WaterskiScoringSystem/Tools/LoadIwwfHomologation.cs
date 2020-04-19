using System;
using System.Collections.Generic;
using System.Data;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WaterskiScoringSystem.Tools {
    class LoadIwwfHomologation {
        private String mySanctionNum;

        public LoadIwwfHomologation() {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
        }

        public bool LoadFile() {
            bool curReturn = false;
            int delimPos = 0;
            String curTourDirectory = "";
            String curFileName = "IwwfHomologationDossier.txt";
            String curDestFileName = mySanctionNum + "HD.TXT";

            try {
                String curDeploymentDirectory = "";
                try {
                    //curDeploymentDirectory = ApplicationDeployment.CurrentDeployment.DataDirectory;
                    curDeploymentDirectory = Application.StartupPath;
                    if ( curDeploymentDirectory == null ) { curDeploymentDirectory = ""; }
                    if ( curDeploymentDirectory.Length < 1 ) {
                        curDeploymentDirectory = "C:\\Users\\AllenFamily\\Documents\\Visual Studio 2010\\Projects\\WaterskiScoringSystem\\WaterskiScoringSystem\\\bin\\Debug";
                    }
                } catch ( Exception ex ) {
                    if ( curDeploymentDirectory == null ) { curDeploymentDirectory = ""; }
                    if ( curDeploymentDirectory.Length < 1 ) {
                        curDeploymentDirectory = "C:\\Users\\AllenFamily\\Documents\\Visual Studio 2010\\Projects\\WaterskiScoringSystem\\WaterskiScoringSystem\\\bin\\Debug";
                    }
                }

                curTourDirectory = Properties.Settings.Default.ExportDirectory;
                if ( curTourDirectory == null ) { curTourDirectory = ""; }
                if ( curTourDirectory.Length > 1 ) {
                    if ( curTourDirectory.Substring( curTourDirectory.Length - 1 ) != "\\" ) {
                        curTourDirectory += "\\";
                    }
                    if (curDeploymentDirectory.Substring( curDeploymentDirectory.Length - 1 ) != "\\") {
                        curDeploymentDirectory += "\\";
                    }

                    //Declare and instantiate a new process component.
                    System.Diagnostics.Process curOSProcess = new System.Diagnostics.Process();

                    //Do not receive an event when the process exits.
                    curOSProcess.EnableRaisingEvents = true;

                    //The "/C" Tells Windows to Run The Command then Terminate 
                    string curCmdLine;
                    curCmdLine = "/C copy \"" + curDeploymentDirectory + curFileName + "\" \"" + curTourDirectory + curDestFileName + "\" \n";

                    System.Diagnostics.Process.Start( "CMD.exe", curCmdLine );
                    curOSProcess.Close();
                    MessageBox.Show( "File " + curDeploymentDirectory + curFileName + "\n copied to \n" + curTourDirectory + curDestFileName );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error loading IwwfHomologation file to tournament directory " + "\n\nError: " + ex.Message );
            }
            return curReturn;


            return true;
        }
    }
}
