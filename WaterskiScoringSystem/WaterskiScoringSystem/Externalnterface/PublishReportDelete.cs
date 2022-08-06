using System;
using System.Data;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Externalnterface {
    public partial class PublishReportDelete : Form {
        private String mySanctionNum;

        public PublishReportDelete() {
            InitializeComponent();
        }

        private void PublishReportDelete_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.PublishReportDelete_Width > 0 ) {
                this.Width = Properties.Settings.Default.PublishReportDelete_Width;
            }
            if (Properties.Settings.Default.PublishReportDelete_Height > 0 ) {
                this.Height = Properties.Settings.Default.PublishReportDelete_Height;
            }
            if (Properties.Settings.Default.PublishReportDelete_Location.X > 0
                && Properties.Settings.Default.PublishReportDelete_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.PublishReportDelete_Location;
            }

            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            loadDataView();
        }

        private void PublishReportDelete_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.PublishReportDelete_Width = this.Size.Width;
                Properties.Settings.Default.PublishReportDelete_Height = this.Size.Height;
                Properties.Settings.Default.PublishReportDelete_Location = this.Location;
            }

        }
        private void loadDataView() {
            Cursor.Current = Cursors.WaitCursor;
            int curViewIdx = 0;

            try {
                DataTable curDataTable = ExportLiveWeb.getReportList( mySanctionNum );
                if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return;

                DataGridView.Rows.Clear();
                DataGridViewRow curViewRow;
                foreach ( DataRow curDataRow in curDataTable.Rows ) {
                    curViewIdx = DataGridView.Rows.Add();
                    curViewRow = DataGridView.Rows[curViewIdx];
                    curViewRow.Cells["PK"].Value = HelperFunctions.getDataRowColValue(curDataRow, "PK", "0");
                    curViewRow.Cells["Event"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Event", "" );
                    curViewRow.Cells["ReportType"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ReportType", "" );
                    curViewRow.Cells["ReportTitle"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ReportTitle", "" );
                }

                curViewIdx = 0;
                DataGridView.CurrentCell = DataGridView.Rows[curViewIdx].Cells["ReportTitle"];
                int curRowPos = curViewIdx + 1;
            
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving list of available published reports \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
        }

		private void DeleteButton_Click( object sender, EventArgs e ) {
            int curViewIdx = DataGridView.CurrentRow.Index;
            String curPk = HelperFunctions.getViewRowColValue( DataGridView.CurrentRow, "PK", "0" );
            if ( ExportLiveWeb.deletePublishedReport( curPk ) ) DataGridView.Rows.RemoveAt( curViewIdx );
        }
    }
}
