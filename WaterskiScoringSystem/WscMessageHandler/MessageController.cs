using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WscMessageHandler {
	public partial class MessageController : Form {
		public MessageController() {
			InitializeComponent();
		}

		private void MessageController_Load( object sender, EventArgs e ) {
			#region Set application window attributes from application configuration file
			if ( Properties.Settings.Default.MessageController_Width > 0 ) {
				this.Width = Properties.Settings.Default.MessageController_Width
			}
			if ( Properties.Settings.Default.MessageController_Height > 0 ) {
				this.Height = Properties.Settings.Default.MessageController_Height;
			}
			if ( Properties.Settings.Default.MessageController_Location.X > 0
				&& Properties.Settings.Default.MessageController_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.MessageController_Location;
			}
			if ( Properties.Settings.Default.AppTitle.Length > 0 ) {
				this.Text = Properties.Settings.Default.AppTitle;
			} else {
				this.Text = Properties.Settings.Default.AppTitle;
			}
			#endregion

			String curAppConnectString = Properties.Settings.Default.waterskiConnectionStringApp;
			if ( checkDbConnection( curAppConnectString ) ) {
				//Eliminate the option to replace database because data was being inadvertently overlayed.
			} else {
				MessageBox.Show( "Database not found at the specified location " + curAppConnectString );
			}




		}
		private bool checkDbConnection( String inConnectString ) {
			bool dbConnGood = false;
			SqlCeConnection myDbConn = null;
			try {
				myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
				myDbConn.ConnectionString = inConnectString;
				myDbConn.Open();
				dbConnGood = true;
			} catch ( Exception ex ) {
				dbConnGood = false;
				MessageBox.Show( "checkDbConnection:Exception: " + ex.Message );
			} finally {
				if ( myDbConn != null ) {
					myDbConn.Close();
				}
			}
			return dbConnGood;
		}
	}
}
