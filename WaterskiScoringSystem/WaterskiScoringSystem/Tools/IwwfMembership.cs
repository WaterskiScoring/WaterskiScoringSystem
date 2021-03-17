using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;

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
		 */

		private static String IwwfWebLocationStage = "http://iwwfsc.qubiteq.com/staging/api/licenses/check";
		private static String IwwfWebLocationProd = " https://ems.iwwf.sport/api/licenses/check";
		private static String IwwfWebLocation = IwwfWebLocationStage;

		private static String authApiKey = "IWWF";
		private static String authApiValue = "cf1vfhl587mtny2eaeri6wfujusrfrnb";

		public static Boolean validateIwwfMembership( String inMemberId, String inTourDate ) {
			return true;
		}

		public static Boolean validateIwwfMembershipBak( String inMemberId, String inTourDate ) {
			Cursor.Current = Cursors.WaitCursor;
			DateTime curTourDate = new DateTime();
			try {
				curTourDate = Convert.ToDateTime( inTourDate );
			} catch ( Exception ex ) {
				MessageBox.Show( "A valid tournament event end date value must be provided \n\nException: " + ex.Message );
			}

			/*
			Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
					{ "IWWFAthleteId", "USA" + "500001316" }
					, { "RefDate", curTourDate.ToString("yyyy-MM-dd") }
				};
			 */
			Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
					{ "IWWFAthleteId", "USA" + inMemberId }
					, { "RefDate", curTourDate.ToString("yyyy-MM-dd") }
				};
			String jsonMsg = JsonConvert.SerializeObject( sendConnectionMsg );

			NameValueCollection curHeaderParams = new NameValueCollection();
			curHeaderParams.Add( "Authorization", authApiKey + " " + authApiValue );

			Dictionary<string, object> respMsg = SendMessageHttp.getMessageDictionaryPostMessage( IwwfWebLocation, curHeaderParams, "application/Json", jsonMsg, null, null );
			if ( respMsg == null ) return false;
			
			Dictionary<string, object> athleteAttrList = getAttributeDictionary( respMsg, "Athlete" );
			if ( athleteAttrList == null ) {
				String respFailedMsg = getAttributeValue( respMsg, "Message" );
				Dictionary<string, object> respMsgModelList = getAttributeDictionary( respMsg, "ModelState" );
				ArrayList respFailedList = getAttributeList( respMsgModelList, "model.IWWFAthleteId" );
				
				String respFailedMsg2 = "";
				if ( respFailedList.Count > 0 ) respFailedMsg2 = (String)respFailedList[0];
				Log.WriteFile( String.Format("validateIwwfMembership:Request Failed: {0} {1}", respFailedMsg, respFailedMsg2) );
				
				Cursor.Current = Cursors.Default;
				return false;
			}

			String iwwfLicensePurchaseLink = getAttributeValue( respMsg, "PurchaseLink" );
			ArrayList licenseList = getAttributeList( respMsg, "Licenses" );
			//Dictionary<string, object> licenseList = getAttributeDictionary( respMsg, "Licenses" );
			if ( licenseList == null || licenseList.Count == 0 ) {
				Log.WriteFile( String.Format( "validateIwwfMembership:Skier found but no IWWF license found for skier {0} {1} {2}" +
					"\nSkier can purchase a license at the following address {3}"
					, athleteAttrList["FirstName"], athleteAttrList["LastName"], athleteAttrList["IWWFAthleteId"], iwwfLicensePurchaseLink ) );
				
				Cursor.Current = Cursors.Default;
				return false;
			
			} else {
				foreach ( Dictionary<string, object>  licenseEntry in licenseList ) {
					Boolean licenseAvailable = readLicenseEntry( licenseEntry );
					if ( licenseAvailable ) return true;
				}
			}

			Log.WriteFile( String.Format( "validateIwwfMembership:Skier found but no IWWF license found for skier {0} {1} {2}" +
				"\nSkier can purchase a license at the following address {3}"
				, athleteAttrList["FirstName"], athleteAttrList["LastName"], athleteAttrList["IWWFAthleteId"], iwwfLicensePurchaseLink ) );

			Cursor.Current = Cursors.Default;
			return false;
		}

		private static Boolean readLicenseEntry( Dictionary<string, object> licenseEntry ) {
			String licenseType = getAttributeValue( licenseEntry, "LicenseDescription" );
			String licenseAvailable = getAttributeValue( licenseEntry, "Available" );
			String licenseEvent = getAttributeValue( licenseEntry, "Event" );

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
	}
}
