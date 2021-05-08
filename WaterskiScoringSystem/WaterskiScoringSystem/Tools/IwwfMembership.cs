using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;

using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
	class IwwfMembership {

		/*
		 * Endpoint for web service
		 * Staging - http://iwwfsc.qubiteq.com/staging/api/licenses/check
		 * Production - https://ems.iwwf.sport/api/licenses/check (after it is activated in production >> N/A yet)
		 * The web service can accept and return either xml or json format depending on the parameters passed
		 * 
		 * POST method
		 * Authorization: IWWF cf1vfhl587mtny2eaeri6wfujusrfrnb
		 * 
		 * cf1vfhl587mtny2eaeri6wfujusrfrnb is an api key for testing purposes
		 * We will provide new api keys for use with the production database
		 * 
		 * http://iwwfsc.qubiteq.com/Staging/Orders
		 * http://iwwfsc.qubiteq.com/Staging/Orders/Process/01039103ab644c7ead8e61872c05902bjumtlm1rejo5cabf1f768vqsg3d1jzllkwwofze1fdg8y8k801gskkcxd4mti0ho9duq
		 * 
		 * Retrieve member information with foreign identifier if not a USA federation skier
		 * http://usawaterski.org/admin/GetForeignMemberJson.asp?MemberId=700040630
		 */

		private static String IwwfWebLocationStage = "http://iwwfsc.qubiteq.com/staging/api/licenses/check";
		private static String IwwfWebLocationProd = "https://ems.iwwf.sport/api/licenses/check";
		private static String GetForeignMember = "http://usawaterski.org/admin/GetForeignMemberJson.asp?MemberId=";
		private static String IwwfWebLocation = IwwfWebLocationProd;

		private static String authApiKey = "IWWF";
		private static String authApiValueProd = "uppdxfsblcgefnrprowjtwjjrnrhismf"; // Prod
		private static String authApiValueStage1 = "cf1vfhl587mtny2eaeri6wfujusrfrnb"; // Staging
		private static String authApiValueStage2 = "9g2yh2wb2hhs4vc4yzjb1n4tsibs2wfq"; // Staging V2
		private static String authApiValue = authApiValueProd;


		public static Boolean validateIwwfMembershipTmp( String inMemberId, String inTourDate ) {
			return true;
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
				Dictionary<string, object> respMsg = readIwwfMembership( inSanctionId, inEditCode, "USA" + inMemberId, curTourDate );
				if ( respMsg == null ) {
					showNoLicenseMsg( inSanctionId, inMemberId, respMsg );
					return false;
				}

				if ( readRespMsg( respMsg ) ) return true;

				DataRow curDataRow = getMemberTourReg( inSanctionId, inMemberId );
				if ( curDataRow == null || (curDataRow["Federation"] == System.DBNull.Value) || ( (String)curDataRow["Federation"] ).ToLower().Equals( "usa" ) ) {
					showNoLicenseMsg( inSanctionId, inMemberId, respMsg );
					return false;
				}

				return validateForeignMembership( inSanctionId, inEditCode, inMemberId, curTourDate );

			} finally {
				Cursor.Current = Cursors.Default;
			}
		}

		private static Boolean validateForeignMembership( String inSanctionId, String inEditCode, String inMemberId, DateTime curTourDate ) {
			NameValueCollection curHeaderParams = new NameValueCollection();
			String requstUrl = GetForeignMember + inMemberId;
			DataTable curDataTable = SendMessageHttp.getMessageResponseDataTable( requstUrl, curHeaderParams, "application/Json", inSanctionId, inEditCode, false );
			if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return false;
			
			DataRow curRow = curDataTable.Rows[0];
			String federationCode = (String)curRow["FederationCode"];
			if ( federationCode.Equals( "USA" ) ) {
				showNoLicenseMsg( inSanctionId, inMemberId, null );
				return false;
			}
			String foreignIDStatus = (String)curRow["ForeignIDStatus"];
			if ( !( foreignIDStatus.Equals( "Available" ) ) ) {
				showNoLicenseMsg( inSanctionId, inMemberId, null );
				return false;
			}
			String foreignFederationID = (String)curRow["ForeignFederationID"];

			Dictionary<string, object> respMsg = readIwwfMembership( inSanctionId, inEditCode, federationCode + foreignFederationID, curTourDate );
			if ( respMsg == null ) {
				showNoLicenseMsg( inSanctionId, inMemberId, respMsg );
				return false;
			}

			if ( readRespMsg( respMsg ) ) return true;
			showNoLicenseMsg( inSanctionId, inMemberId, respMsg );
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

		private static void showNoLicenseMsg( String inSanctionId, String inMemberId, Dictionary<string, object> respMsg ) {
			String msg = "";
			String newLine = "\r\n";
			ShowMessage showMessage = new ShowMessage();

			if ( respMsg == null ) {
				DataRow curDataRow = getMemberTourReg( inSanctionId, inMemberId );
				showMessage.Message = (String)curDataRow["SkierName"] + " doesn't have an active IWWF license therefore not permitted to ski in class L/R.  Event class changed to E";
				showMessage.ShowDialog();
				return;
			}

			Dictionary<string, object> athleteAttrList = getAttributeDictionary( respMsg, "Athlete" );
			if ( athleteAttrList == null ) {
				DataRow curDataRow = getMemberTourReg( inSanctionId, inMemberId );
				msg = String.Format( "{0} doesn't have an IWWF license available "
					+ "{1}Skier not permitted to ski in class L/R, event class changed to E"
					, curDataRow["SkierName"], newLine );
				Log.WriteFile( msg );
				Cursor.Current = Cursors.Default;
				showMessage.Message = msg;
				showMessage.ShowDialog();
				return;
			}

			String iwwfLicensePurchaseLink = getAttributeValue( respMsg, "PurchaseLink" );
			ArrayList licenseList = getAttributeList( respMsg, "Licenses" );
			if ( licenseList == null || licenseList.Count == 0 ) {
				msg = String.Format( "{0} {1} {2} found but IWWF license not available "
					+ "{3}Skier not permitted to ski in class L/R, event class changed to E"
					+ "{4}Skier can purchase a license at"
					+ "{5}{6}"
					, athleteAttrList["FirstName"], athleteAttrList["LastName"], athleteAttrList["IWWFAthleteId"], newLine, newLine, newLine, iwwfLicensePurchaseLink );
				Log.WriteFile( msg );
				Cursor.Current = Cursors.Default;
				showMessage.Message = msg;
				showMessage.ShowDialog();
				return;
			}

			msg = String.Format( "{0} {1} {2} found but IWWF license not available"
				+ "{3}Skier not permitted to ski in class L/R, event class changed to E"
				+ "{4}Skier can purchase a license at {5}{6}"
				, athleteAttrList["FirstName"], athleteAttrList["LastName"], athleteAttrList["IWWFAthleteId"]
				, newLine, newLine, newLine, iwwfLicensePurchaseLink );
			Log.WriteFile( msg  );
			Cursor.Current = Cursors.Default;
			showMessage.Message = msg;
			showMessage.ShowDialog();
		}

		private static Boolean readRespMsg( Dictionary<string, object> respMsg ) {
			Dictionary<string, object> athleteAttrList = getAttributeDictionary( respMsg, "Athlete" );
			if ( athleteAttrList == null ) {
				String respFailedMsg = getAttributeValue( respMsg, "Message" );
				Dictionary<string, object> respMsgModelList = getAttributeDictionary( respMsg, "ModelState" );
				ArrayList respFailedList = getAttributeList( respMsgModelList, "model.IWWFAthleteId" );

				String respFailedMsg2 = "";
				if ( respFailedList.Count > 0 ) respFailedMsg2 = (String)respFailedList[0];
				Log.WriteFile( String.Format( "validateIwwfMembership:Request Failed: {0} {1}", respFailedMsg, respFailedMsg2 ) );

				Cursor.Current = Cursors.Default;
				return false;
			}

			String iwwfLicensePurchaseLink = getAttributeValue( respMsg, "PurchaseLink" );
			ArrayList licenseList = getAttributeList( respMsg, "Licenses" );
			if ( licenseList == null || licenseList.Count == 0 ) {
				String bulkLicenseAvailable = getAttributeValue( respMsg, "FedBulkAgreement" );
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
			String licenseType = getAttributeValue( licenseEntry, "LicenseDescription" );
			String licenseAvailable = getAttributeValue( licenseEntry, "Available" );
			String licenseEvent = getAttributeValue( licenseEntry, "UsedInCompetition" );

			if ( licenseAvailable.Equals( "True" ) ) {
				Cursor.Current = Cursors.Default;
				return true;
			}

			if ( licenseType.Equals( "Single Competition" ) && licenseEvent.Equals( Properties.Settings.Default.AppSanctionNum ) ) {
				Cursor.Current = Cursors.Default;
				return true;
			}

			return false;
		}

		private static String getAttributeValue( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return "";

			if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Int32" ) ) {
				return ( (int)msgAttributeList[keyName] ).ToString();

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Decimal" ) ) {
				return ( (decimal)msgAttributeList[keyName] ).ToString();

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.String" ) ) {
				return ( (String)msgAttributeList[keyName] );
			
			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Boolean" ) ) {
				return ((Boolean)msgAttributeList[keyName] ).ToString();
			}

			return "";
		}

		private static Dictionary<string, object> getAttributeDictionary( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return null;
			return (Dictionary<string, object>)msgAttributeList[keyName];
		}

		private static ArrayList getAttributeList( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return null;
			return (ArrayList)msgAttributeList[keyName];
		}
		private static DataRow getMemberTourReg( String inSanctionId, String inMemberId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct TR.MemberId, TR.SkierName, TR.Federation " );
			curSqlStmt.Append( "FROM TourReg TR " );
			curSqlStmt.Append( "WHERE TR.SanctionId = '" + inSanctionId + "' AND TR.MemberId = '" + inMemberId + "'" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return curDataTable.Rows[0];
			} else {
				return null;
			}
		}
	}
}
