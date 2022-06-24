using System;
using System.IO;
using System.Windows.Forms;

using LiveWebMessageHandler.Common;
using LiveWebMessageHandler.Message;


namespace LiveWebMessageHandler {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( string[] args ) {
			String methodName = "HttpMessageHandler:Program: ";

			try {
				//Application.SetHighDpiMode( HighDpiMode.SystemAware );
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );
				Application.SetUnhandledExceptionMode( UnhandledExceptionMode.ThrowException );

				if ( args.Length != 2 ) {
					String msg = String.Format( "Error: {0} Expecting 3 input arguments but received {0}", methodName, args.Length );
					Log.WriteFile( msg );
					MessageBox.Show( msg );
					return;
				}

				Properties.Settings.Default.SanctionNum = args[0];
				Properties.Settings.Default.DatabaseFilename = args[1];
				
				if ( Properties.Settings.Default.DatabaseFilename.Length <= 0 
					&& File.Exists( Properties.Settings.Default.DatabaseFilename ) 
					) {
					String msg = String.Format( "Error: {0} Database file not provided or not found: File={1}"
						, methodName, Properties.Settings.Default.DatabaseFilename );
					Log.WriteFile( msg );
					MessageBox.Show( msg );
				}

				Properties.Settings.Default.DatabaseConnectionString =
					String.Format( "Data Source = {0}; Password = waterski; Persist Security Info = True"
					, Properties.Settings.Default.DatabaseFilename );
				Application.Run( new Controller() );

			} catch ( Exception excp ) {
				Log.WriteFile( "HttpMessageHandler:Program: An exception has been enountered. Exception: "
					+ excp.Message + ": StackTrace: "
					+ excp.StackTrace );
				MessageBox.Show( "HttpMessageHandler:Program: An exception has been enountered."
					+ "\n Exception: " + excp.Message
					+ "\n Please contact the development team and send the [SanctionId]-HttpMessageHandler.log file"
					+ "\n StackTrace: " + excp.StackTrace );
			}
		}
	}
}
