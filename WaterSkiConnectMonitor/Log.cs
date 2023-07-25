using System;
using System.IO;
using System.Text;

namespace WaterSkiConnectMonitor {
	class Log {
		private static String myLogFileExtn = "-WscMonitor.log";
		private static String myLogFileName = "";
		private static object logWriteLock = new object();

		private Log() {
			// Load data rating values
		}

		public static bool OpenFile( String filename ) {
			try {
				String curPath = Environment.GetFolderPath( Environment.SpecialFolder.Desktop );
				myLogFileName = Path.Combine( curPath, filename + myLogFileExtn );
				if ( !( File.Exists( myLogFileName ) ) ) {
					FileStream fileStream = File.Create( myLogFileName );
					fileStream.Close();
					WriteFile( "Log file created" );
					Console.WriteLine( "Monitor will log messages to " + myLogFileName );
				}
				return true;

			} catch ( Exception ex ) {
				Console.WriteLine( String.Format( "{0} Exception encountered opening existing log file  {1} for extending: Error: {2}", DateTime.Now, filename, ex.Message ) );
				Console.WriteLine( "------------------------------------------------------------" );
				Console.WriteLine( "" );
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
				Console.WriteLine( String.Format( "{0} Exception encountered writing to log file  {1} for extending: Error: {2}", DateTime.Now, myLogFileName, ex.Message ) );
				Console.WriteLine( "------------------------------------------------------------" );
				Console.WriteLine( "" );
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
				Console.WriteLine( String.Format( "{0} Exception encountered writing to log file  {1} for extending: Error: {2}", DateTime.Now, myLogFileName, ex.Message ) );
				Console.WriteLine( "------------------------------------------------------------" );
				Console.WriteLine( "" );
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
