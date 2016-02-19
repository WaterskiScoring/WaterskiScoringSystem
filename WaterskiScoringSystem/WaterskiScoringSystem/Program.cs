using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WaterskiScoringSystem
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new SystemMain() );
             */

            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault( false );
                Application.SetUnhandledExceptionMode( UnhandledExceptionMode.ThrowException );
                Application.Run( new SystemMain() );
            } catch (Exception excp) {
                //Not sure how or why it gets here but adding a try catch to ensure exception is handled.
                MessageBox.Show( "An exception has been enountered."
                + "\n If error is persistent and you can identify your most recent activities, please submit issue to development team. \n\n" + excp.Message );
            }
        }
    }
}
