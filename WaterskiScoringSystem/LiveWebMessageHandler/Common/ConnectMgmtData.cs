﻿using System;
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
				String curMsg = String.Format( "{0}Unable to connect to database {1}"
					, curMethodName, Properties.Settings.Default.DatabaseConnectionString );
				MessageBox.Show( curMsg );
				return false;
			}
			
			tourRow = HelperFunctions.getTourData();
			if ( tourRow == null ) {
				String curMsg = String.Format( "{0}Tournament data for {1} was not found"
					, curMethodName, mySanctionNum );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
				return false;
			}
			Properties.Settings.Default.DataDirectory = (String)tourRow["TourDataLoc"];
			if ( !( Log.OpenFile( mySanctionNum ) ) ) return false;

			ExportLiveWeb.exportTourData( mySanctionNum );

			Log.WriteFile( String.Format( "{0}Connected to database {1}"
				, curMethodName, Properties.Settings.Default.DatabaseConnectionString ) );

			return true;
		}

	}
}
