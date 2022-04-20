using System;
using System.Data;
using System.Windows.Forms;

using SocketIOClient;

namespace WscMessageHandler.Common {
	class ConnectMgmtData {
		private static readonly String myWscApplicationKey = "CAD2FB59-3CCB-4691-9D26-7D68C2222788";
		private static readonly String myWscWebLocationDefault = "http://ewscdata.com:40000/";
		
		private static String mySanctionNum = "";
		private static String myEventSubId = "";
		private static bool myUseJumpTimes = false;

		public static DataRow tourRow = null;
		public static SocketIO socketClient = null;

		public static String wscApplicationKey {
			get { return myWscApplicationKey; }
		}

		public static String wscWebLocationDefault {
			get { return myWscWebLocationDefault; }
		}

		public static String sanctionNum {
			get { return mySanctionNum; }
			set { mySanctionNum = value; }
		}

		public static String eventSubId {
			get { return myEventSubId; }
			set { myEventSubId = value; }
		}

		public static Boolean useJumpTimes {
			get { return myUseJumpTimes; }
			set { myUseJumpTimes = value; }
		}
		
		public static bool initConnectMgmtData() {
			String curMethodName = "ConnectMgmtData:initConnectMgmtData: ";

			mySanctionNum = Properties.Settings.Default.SanctionNum;
			Log.OpenFile( mySanctionNum );

			bool curDbOpen = DataAccess.DataAccessOpen();
			if ( !( curDbOpen ) ) {
				String curMsg = String.Format( "{0}Unable to connect to database {1}"
					, curMethodName, Properties.Settings.Default.DatabaseConnectionString );
				Log.WriteFile( curMsg );
				return false;
			}

			Log.WriteFile( String.Format( "{0}Connected to database {1}"
				, curMethodName, Properties.Settings.Default.DatabaseConnectionString ) );

			tourRow = HelperFunctions.getTourData();
			if ( tourRow == null ) {
				String curMsg = String.Format( "{0}Tournament data for {1} was not found"
					, curMethodName, mySanctionNum );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
				return false;
			}
			return true;
		}

	}
}
