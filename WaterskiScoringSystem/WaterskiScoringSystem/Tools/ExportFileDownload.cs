using LiveWebMessageHandler.Common;
using System;
using System.Data;
using System.Windows.Forms;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Common;
using HelperFunctions = WaterskiScoringSystem.Common.HelperFunctions;
using System.IO;

namespace WaterskiScoringSystem.Tools {
	public partial class ExportFileDownload : Form {
        private String mySanctionNum;
        
        public ExportFileDownload() {
			InitializeComponent();
		}

        private void ExportFileDownload_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.ExportFileDownload_Width > 0 ) {
                this.Width = Properties.Settings.Default.ExportFileDownload_Width;
            }
            if (Properties.Settings.Default.ExportFileDownload_Height > 0 ) {
                this.Height = Properties.Settings.Default.ExportFileDownload_Height;
            }
            if (Properties.Settings.Default.ExportFileDownload_Location.X > 0
                && Properties.Settings.Default.ExportFileDownload_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.ExportFileDownload_Location;
            }

            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            loadDataView();
        }
        
        private void ExportFileDownload_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.ExportFileDownload_Width = this.Size.Width;
                Properties.Settings.Default.ExportFileDownload_Height = this.Size.Height;
                Properties.Settings.Default.ExportFileDownload_Location = this.Location;
            }
        }

        private void loadDataView() {
            Cursor.Current = Cursors.WaitCursor;
            int curViewIdx = 0;

            try {
                DataTable curDataTable = ExportLiveWeb.getExportList( mySanctionNum );
                if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return;

                DataGridView.Rows.Clear();
                DataGridViewRow curViewRow;
                foreach ( DataRow curDataRow in curDataTable.Rows ) {
                    curViewIdx = DataGridView.Rows.Add();
                    curViewRow = DataGridView.Rows[curViewIdx];
                    curViewRow.Cells["PK"].Value = HelperFunctions.getDataRowColValue( curDataRow, "PK", "0" );
                    curViewRow.Cells["ReportTitle"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ReportTitle", "" );
                    // "/public_html/scoring/Tournament/22E030"
                    String curExportFileRef = HelperFunctions.getDataRowColValue( curDataRow, "ReportFilePath", "" );
                    if ( curExportFileRef.Length > 21 ) {
                        String curExportFileUri = String.Format( "http://www.waterskiresults.com/{0}", curExportFileRef.Substring( 35 ) );
                        curViewRow.Cells["ExportFileUri"].Value = curExportFileUri;
                    } else {
                        curViewRow.Cells["ExportFileUri"].Value = "File not available";
                    }
                }

                curViewIdx = 0;
                DataGridView.CurrentCell = DataGridView.Rows[curViewIdx].Cells["ReportTitle"];
                int curRowPos = curViewIdx + 1;

            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving list of available export files \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
        }

        private void DownloadButton_Click( object sender, EventArgs e ) {
            int curViewIdx = DataGridView.CurrentRow.Index;
            String curExportFileUri = HelperFunctions.getViewRowColValue( DataGridView.CurrentRow, "ExportFileUri", "" );

            String curPath = Properties.Settings.Default.ExportDirectory;
            using ( FolderBrowserDialog curFolderDialog = new FolderBrowserDialog() ) {
                curFolderDialog.ShowNewFolderButton = true;
                curFolderDialog.RootFolder = Environment.SpecialFolder.Desktop;
                curFolderDialog.SelectedPath = @curPath;
                if ( FolderBrowserLauncher.ShowFolderBrowser( curFolderDialog, Form.ActiveForm ) == DialogResult.OK ) {
                    curPath = curFolderDialog.SelectedPath;
                    if ( Log.isDirectoryValid( curPath ) ) {
                        int curDelimIdx = curExportFileUri.LastIndexOf( "/" ) + 1;
                        String curDownloadFilename = Path.Combine( curPath, curExportFileUri.Substring( curDelimIdx ) );
                        if ( SendMessageHttp.getDownloadFile( curExportFileUri, curDownloadFilename ) ) {
                            ImportData myImportData = new ImportData();
                            myImportData.importData( curDownloadFilename );
                        }

                    } else {
                        MessageBox.Show( "A valid directory must be selected" );
                    }
                }
            }



        }

    }
}
