using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Externalnterface {
	class IwwfMembership {

		/*
		 * Endpoint for web service
		 * Staging - http://iwwfsc.qubiteq.com/staging/api/licenses/check
		 * Production - https://ems.iwwf.sport/api/licenses/check (after it is activated in production >> N/A yet)
		 * The web service can accept and return either xml or json format depending on the parameters passed
		 * 
		 * Retrieve member information with foreign identifier if not a USA federation skier
		 * https://www.usawaterski.org/admin/GetForeignMemberJson.asp?MemberId=700040630
		 * https://test.usawaterski.org/admin/GetForeignMemberJson.asp?MemberId=700040630
		 */
		private static String IwwfWebLocation = Properties.Settings.Default.UriIwwfEms + "/licenses/check";
		private static String GetForeignMember = Properties.Settings.Default.UriUsaWaterski + "/admin/GetForeignMemberJson.asp?MemberId=";

		private static String authApiKey = "IWWF";
		//private static String authApiValueProd = "uppdxfsblcgefnrprowjtwjjrnrhismf"; // Prod Obsolete as 8/24/2023
		//private static String authApiValueStage1 = "cf1vfhl587mtny2eaeri6wfujusrfrnb"; // Staging Obsolete as 8/24/2023
		private static String authApiValueProd = "57c539ad24fb434aa0093d8f26cef57e"; // Prod Active as of 8/24/2023
		//private static String authApiValueStage1 = "b85e309e661b4f17a69fe8b9377af7ff"; // Staging Active as of 8/24/2023
		private static String authApiValue = authApiValueProd;

		//private static Boolean showWarnMessage = true;
		//private static StringBuilder importWarningMessages = new StringBuilder( "" );
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

		public static Boolean validateIwwfMembership( String inSanctionId, String inEditCode, String inMemberId, String inTourDate ) {
			Cursor.Current = Cursors.WaitCursor;
			DateTime curTourDate = new DateTime();
			try {
				curTourDate = Convert.ToDateTime( inTourDate );
			} catch ( Exception ex ) {
				Log.WriteFile( "A invalid tournament event end date detected: Exception: " + ex.Message );
			}

			try {
				String curIwwfAthleteId;
				String curFedCode = "USA";
				DataRow curDataRow = getMemberTourReg( inSanctionId, inMemberId );
				if ( curDataRow == null ) {
					showNoLicenseMsg( inSanctionId, inMemberId, "N/A", null );
					return false;
				}

				curFedCode = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" ).ToUpper();
				if ( HelperFunctions.isObjectEmpty( curFedCode ) ) curFedCode = "USA";
				if ( curFedCode.Equals("USA" ) ) {
					curIwwfAthleteId = curFedCode + inMemberId;
					Dictionary<string, object> respMsg = readIwwfMembership( inSanctionId, inEditCode, curIwwfAthleteId, curTourDate );
					if ( respMsg == null ) {
						showNoLicenseMsg( inSanctionId, inMemberId, "N/A", respMsg );
						return false;
					}
					if ( readRespMsg( curIwwfAthleteId, respMsg ) ) return true;
					showNoLicenseMsg( inSanctionId, inMemberId, "N/A", respMsg );
					return false;
				} 

				return validateForeignMembership( curDataRow, inSanctionId, inEditCode, inMemberId, curTourDate );

			} catch ( Exception ex ) {
				String curMsg = String.Format( "Exception encountered attempting to validate IWWF license for AWSA MemberId {0}{1}{2}"
					, inMemberId, System.Environment.NewLine, ex.Message );
				Log.WriteFile( curMsg );
				Cursor.Current = Cursors.Default;
				messageHandler( curMsg );
				return false;

			} finally {
				Cursor.Current = Cursors.Default;
			}
		}

		private static Boolean validateForeignMembership( DataRow curTourRegRow, String inSanctionId, String inEditCode, String inMemberId, DateTime curTourDate ) {
			String curFedCode, curForeignFedID, curIWWFAthleteId;
			Dictionary<string, object> respMsg;

			// Check to see if the skiers foreign member number is already available in the tournament.
			// If it is then use that to validate the license
			curFedCode = HelperFunctions.getDataRowColValue( curTourRegRow, "Federation", "" ).ToUpper();
			curForeignFedID = HelperFunctions.getDataRowColValue( curTourRegRow, "ForeignFederationID", "" );

			// Check to see if the foreign federation number also contains the federation code as the first 3 charaters
			// If it does use it otherwise concatenate the federation and the foreign member number
			if ( HelperFunctions.isObjectPopulated( curFedCode ) && HelperFunctions.isObjectPopulated( curForeignFedID ) ) {
				if ( curForeignFedID.Length > curFedCode.Length && curForeignFedID.Substring( 0, curFedCode.Length ).ToUpper().Equals( curFedCode ) ) {
					curIWWFAthleteId = curForeignFedID;
				} else {
					curIWWFAthleteId = curFedCode + curForeignFedID;
				}
				respMsg = readIwwfMembership( inSanctionId, inEditCode, curIWWFAthleteId, curTourDate );
				if ( readRespMsg( curIWWFAthleteId, respMsg ) ) return true;
			}

			// Foreign member number is not in the current tournament so accessing AWSA to see if there is more current information to use for the license validation
			NameValueCollection curHeaderParams = new NameValueCollection();
			String requstUrl = GetForeignMember + inMemberId;
			DataTable curDataTable = SendMessageHttp.getMessageResponseDataTable( requstUrl, curHeaderParams, "application/Json", inSanctionId, inEditCode, false );
			if ( curDataTable == null ) {
				MessageBox.Show( String.Format("License validation failure because there was an error attempting to retrieve AWSA member data for {0} ({1})"
					, HelperFunctions.getDataRowColValue( curTourRegRow, "SkierName", "" ), inMemberId ) );
				return false;
			}
			if ( curDataTable.Rows.Count == 0 ) {
				showNoLicenseMsg( inSanctionId, inMemberId, inMemberId, null );
				return false;
			}

			DataRow curRespDataRow = curDataTable.Rows[0];
			curFedCode = HelperFunctions.getDataRowColValue( curRespDataRow, "FederationCode", "" ).ToUpper();
			if ( curFedCode.Equals( "USA" ) ) {
				showNoLicenseMsg( inSanctionId, inMemberId, "N/A", null );
				return false;
			}
			String foreignIDStatus = HelperFunctions.getDataRowColValue( curRespDataRow, "ForeignIDStatus", "" );
			if ( !( foreignIDStatus.Equals( "Available" ) ) ) {
				showNoLicenseMsg( inSanctionId, inMemberId, "N/A", null );
				return false;
			}
			// Check to see if the foreign federation number also contains the federation code as the first 3 charaters
			// If it does use it otherwise concatenate the federation and the foreign member number
			curForeignFedID = HelperFunctions.getDataRowColValue( curRespDataRow, "ForeignFederationID", "" );
			if ( HelperFunctions.isObjectEmpty( curForeignFedID) ) {
				showNoLicenseMsg( inSanctionId, inMemberId, curForeignFedID, null );
				return false;
			}
			if ( curForeignFedID.Length > curFedCode.Length && curForeignFedID.Substring( 0, curFedCode.Length ).ToUpper().Equals( curFedCode ) ) {
				curIWWFAthleteId = curForeignFedID;
			} else {
				curIWWFAthleteId = curFedCode + curForeignFedID;
			}
			respMsg = readIwwfMembership( inSanctionId, inEditCode, curIWWFAthleteId, curTourDate );
			if ( respMsg == null ) {
				showNoLicenseMsg( inSanctionId, inMemberId, curIWWFAthleteId, respMsg );
				return false;
			}

			if ( readRespMsg( curIWWFAthleteId, respMsg ) ) return true;
			showNoLicenseMsg( inSanctionId, inMemberId, curIWWFAthleteId, respMsg );
			return false;
		}

		private static Dictionary<string, object> readIwwfMembership( String inSanctionId, String inEditCode, String inIWWFAthleteId, DateTime curTourDate ) {
			Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
					{ "IWWFAthleteId", inIWWFAthleteId }
					, { "RefDate", curTourDate.ToString("yyyy-MM-dd") }
				};
			String jsonMsg = JsonConvert.SerializeObject( sendConnectionMsg );

			NameValueCollection curHeaderParams = new NameValueCollection();
			curHeaderParams.Add( "Authorization", authApiKey + " " + authApiValue );

			return SendMessageHttp.getMessageDictionaryPostMessage( IwwfWebLocation, curHeaderParams, "application/Json", jsonMsg, null, null );
		}

		private static void showNoLicenseMsg( String inSanctionId, String inMemberIdAwsa, String inMemberIdForeign, Dictionary<string, object> respMsg ) {
			String msg = "";

			if ( respMsg == null ) {
				DataRow curDataRow = getMemberTourReg( inSanctionId, inMemberIdAwsa );
				msg = String.Format( "Skier {0} with AWSA MemberId {1} (Foreign MemberId {2})"
					+ "{3}doesn't have an active IWWF license therefore not permitted to ski in class L/R"
					+ "{4}Event class changed to E"
					, (String)curDataRow["SkierName"], inMemberIdAwsa, inMemberIdForeign, System.Environment.NewLine, System.Environment.NewLine );
				Cursor.Current = Cursors.Default;
				messageHandler( msg );
				return;
			}

			Dictionary<string, object> athleteAttrList = HelperFunctions.getAttributeDictionary( respMsg, "Athlete" );
			if ( athleteAttrList == null ) {
				DataRow curDataRow = getMemberTourReg( inSanctionId, inMemberIdAwsa );
				msg = String.Format( "Skier {0} with AWSA MemberId {1} (Foreign MemberId {2})"
					+ "{3}doesn't have an active IWWF license therefore not permitted to ski in class L/R"
					+ "{4}Event class changed to E"
					, (String)curDataRow["SkierName"], inMemberIdAwsa, inMemberIdForeign, System.Environment.NewLine, System.Environment.NewLine );
				Log.WriteFile( msg );
				Cursor.Current = Cursors.Default;
				messageHandler( msg );
				return;
			}

			String iwwfLicensePurchaseLink =  HelperFunctions.getAttributeValue( respMsg, "PurchaseLink" );
			ArrayList licenseList =  HelperFunctions.getAttributeList( respMsg, "Licenses" );
			if ( licenseList == null || licenseList.Count == 0 ) {
				msg = String.Format( "Skier {0} with AWSA MemberId {1} (Foreign MemberId {2})"
					+ "{3}doesn't have an active IWWF license therefore not permitted to ski in class L/R"
					+ "{4}Event class changed to E"
					+ "{5}Skier can purchase a license at"
					+ "{6}{7}"
					, athleteAttrList["LastName"] + ", " + athleteAttrList["FirstName"], inMemberIdAwsa, inMemberIdForeign, System.Environment.NewLine, System.Environment.NewLine
					, System.Environment.NewLine, System.Environment.NewLine, iwwfLicensePurchaseLink );
				Log.WriteFile( msg );
				Cursor.Current = Cursors.Default;
				messageHandler( msg );
				return;
			}

			msg = String.Format( "Skier {0} with AWSA MemberId {1} (Foreign MemberId {2})"
				+ "{3}doesn't have an active IWWF license therefore not permitted to ski in class L/R"
				+ "{4}Event class changed to E"
				+ "{5}Skier can purchase a license at"
				+ "{6}{7}"
				, athleteAttrList["LastName"] + ", " + athleteAttrList["FirstName"], inMemberIdAwsa, inMemberIdForeign, System.Environment.NewLine, System.Environment.NewLine
				, System.Environment.NewLine, System.Environment.NewLine, iwwfLicensePurchaseLink );
			Log.WriteFile( msg  );
			Cursor.Current = Cursors.Default;
			messageHandler( msg );
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

		private static Boolean readRespMsg( String inIWWFAthleteId, Dictionary<string, object> respMsg ) {
			Dictionary<string, object> athleteAttrList =  HelperFunctions.getAttributeDictionary( respMsg, "Athlete" );
			if ( athleteAttrList == null ) {
				String respFailedMsg =  HelperFunctions.getAttributeValue( respMsg, "Message" );
				Dictionary<string, object> respMsgModelList =  HelperFunctions.getAttributeDictionary( respMsg, "ModelState" );
				ArrayList respFailedList = HelperFunctions.getAttributeList( respMsgModelList, "model.IWWFAthleteId" );

				String respFailedMsg2 = "";
				if ( respFailedList == null ) {
					Log.WriteFile( String.Format("validateIwwfMembership:{0}:Request Failed", inIWWFAthleteId) );
				
				} else {
					if ( respFailedList.Count > 0 ) respFailedMsg2 = (String)respFailedList[0];
					Log.WriteFile( String.Format( "validateIwwfMembership:{0}:Request Failed: {1} {2}", inIWWFAthleteId, respFailedMsg, respFailedMsg2 ) );
				}

				Cursor.Current = Cursors.Default;
				return false;
			}

			String iwwfLicensePurchaseLink =  HelperFunctions.getAttributeValue( respMsg, "PurchaseLink" );
			ArrayList licenseList =  HelperFunctions.getAttributeList( respMsg, "Licenses" );
			if ( licenseList == null || licenseList.Count == 0 ) {
				String bulkLicenseAvailable =  HelperFunctions.getAttributeValue( respMsg, "FedBulkAgreement" );
				if ( bulkLicenseAvailable.ToLower().Equals( "true" ) ) return true;

				Log.WriteFile( String.Format( "validateIwwfMembership:Skier found but no IWWF license found for skier {0} {1} {2}" +
					"\nSkier can purchase a license at the following address {3}"
					, athleteAttrList["FirstName"], athleteAttrList["LastName"], athleteAttrList["IWWFAthleteId"], iwwfLicensePurchaseLink ) );

				Cursor.Current = Cursors.Default;
				return false;

			} else {
				foreach ( Dictionary<string, object> licenseEntry in licenseList ) {
					Boolean licenseAvailable = readLicenseEntry( licenseEntry );
					if ( licenseAvailable ) return true;
				}
			}
			
			Log.WriteFile( String.Format( "validateIwwfMembership:Skier found but no IWWF license found for skier {0} {1} {2}" +
				"\nSkier can purchase a license at the following address {3}"
				, athleteAttrList["FirstName"], athleteAttrList["LastName"], athleteAttrList["IWWFAthleteId"], iwwfLicensePurchaseLink ) );
			return false;
		}
		
		private static Boolean readLicenseEntry( Dictionary<string, object> licenseEntry ) {
			String licenseType =  HelperFunctions.getAttributeValue( licenseEntry, "LicenseDescription" );
			String licenseAvailable =  HelperFunctions.getAttributeValue( licenseEntry, "Available" );
			String licenseEvent =  HelperFunctions.getAttributeValue( licenseEntry, "UsedInCompetition" );

			if ( licenseAvailable.Equals( "True" ) ) {
				Cursor.Current = Cursors.Default;
				return true;
			}

			/*
			if ( licenseType.Equals( "Single Competition" ) && licenseEvent.Equals( Properties.Settings.Default.AppSanctionNum ) ) {
				Cursor.Current = Cursors.Default;
				return true;
			}
			 */

			return false;
		}

		private static DataRow getMemberTourReg( String inSanctionId, String inMemberId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct MemberId, SkierName, Federation, ForeignFederationID " );
			curSqlStmt.Append( "FROM TourReg " );
			curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' AND MemberId = '" + inMemberId + "'" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return curDataTable.Rows[0];
			} else {
				return null;
			}
		}
	}
}
