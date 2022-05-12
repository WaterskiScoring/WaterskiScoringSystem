using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ExportData {
        private String tabDelim = "\t";
        private ProgressWindow myProgressInfo;

        public ExportData() {
        }

        public Boolean exportTourData( String inSanctionId ) {
            return exportTourData( inSanctionId, null );

        }
        public Boolean exportTourData( String inSanctionId, String inFullFileName ) {
            String curMethodName = "exportTourData";
            Boolean returnStatus = false;
            Log.WriteFile( curMethodName + ":begin: " );

            String[] exportTableName = {
                "Tournament"
                , "TourReg"
                , "TourProperties"
                , "EventReg"
                , "EventRunOrder"
				, "EventRunOrderFilters"
				, "TeamList"
                , "TeamOrder"
                , "SlalomScore"
                , "SlalomRecap"
                , "TrickScore"
                , "TrickVideo"
                , "TrickPass"
                , "JumpScore"
                , "JumpRecap"
				, "JumpMeterSetup"
                , "JumpVideoSetup"
				, "BoatTime"
				, "BoatPath"
				, "JumpMeasurement"
				, "TourBoatUse"
                , "OfficialWork"
                , "OfficialWorkAsgmt"
                , "SafetyCheckList"
                , "DivOrder"
            };
            String[] exportSelectStmt = new String[exportTableName.Length];

            for ( int idx = 0; idx < exportTableName.Length; idx++ ) {
                exportSelectStmt[idx] = "Select * from " + exportTableName[idx] + " where SanctionId = '" + inSanctionId + "'";
            }
            if ( inFullFileName == null ) {
                returnStatus = exportData( exportTableName, exportSelectStmt );
            } else {
                returnStatus = exportData( exportTableName, exportSelectStmt, inFullFileName );
            }
            Log.WriteFile( curMethodName + ":conplete: " );

            return returnStatus;
        }

        public Boolean exportData( String[] inTableName, String[] inSelectStmt ) {
            Boolean returnStatus = false;
            StreamWriter outBuffer = HelperFunctions.getExportFile();
            StringBuilder inReturnMessage = new StringBuilder( "" );

            if ( outBuffer != null ) {
                String curMsg = "Exporting selected data for " + inTableName.Length + " data types";
                myProgressInfo = new ProgressWindow();
                myProgressInfo.setProgessMsg(curMsg);
                myProgressInfo.Show();
                myProgressInfo.Refresh();
                myProgressInfo.setProgressMax( inTableName.Length );

                for ( int idx = 0; idx < inTableName.Length; idx++ ) {
                    myProgressInfo.setProgessMsg(curMsg + ": " + inTableName[idx] + "Inprogress " + "|");
                    myProgressInfo.Refresh();
                    returnStatus = exportData( inTableName[idx], inSelectStmt[idx], outBuffer, false, inReturnMessage );

                    myProgressInfo.setProgressValue( idx + 1 );
                    //myProgressInfo.setProgessMsg( "This is a new message" );
                    myProgressInfo.Refresh();
                }
                
                outBuffer.Close();
                myProgressInfo.Close();
                MessageBox.Show( inReturnMessage.ToString() );
            }
            return returnStatus;
        }
        public Boolean exportData( String[] inTableName, String[] inSelectStmt, String inFullFileName ) {
            Boolean returnStatus = false;
            StreamWriter outBuffer = File.CreateText( inFullFileName );
            StringBuilder inReturnMessage = new StringBuilder( "" );

            if (outBuffer != null) {
                myProgressInfo = new ProgressWindow();
                myProgressInfo.setProgessMsg( "Exporting selected data for " + inTableName.Length + " data types" );
                myProgressInfo.Show();
                myProgressInfo.Refresh();
                myProgressInfo.setProgressMax( inTableName.Length );

                for ( int idx = 0; idx < inTableName.Length; idx++ ) {
                    returnStatus = exportData( inTableName[idx], inSelectStmt[idx], outBuffer, false, inReturnMessage );

                    myProgressInfo.setProgressValue( idx + 1 );
                    myProgressInfo.Refresh();
                }

                outBuffer.Close();
                myProgressInfo.Close();
                if (inReturnMessage.Length > 1) {
                    MessageBox.Show( inReturnMessage.ToString() );
                }
            }
            return returnStatus;
        }

		public Boolean exportString (String msg, String inFileName ) {
			String curMethodName = "exportData";
			String curMsg = "";
			StreamWriter outBuffer = null;

			try {
				outBuffer = HelperFunctions.getExportFile( null, inFileName );
				if ( outBuffer == null ) {
					curMsg = "Export file not available";
					MessageBox.Show( curMsg );
					return false;
				} 
				
				outBuffer.WriteLine( msg );
				
				return true;

			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Could not write file from data table\n\nError: " + ex.Message );
				curMsg = curMethodName + ":Exception=" + ex.Message;
				Log.WriteFile( curMsg );
				return false;
			
			} finally {
				if ( outBuffer != null ) outBuffer.Close();
			}
		}

		public Boolean exportData( String inTableName, String inSelectStmt ) {
            Boolean returnStatus = false;
            StreamWriter outBuffer = HelperFunctions.getExportFile();
            if ( outBuffer != null ) {
                returnStatus = exportData( inTableName, inSelectStmt, outBuffer, true );
                if ( returnStatus ) { outBuffer.Close(); }
            }
            return returnStatus;
        }
        public Boolean exportData( String inTableName, String inSelectStmt, StreamWriter outBuffer ) {
            return exportData( inTableName, inSelectStmt, outBuffer, true, null );
        }
        public Boolean exportData(String inTableName, String inSelectStmt, StreamWriter outBuffer, bool inShowMsg) {
            return exportData( inTableName, inSelectStmt, outBuffer, inShowMsg, null );
        }
        public Boolean exportData(String inTableName, String inSelectStmt, StreamWriter outBuffer, bool inShowMsg, StringBuilder inReturnMessage) {
            String curMethodName = "exportData";
            String curMsg = "";
            String curValue;
            Boolean returnStatus = false;
            int colCount = 0;
            DataTable curDataTable = new DataTable();
            StringBuilder outLine = new StringBuilder( "" );

            try {
                Log.WriteFile( curMethodName + ":begin: " + "Table=" + inTableName + " " + inSelectStmt );
                outBuffer.WriteLine( "Table:" + tabDelim + inTableName );

                curDataTable = DataAccess.getDataTable( inSelectStmt, inShowMsg );
                if ( curDataTable == null ) {
                    curMsg = "No data found";
                } else {
                    colCount = 0;
                    foreach ( DataColumn curCol in curDataTable.Columns ) {
                        colCount++;
                        if ( colCount < curDataTable.Columns.Count ) {
                            outLine.Append( curCol.ColumnName + tabDelim );
                        } else {
                            outLine.Append( curCol.ColumnName );
                        }
                    }
                    outBuffer.WriteLine( outLine.ToString() );
                    outLine = new StringBuilder( "" );

                    foreach ( DataRow curRow in curDataTable.Rows ) {
                        colCount = 0;
                        foreach ( DataColumn curCol in curDataTable.Columns ) {
                            colCount++;
                            curValue = curRow[curCol.ColumnName].ToString();
                            curValue = curValue.Replace( '\n', ' ' );
                            curValue = curValue.Replace( '\r', ' ' );
                            curValue = curValue.Replace( '\t', ' ' );
                            if ( colCount < curDataTable.Columns.Count ) {
                                outLine.Append( curValue + tabDelim );
                            } else {
                                outLine.Append( curValue );
                            }
                        }
                        outBuffer.WriteLine( outLine.ToString() );
                        outLine = new StringBuilder( "" );
                    }
                    returnStatus = true;
                    if ( curDataTable.Rows.Count > 0 ) {
                        curMsg = curDataTable.Rows.Count + " " + inTableName + " rows found and written";
                    } else {
                        curMsg = "No rows found on " + inTableName;
                    }
                    if (inShowMsg) {
                        MessageBox.Show( curMsg );
                    } else {
                        if (inReturnMessage != null) {
                            if (inReturnMessage.ToString().Length > 0) {
                                inReturnMessage.Append( "\n" );
                            }
                            inReturnMessage.Append( curMsg );
                        }
                    }
                }
                Log.WriteFile( curMethodName + ":conplete:" + curMsg );
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not write file\n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            }

            return returnStatus;
        }
        public Boolean exportData( DataGridView inDataGrid ) {
            return exportData( inDataGrid, null );
        }
        public Boolean exportData( DataGridView inDataGrid, String inFullFileName ) {
            String curMethodName = "exportData";
            String curMsg = "";
            Boolean returnStatus = false;
            String curValue;
            int colCount = 0;
            StringBuilder outLine = new StringBuilder( "" );
			StreamWriter outBuffer = null;

            try {
                Log.WriteFile( curMethodName + ":begin: " + "DataGrid=" + inDataGrid.Name );
                outBuffer = HelperFunctions.getExportFile( null, inFullFileName );
                if ( outBuffer == null ) {
                    curMsg = "Export file not available";
                } else {
                    colCount = 0;
                    foreach ( DataGridViewColumn curCol in inDataGrid.Columns ) {
                        if ( curCol.Visible ) {
                            colCount++;
                            if ( colCount > 1 ) {
                                outLine.Append( tabDelim + curCol.HeaderText );
                            } else {
                                outLine.Append( curCol.Name );
                            }
                        }
                    }
                    outBuffer.WriteLine( outLine.ToString() );
                    outLine = new StringBuilder( "" );

                    foreach ( DataGridViewRow curRow in inDataGrid.Rows ) {
                        colCount = 0;
                        foreach ( DataGridViewColumn curCol in inDataGrid.Columns ) {
                            if ( curCol.Visible ) {
                                colCount++;
                                if ( curRow.Cells[curCol.Name].Value == null ) {
                                    curValue = "";
                                } else {
                                    curValue = curRow.Cells[curCol.Name].Value.ToString();
                                }
                                curValue = curValue.Replace( '\n', ' ' );
                                curValue = curValue.Replace( '\r', ' ' );
                                curValue = curValue.Replace( '\t', ' ' );
                                if ( colCount > 1 ) {
                                    outLine.Append( tabDelim + curValue );
                                } else {
                                    outLine.Append( curValue );
                                }
                            }
                        }
                        outBuffer.WriteLine( outLine.ToString() );
                        outLine = new StringBuilder( "" );
                    }
                    returnStatus = true;
                    outBuffer.Close();

                    if ( inDataGrid.Rows.Count > 0 ) {
                        curMsg = inDataGrid.Rows.Count + " rows found and written";
                    } else {
                        curMsg = "No rows found";
                    }
                    MessageBox.Show( curMsg );
                }
                
				Log.WriteFile( curMethodName + ":conplete:" + curMsg );
            
			} catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not write file from data table\n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                returnStatus = false;
			
			} finally {
				if ( outBuffer != null ) outBuffer.Close();
			}
			
			return returnStatus;
        }
        public Boolean exportData( DataTable inDataTable ) {
            return exportData( inDataTable, null );
        }
        public Boolean exportData( DataTable inDataTable, String inFullFileName ) {
            String curMethodName = "exportData";
            String curMsg = "";
            Boolean returnStatus = false;
            String curValue;
            int colCount = 0;
            StringBuilder outLine = new StringBuilder( "" );
			StreamWriter outBuffer = null;

            try {
                Log.WriteFile( curMethodName + ":begin: " + "DataTable=" + inDataTable.TableName );
                outBuffer = HelperFunctions.getExportFile( null, inFullFileName );
                if ( outBuffer == null ) {
                    curMsg = "Export file not available";
                } else {
                    colCount = 0;
                    foreach ( DataColumn curCol in inDataTable.Columns ) {
                        colCount++;
                        if ( colCount < inDataTable.Columns.Count ) {
                            outLine.Append( curCol.ColumnName + tabDelim );
                        } else {
                            outLine.Append( curCol.ColumnName );
                        }
                    }
                    outBuffer.WriteLine( outLine.ToString() );
                    outLine = new StringBuilder( "" );

                    foreach ( DataRow curRow in inDataTable.Rows ) {
                        colCount = 0;
                        foreach ( DataColumn curCol in inDataTable.Columns ) {
                            colCount++;
                            curValue = curRow[curCol.ColumnName].ToString();
                            curValue = curValue.Replace( '\n', ' ' );
                            curValue = curValue.Replace( '\r', ' ' );
                            curValue = curValue.Replace( '\t', ' ' );
                            if ( colCount < inDataTable.Columns.Count ) {
                                outLine.Append( curValue + tabDelim );
                            } else {
                                outLine.Append( curValue );
                            }
                        }
                        outBuffer.WriteLine( outLine.ToString() );
                        outLine = new StringBuilder( "" );
                    }
                    returnStatus = true;
                    outBuffer.Close();

                    if ( inDataTable.Rows.Count > 0 ) {
                        curMsg = inDataTable.Rows.Count + " rows found and written" ;
                    } else {
                        curMsg = "No rows found";
                    }
                    MessageBox.Show( curMsg );
                }
                
				Log.WriteFile( curMethodName + ":conplete:" + curMsg );
            
			} catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not write file from data table\n\nException: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                returnStatus = false;
			
			} finally {
				if ( outBuffer != null ) outBuffer.Close();
			}
			
			return returnStatus;
        }

        public Boolean exportDataAsHtml( DataGridView inDataGrid, String inTitle, String inSubTitle, String inFooter ) {
            return exportDataAsHtml( inDataGrid, inTitle, inSubTitle, inFooter, null );
        }
        public Boolean exportDataAsHtml( DataGridView inDataGrid, String inTitle, String inSubTitle, String inFooter, String inFileName ) {
            String curMethodName = "exportDataAsHtml";
            String curMsg = "";
            Boolean returnStatus = false;
            String curValue;
            String curFileFilter = "HTML files (*.htm)|*.htm|All files (*.*)|*.*";
            StringBuilder outLine = new StringBuilder( "" );
			StreamWriter outBuffer = null;

            try {
                curMsg = "DataGrid=" + inDataGrid.Name;
                Log.WriteFile( curMethodName + ":begin: " + curMsg );
                outBuffer = HelperFunctions.getExportFile( curFileFilter, inFileName );
                if ( outBuffer == null ) {
                    curMsg = "Output file not available";
                
				} else {
                    String curDateString = DateTime.Now.ToString( "MMMM d, yyyy HH:mm:ss" );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "<html><head>" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "<title>" + inTitle + "</title>" );
                    outBuffer.WriteLine( outLine.ToString() );
                    outLine = new StringBuilder( "" );
                    outLine.Append( "<style>" );
                    outLine.Append( "Body {BACKGROUND-COLOR:#FFFFFF; COLOR:#000000;" );
                    outLine.Append( " Font-Family:Arial,Helvetica,sans-serif; FONT-SIZE:1.0em; Font-Style:normal;Font-Weight:normal;} " );
                    outLine.Append( Environment.NewLine + " .ReportTitle {Width: 100%; Text-Align:Center; FONT-SIZE:1.5em; FONT-WEIGHT:bold;}" );
                    outLine.Append( Environment.NewLine + " .SubTitle {Width: 100%; Text-Align:Center; FONT-SIZE:1.25em; FONT-WEIGHT:normal;}" );
                    outLine.Append( Environment.NewLine + " .Footer {Width: 100%; Text-Align:Center; FONT-SIZE:.75em; FONT-WEIGHT:normal;}" );
                    outLine.Append( Environment.NewLine + " .DataGridView {Width: 100%; Text-Align:Center; Border: #AAAAAA .15em solid; }" );
                    outLine.Append( Environment.NewLine + " th {Text-Align:Center; Border:#DDDDDD .1em solid; Margin:0pt; Padding:0px 0px 0px 0px; Font-Size:.8em; Font-Weight:Bold;}" );
                    outLine.Append( Environment.NewLine + " td {Text-Align:Center; Border:#DDDDDD .1em solid; Margin:0pt; Padding:0px 0px 0px 0px; Font-Family:Verdana,Helvetica,sans-serif; Font-Size:.8em; Font-Weight:normal;}" );
                    outLine.Append( Environment.NewLine + "</style>" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "</head><body>" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "<div Class=\"ReportTitle\">" + inTitle + "</div>" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "<div Class=\"SubTitle\">" + inSubTitle + " as of " + curDateString + "</div>" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "<div id=\"MainContent\"><br>" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "<Table Class=\"DataGridView\">" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "<tr>" );
                    foreach ( DataGridViewColumn curCol in inDataGrid.Columns ) {
                        if ( curCol.Visible ) {
                            outLine.Append( "<th>" + curCol.HeaderText + "</th>" );
                        }
                    }
                    outLine.Append( "</tr>" );
                    outBuffer.WriteLine( outLine.ToString() );

                    foreach ( DataGridViewRow curRow in inDataGrid.Rows ) {
                        outLine = new StringBuilder( "" );
                        outLine.Append( "<tr>" );
                        foreach ( DataGridViewColumn curCol in inDataGrid.Columns ) {
                            if ( curCol.Visible ) {
                                if ( curRow.Cells[curCol.Name].Value == null ) {
                                    curValue = "&nbsp;";
                                } else {
                                    //String curX = inDataGrid.Columns[curCol.Name].
                                    //if ( curRow[curRoundName].GetType() == System.Type.GetType( "System.Byte" ) ) {
                                    curValue = curRow.Cells[curCol.Name].Value.ToString();
                                    if (curValue.Length == 0) curValue = "&nbsp;";
                                }
                                if ( curValue == null ) curValue = "&nbsp;";
                                outLine.Append( "<td>" + curValue + "</td>" );
                            }
                        }
                        outLine.Append( "</tr>" );
                        outBuffer.WriteLine( outLine.ToString() );
                    }

                    outLine = new StringBuilder( "" );
                    outLine.Append( "</table>"
                        + "<br><div Class=\"footer\">" + inFooter + "</div>"
                        );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "</div></body></html>" );
                    outBuffer.WriteLine( outLine.ToString() );

                    returnStatus = true;
                    outBuffer.Close();

                    if ( inDataGrid.Rows.Count > 0 ) {
                        curMsg = inDataGrid.Rows.Count + " rows found and written";
                    } else {
                        curMsg = "No rows found";
                    }
                    
					MessageBox.Show( curMsg );
                }
                
				Log.WriteFile( curMethodName + ":conplete: " + curMsg );
            
			} catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not write file from DataGridView\n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                returnStatus = false;
			
			} finally {
				if ( outBuffer != null ) outBuffer.Close();
			}
			
			return returnStatus;
        }

        private string findColValue( string inColName, string[] inputColNames, string[] inputCols ) {
            int idx = 0;
            foreach ( string colName in inputColNames ) {
                if ( inColName.Equals( colName ) ) {
                    return inputCols[idx].ToString();
                }
                idx++;
            }
            return null;
        }

    }
}
