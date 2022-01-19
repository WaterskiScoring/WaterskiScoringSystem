using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {

            try {
				//Application.SetHighDpiMode( HighDpiMode.SystemAware );
				Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault( false );
                Application.SetUnhandledExceptionMode( UnhandledExceptionMode.ThrowException );
                Application.Run( new SystemMain() );

            } catch (Exception excp) {
				Log.WriteFile( "WaterskiScoringSystem:Main: An exception has been enountered. Exception: " + excp.Message + ": StackTrace: " + excp.StackTrace );
				MessageBox.Show( "An exception has been enountered."
					+ "\n Exception: " + excp.Message
					+ "\n Please contact the development team and send the [SanctionId]-log.log file" );
            }
        }
    }
}
