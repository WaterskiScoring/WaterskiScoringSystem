using System;
using System.Windows.Forms;

using BoatPathMonitorSim.Common;
using BoatPathMonitorSim.Message;

namespace BoatPathMonitorSim {
		public static class Program {
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
					Application.Run( new Controller() );

				} catch ( Exception excp ) {
					Log.WriteFile( "BoatPathMonitorSim:Program: An exception has been enountered. "
						+ "Exception: " + excp.Message 
						+ ": StackTrace: " + excp.StackTrace );
					MessageBox.Show( "BoatPathMonitorSim:Program: An exception has been enountered."
						+ "\n Exception: " + excp.Message
						+ "\n Please contact the development team and send the [SanctionId]-bpms-sim.log file"
						+ "\n StackTrace: " + excp.StackTrace );
				}
			}
		}
}
