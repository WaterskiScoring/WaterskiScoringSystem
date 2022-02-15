using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BoatPathMonitorSim.Common {

	class Log {
		private static String myLogFileExtn = "-bpms-sim.log";
		private static String myLogFileName = "";
		private static object logWriteLock = new object();

		private Log() {
			// Load data rating values
		}

		public static bool OpenFile( String filename ) {
			String curPath = Properties.Settings.Default.DataDirectory;
			if ( curPath.EndsWith( "\\" ) ) {
				myLogFileName = curPath + filename + myLogFileExtn;

			} else {
				myLogFileName = curPath + "\\" + filename + myLogFileExtn;
			}
			try {
				if ( !( File.Exists( myLogFileName ) ) ) {
					FileStream fileStream = File.Create( myLogFileName );
					fileStream.Close();
					WriteFile( "Log file created" );
				}
				return true;

			} catch ( Exception ex ) {
				MessageBox.Show( "Exception encountered opening existing log file for extending: " + "\n\nError: " + ex.Message );
				return false;
			}
		}

		public static void WriteFile( String[] inLogRecords ) {
			try {
				if ( myLogFileName == null || myLogFileName.Length == 0 ) return;
				lock ( logWriteLock ) {
					using ( var fileStream = new FileStream( myLogFileName, FileMode.Append ) ) {
						String dateString = DateTime.Now.ToString( "MMM dd yyyy hh:mm:ss tt" );
						foreach ( String curLogRecord in inLogRecords ) {
							Byte[] writeData = Encoding.UTF8.GetBytes( Environment.NewLine + dateString + ": Message: " + curLogRecord );
							fileStream.Write( writeData, 0, writeData.Length );
						}
					}
				}

			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "Exception encountered writing to log file {0}\n\nException: {1} ", myLogFileName, ex.Message ) );
			}
		}

		public static void WriteFile( String inLogRecord ) {
			try {
				if ( myLogFileName == null || myLogFileName.Length == 0 ) return;
				lock ( logWriteLock ) {
					using ( var fileStream = new FileStream( myLogFileName, FileMode.Append ) ) {
						String dateString = DateTime.Now.ToString( "MMM dd yyyy hh:mm:ss tt" );
						Byte[] writeData = Encoding.UTF8.GetBytes( Environment.NewLine + dateString + ": Message: " + inLogRecord );
						fileStream.Write( writeData, 0, writeData.Length );
					}
				}

			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "Exception encountered writing to log file {0}\n\nException: {1} ", myLogFileName, ex.Message ) );
			}
		}

		public static Boolean isDirectoryValid( String inLocName ) {
			try {
				if ( Directory.Exists( inLocName ) ) return true;
				return false;

			} catch {
				return false;
			}
		}

	}
}
