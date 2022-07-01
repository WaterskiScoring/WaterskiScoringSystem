using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

using LiveWebMessageHandler.Message;

namespace LiveWebMessageHandler {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( string[] args ) {
			String methodName = "LiveWebMessageHandler: Program: ";

			try {
				//Application.SetHighDpiMode( HighDpiMode.SystemAware );
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );
				Application.SetUnhandledExceptionMode( UnhandledExceptionMode.ThrowException );

				if ( args.Length != 2 ) {
					int curIdx = 1;
					StringBuilder curMsgArgs = new StringBuilder( "Input Arguments: " );
					foreach(String curArg in args ) {
						curMsgArgs.Append( String.Format( "\nArg{0}={1}", curIdx, curArg ) );
						curIdx++;
					}
					String msg = String.Format( "Error: {0} Expecting 2 input arguments but received {1}\n\n{2}"
						, methodName, args.Length, curMsgArgs.ToString() );
					MessageBox.Show( msg );
					return;
				}

				Properties.Settings.Default.SanctionNum = args[0];
				Properties.Settings.Default.DatabaseFilename = args[1];
				
				if ( Properties.Settings.Default.DatabaseFilename.Length <= 0 
					&& File.Exists( Properties.Settings.Default.DatabaseFilename ) 
					) {
					String msg = String.Format( "{0} Database file not provided or not found: File={1}"
						, methodName, Properties.Settings.Default.DatabaseFilename );
					MessageBox.Show( msg );
				}

				Properties.Settings.Default.DatabaseConnectionString =
					String.Format( "Data Source = {0}; Password = waterski; Persist Security Info = True"
					, Properties.Settings.Default.DatabaseFilename );
				Application.Run( new Controller() );

			} catch ( Exception ex ) {
				String msg = String.Format( "{0} An exception has been enountered. Exception: {1} \n\n{2}"
					, methodName, ex.Message, ex.StackTrace );
				MessageBox.Show( msg );
				return;
			}
		}
	}
}
