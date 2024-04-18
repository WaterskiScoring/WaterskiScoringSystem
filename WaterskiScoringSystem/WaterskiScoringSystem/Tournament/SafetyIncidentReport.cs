using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
	public partial class SafetyIncidentReport : Form {
		#region instance variables
		private String myWindowTitle;
		private String mySanctionNum;
		private String myMemberId;
		private DataRow myTourRow;
		#endregion

		public SafetyIncidentReport() {
			InitializeComponent();
		}

		public string MemberId {
			get => myMemberId;
			set => myMemberId = value;
		}

		private void SafetyIncidentReport_Load( object sender, EventArgs e ) {
			if ( Properties.Settings.Default.SafetyIncident_Width > 0 ) {
				this.Width = Properties.Settings.Default.SafetyIncident_Width;
			}
			if ( Properties.Settings.Default.SafetyIncident_Height > 0 ) {
				this.Height = Properties.Settings.Default.SafetyIncident_Height;
			}
			if ( Properties.Settings.Default.SafetyIncident_Location.X > 0
				&& Properties.Settings.Default.SafetyIncident_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.SafetyIncident_Location;
			}

			bool isSanctionAvailable = true;
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				isSanctionAvailable = false;

			} else if ( mySanctionNum.Length < 6 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				isSanctionAvailable = false;
			} else {
				//Retrieve selected tournament attributes
				myTourRow = getTourData();
				if ( myTourRow == null ) {
					MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
					isSanctionAvailable = false;
				}
			}
			if ( !isSanctionAvailable ) {
				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 15;
				curTimerObj.Tick += new EventHandler( CloseWindowTimer );
				curTimerObj.Start();
				return;
			}

			loadDisplayData();

		}

		private void CloseWindowTimer( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( CloseWindowTimer );
			this.Close();
		}

		private void SafetyIncidentReport_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.SafetyIncident_Width = this.Size.Width;
				Properties.Settings.Default.SafetyIncident_Height = this.Size.Height;
				Properties.Settings.Default.SafetyIncident_Location = this.Location;
			}
		}

		private void loadDisplayData() {
			Dictionary<string, object> curEntry = getSafetyIncidentMemberPrep();
			if ( curEntry == null ) {
				MessageBox.Show( "Data for tournament and member were not found" );
				return;
			}

			ClubNameTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "TSponsor" );
			ClubMemberIdTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "TSponsorID" );
			EventNameTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "TName" );
			EventDateTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "DateTourStart" ) + " - " + HelperFunctions.getAttributeValue( curEntry, "DateTourEnd" );
			EventLocationTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "TSite" )
				+ " (" + HelperFunctions.getAttributeValue( curEntry, "TSiteID" ) + ") "
				+ " " + HelperFunctions.getAttributeValue( curEntry, "TCity" )
				+ ", " + HelperFunctions.getAttributeValue( curEntry, "TState" );

			SancetionNumberTextbox.Text = mySanctionNum;

			String curEventRegion = "(3 - Event ) Unidentified Region";
			if ( mySanctionNum.Substring( 2, 1 ).Equals( "E" ) ) {
				curEventRegion = " (3 - Event ) Eastern Region";
			} else if ( mySanctionNum.Substring( 2, 1 ).Equals( "S" ) ) {
				curEventRegion = " (3 - Event ) Southern Region";
			} else if ( mySanctionNum.Substring( 2, 1 ).Equals( "C" ) ) {
				curEventRegion = " (3 - Event ) South Central Region";
			} else if ( mySanctionNum.Substring( 2, 1 ).Equals( "M" ) ) {
				curEventRegion = " (3 - Event ) Midwest Region";
			} else if ( mySanctionNum.Substring( 2, 1 ).Equals( "W" ) ) {
				curEventRegion = " (3 - Event ) Western Region";
			} else if ( mySanctionNum.Substring( 2, 1 ).Equals( "U" ) ) {
				curEventRegion = " (3 - Event ) Collegiate";
			}
			SportDisciplineTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "SptsGrpID" ) + curEventRegion;

			MemberIdTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "MemberId" );
			InjuredPersonNameTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "FirstName" ) 
				+ " " + HelperFunctions.getAttributeValue( curEntry, "LastName" );
			DateOfBirthTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "DateOfBirth" );
			GenderCombobox.Text = HelperFunctions.getAttributeValue( curEntry, "Sex" );
			HomeAddressTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "Address1" )
				+ " " + HelperFunctions.getAttributeValue( curEntry, "Address2" ) 
				+ " " + HelperFunctions.getAttributeValue( curEntry, "City" )
				+ ", " + HelperFunctions.getAttributeValue( curEntry, "State" )	;
			PhoneNumberTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "Phone" );
			MobilePhoneTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "MobilePhone" );
			ParentGuardianTextbox.Text = "";
			ParentGuardianPhoneTextbox.Text = "";

			MemberStatusTextbox.Text = HelperFunctions.getAttributeValue( curEntry, "membershipStatusText" );
			IndividualTypeCombobox.Text = "Athlete";

			String curSafetyDirector = HelperFunctions.getDataRowColValue( myTourRow, "ChiefSafetyName", "" );
			String curSafetyDirPhone = HelperFunctions.getDataRowColValue( myTourRow, "SafetyDirPhone", "" );
			String curSafetyDirEmail = HelperFunctions.getDataRowColValue( myTourRow, "SafetyDirEmail", "" );
			if ( HelperFunctions.isObjectPopulated( curSafetyDirector ) ) {
				String[] curSafetyDirInfo = new string[4];
				curSafetyDirInfo[0] = "Click on save button to write this information to a text file that should be sent to";
				curSafetyDirInfo[1] = "Chief Safety Director: " + curSafetyDirector;
				curSafetyDirInfo[2] = "Phone: " + curSafetyDirPhone;
				curSafetyDirInfo[3] = "Email: " + curSafetyDirEmail;
				ChiefSafetyTextBox.Lines = curSafetyDirInfo;
			}
		}

		/*
		 * Retrieve sanction, LOC, and member data needed to complete a safety incident report
		 * This data will be saved to a text file and provided to the chief safety director
		 * getSafetyIncidentMemberPrep
		 */
		private Dictionary<string, object> getSafetyIncidentMemberPrep() {
			String curSanctionEditCode = HelperFunctions.getDataRowColValue(myTourRow, "SanctionEditCode", "");

			String curContentType = "application/json; charset=UTF-8";
			String curOfficialExportListUrl = Properties.Settings.Default.UriUsaWaterski + "/admin/GetSafetyIncidentMemberJson.asp";
			String curReqstUrl = curOfficialExportListUrl + "?MemberId=" + myMemberId;

			NameValueCollection curHeaderParams = new NameValueCollection();
			List<object> curResponseDataList = null;

			Cursor.Current = Cursors.WaitCursor;
			curResponseDataList = SendMessageHttp.getMessageResponseJsonArray( curReqstUrl, curHeaderParams, curContentType, mySanctionNum, curSanctionEditCode, false );
			if ( curResponseDataList != null && curResponseDataList.Count > 0 ) {
				StringBuilder curMsg = new StringBuilder( "" );
				return (Dictionary<string, object>)curResponseDataList.ElementAt( 0 );

			} else {
				MessageBox.Show( "Sanction not found or hasn't been approved Sanction: " + mySanctionNum );
				return null;
			}
		}

		private void SafetyProgramHref_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e ) {
			try {
				String curLinkUri = ((LinkLabel)sender).Text.Substring( e.Link.Start, e.Link.Length );
				VisitLink( curLinkUri );
			
			} catch ( Exception ex ) {
				MessageBox.Show( "Unable to open link that was clicked.  " + ex.Message );
			}
		}

		private void VisitLink(String inLineUri) {
			SafetyProgramHref.LinkVisited = true;
			System.Diagnostics.Process.Start( inLineUri );
		}

		/*
		 * Save form data to a text file
		 * File should be supplied to the Chief Safety Director for the pusposes of completing the Safety Incident Report
		 */
		private void SaveButton_Click( object sender, EventArgs e ) {
			StringBuilder outLine = new StringBuilder( "" );

			String curFilename = mySanctionNum + "SafetyIncidentReportData.txt";
			StreamWriter outBuffer = HelperFunctions.getExportFile( null, curFilename );
			if ( outBuffer == null ) return;

			Log.WriteFile( "Save safety incident report data file: " + curFilename );

			try {
				outLine = new StringBuilder( "" );
				outLine.Append( "SancetionNumber: " + SancetionNumberTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "EventName: " + EventNameTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "EventDate: " + EventDateTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "EventLocation: " + EventLocationTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "EventType: " + EventTypeTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );

				outBuffer.WriteLine( "" );
				outLine = new StringBuilder( "" );
				outLine.Append( "ClubName: " + ClubNameTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "ClubMemberId: " + ClubMemberIdTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "SportDiscipline: " + SportDisciplineTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );

				outBuffer.WriteLine( "" );
				outLine = new StringBuilder( "" );
				outLine.Append( "IndividualType: " + IndividualTypeCombobox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "MemberId: " + MemberIdTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "MemberStatus: " + MemberStatusTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "InjuredPersonName: " + InjuredPersonNameTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "DateOfBirth: " + DateOfBirthTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "Gender: " + GenderCombobox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "HomeAddress: " + HomeAddressTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "PhoneNumber: " + PhoneNumberTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "MobilePhone: " + MobilePhoneTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );

				outBuffer.WriteLine( "" );
				outLine = new StringBuilder( "" );
				outLine.Append( "ParentGuardian: " + ParentGuardianTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );
				outLine = new StringBuilder( "" );
				outLine.Append( "ParentGuardianPhone: " + ParentGuardianPhoneTextbox.Text );
				outBuffer.WriteLine( outLine.ToString() );

				outBuffer.WriteLine( "" );
				outLine = new StringBuilder( "" );
				outLine.Append( "SafetyProgramHref: " + SafetyProgramHref.Text );
				outBuffer.WriteLine( outLine.ToString() );

				outBuffer.WriteLine( "" );
				outLine = new StringBuilder( "" );
				outBuffer.WriteLine( "SafetyDirector: " + HelperFunctions.getDataRowColValue( myTourRow, "ChiefSafetyName", "" ) );
				outBuffer.WriteLine( "Phone: " + HelperFunctions.getDataRowColValue( myTourRow, "SafetyDirPhone", "" ) );
				outBuffer.WriteLine( "Email: " + HelperFunctions.getDataRowColValue( myTourRow, "SafetyDirEmail", "" ) );

				outBuffer.Flush();
				outBuffer.Close();

				MessageBox.Show( "Safety incident report tournament related inforamtion save complete" );

			} catch ( Exception ex ) {
				MessageBox.Show( "Exception encountered saving safety incident report information: " + ex.Message );
			}
		}

		private DataRow getTourData() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT T.SanctionId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation, SanctionEditCode" );
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation, TourDataLoc" );
			curSqlStmt.Append( ", T.ContactMemberId, TourRegCO.SkierName AS ContactName" );
			curSqlStmt.Append( ", T.SafetyDirMemberId, TourRegCS.SkierName AS ChiefSafetyName, SafetyDirPhone, SafetyDirEmail " );
			curSqlStmt.Append( "FROM Tournament T " );
			curSqlStmt.Append( "  LEFT OUTER JOIN TourReg AS TourRegCS ON TourRegCS.SanctionId = T.SanctionId AND TourRegCS.MemberId = T.SafetyDirMemberId" );
			curSqlStmt.Append( "  LEFT OUTER JOIN TourReg AS TourRegCO ON TourRegCO.SanctionId = T.SanctionId AND TourRegCO.MemberId = T.ContactMemberId" );
			curSqlStmt.Append( "  LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable != null && curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];
			return null;
		}

	}
}
