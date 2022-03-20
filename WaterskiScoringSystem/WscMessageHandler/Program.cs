using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using WscMessageHandler.Common;

using WscMessageHandler.Message;

namespace WscMessageHandler {
	public static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// [STAThread]
		public static void Main( String[] args ) {
			try {
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );

				if ( args.Length >= 3 ) {
					Properties.Settings.Default.SanctionNum = args[0];
					Properties.Settings.Default.DatabaseFilename = args[1];
					Properties.Settings.Default.DataDirectory = args[2];
					Properties.Settings.Default.DatabaseConnectionString = 
						String.Format( "Data Source = {0}; Password = waterski; Persist Security Info = True", Properties.Settings.Default.DatabaseFilename );

					Application.Run( new Controller() );

				} else {
					throw new Exception( "Program start requires 3 parameters (SanctionNumber, DatabaseFileUri, DataDirectory" );
				}

			} catch ( Exception excp ) {
				Log.WriteFile( "WscMessageHandler:Main: An exception has been enountered. Exception: " + excp.Message + ": StackTrace: " + excp.StackTrace );
				MessageBox.Show( "An exception has been enountered."
					+ "\n Exception: " + excp.Message
					+ "\n Please contact the development team and send the [SanctionId]-log.log file"
					+ "\n StackTrace: " + excp.StackTrace );
			}

		}
	}
}
