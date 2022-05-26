using System;
using System.Windows.Forms;

using HttpMessageHandler.Common;
using HttpMessageHandler.Message;

namespace HttpMessageHandler {
	public static class Program {
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

				if ( args.Length != 3 ) {
					String msg = String.Format( "{0} Expecting 3 input arguments but received {0}", methodName, args.Length );
					Log.WriteFile( msg );
					MessageBox.Show( msg );
				}

				Properties.Settings.Default.SanctionNum = args[0];
				Properties.Settings.Default.DataDirectory = args[1];
				Properties.Settings.Default.DatabaseFilename = args[2];

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
