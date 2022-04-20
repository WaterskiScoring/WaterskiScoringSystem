using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BoatPathMonitorSim.Common;

namespace BoatPathMonitorSim.Message {
	public partial class ConnectDialog : Form {
		private static String myWscWebLocation = "";
		private bool isDataLoading = true;

		private String myDialogCommand = "";
		private bool myWscConnected = false;
		private Point myWindowLocation = new Point( 0, 0 );

		public ConnectDialog() {
			InitializeComponent();
		}

		public static String WscWebLocation {
			get { return myWscWebLocation; }
			set { myWscWebLocation = value; }
		}

		public String dialogCommand {
			get { return myDialogCommand; }
			set { myDialogCommand = value; }
		}

		public System.Drawing.Point windowLocation {
			get { return myWindowLocation; }
			set { myWindowLocation = value; }
		}

		public Boolean isWscConnected {
			get { return myWscConnected; }
			set { myWscConnected = value; }
		}

		private void ConnectDialog_Load( object sender, EventArgs e ) {
			this.Location = myWindowLocation;

			myWscWebLocation = ConnectMgmtData.wscWebLocationDefault;
			serverUriTextBox.Text = myWscWebLocation;
			eventSubIdTextBox.Text = ConnectMgmtData.eventSubId;
			UseJumpTimesCheckBox.Checked = ConnectMgmtData.useJumpTimes;

			if ( isWscConnected ) {
				MessageLabel.Text = "Listener is connected and active";

			} else {
				MessageLabel.Text = "Listener is not connected";
			}

			if ( Properties.Settings.Default.DatabaseFilename.Length > 0 ) {
				ShowTourList();
			} else {
				MessageBox.Show( "Database file not available, must select a database file" );
			}
		}

		private void execWscConnect_Click( object sender, EventArgs e ) {
			Properties.Settings.Default.SanctionNum = sanctionNumTextbox.Text;
			ConnectMgmtData.sanctionNum = sanctionNumTextbox.Text;
			ConnectMgmtData.eventSubId = eventSubIdTextBox.Text;
			ConnectMgmtData.useJumpTimes = false;
			if ( UseJumpTimesCheckBox.Checked ) ConnectMgmtData.useJumpTimes = true;
			myDialogCommand = "Connected";
		}

		private void execWscClose_Click( object sender, EventArgs e ) {
			myDialogCommand = "Disconnected";
		}

		private void showAppsConnected_Click( object sender, EventArgs e ) {
			myDialogCommand = "ShowAppsConnected";
		}


		private void showWscPin_Click( object sender, EventArgs e ) {
			myDialogCommand = "ShowPin";
		}

		private void UseJumpTimesCheckBox_CheckedChanged( object sender, EventArgs e ) {
			this.Location = myWindowLocation;
			ConnectMgmtData.useJumpTimes = UseJumpTimesCheckBox.Checked;
		}

		private void SelectDatabaseButton_Click( object sender, EventArgs e ) {
			DataAccess.DataAccessClose( true );
			if ( DataAccess.getNewDatabaseFile() ) ShowTourList();
		}

		private void ShowTourList() {
			isDataLoading = true;
			String curSelectedValue = "";
			int curSelectedIdx = -1;
			databaseFilenameTextBox.Text = Properties.Settings.Default.DatabaseFilename;

			ArrayList curDropdownList = new ArrayList();
			TourListComboBox.DisplayMember = "ItemName";
			TourListComboBox.ValueMember = "ItemValue";

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select SanctionId, Name, TourDataLoc, EventDates from Tournament Order by EventDates DESC" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			foreach ( DataRow curRow in curDataTable.Rows ) {
				curDropdownList.Add( String.Format( "{0} / {1}", (String)curRow["SanctionId"], (String)curRow["Name"] ) );
				if ( ( (String)curRow["SanctionId"] ).Equals( Properties.Settings.Default.SanctionNum ) ) {
					sanctionNumTextbox.Text = (String)curRow["SanctionId"];
					curSelectedValue = String.Format( "{0} / {1}", (String)curRow["SanctionId"], (String)curRow["Name"] );
					curSelectedIdx = curDropdownList.Count - 1;
				}
			}

			TourListComboBox.DataSource = curDropdownList;
			isDataLoading = false;

			if ( curSelectedIdx < 0 ) {
				if ( curDataTable.Rows.Count > 0 ) {
					TourListComboBox.SelectedIndex = 0;
					TourListComboBox_SelectedIndexChanged( null, null );
				}

			} else {
				TourListComboBox.SelectedIndex = curSelectedIdx;
				TourListComboBox_SelectedIndexChanged( null, null );
			}
		}

		private void TourListComboBox_SelectedIndexChanged( object sender, EventArgs e ) {
			if ( isDataLoading ) return;
			String curTourName = TourListComboBox.SelectedItem.ToString();
			String curSanctionId = curTourName.Substring( 0, 6 );
			DataRow curTourRow = HelperFunctions.getTourData( curSanctionId );
			if ( curTourRow == null ) {
				MessageBox.Show( "Tournament not found for " + curTourName );
				return;
			}

			sanctionNumTextbox.Text = (String)curTourRow["SanctionId"];
			Properties.Settings.Default.DataDirectory = (String)curTourRow["TourDataLoc"];
		}
	}
}
