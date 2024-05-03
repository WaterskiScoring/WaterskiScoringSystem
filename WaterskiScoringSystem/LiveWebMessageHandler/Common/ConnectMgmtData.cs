using System;
using System.Data;
using System.Windows.Forms;

using LiveWebMessageHandler.Externalnterface;

namespace LiveWebMessageHandler.Common {
	class ConnectMgmtData {
		private static String mySanctionNum = "";
		public static DataRow tourRow = null;

		public static String sanctionNum {
			get { return mySanctionNum; }
			set { mySanctionNum = value; }
		}

		public static bool isLiveWebConnected {
			get { return ( ExportLiveWeb.LiveWebScoreboardUri != null && ExportLiveWeb.LiveWebScoreboardUri.Length > 0 ); }
		}

		public static bool initConnectMgmtData() {
			String curMethodName = "ConnectMgmtData:initConnectMgmtData: ";

			mySanctionNum = Properties.Settings.Default.SanctionNum;
			bool curDbOpen = DataAccess.DataAccessOpen();
			if ( !( curDbOpen ) ) {
				ExportLiveWeb.LastErrorMsg = String.Format( "{0}Unable to connect to database {1}", curMethodName, Properties.Settings.Default.DatabaseConnectionString );
				return false;
			}
			
			tourRow = HelperFunctions.getTourData();
			if ( tourRow == null ) {
				ExportLiveWeb.LastErrorMsg = String.Format( "{0}Tournament data for {1} was not found", curMethodName, mySanctionNum );
				Log.WriteFile( ExportLiveWeb.LastErrorMsg );
				return false;
			}
			Properties.Settings.Default.DataDirectory = (String)tourRow["TourDataLoc"];
			if ( !( Log.OpenFile( mySanctionNum ) ) ) return false;

			ExportLiveWeb.exportTourData( mySanctionNum );
			if ( HelperFunctions.isObjectEmpty( ExportLiveWeb.LastErrorMsg ) ) {
				Log.WriteFile( String.Format( "{0}Connected to database {1}", curMethodName, Properties.Settings.Default.DatabaseConnectionString ) );
				return true;
			}

			return false;
		}

	}
}
