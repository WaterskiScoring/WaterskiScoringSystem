using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Externalnterface {
    class ExportLiveWeb {
		private static String LiveWebScoreboardApi = Properties.Settings.Default.UriWaterskiResultsApi + "/ImportScores";
		private static String LiveWebPublishFileApi = Properties.Settings.Default.UriWaterskiResultsApi + "/ImportFiles";

		private static String myReportFileUploadContentType = "application/x-www-form-urlencoded;charset=UTF-8";

		public ExportLiveWeb() {
        }

		public static Boolean uploadReportFile( String inReportType, String inEvent, String inSanctionId ) {
			String curFileFormName = "PublishFile";
			NameValueCollection curHeaderParams = null;
			NameValueCollection curFormData = new NameValueCollection();

			String curReportFileName = getImportFile();
			if ( curReportFileName == null ) return false;

			PublishReportDialog curDialog = new PublishReportDialog();
			curDialog.ReportType = inReportType;
			curDialog.Event = inEvent;
			curDialog.ReportFileName = curReportFileName;
			curDialog.ReportTitle = Path.GetFileName( curReportFileName );
			DialogResult curDialogResult = curDialog.ShowDialog();
			if ( curDialogResult != DialogResult.OK ) return false;

			String curUploadUrl = String.Format( "{0}?ReportType={1}&SkiEvent={2}&SanctionId={3}&ReportTitle={4}"
				, LiveWebPublishFileApi, inReportType, inEvent, inSanctionId, curDialog.ReportTitle );
			curFormData.Add( "PublishFilename", curReportFileName );
			curFormData.Add( "PublishFilenameBase", Path.GetFileName( curReportFileName ) );

			Dictionary<string, object> curResponseDataList = SendMessageHttp.sendMessagePostFileUpload( 
				curUploadUrl, curReportFileName, curFileFormName, curHeaderParams, curFormData, null, null );
			if ( curResponseDataList == null ) return false;

			String curMessage = "";
			foreach ( KeyValuePair<String, object> curEntry in curResponseDataList ) {
				if ( ((String)curEntry.Key).Equals( "Message" ) ) {
					curMessage = (String)curEntry.Value;
					break;
				}
			}
			if ( curMessage.Length == 0 ) curMessage = "No message returned from report upload service";
			MessageBox.Show( curMessage );

			return true;
		}

        public static void uploadExportFile( String inReportType, String inEvent, String inSanctionId ) {
            String curFileFormName = "PublishFile";
            NameValueCollection curHeaderParams = null;
            NameValueCollection curFormData = new NameValueCollection();

            String curExportFileName = getExportFilename();
            if ( curExportFileName == null ) return;

            PublishExportDialog curDialog = new PublishExportDialog();
            curDialog.ReportType = inReportType;
            curDialog.Event = inEvent;
            curDialog.ReportFileName = curExportFileName;
            curDialog.ReportTitle = Path.GetFileName( curExportFileName );
            DialogResult curDialogResult = curDialog.ShowDialog();
            if ( curDialogResult != DialogResult.OK ) return;

            String curUploadUrl = String.Format( "{0}?ReportType={1}&SkiEvent={2}&SanctionId={3}&ReportTitle={4}"
                , LiveWebPublishFileApi, inReportType, inEvent, inSanctionId, curDialog.ReportTitle );
            curFormData.Add( "PublishFilename", curExportFileName );
            curFormData.Add( "PublishFilenameBase", Path.GetFileName( curExportFileName ) );

            Dictionary<string, object> curResponseDataList = SendMessageHttp.sendMessagePostFileUpload(
                curUploadUrl, curExportFileName, curFileFormName, curHeaderParams, curFormData, null, null );
            if ( curResponseDataList == null ) return;

            String curMessage = "";
            foreach ( KeyValuePair<String, object> curEntry in curResponseDataList ) {
                if ( ( (String)curEntry.Key ).Equals( "Message" ) ) {
                    curMessage = (String)curEntry.Value;
                    break;
                }
            }
            if ( curMessage.Length == 0 ) curMessage = "No message returned from report upload service";
            MessageBox.Show( curMessage );
        }

        public static DataTable getReportList( String inSanctionId ) {
            String curReqstUrl = String.Format( "{0}?SanctionId={1}&ReportType=All", LiveWebPublishFileApi, inSanctionId );
            String curContentType = "application/json; charset=UTF-8";

            NameValueCollection curHeaderParams = new NameValueCollection();
            Cursor.Current = Cursors.WaitCursor;
            DataTable curDataTable = SendMessageHttp.getMessageResponseDataTable( curReqstUrl, curHeaderParams, curContentType, null, null, false );
            if ( curDataTable != null ) {
                Cursor.Current = Cursors.Default;
                return curDataTable;

            } else {
                return null;
            }
        }

        public static DataTable getExportList( String inSanctionId ) {
            String curReqstUrl = String.Format( "{0}?SanctionId={1}&ReportType=Export", LiveWebPublishFileApi, inSanctionId );
            String curContentType = "application/json; charset=UTF-8";

            NameValueCollection curHeaderParams = new NameValueCollection();
            Cursor.Current = Cursors.WaitCursor;
            DataTable curDataTable = SendMessageHttp.getMessageResponseDataTable( curReqstUrl, curHeaderParams, curContentType, null, null, false );
            if ( curDataTable != null ) {
                Cursor.Current = Cursors.Default;
                return curDataTable;

            } else {
                return null;
            }
        }

        public static bool deletePublishedReport( String inReportPk ) {
			String curReqstUrl = String.Format( "{0}?PK={1}", LiveWebPublishFileApi, inReportPk );
			String curContentType = "application/json; charset=UTF-8";

			NameValueCollection curHeaderParams = new NameValueCollection();
            Cursor.Current = Cursors.WaitCursor;
			Dictionary<string, object> curRespMsg = SendMessageHttp.deleteMessagePostJsonResp( curReqstUrl, curHeaderParams, curContentType, "" );
            if ( curRespMsg != null ) {
                Cursor.Current = Cursors.Default;
                MessageBox.Show( HelperFunctions.getAttributeValue( curRespMsg, "Message" ));
                return true;
            }

			return false;
		}

		private static String getImportFile() {
			OpenFileDialog myFileDialog = new OpenFileDialog();

			try {
				String curPath = Properties.Settings.Default.ExportDirectory;
				if ( curPath.Length < 2 ) curPath = System.IO.Directory.GetCurrentDirectory();
				myFileDialog.InitialDirectory = curPath;
				myFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
				myFileDialog.FilterIndex = 2;
				if ( myFileDialog.ShowDialog() != DialogResult.OK ) return null;

				String curFileName = myFileDialog.FileName;
				if ( curFileName == null ) return null;
				return curFileName;

			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Could not select a report file" + "\n\nError: " + ex.Message );
				return null;
			}
		}

        private static String getExportFilename() {
            OpenFileDialog myFileDialog = new OpenFileDialog();

            try {
                String curPath = Properties.Settings.Default.ExportDirectory;
                if ( curPath.Length < 2 ) curPath = System.IO.Directory.GetCurrentDirectory();
                myFileDialog.InitialDirectory = curPath;
                myFileDialog.Filter = "PDF files (*.txt)|*.txt|All files (*.*)|*.*";
                myFileDialog.FilterIndex = 2;
                if ( myFileDialog.ShowDialog() != DialogResult.OK ) return null;

                String curFileName = myFileDialog.FileName;
                if ( curFileName == null ) return null;
                return curFileName;

            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not select a report file" + "\n\nError: " + ex.Message );
                return null;
            }
        }

    }
}
