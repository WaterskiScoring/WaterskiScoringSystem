using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Externalnterface {
    class ExportLiveWeb {
        private static String LiveWebLocation = "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
		private static String myReportFileUploadUrl = "http://www.waterskiresults.com/WfwWeb/WfwFileUpload.php";
        private static String myExportFileDownloadUrl = "http://www.waterskiresults.com/WfwWeb/wfwGetPublishExportList.php";
        private static String myReportListUrl = "http://www.waterskiresults.com/WfwWeb/wfwGetPublishReportList.php";
        private static String myDeletePublishReportUrl = "http://www.waterskiresults.com/WfwWeb/wfwDeletePublishReport.php";
        
        private static String myReportFileUploadContentType = "application/x-www-form-urlencoded;charset=UTF-8";

		public ExportLiveWeb() {
        }

        public static Boolean exportSkiersTrickVideo(String inEvent, String inSanctionId, byte inRound, String inEventGroup) {
            String curMethodName = "ExportLiveWeb: exportSkiersTrickVideo: ";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            Boolean returnStatus = false;
            int curLineCount = 0;
            DataTable curDataTable = new DataTable();
            ProgressWindow myProgressInfo = new ProgressWindow();

            try {
                curSqlStmt.Append( "Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round " );
                curSqlStmt.Append( "From TrickVideo S " );
                curSqlStmt.Append( "Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup " );
                curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId " );
                curSqlStmt.Append( "      AND ER.AgeGroup = S.AgeGroup AND ER.Event = 'Trick' " );
                curSqlStmt.Append( "Where S.SanctionId = '" + inSanctionId + "' " );
                curSqlStmt.Append( "AND (LEN(Pass1VideoUrl) > 1 or LEN(Pass2VideoUrl) > 1)" );
                curSqlStmt.Append( "Order by S.SanctionId, S.Round, S.AgeGroup, S.MemberId" );
                curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

                myProgressInfo.setProgressMin( 1 );
                myProgressInfo.setProgressMax( curDataTable.Rows.Count );

                String curMemberId = "", curAgeGroup = "", curSkierName = "";
                byte curRound = 0;
                foreach (DataRow curRow in curDataTable.Rows) {
                    curMemberId = (String)curRow["MemberId"];
                    curAgeGroup = (String)curRow["AgeGroup"];
                    curSkierName = (String)curRow["SkierName"];
                    curRound = (Byte)curRow["Round"];

                    curLineCount++;
                    myProgressInfo.setProgressValue( curLineCount );
                    myProgressInfo.setProgessMsg( "Processing " + curSkierName );
                    myProgressInfo.Show();
                    myProgressInfo.Refresh();

					returnStatus = ExportLiveWeb.exportCurrentSkierTrickVideo( inSanctionId, curMemberId, curAgeGroup, curRound );
					if ( returnStatus ) {
						continue;
					
                    } else {
						Log.WriteFile( String.Format( "Error encountered sending {0} scores for {1} {2}, terminating request", inEvent, curAgeGroup, curSkierName ) );
						break;
					}
				}

			} catch (Exception ex) {
				Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
				return false;

			} finally {
				myProgressInfo.Close();
			}

			return true;
        }

		public static Boolean exportCurrentSkierTrickVideo(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound) {
            String curMethodName = "ExportLiveWeb: exportCurrentSkierTrickVideo: ";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );
            Boolean returnStatus = false;

            try {
                curSqlStmt = new StringBuilder( "" );
                curXml.Append( "<LiveWebRequest>" );

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT * FROM TrickVideo " );
                curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                curSqlStmt.Append( "And Round = " + inRound );
                curXml.Append( exportData( "TrickVideo", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Insert" ) );

                curXml.Append( "</LiveWebRequest>" );
				returnStatus = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
				if ( returnStatus == false ) return false;

			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
				return false;
			}

			return true;
        }

        public static Boolean exportMessage(String inXmlMessage) {
            String curMethodName = "ExportLiveWeb: exportMessage: ";

            try {
                bool sendComplete = SendMessageHttp.sendMessagePostXml( LiveWebLocation, inXmlMessage );
				if ( sendComplete ) {
                    Log.WriteFile( curMethodName + ":" + inXmlMessage );
					return true;

				} else {
					MessageBox.Show( curMethodName + "failed" );
					return false;
				}

			} catch (Exception ex) {
				Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
				return false;
			}

		}

		public static Boolean uploadReportFile( String inReportType, String inEvent, String inSanctionId ) {
			String curFileFormName = "PublishReport";
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

			String curUploadUrl = String.Format( "{0}?reportType={1}&skiEvent={2}&sanctionNum={3}&reportTitle={4}"
				, myReportFileUploadUrl, inReportType, inEvent, inSanctionId, curDialog.ReportTitle );
			curFormData.Add( "reportFilename", curReportFileName );
			curFormData.Add( "reportFilenameBase", Path.GetFileName( curReportFileName ) );

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
            String curFileFormName = "PublishReport";
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

            String curUploadUrl = String.Format( "{0}?reportType={1}&skiEvent={2}&sanctionNum={3}&reportTitle={4}"
                , myReportFileUploadUrl, inReportType, inEvent, inSanctionId, curDialog.ReportTitle );
            curFormData.Add( "reportFilename", curExportFileName );
            curFormData.Add( "reportFilenameBase", Path.GetFileName( curExportFileName ) );

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
            String curReqstUrl = String.Format( "{0}?sanctionNum={1}", myReportListUrl, inSanctionId );
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
            String curReqstUrl = String.Format( "{0}?sanctionNum={1}", myExportFileDownloadUrl, inSanctionId );
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
            String curReqstUrl = String.Format( "{0}?PK={1}", myDeletePublishReportUrl, inReportPk );

            String curContentType = "application/json; charset=UTF-8";

            NameValueCollection curHeaderParams = new NameValueCollection();
            Cursor.Current = Cursors.WaitCursor;
            Dictionary<string, object> curRespMsg = SendMessageHttp.getMessageResponseJson( curReqstUrl, curHeaderParams, curContentType, null, null, false );
            if ( curRespMsg != null ) {
                Cursor.Current = Cursors.Default;
                MessageBox.Show( HelperFunctions.getAttributeValue( curRespMsg, "Message" ));
                return true;

            } else {
                return false;
            }

        }

        public static String exportData(String inTableName, String[] inKeyColumns, String inSqlStmt, String inCmd) {
            String curMethodName = "ExportLiveWeb: exportData: ";
            char[] singleQuoteDelim = new char[] { '\'' };
            String curMsg = "";
            String curValue;
            DataTable curDataTable = new DataTable();
            StringBuilder curXml = new StringBuilder( "" );

            try {
                curDataTable = DataAccess.getDataTable( inSqlStmt );
                if (curDataTable == null) {
                    //curMsg = "No data found";
                } else {
                    curXml.Append( "<Table name=\"" + inTableName + "\" command=\"" + inCmd + "\" >" );
                    curXml.Append( "<Columns count=\"" + curDataTable.Columns.Count + "\">" );
                    foreach (DataColumn curColumn in curDataTable.Columns) {
                        if ( !(curColumn.ColumnName.ToUpper().Equals( "PK" )) ) {
                            curXml.Append( "<Column>" + curColumn.ColumnName + "</Column>" );
                        }
                    }
                    curXml.Append( "</Columns>" );

                    curXml.Append( "<Keys count=\"" + inKeyColumns.Length + "\">" );
                    foreach (String curColumn in inKeyColumns) {
                        curXml.Append( "<Key>" + curColumn + "</Key>" );
                    }
                    curXml.Append( "</Keys>" );

                    if (curDataTable.Rows.Count == 0 && inCmd.ToLower().Equals( "delete" )) {
                        //curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                        String[] curDelimValue = {" And " };
                        int curDelimPos = inSqlStmt.ToLower().IndexOf( "where" );
                        String curData = inSqlStmt.Substring( curDelimPos + 6 );
                        String[] curDataList = curData.Split( curDelimValue, StringSplitOptions.RemoveEmptyEntries );
                        curXml.Append( "<Rows count=\"1\">" );
                        curXml.Append( "<Row colCount=\"" + curDataList.Length + "\">" );
                        for (int curIdx = 0; curIdx < curDataList.Length; curIdx++) {
                            String[] curDataDef = curDataList[curIdx].Split( '=' );
                            curXml.Append( "<" + curDataDef[0].Trim() + ">");
                            curValue = curDataDef[1].Trim();
                            if (curValue.Substring( 0, 1 ).Equals( "'" )) {
                                curXml.Append( curValue.Substring( 1, curValue.Length - 2 ) );
                            } else {
                                curXml.Append( curValue );
                            }
                            curXml.Append( "</" + curDataDef[0].Trim() + ">" );
                        }
                        curXml.Append( "</Row>" );
                        curXml.Append( "</Rows>" );

					} else {
						if ( curDataTable.Rows.Count == 0 ) return "";

						curXml.Append( "<Rows count=\"" + curDataTable.Rows.Count + "\">" );
                        foreach (DataRow curRow in curDataTable.Rows) {
                            curXml.Append( "<Row colCount=\"" + curDataTable.Columns.Count + "\">" );
                            foreach (DataColumn curColumn in curDataTable.Columns) {
                                if (!( curColumn.ColumnName.ToUpper().Equals( "PK" ) )) {
                                    if ( curColumn.ColumnName.ToLower().Equals( "lastupdatedate" ) || curColumn.ColumnName.ToLower().Equals( "insertdate" ) ) {
                                        curValue = ( (DateTime)curRow[curColumn.ColumnName] ).ToString( "yyyy-MM-dd HH:mm:ss" );
                                        curValue = encodeXmlValue( curValue );
                                        curXml.Append( "<" + curColumn.ColumnName + ">" + curValue + "</" + curColumn.ColumnName + ">" );
                                    } else {
                                        curValue = stripLineFeedChar( HelperFunctions.getDataRowColValue( curRow, curColumn.ColumnName, "" ) );
                                        curValue = HelperFunctions.stringReplace( curValue, singleQuoteDelim, "''" );
                                        curValue = encodeXmlValue( curValue );
                                        curXml.Append( "<" + curColumn.ColumnName + ">" + curValue + "</" + curColumn.ColumnName + ">" );
                                    }
                                }
                            }
                            curXml.Append( "</Row>" );
                        }
                        curXml.Append( "</Rows>" );
                    }
                    curXml.Append( "</Table>" );
                    Log.WriteFile( curMethodName + ":" + curXml.ToString() );
                }
                if (curMsg.Length > 1) {
                    MessageBox.Show( curMsg );
                }
                Log.WriteFile( curMethodName + ":conplete:" + curMsg );

			} catch (Exception ex) {
				Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
			}

			return curXml.ToString();
        }

        public static String encodeXmlValue(String inValue) {
            //String curEncodedXml = inValue.Replace( "&", "&amp;" ).Replace( "<", "&lt;" ).Replace( ">", "&gt;" ).Replace( "\"", "&quot;" ).Replace( "'", "&apos;" );
            String curEncodedXml = System.Security.SecurityElement.Escape( inValue );
            return curEncodedXml;
        }
        
        public static String stripLineFeedChar(String inValue) {
            String curValue = inValue;
            curValue = curValue.Replace( '\n', ' ' );
            curValue = curValue.Replace( '\r', ' ' );
            curValue = curValue.Replace( '\t', ' ' );
            return curValue;
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
