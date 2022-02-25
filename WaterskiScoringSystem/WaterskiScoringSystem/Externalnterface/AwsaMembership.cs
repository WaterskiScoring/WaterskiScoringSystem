using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Externalnterface {
	class AwsaMembership {
		/*
			API Endpoint	https://portal-prodapp.integrasssports.com
			clientid	c81e728d9d4c2f636f067f89cc14862c
			clientsecretkey	FOjLVhmRFW9t4jSeiRYy9nO48RZq30WzEzSpLwmM
		 */
		private static readonly String myApiEndpoint = "https://portal-prodapp.integrasssports.com";
		private static readonly String myApiCommand = "/api/member/verify";
		private static readonly String myAuthKeyClientid = "c81e728d9d4c2f636f067f89cc14862c";
		private static readonly String myAuthKeyClientSecretKey = "FOjLVhmRFW9t4jSeiRYy9nO48RZq30WzEzSpLwmM";

		private static Boolean myWarnMessageActive = true;
		private static StringBuilder myImportWarningMessages = new StringBuilder( "" );

		public static Boolean warnMessageActive {
			get => myWarnMessageActive;
			set {
				myWarnMessageActive = value;
				myImportWarningMessages = new StringBuilder( "" );
			}
		}

		public static void showBulkWarnMessage() {
			if ( myImportWarningMessages.Length == 0 ) return;
			
			ShowMessage showMessage = new ShowMessage();
			showMessage.Message = myImportWarningMessages.ToString();
			showMessage.ShowDialog();
		}

		public static Boolean validateIwwfMembership( String inSanctionId, String inMemberId, String inTourDate ) {
			Cursor.Current = Cursors.WaitCursor;
			DateTime curTourDate = new DateTime();
			try {
				curTourDate = Convert.ToDateTime( inTourDate );
			} catch ( Exception ex ) {
				Log.WriteFile( "A invalid tournament event end date detected: Exception: " + ex.Message );
			}

			try {
				Dictionary<string, object> respMsg = readMembershipEntry( inSanctionId, inMemberId );
				if ( respMsg == null ) {
					showErrorMsg( inSanctionId, inMemberId, "N/A", respMsg );
					return false;
				}

				return readRespMsg( respMsg );

			} finally {
				Cursor.Current = Cursors.Default;
			}
		}

		private static Dictionary<string, object> readMembershipEntry( String inSanctionId, String inMemberId ) {
			Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
					{ "username", inMemberId }
				};
			String jsonMsg = JsonConvert.SerializeObject( sendConnectionMsg );

			NameValueCollection curHeaderParams = new NameValueCollection();
			//curHeaderParams.Add( "Authorization", authApiKey + " " + authApiValue );
			curHeaderParams.Add( "clientId", myAuthKeyClientid );
			curHeaderParams.Add( "clientSecretKey", myAuthKeyClientSecretKey );

			return SendMessageHttp.getMessageDictionaryPostMessage( myApiEndpoint, curHeaderParams, "application/Json", jsonMsg, null, null );
		}

		private static Boolean readRespMsg( Dictionary<string, object> respMsg ) {
			Dictionary<string, object> athleteAttrList = HelperFunctions.getAttributeDictionary( respMsg, "Athlete" );

			Log.WriteFile( String.Format( "validateAwsaMembership:Skier not found {0} {1} {2}"
				, athleteAttrList["FirstName"], athleteAttrList["LastName"], athleteAttrList["MemberId"] ) );
			return false;
		}

		private static void showErrorMsg( String inSanctionId, String inMemberId, String inSkierName, Dictionary<string, object> respMsg ) {
			String msg = "";

			if ( respMsg == null ) {
				msg = String.Format( "Skier {0} ({1}) not found", inSkierName, inMemberId );
				Cursor.Current = Cursors.Default;
				messageHandler( msg );
				return;
			}
			
			Cursor.Current = Cursors.Default;
		}

		private static void messageHandler( String msg ) {
			if ( myWarnMessageActive ) {
				ShowMessage showMessage = new ShowMessage();
				showMessage.Message = msg;
				showMessage.ShowDialog();

			} else {
				myImportWarningMessages.Append( msg + System.Environment.NewLine + System.Environment.NewLine );
			}
		}

	}
}
