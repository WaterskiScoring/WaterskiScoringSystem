using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Tools {
    class ExportRecordAppData {
        private String tabDelim = "\t";
        private String myRecordAppFormFileName = "IWWFRecordApplicationFormTemplate.xlsx";
        private String myExcelSheetName_IDSheet = "ID & CERTIFICATION PAGE";
        private String myDeploymentDirectory = "";

        //private const String myExcelAppUID = "Excel.Application";
        private const String myExcelAppUID = "Excel.ApplicationX";
        private object myExcelApp = null;
        private object myExcelWorkBooks, myExcelWorkBook, myExcelSheets, myExcelActiveSheet, myExcelRange;

        private StreamWriter myOutBuffer = null;

        public ExportRecordAppData() {
            String curMethodName = "ExportRecordAppData: ";
            try {
                //Create Excel application object
                Type curAppType = Type.GetTypeFromProgID( myExcelAppUID );
                if ( curAppType == null ) {
                    myExcelApp = null;
                    Log.WriteFile( curMethodName + "Excel not available" );
                    MessageBox.Show( "Excel not available" );

                } else {
                    myExcelApp = Activator.CreateInstance( curAppType );
                    MessageBox.Show( "ExcelApp instance created of type: " + myExcelApp.GetType() );
                    // make it visible
                    myExcelApp.GetType().InvokeMember( "Visible", BindingFlags.SetProperty, null, myExcelApp, new object[] { true } );
                    myExcelApp.GetType().InvokeMember( "DisplayAlerts", BindingFlags.SetProperty, null, myExcelApp, new object[] { false } );

                    try {
                        myDeploymentDirectory = Application.StartupPath;
                        if ( myDeploymentDirectory == null ) { myDeploymentDirectory = ""; }
                        if ( myDeploymentDirectory.Length < 1 ) {
                            myDeploymentDirectory = "C:\\Users\\AllenFamily\\Documents\\Visual Studio 2010\\Projects\\WaterskiScoringSystem\\WaterskiScoringSystem\\\bin\\Debug";
                        }

                    } catch ( Exception ex ) {
                        myExcelApp = null;
                        String curMsg = "Error initializing record application export processing" + "\n\nError: " + ex.Message;
                        Log.WriteFile( curMethodName + curMsg );
                        MessageBox.Show( curMsg );
                        //if ( myDeploymentDirectory == null ) { myDeploymentDirectory = ""; }
                        //if ( myDeploymentDirectory.Length < 1 ) {
                        //myDeploymentDirectory = "C:\\Users\\AllenFamily\\Documents\\Visual Studio 2010\\Projects\\WaterskiScoringSystem\\WaterskiScoringSystem\\\bin\\Debug";
                        //}
                    }
                }

            } catch ( Exception ex ) {
                myExcelApp = null;
                String curMsg = "Error initializing record application export processing" + "\n\nError: " + ex.Message;
                Log.WriteFile( curMethodName + curMsg );
                MessageBox.Show( curMsg );
            }
        }

        public bool ExportData( String inSanctionId, String inEvent, String inMemberId, String inDiv, String inEventGroup, Int16 inRound ) {
            String curMethodName = "ExportRecordAppData:ExportData: ";
            String curMsg = "";

            try {
                DataRow curTourRow = getTourData( inSanctionId );
                if ( curTourRow == null ) {
                    curMsg = "Tournament data not found, export bypassed";
                    Log.WriteFile( curMethodName + ":conplete: " + curMsg );
                    MessageBox.Show( curMsg );
                    return false;
                }

                writeRecordDataTextFile( curTourRow, inEvent, inMemberId, inDiv, inEventGroup, inRound );

                if ( myExcelApp != null ) {
                    writeRecordDataExcelFile( curTourRow, inEvent, inMemberId, inDiv, inEventGroup, inRound );
                }

            } catch ( Exception ex ) {
                MessageBox.Show( "Error:" + curMethodName + " Exception encountered in ExportRecordAppData processing:\n\n" + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );

            }
            return false;
        }

        private void writeRecordDataTextFile( DataRow inTourRow, String inEvent, String inMemberId, String inDiv, String inEventGroup, Int16 inRound ) {
            String curMethodName = "ExportRecordAppData:writeRecordDataTextFile: ";
            String curMsg = "";

            try {
                String curSanctionId = HelperFunctions.getDataRowColValue( inTourRow, "SanctionId", "" );
                String curSkierName = getSkierName( curSanctionId, inMemberId, inDiv );
                curSkierName = curSkierName.Replace( " ", "_" );
                String curFileName = String.Format( "RecordData_{0}_{1}_{2}_{3}_R{4}.txt"
                    , inEvent, curSkierName, inMemberId, inDiv, inRound );

                StringBuilder outLine = new StringBuilder( "" );
                myOutBuffer = getExportFile( curFileName );
                if ( myOutBuffer == null ) return;

                Log.WriteFile( "Export Record Data: " + curFileName );

                writeTourData( outLine, inTourRow );
                writeEventOfficials( outLine, curSanctionId, inEvent, inEventGroup, inRound.ToString() );
                writeSkierInfo( outLine, curSanctionId, inEvent, inMemberId, inDiv, inRound.ToString() );
                writePerfData( outLine, curSanctionId, inEvent, inMemberId, inDiv, inRound.ToString() );
                myOutBuffer.Close();

                curMsg = curMethodName + "data successfully exported";
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );

            } catch ( Exception ex ) {
                curMsg = curMethodName + "Exception=" + ex.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMsg );
            }
        }

        private void writeRecordDataExcelFile( DataRow inTourRow, String inEvent, String inMemberId, String inDiv, String inEventGroup, Int16 inRound ) {
            String curMethodName = "ExportRecordAppData:writeRecordDataExcelFile";
            String curMsg = "";
            String curSanctionId = HelperFunctions.getDataRowColValue( inTourRow, "SanctionId", "" );

            openExcelWorkbook( curSanctionId, inEvent, inMemberId, inDiv, inRound );
            removeUnusedSheets( inEvent );

            myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { myExcelSheetName_IDSheet } );
            myExcelActiveSheet.GetType().InvokeMember( "Activate", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );
            writeTourDataExcel( inTourRow, inEvent );
            writeSkierInfoExcel( curSanctionId, inEvent, inMemberId, inDiv, inRound.ToString() );

            myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { inEvent } );
            myExcelActiveSheet.GetType().InvokeMember( "Activate", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );
            writeEventOfficialsExcel( curSanctionId, inEvent, inEventGroup, inRound.ToString() );
            writeSkierBoatInfoExcel( curSanctionId, inEvent, inMemberId, inDiv, inRound.ToString() );
            writePerfDataExcel( curSanctionId, inEvent, inMemberId, inDiv, inRound.ToString() );

            saveExcelWorkbook( curSanctionId, inEvent, inMemberId, inDiv, inRound );

            curMsg = curMethodName + ":conplete";
            MessageBox.Show( curMsg );
            Log.WriteFile( curMsg );
        }

        private bool openExcelWorkbook( String inSanctionId, String inEvent, String inMemberId, String inDiv, Int16 inRound ) {
            String curMethodName = "ExportRecordAppData:openExcelWorkbook: ";
            
            try {
                String curSkierName = getSkierName( inSanctionId, inMemberId, inDiv );
                curSkierName = curSkierName.Replace( " ", "_" );
                String curFileName = String.Format( "\\RecordData_{0}_{1}_{2}_{3}_R{4}"
                    , inEvent, curSkierName, inMemberId, inDiv, inRound );

                String curExcelTemplateFileName = myDeploymentDirectory + "\\" + myRecordAppFormFileName;
                String curExcelFileName = Properties.Settings.Default.ExportDirectory + curFileName;
                MessageBox.Show( "ExcelFileName: " + curExcelFileName );
                
                myExcelWorkBooks = myExcelApp.GetType().InvokeMember( "Workbooks", BindingFlags.GetProperty, null, myExcelApp, null );
                myExcelWorkBook = myExcelWorkBooks.GetType().InvokeMember( "Open", BindingFlags.InvokeMethod, null, myExcelWorkBooks, new object[] { curExcelTemplateFileName, true } );
                myExcelSheets = myExcelWorkBook.GetType().InvokeMember( "Worksheets", BindingFlags.GetProperty, null, myExcelWorkBook, null );

                myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { myExcelSheetName_IDSheet } );
                myExcelActiveSheet.GetType().InvokeMember( "Activate", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );
                myExcelWorkBook.GetType().InvokeMember( "SaveAs", BindingFlags.InvokeMethod, null, myExcelWorkBook, new object[] { curExcelFileName } );
                return true;
            
            } catch ( Exception ex ) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMsg );
                return false;
            }
        }

        private bool saveExcelWorkbook( String inSanctionId, String inEvent, String inMemberId, String inDiv, Int16 inRound ) {
            String curMethodName = "ExportRecordAppData:saveExcelWorkbook: ";

            try {
                myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { myExcelSheetName_IDSheet } );
                myExcelActiveSheet.GetType().InvokeMember( "Activate", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );

                myExcelWorkBook.GetType().InvokeMember( "Save", BindingFlags.InvokeMethod, null, myExcelWorkBook, null );
                myExcelApp.GetType().InvokeMember( "DisplayAlerts", BindingFlags.SetProperty, null, myExcelApp, new object[] { false } );
                myExcelWorkBook.GetType().InvokeMember( "Close", BindingFlags.InvokeMethod, null, myExcelWorkBook, new object[] { true } );
                myExcelApp.GetType().InvokeMember( "DisplayAlerts", BindingFlags.SetProperty, null, myExcelApp, new object[] { true } );
                myExcelApp.GetType().InvokeMember( "Quit", System.Reflection.BindingFlags.InvokeMethod, null, myExcelApp, null );

                return true;

            } catch ( Exception ex ) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMsg );
                return false;
            }
        }

        private bool removeUnusedSheets( String inEvent ) {
            String curMethodName = "ExportRecordAppData:removeUnusedSheets: ";

            try {
                if ( inEvent.Equals( "Slalom" ) ) {
                    myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { "Trick" } );
                    myExcelActiveSheet.GetType().InvokeMember( "Delete", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );

                    myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { "Jump" } );
                    myExcelActiveSheet.GetType().InvokeMember( "Delete", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );

                } else if ( inEvent.Equals( "Trick" ) ) {
                    myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { "Slalom" } );
                    myExcelActiveSheet.GetType().InvokeMember( "Delete", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );

                    myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { "Jump" } );
                    myExcelActiveSheet.GetType().InvokeMember( "Delete", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );

                } else if ( inEvent.Equals( "Jump" ) ) {
                    myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { "Slalom" } );
                    myExcelActiveSheet.GetType().InvokeMember( "Delete", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );

                    myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { "Trick" } );
                    myExcelActiveSheet.GetType().InvokeMember( "Delete", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );
                }
            
            } catch ( Exception ex ) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMsg );
                return false;
            }

            return true;
        }

        //Write tournament information to Excel spreadsheet
        private bool writeTourDataExcel( DataRow inTourRow, String inEvent ) {
            String curMethodName = "ExportRecordAppData:writeTourDataExcel: ";
            
            try {
                DataRow curOfficialRow = null;
                DataRow curTechRow = getTechController( inTourRow["SanctionId"].ToString() );

                setCellValue(26, "B", HelperFunctions.getDataRowColValue( inTourRow, "Name", "" ) );
                setCellValue(26, "G", HelperFunctions.getDataRowColValue( inTourRow, "SanctionId", "" ) );
                String curEventLocation = HelperFunctions.getDataRowColValue( inTourRow, "EventLocation", "" );
                setCellValue( 28, "B", curEventLocation );
                if ( curEventLocation.Contains(" (")) {
                    int curDelim = curEventLocation.IndexOf( " (" );
                    int curDelimEnd = curEventLocation.IndexOf( ")" );
                    setCellValue( 28, "G", curEventLocation.Substring( curDelim + 2, curDelimEnd - curDelim - 2 ) );
                }

                curOfficialRow = getOfficialInfo( inTourRow["SanctionId"].ToString(), inTourRow["ChiefJudgeMemberId"].ToString() );
				if ( curOfficialRow  != null ) {
                    setCellValue( 41, "B", transformName( HelperFunctions.getDataRowColValue( inTourRow, "ChiefJudgeName", "" ) ) );
                    setCellValue( 41, "E", HelperFunctions.getDataRowColValue( curOfficialRow, "Judge" + inEvent + "Rating", "" ));
                }

                if ( curTechRow != null ) {
                    setCellValue( 43, "B", transformName( HelperFunctions.getDataRowColValue( curTechRow, "ChiefTechName", "" ) ) );
                    setCellValue( 43, "E", HelperFunctions.getDataRowColValue( curTechRow, "TechOfficialRating", "" ) );
                }

                curOfficialRow = getOfficialInfo( inTourRow["SanctionId"].ToString(), inTourRow["ChiefScorerMemberId"].ToString() );
                if ( curOfficialRow != null ) {
                    setCellValue( 45, "B", transformName( HelperFunctions.getDataRowColValue( inTourRow, "ChiefScorerName", "" ) ) );
                    setCellValue( 45, "E", HelperFunctions.getDataRowColValue( curOfficialRow, "Scorer" + inEvent + "Rating", "" ) );
                }

                curOfficialRow = getOfficialInfo( inTourRow["SanctionId"].ToString(), inTourRow["ChiefDriverMemberId"].ToString() );
				if ( curOfficialRow != null ) {
                    setCellValue( 47, "B", transformName( HelperFunctions.getDataRowColValue( inTourRow, "ChiefDriverName", "" ) ) );
                    setCellValue( 47, "E", HelperFunctions.getDataRowColValue( curOfficialRow, "Driver" + inEvent + "Rating", "" ) );
				}

                return true;

			} catch (Exception ex) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
        }

        private bool writeSkierInfoExcel( String inSanctionId, String inEvent, String inMemberId, String inDiv, String inRound) {
            String curMethodName = "ExportRecordAppData:writeSkierInfoExcel: ";
            DataRow curRow = null;

            try {
                DataTable curDataTable = getSkierInfo( inSanctionId, inEvent, inMemberId, inDiv, inRound );
                if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return false;

                curRow = curDataTable.Rows[0];
                setCellValue( 31, "B", transformName( HelperFunctions.getDataRowColValue( curRow, "SkierName", "" ) ) );
                setCellValue( 31, "F", HelperFunctions.getDataRowColValue( curRow, "Federation", "" ) );
                setCellValue( 33, "F", HelperFunctions.getDataRowColValue( curRow, "MemberId", "" ) );
                setCellValue( 35, "F", HelperFunctions.getDataRowColValue( curRow, "SkiYearAge", "" ) );
                setCellValue( 37, "B", HelperFunctions.getDataRowColValue( curRow, "City", "" ) + ", " + HelperFunctions.getDataRowColValue( curRow, "State", "" ) );

                setCellValue( 20, "B", HelperFunctions.getDataRowColValue( curRow, "EventClass", "" ) );
                setCellValue( 22, "B", HelperFunctions.getDataRowColValue( curRow, "Div", "" ) );
                setCellValue( 22, "D", inEvent );
                setCellValue( 24, "G", inRound );
                setCellValue( 24, "B", curRow["InsertDate"].ToString() );

                if ( inEvent.Equals( "Slalom" ) ) {
                    String curValue = String.Format( "{0} : {1} @ {2} {3} ({4}kph {5}M)"
                        , HelperFunctions.getDataRowColValueDecimal( curRow, "Score", "0", 2 )
                        , HelperFunctions.getDataRowColValueDecimal( curRow, "FinalPassScore", "0", 2 )
                        , HelperFunctions.getDataRowColValue( curRow, "FinalLenOff", "" )
                        , HelperFunctions.getDataRowColValueDecimal( curRow, "FinalSpeedMPH", "0", 0 )
                        , HelperFunctions.getDataRowColValueDecimal( curRow, "FinalSpeedKph", "0", 0 )
                        , HelperFunctions.getDataRowColValueDecimal( curRow, "FinalLen", "18.25", 2 )
                        );

                    setCellValue( 22, "G", curValue );

                } else if ( inEvent.Equals( "Jump" ) ) {
                    String curValue = String.Format( "{0}M ({1} Feet) @ {2}kph ({3}mph) {4} Height)"
                        , HelperFunctions.getDataRowColValueDecimal( curRow, "ScoreMeters", "0", 1 )
                        , HelperFunctions.getDataRowColValueDecimal( curRow, "Score", "0", 0 )
                        , HelperFunctions.getDataRowColValueDecimal( curRow, "BoatSpeedKph", "0", 0 )
                        , HelperFunctions.getDataRowColValueDecimal( curRow, "BoatSpeedMph", "0", 1 )
                        , HelperFunctions.getDataRowColValueDecimal( curRow, "RampHeight", "5", 0 )
                        );

                    setCellValue( 22, "G", curValue );

                } else {
                    setCellValue( 22, "G", HelperFunctions.getDataRowColValueDecimal( curRow, "Score", "0", 0 ) );
                }

                return true;
            
            } catch (Exception ex) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
        }

        private bool writeEventOfficialsExcel( String inSanctionId, String inEvent, String inEventGroup, String inRound ) {
            String curMethodName = "ExportRecordAppData:writeEventOfficialsExcel: ";
            DataRow curRow = null, curOfficialRow = null;

            String curRatingCol;
            int curIdx = 0;
            int curExcelRow = 4;
            int curMaxJudges = 0; //Slalom=5, trick=7, jump=4
            if ( inEvent.Equals( "Slalom" ) ) curMaxJudges = 5;
            else if ( inEvent.Equals( "Trick" ) ) curMaxJudges = 7;
            else if ( inEvent.Equals( "Jump" ) ) curMaxJudges = 4;

            try {
                DataTable curDataTable = getEventOfficials( inSanctionId, inEvent, inEventGroup, inRound );
                DataRow[] curRowsFound = curDataTable.Select( "WorkAsgmt = 'Scorer'" );
                if ( curRowsFound.Length > 0 ) {
                    curIdx = 0;
                    curRow = curRowsFound[curIdx];
                    curOfficialRow = getOfficialInfo( inSanctionId, HelperFunctions.getDataRowColValue( curRow, "MemberId", "" ) );
                    if ( curOfficialRow != null ) {
                        setCellValue( curExcelRow, "C", transformName( HelperFunctions.getDataRowColValue( curOfficialRow, "SkierName", "" ) ) );
                        curRatingCol = "G";
                        if ( inEvent.Equals("Slalom")) curRatingCol = "I";
                        setCellValue( curExcelRow, curRatingCol, HelperFunctions.getDataRowColValue( curOfficialRow, "Scorer" + inEvent + "Rating", "" ) );
                    }
                }
                curExcelRow++;

                curRowsFound = curDataTable.Select( "WorkAsgmt = 'Driver'" );
                if ( curRowsFound.Length > 0 ) {
                    curIdx = 0;
                    curRow = curRowsFound[curIdx];
                    curOfficialRow = getOfficialInfo( inSanctionId, HelperFunctions.getDataRowColValue( curRow, "MemberId", "" ) );
                    if ( curOfficialRow != null ) {
                        setCellValue( curExcelRow, "C", transformName( HelperFunctions.getDataRowColValue( curOfficialRow, "SkierName", "" ) ) );
                        curRatingCol = "G";
                        if ( inEvent.Equals( "Slalom" ) ) curRatingCol = "I";
                        setCellValue( curExcelRow, curRatingCol, HelperFunctions.getDataRowColValue( curOfficialRow, "Driver" + inEvent + "Rating", "" ) );
                    }
                }
                curExcelRow++;

                curRowsFound = curDataTable.Select( "WorkAsgmt = 'Boat Judge'" );
                if ( curRowsFound.Length > 0 ) {
                    curIdx = 0;
                    curRow = curRowsFound[curIdx];
                    curOfficialRow = getOfficialInfo( inSanctionId, HelperFunctions.getDataRowColValue( curRow, "MemberId", "" ) );
                    if ( curOfficialRow != null ) {
                        setCellValue( curExcelRow, "C", transformName( HelperFunctions.getDataRowColValue( curOfficialRow, "SkierName", "" ) ) );
                        curRatingCol = "G";
                        if ( inEvent.Equals( "Slalom" ) ) curRatingCol = "I";
                        setCellValue( curExcelRow, curRatingCol, HelperFunctions.getDataRowColValue( curOfficialRow, "Judge" + inEvent + "Rating", "" ) );
                    }
                }
                curExcelRow++;

                curRowsFound = curDataTable.Select( "WorkAsgmt = 'Event Judge' OR WorkAsgmt = 'End Course Official'" );
                if ( curRowsFound.Length > 0 ) {
                    for ( curIdx = 0; curIdx < (curRowsFound.Length) && (curIdx < curMaxJudges ) ; curIdx++ ) {
                        curRow = curRowsFound[curIdx];
                        curOfficialRow = getOfficialInfo( inSanctionId, HelperFunctions.getDataRowColValue( curRow, "MemberId", "" ) );
                        if ( curOfficialRow != null ) {
                            setCellValue( curExcelRow, "C", transformName( HelperFunctions.getDataRowColValue( curOfficialRow, "SkierName", "" ) ) );
                            curRatingCol = "G";
                            if ( inEvent.Equals( "Slalom" ) ) curRatingCol = "I";
                            setCellValue( curExcelRow, curRatingCol, HelperFunctions.getDataRowColValue( curOfficialRow, "Judge" + inEvent + "Rating", "" ) );
                            curExcelRow++;
                        }
                    }
                }

                return true;
            
            } catch ( Exception ex ) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
        }

        private bool writeSkierBoatInfoExcel( String inSanctionId, String inEvent, String inMemberId, String inDiv, String inRound ) {
            String curMethodName = "ExportRecordAppData:writeSkierBoatInfoExcel: ";
            int curExcelRow = 0;
            DataRow curRow = null;
            String curBoatModel = "", curExcelCol1 = "C", curExcelCol2 = "";
            String[] curBoatInfo = null;

            try {
                DataTable curDataTable = getSkierBoatInfo( inSanctionId, inMemberId, inDiv, inRound, inEvent );
                if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return false;

                curRow = curDataTable.Rows[0];
                curBoatModel = HelperFunctions.getDataRowColValue( curRow, "BoatModel", "" );

                if ( inEvent.Equals( "Slalom" ) ) {
                    curExcelRow = 14;
                    curExcelCol2 = "I";
                    
                    setCellValue( curExcelRow, curExcelCol1, curBoatModel );
                    setCellValue( curExcelRow + 1, curExcelCol1, HelperFunctions.getDataRowColValue( curRow, "ModelYear", "" ) );
                    setCellValue( curExcelRow + 1, curExcelCol2, HelperFunctions.getDataRowColValue( curRow, "SpeedControlVersion", "" ) );

                } else if ( inEvent.Equals( "Trick" ) ) {
                    curExcelRow = 16;
                    curExcelCol2 = "C";

                    setCellValue( curExcelRow, curExcelCol1, curBoatModel );
                    setCellValue( curExcelRow + 1, curExcelCol1, HelperFunctions.getDataRowColValue( curRow, "ModelYear", "" ) );
                    curExcelRow = 19;
                    setCellValue( curExcelRow + 1, curExcelCol2, HelperFunctions.getDataRowColValue( curRow, "SpeedControlVersion", "" ) );

                } else if ( inEvent.Equals( "Jump" ) ) {
                    curExcelRow = 13;
                    curExcelCol2 = "G";

                    setCellValue( curExcelRow, curExcelCol1, curBoatModel );
                    setCellValue( curExcelRow + 1, curExcelCol1, HelperFunctions.getDataRowColValue( curRow, "ModelYear", "" ) );
                    setCellValue( curExcelRow + 1, curExcelCol2, HelperFunctions.getDataRowColValue( curRow, "SpeedControlVersion", "" ) );
                }

                if ( HelperFunctions.isObjectPopulated( curBoatModel) ) {
                    curBoatInfo = curBoatModel.Split( ' ' );
                    if ( curBoatInfo.Length > 0 ) {
                        if ( curBoatInfo.Equals( "Ski" ) ) {
                            setCellValue( curExcelRow, curExcelCol2, curBoatInfo[0] + "" + curBoatInfo[1] );
                        } else {
                            setCellValue( curExcelRow, curExcelCol2, curBoatInfo[0] );
                        }
                    }
                }

                return true;

            } catch ( Exception ex ) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
        }

        private bool writePerfDataExcel( String inSanctionId, String inEvent, String inMemberId, String inDiv, String inRound ) {
            String curMethodName = "ExportRecordAppData:writeSkierInfoExcel: ";

            try {
                if ( inEvent.Equals( "Slalom" ) ) {
                    writeSlalomDataDetailExcel( 19, inSanctionId, inMemberId, inDiv, inRound );
                
                } else if ( inEvent.Equals( "Trick" ) ) {
                    writeTrickDataDetailExcel( inSanctionId, inMemberId, inDiv, inRound );
                
                } else if ( inEvent.Equals( "Jump" ) ) {
                    writeJumpDataDetailExcel( 18, inSanctionId, inMemberId, inDiv, inRound );
                }
                
                return true;

            } catch ( Exception ex ) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
        }

        private bool writeSlalomDataDetailExcel(int inStartRow, String inSanctionId, String inMemberId, String inDiv, String inRound ) {
            int curExcelRow = inStartRow;
            int curExcelCol = 0;
            StringBuilder curRerideNotes = new StringBuilder( "" );


            DataTable curDataTable = getSkierSlalomRecap( inSanctionId, inMemberId, inDiv, inRound );
            if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return false;

            foreach ( DataRow curRow in curDataTable.Rows ) {
                decimal curPassLineLength = HelperFunctions.getDataRowColValueDecimal( curRow, "PassLineLength", 18.25M );
                curExcelCol = 2 + getSlalomLineData( curPassLineLength );

                String curSkierRunNum = HelperFunctions.getDataRowColValue( curRow, "SkierRunNum", "0" );
                String curRerideReason = HelperFunctions.getDataRowColValue( curRow, "RerideReason", "" );
                if ( HelperFunctions.isObjectPopulated(curRerideReason)) {
                    if ( curRerideNotes.Length > 1 ) curRerideNotes.Append( "\n" );
                    curRerideNotes.Append( String.Format("Pass: {0} Reride Note: {1}", curSkierRunNum, curRerideReason ));
                }

                setCellValue( curExcelRow, curExcelCol, HelperFunctions.getDataRowColValueDecimal( curRow, "Judge1Score", "0", 2 ) );
                setCellValue( curExcelRow + 1, curExcelCol, HelperFunctions.getDataRowColValueDecimal( curRow, "Judge2Score", "0", 2 ) );
                setCellValue( curExcelRow + 2, curExcelCol, HelperFunctions.getDataRowColValueDecimal( curRow, "Judge3Score", "0", 2 ) );

                if ( HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curRow, "Judge4Score", "" ) ) ) {
                    setCellValue( curExcelRow + 3, curExcelCol, HelperFunctions.getDataRowColValueDecimal( curRow, "Judge4Score", "0", 2 ) );
                } else {
                    setCellValue( curExcelRow + 3, curExcelCol, "" );
                }
                if ( HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curRow, "Judge5Score", "" ) ) ) {
                    setCellValue( curExcelRow + 4, curExcelCol, HelperFunctions.getDataRowColValueDecimal( curRow, "Judge5Score", "0", 2 ) );
                } else {
                    setCellValue( curExcelRow + 4, curExcelCol, "" );
                }

                setCellValue( curExcelRow + 7, curExcelCol, HelperFunctions.getDataRowColValueDecimal( curRow, "Score", "0", 2 ) );
                
                for ( int curIdx = 0; curIdx < 7; curIdx++ ) setCellValue( curExcelRow + 11 + curIdx, curExcelCol, "" );
                String curPassScore = HelperFunctions.getDataRowColValue( curRow, "Score", "" );
                int curTimeBuoy = 1 + Convert.ToInt32( curPassScore.Substring( 0, 1 ) );
                setCellValue( curExcelRow + 10 + curTimeBuoy, curExcelCol, HelperFunctions.getDataRowColValueDecimal( curRow, "BoatTime", "0", 2 ) );
            }

            if ( curRerideNotes.Length > 1 ) {
                setCellValue( inStartRow, "L", curRerideNotes.ToString() );
            }

            return exportBoatPathSlalomDataExcel( inMemberId, inRound, curDataTable );
        }

        private bool writeTrickDataDetailExcel( String inSanctionId, String inMemberId, String inDiv, String inRound ) {
            int curExcelRowStart = 25, curExcelRowSubtotals = 50;
            int curTotalPass1 = 0, curTotalPass2 = 0;
            String curResult;

            DataTable curPass1DataTable = getSkierTrickPass( inSanctionId, inMemberId, inDiv, inRound, "1" );
            DataTable curPass2DataTable = getSkierTrickPass( inSanctionId, inMemberId, inDiv, inRound, "2" );

            int curExcelRow = curExcelRowStart;
            foreach ( DataRow curRow in curPass1DataTable.Rows ) {
                curResult = HelperFunctions.getDataRowColValue( curRow, "Results", "Credit" );
                setCellValue( curExcelRow, "C", HelperFunctions.getDataRowColValue( curRow, "Code", "N/A" ) );
                if ( curResult.Equals("Credit") ) {
                    setCellValue( curExcelRow, "D", HelperFunctions.getDataRowColValueDecimal( curRow, "Score", "0", 0 ) );
                    curTotalPass1 = curTotalPass1 + Convert.ToInt32( HelperFunctions.getDataRowColValueDecimal( curRow, "Score", "0", 0 ) );
                } else {
                    setCellValue( curExcelRow, "D", curResult );
                }
                curExcelRow++;
            }

            curExcelRow = curExcelRowStart;
            foreach ( DataRow curRow in curPass2DataTable.Rows ) {
                curResult = HelperFunctions.getDataRowColValue( curRow, "Results", "Credit" );
                setCellValue( curExcelRow, "F", HelperFunctions.getDataRowColValue( curRow, "Code", "N/A" ) );
                if ( curResult.Equals( "Credit" ) ) {
                    setCellValue( curExcelRow, "G", HelperFunctions.getDataRowColValueDecimal( curRow, "Score", "0", 0 ) );
                    curTotalPass2 = curTotalPass2 + Convert.ToInt32( HelperFunctions.getDataRowColValueDecimal( curRow, "Score", "0", 0 ) );
                } else {
                    setCellValue( curExcelRow, "G", curResult );
                }
                curExcelRow++;
            }

            curExcelRow = curExcelRowSubtotals;
            setCellValue( curExcelRow, "D", curTotalPass1.ToString() );
            setCellValue( curExcelRow, "G", curTotalPass2.ToString() );

            setCellValue( curExcelRow + 3, "G", curTotalPass1.ToString() );
            setCellValue( curExcelRow + 4, "G", curTotalPass2.ToString() );
            setCellValue( curExcelRow + 5, "G", (curTotalPass1 + curTotalPass2).ToString() );

            /*
            setCellValue( curExcelRow, "A", "Note:" );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "A" + curExcelRow.ToString() } );
            setCellValue( curExcelRow, "B", "Attach to this form, copies of the trick run called by the the judges' sheets." );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
             */

            return true;
        }

        private bool writeJumpDataDetailExcel( int inStartRow, String inSanctionId, String inMemberId, String inDiv, String inRound ) {
            int curExcelRow = inStartRow;

            /*
            SELECT R.PassNum, R.LastUpdateDate, R.RampHeight, R.BoatSpeed as BoatSpeedKph, MinValue as BoatSpeedMph
            , R.ScoreFeet, R.ScoreMeters, Results
            , BoatSplitTime2 as Split82Time, BoatEndTime as Split41Time, BoatSplitTime as Split52Time
            , ReturnToBase, ScoreProt, Reride, RerideReason, R.Note as RideNote, S.Note as ScoreNote
             */
            DataTable curDataTable = getSkierJumpRecap( inSanctionId, inMemberId, inDiv, inRound );
            DataRow curRow = curDataTable.Rows[0];
            setCellValue( curExcelRow, "C", HelperFunctions.getDataRowColValueDecimal( curRow, "ScoreMeters", "0", 1 ) );
            setCellValue( curExcelRow, "E", HelperFunctions.getDataRowColValueDecimal( curRow, "ScoreFeet", "0", 0 ) );
            setCellValue( curExcelRow + 1, "C", HelperFunctions.getDataRowColValue( curRow, "RampHeight", "" ) );
            setCellValue( curExcelRow + 2, "C", HelperFunctions.getDataRowColValueDecimal( curRow, "BoatSpeedKph", "0", 0 ) );
            setCellValue( curExcelRow + 2, "E", HelperFunctions.getDataRowColValueDecimal( curRow, "BoatSpeedMph", "0", 0 ) );

            setCellValue( curExcelRow + 5, "C", HelperFunctions.getDataRowColValueDecimal( curRow, "Split52Time", "0", 2 ) );
            setCellValue( curExcelRow + 5, "E", HelperFunctions.getDataRowColValueDecimal( curRow, "Split82Time", "0", 2 ) );
            setCellValue( curExcelRow + 5, "G", HelperFunctions.getDataRowColValueDecimal( curRow, "Split41Time", "0", 2 ) );

            curExcelRow = inStartRow + 9;
            setCellValue( curExcelRow, "C", HelperFunctions.getDataRowColValue( curRow, "SkierBoatPath", "" ) );

            return exportBoatPathJumpDataExcel( inMemberId, inRound, curRow );
        }

        //Write tournament information
        private bool writeTourData(StringBuilder outLine, DataRow inTourRow) {
            String curMethodName = "ExportRecordAppData:writeTourData: ";
            String curSanctionId = HelperFunctions.getDataRowColValue( inTourRow, "SanctionId", "" );
            
            try {
                DataRow curOfficialRow = null;

                outLine = new StringBuilder( "" );
                outLine.Append( "Tournament summary as of " + DateTime.Now.ToString( "HH:MM:ss  MM-dd-yyyy" ) );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "SanctionId" + tabDelim + "Name" + tabDelim + "Class" + tabDelim + "Rules" + tabDelim + "EventDates" + tabDelim + "EventLocation" + tabDelim + "Tour Director" );
                outLine.Append( tabDelim + "Slalom Rds" + tabDelim + "Trick Rds" + tabDelim + "Jump Rds" );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( curSanctionId );
                outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "Name", "" ) );
                outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "Class", "" ) );
                outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "Rules", "" ) );
                outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "EventDates", "" ) );
                outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "EventLocation", "" ) );
                outLine.Append( tabDelim + transformName( HelperFunctions.getDataRowColValue( inTourRow, "ContactName", "" ) ) );
                outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "SlalomRounds", "" ) );
                outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "TrickRounds", "" ) );
                outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "JumpRounds", "" ) );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Chief Judge" + tabDelim + transformName( HelperFunctions.getDataRowColValue( inTourRow, "ChiefJudgeName", "" ) ) );
                HelperFunctions.getDataRowColValue( inTourRow, "ChiefJudgeMemberId", "" );
                curOfficialRow = getOfficialInfo( curSanctionId, inTourRow["ChiefJudgeMemberId"].ToString() );
				if ( curOfficialRow  != null ) {
					outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "JudgeSlalomRating", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "ChiefJudgeAddress", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "ChiefJudgePhone", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "ChiefJudgeEmail", "" ) );
					myOutBuffer.WriteLine( outLine.ToString() );
				}

				outLine = new StringBuilder( "" );
                outLine.Append( "Chief Driver" + tabDelim + transformName( HelperFunctions.getDataRowColValue( inTourRow, "ChiefDriverName", "" ) ) );
                curOfficialRow = getOfficialInfo( curSanctionId, HelperFunctions.getDataRowColValue( inTourRow, "ChiefJudgeMemberId", "" ) );
				if ( curOfficialRow != null ) {
					outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "DriverSlalomRating", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "ChiefDriverAddress", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "ChiefDriverPhone", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "ChiefDriverEmail", "" ) );
					myOutBuffer.WriteLine( outLine.ToString() );
				}

				outLine = new StringBuilder( "" );
                outLine.Append( "Chief Scorer" + tabDelim + transformName( HelperFunctions.getDataRowColValue( inTourRow, "ChiefScorerName", "" ) ) );
                curOfficialRow = getOfficialInfo( curSanctionId, HelperFunctions.getDataRowColValue( inTourRow, "ChiefScorerMemberId", "" ) );
				if ( curOfficialRow != null ) {
					outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "ScorerSlalomRating", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "ChiefScorerAddress", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "ChiefScorerPhone", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "ChiefScorerEmail", "" ) );
					myOutBuffer.WriteLine( outLine.ToString() );
				}

				outLine = new StringBuilder( "" );
                outLine.Append( "Safety Director" + tabDelim + transformName( HelperFunctions.getDataRowColValue( inTourRow, "ChiefSafetyName", "" ) ) );
                curOfficialRow = getOfficialInfo( curSanctionId, HelperFunctions.getDataRowColValue( inTourRow, "SafetyDirMemberId", "" ) );
				if ( curOfficialRow != null ) {
					outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "SafetyOfficialRating", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "SafetyDirAddress", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "SafetyDirPhone", "" ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( inTourRow, "SafetyDirEmail", "" ) );
					myOutBuffer.WriteLine( outLine.ToString() );
				}

				outLine = new StringBuilder( "" );
                outLine.Append( "Tech Controller" );
                DataRow curTechRow = getTechController( curSanctionId );
                if ( curTechRow == null) {
                    outLine.Append( tabDelim + "" + tabDelim + "" );
                } else {
                    outLine.Append( tabDelim + transformName( HelperFunctions.getDataRowColValue( curTechRow, "ChiefTechName", "" ) ) );
                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curTechRow, "TechOfficialRating", "" ) );
                }
                myOutBuffer.WriteLine( outLine.ToString() );

                return true;
            
            } catch (Exception ex) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
        }

        private bool writeEventOfficials(StringBuilder outLine, String inSanctionId, String inEvent, String inEventGroup, String inRound) {
            String curMethodName = "ExportRecordAppData:writeEventOfficials: ";
            DataRow curRow = null, curOfficialRow = null;

            try {
                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );
                outLine.Append( "Assignment" + tabDelim + "Name" + tabDelim + "Rating" );
                myOutBuffer.WriteLine( outLine.ToString() );

                DataTable curDataTable = getEventOfficials( inSanctionId, inEvent, inEventGroup, inRound );
                DataRow[] curRowsFound = curDataTable.Select( "WorkAsgmt = 'Boat Judge'" );
                if (curRowsFound.Length > 0) {
                    curRow = curRowsFound[0];
                    curOfficialRow = getOfficialInfo( inSanctionId, HelperFunctions.getDataRowColValue( curRow, "MemberId", "" ) );
                    if (curOfficialRow != null) {
                        outLine = new StringBuilder( "" );
                        outLine.Append( HelperFunctions.getDataRowColValue( curRow, "WorkAsgmt", "WorkAsgmt" ) );
                        outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "SkierName", "" ) );
                        if ( inEvent.Equals( "Slalom" )) {
                            outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "JudgeSlalomRating", "" ) );
                        } else if (inEvent.Equals( "Trick" )) {
                            outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "JudgeTrickRating", "" ) );
                        } else if (inEvent.Equals( "Jump" )) {
                            outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "JudgeJumpRating", "" ) );
                        }
                        myOutBuffer.WriteLine( outLine.ToString() );
                    }
                
                } else {
                    outLine = new StringBuilder( "" );
                    outLine.Append( "Boat Judge" + tabDelim + " " + tabDelim + " " );
                    myOutBuffer.WriteLine( outLine.ToString() );
                }
                
                curRowsFound = curDataTable.Select( "WorkAsgmt = 'Driver'" );
                if (curRowsFound.Length > 0) {
                    curRow = curRowsFound[0];
                    curOfficialRow = getOfficialInfo( inSanctionId, HelperFunctions.getDataRowColValue( curRow, "MemberId", "" ) );
                    if ( curOfficialRow != null ) {
                        outLine = new StringBuilder( "" );
                        outLine.Append( HelperFunctions.getDataRowColValue( curRow, "WorkAsgmt", "WorkAsgmt" ) );
                        outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "SkierName", "" ) );
                        if ( inEvent.Equals( "Slalom" ) ) {
                            outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "DriverSlalomRating", "" ) );
                        } else if ( inEvent.Equals( "Trick" ) ) {
                            outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "DriverTrickRating", "" ) );
                        } else if ( inEvent.Equals( "Jump" ) ) {
                            outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "DriverJumpRating", "" ) );
                        }
                        myOutBuffer.WriteLine( outLine.ToString() );
                    }

                } else {
                    outLine = new StringBuilder( "" );
                    outLine.Append( "Driver" + tabDelim + " " + tabDelim + " " );
                    myOutBuffer.WriteLine( outLine.ToString() );
                }

                curRowsFound = curDataTable.Select( "WorkAsgmt = 'Event Judge' OR WorkAsgmt = 'End Course Official'" );
                if (curRowsFound.Length > 0) {
                    for (int curIdx = 0; curIdx < 5; curIdx++) {
                        outLine = new StringBuilder( "" );
                        if (curIdx < curRowsFound.Length) {
                            curRow = curRowsFound[curIdx];
                            curOfficialRow = getOfficialInfo( inSanctionId, HelperFunctions.getDataRowColValue( curRow, "MemberId", "" ) );
                            if (curOfficialRow != null) {
                                outLine = new StringBuilder( "" );
                                outLine.Append( HelperFunctions.getDataRowColValue( curRow, "WorkAsgmt", "WorkAsgmt" ) );
                                outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "SkierName", "" ) );
                                if ( inEvent.Equals( "Slalom" ) ) {
                                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "JudgeSlalomRating", "" ) );
                                } else if ( inEvent.Equals( "Trick" ) ) {
                                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "JudgeTrickRating", "" ) );
                                } else if ( inEvent.Equals( "Jump" ) ) {
                                    outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "JudgeJumpRating", "" ) );
                                }
                                myOutBuffer.WriteLine( outLine.ToString() );
                            }
                        
                        } else {
                            outLine.Append( "Event Judge" + tabDelim + " " + tabDelim + " " );
                            myOutBuffer.WriteLine( outLine.ToString() );
                        }
                    }
                
                } else {
                    outLine = new StringBuilder( "" );
                    outLine.Append( "Event Judge" + tabDelim + " " + tabDelim + " " );
                    myOutBuffer.WriteLine( outLine.ToString() );
                    myOutBuffer.WriteLine( outLine.ToString() );
                    myOutBuffer.WriteLine( outLine.ToString() );
                    myOutBuffer.WriteLine( outLine.ToString() );
                    myOutBuffer.WriteLine( outLine.ToString() );
                }
                
                curRowsFound = curDataTable.Select( "WorkAsgmt = 'Scorer'" );
                if (curRowsFound.Length > 0) {
                    curRow = curRowsFound[0];
                    curOfficialRow = getOfficialInfo( inSanctionId, HelperFunctions.getDataRowColValue( curRow, "MemberId", "" ) );
                    if ( curOfficialRow != null ) {
                        outLine = new StringBuilder( "" );
                        outLine.Append( HelperFunctions.getDataRowColValue( curRow, "WorkAsgmt", "WorkAsgmt" ) );
                        outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "SkierName", "" ) );
                        if ( inEvent.Equals( "Slalom" ) ) {
                            outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "ScorerSlalomRating", "" ) );
                        } else if ( inEvent.Equals( "Trick" ) ) {
                            outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "ScorerTrickRating", "" ) );
                        } else if ( inEvent.Equals( "Jump" ) ) {
                            outLine.Append( tabDelim + HelperFunctions.getDataRowColValue( curOfficialRow, "ScorerJumpRating", "" ) );
                        }
                        myOutBuffer.WriteLine( outLine.ToString() );
                    }

                } else {
                    outLine = new StringBuilder( "" );
                    outLine.Append( "Scorer" + tabDelim + " " + tabDelim + " " );
                    myOutBuffer.WriteLine( outLine.ToString() );
                }

                return true;

            } catch (Exception ex) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
        }

        private bool writeSkierInfo(StringBuilder outLine, String inSanctionId, String inEvent, String inMemberId, String inDiv, String inRound) {
            String curMethodName = "ExportRecordAppData:writeSkierInfo: ";

            try {
                exportDataTable( outLine, getSkierInfo( inSanctionId, inEvent, inMemberId, inDiv, inRound ) );
                return true;
            
            } catch (Exception ex) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
        }

        private bool writePerfData(StringBuilder outLine, String inSanctionId, String inEvent, String inMemberId, String inDiv, String inRound) {
			try {
                if (inEvent.Equals( "Slalom" )) {
					DataTable curDataTable = getSkierSlalomRecap( inSanctionId, inMemberId, inDiv, inRound );
					exportDataTable( outLine, curDataTable );
					exportBoatPathSlalomDataText( outLine, inMemberId, inRound, curDataTable );

				} else if (inEvent.Equals( "Trick" )) {
                    DataTable curDataTable = getSkierTrickPass( inSanctionId, inMemberId, inDiv, inRound, "1"  );
                    exportDataTable( outLine, curDataTable );
                    outLine = new StringBuilder( "" );
                    for (int curIdx = curDataTable.Rows.Count + 1; curIdx < 26; curIdx++) {
                        myOutBuffer.WriteLine( outLine.ToString() );
                    }
                    curDataTable = getSkierTrickPass( inSanctionId, inMemberId, inDiv, inRound, "2" );
                    exportDataTable( outLine, curDataTable );
                    outLine = new StringBuilder( "" );
                    for (int curIdx = curDataTable.Rows.Count + 1; curIdx < 26; curIdx++) {
                        myOutBuffer.WriteLine( outLine.ToString() );
                    }

				} else if (inEvent.Equals( "Jump" )) {
					DataTable curDataTable = getSkierJumpRecap( inSanctionId, inMemberId, inDiv, inRound );
					exportDataTable( outLine, curDataTable );
					exportBoatPathJumpDataText( outLine, inMemberId, inRound, curDataTable );
				}

				return true;

			} catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData writeEventOfficials method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private bool exportDataTableExcel(int inStartRow, DataTable inDataTable) {
            String curMethodName = "ExportRecordAppData:exportDataTableExcel: ";
            int colCount = 0, curExcelRow = inStartRow;
            String curValue;

            try {
                colCount = 0;
                foreach (DataColumn curCol in inDataTable.Columns) {
                    colCount++;
                    setCellValue(curExcelRow, colCount, curCol.ColumnName);
                }

                foreach (DataRow curRow in inDataTable.Rows) {
                    colCount = 0;
                    curExcelRow++;
                    foreach (DataColumn curCol in inDataTable.Columns) {
                        curValue = cleanLabel( curRow[curCol.ColumnName].ToString() );
                        if (curCol.ColumnName.ToLower().Equals( "memberid" )) {
							//String curTempValue = "'" + curValue;
							//curValue = curTempValue;

						} else if (curCol.ColumnName.ToLower().Equals( "skiername" )) {
                            String curTempValue = transformName( curValue );
                            curValue = curTempValue;
                            colCount++;
                            setCellValue(curExcelRow, colCount, curValue);

						} else {
                            colCount++;
                            setCellValue(curExcelRow, colCount, curValue);
                        }
                    }
                }

                return true;
            
            } catch (Exception ex) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
        }

        private bool exportDataTable(StringBuilder outLine, DataTable inDataTable) {
            String curMethodName = "ExportRecordAppData:exportDataTable: ";
            int colCount = 0;
            String curValue;
            myOutBuffer.WriteLine( outLine.ToString() );

            try {
                outLine = new StringBuilder( "" );
                colCount = 0;
                foreach (DataColumn curCol in inDataTable.Columns) {
                    colCount++;
                    if (colCount < inDataTable.Columns.Count) {
                        outLine.Append( curCol.ColumnName + tabDelim );
                    } else {
                        outLine.Append( curCol.ColumnName );
                    }
                }
                myOutBuffer.WriteLine( outLine.ToString() );

                foreach (DataRow curRow in inDataTable.Rows) {
                    outLine = new StringBuilder( "" );
                    colCount = 0;
                    foreach (DataColumn curCol in inDataTable.Columns) {
                        colCount++;
                        curValue = curRow[curCol.ColumnName].ToString();
                        curValue = curValue.Replace( '\n', ' ' );
                        curValue = curValue.Replace( '\r', ' ' );
                        curValue = curValue.Replace( '\t', ' ' );
                        if (curCol.ColumnName.ToLower().Equals( "memberid" )) {
                            //String curTempValue = "'" + curValue;
                            //curValue = curTempValue;
                        } else if (curCol.ColumnName.ToLower().Equals( "skiername" )) {
                            String curTempValue = transformName(curValue);
                            curValue = curTempValue;
                        }
                        if (colCount < inDataTable.Columns.Count) {
                            outLine.Append( curValue + tabDelim );
                        } else {
                            outLine.Append( curValue );
                        }
                    }
                    myOutBuffer.WriteLine( outLine.ToString() );
                    outLine = new StringBuilder( "" );
                }

                return true;

			} catch (Exception ex) {
                String curMsg = curMethodName + "Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
        }

		private bool exportBoatPathSlalomDataExcel( String memberId, String round, DataTable curDataTable ) {
            String curMethodName = "ExportRecordAppData:exportBoatPathSlalomDataExcel";
            int curExcelRow = 40;
			DataRow boatPathRow;
			Decimal passScore;
			Int16 passNum;

			try {
                for ( int curIdx = curDataTable.Rows.Count - 2; curIdx < curDataTable.Rows.Count; curIdx++ ) {
                    DataRow curRow = curDataTable.Rows[curIdx];
                    passNum = (Int16)curRow["SkierRunNum"];
					passScore = (Decimal)curRow["Score"];
                    
                    decimal curPassLineLength = HelperFunctions.getDataRowColValueDecimal( curRow, "PassLineLength", 18.25M );
                    byte curPassSpeedKph = Convert.ToByte( HelperFunctions.getDataRowColValue( curRow, "PassSpeedKph", "0" ));
                    boatPathRow = WscHandler.getBoatPath( "Slalom", memberId, round, passNum.ToString(), curPassLineLength, curPassSpeedKph );
					if ( boatPathRow == null ) continue;

					int curColNum = 4;
					for ( int colIdx = 1; colIdx <= 7; colIdx++, curColNum++ ) {
						if ( passScore < ( colIdx - 1 ) ) break;
                        setCellValue( curExcelRow, curColNum, HelperFunctions.getDataRowColValueDecimal( boatPathRow, "boatTimeBuoy" + colIdx, "0", 2 ) );
					}

					curExcelRow++;
                    curColNum = 3;
                    for ( int colIdx = 0; colIdx <= 6; colIdx++, curColNum++ ) {
                        if ( passScore < colIdx ) break;
                        setCellValue( curExcelRow, curColNum, HelperFunctions.getDataRowColValueDecimal( boatPathRow, "PathDevBuoy" + colIdx, "0", 2 ) );
                    }

                    curExcelRow++;
                    curColNum = 3;
                    for ( int colIdx = 0; colIdx <= 6; colIdx++, curColNum++ ) {
                        if ( passScore < colIdx ) break;
                        setCellValue( curExcelRow, curColNum, HelperFunctions.getDataRowColValueDecimal( boatPathRow, "PathDevCum" + colIdx, "0", 2 ) );
                    }
                    curExcelRow++;
                    curExcelRow++;
                }

            } catch ( Exception ex ) {
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }

            return true;
		}

		private void exportBoatPathSlalomDataText( StringBuilder outLine, String memberId, String round, DataTable curDataTable ) {
			DataRow boatPathRow;
			Decimal passScore;
			Int16 passNum;

			try {
				Boolean writeHeader = true;
				foreach ( DataRow curRow in curDataTable.Rows ) {
					passNum = (Int16)curRow["SkierRunNum"];
					passScore = (Decimal)curRow["Score"];
					boatPathRow = WscHandler.getBoatPath( "Slalom", memberId, round, passNum.ToString(), (decimal)curRow["PassLineLength"], (byte)curRow["PassSpeedKph"] );
					if ( boatPathRow == null ) continue;

					if ( writeHeader ) {
						writeHeader = false;
						myOutBuffer.WriteLine( outLine.ToString() );

						outLine = new StringBuilder( "Boat Path Monitoring Data" );
						myOutBuffer.WriteLine( outLine.ToString() );

						outLine = new StringBuilder( "" );
						outLine.Append( "Pass" + tabDelim );
						outLine.Append( "Desc" + tabDelim );
						outLine.Append( "Gate" + tabDelim );
						outLine.Append( "Buoy 1" + tabDelim );
						outLine.Append( "Buoy 2" + tabDelim );
						outLine.Append( "Buoy 3" + tabDelim );
						outLine.Append( "Buoy 4" + tabDelim );
						outLine.Append( "Buoy 5" + tabDelim );
						outLine.Append( "Buoy 6" + tabDelim );
						outLine.Append( "Exit" + tabDelim );
						myOutBuffer.WriteLine( outLine.ToString() );
					}

					outLine = new StringBuilder( ( (byte)boatPathRow["PassNumber"] ).ToString() + tabDelim + "Time" + tabDelim );
					for ( int rowIdx = 1; rowIdx <= 7; rowIdx++ ) {
						if ( passScore < ( rowIdx - 1 ) ) break;
						outLine.Append( tabDelim + ( (Decimal)boatPathRow["boatTimeBuoy" + rowIdx] ).ToString() );
					}
					myOutBuffer.WriteLine( outLine.ToString() );

					outLine = new StringBuilder( tabDelim + "Dev" );
					for ( int rowIdx = 0; rowIdx <= 6; rowIdx++ ) {
						if ( passScore < ( rowIdx - 1 ) ) break;
						outLine.Append( tabDelim + ( (Decimal)boatPathRow["PathDevBuoy" + rowIdx] ).ToString() );
					}
					myOutBuffer.WriteLine( outLine.ToString() );

					outLine = new StringBuilder( tabDelim + "Cum" );
					for ( int rowIdx = 0; rowIdx <= 6; rowIdx++ ) {
						if ( passScore < ( rowIdx - 1 ) ) break;
						outLine.Append( tabDelim + ( (Decimal)boatPathRow["PathDevCum" + rowIdx] ).ToString() );
					}
					myOutBuffer.WriteLine( outLine.ToString() );
				}

			} catch ( Exception ex ) {
				MessageBox.Show( "Exception encountered in ExportRecordAppData exportBoatPathSlalomDataText method"
					+ "\n\nException: " + ex.Message
				);
			}
		}

        /*
         * 180,ST,NT,MT,ET and EC is good for me as points
        Idx == 0 "180M"
        Idx == 1 "ST"
        Idx == 2 "52M"
        Idx == 3 "82M"
        Idx == 4 "41M"
        Idx == 5 "EC"
         */
        private bool exportBoatPathJumpDataExcel( String memberId, String round, DataRow inPassRow ) {
            String curMethodName = "ExportRecordAppData:exportBoatPathJumpDataExcel";
            int curExcelRow = 29;

            try {
                String curPassNum = HelperFunctions.getDataRowColValue( inPassRow, "PassNum", "0" );
                byte curBoatSpeedKph = Convert.ToByte( HelperFunctions.getDataRowColValueDecimal( inPassRow, "BoatSpeedKph", "0", 0 ) );
                DataRow boatPathRow = WscHandler.getBoatPath( "Jump", memberId, round, curPassNum, (decimal)0, curBoatSpeedKph );
                if ( boatPathRow == null ) return true;

                setCellValue( curExcelRow, "B", HelperFunctions.getDataRowColValueDecimal( boatPathRow, "PathDevBuoy1", "0", 2 ) );
                setCellValue( curExcelRow, "D", HelperFunctions.getDataRowColValueDecimal( boatPathRow, "PathDevBuoy2", "0", 2 ) );
                setCellValue( curExcelRow, "F", HelperFunctions.getDataRowColValueDecimal( boatPathRow, "PathDevBuoy3", "0", 2 ) );
                setCellValue( curExcelRow, "H", HelperFunctions.getDataRowColValueDecimal( boatPathRow, "PathDevBuoy4", "0", 2 ) );

            } catch ( Exception ex ) {
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }

            return true;
        }

        private void exportBoatPathJumpDataText( StringBuilder outLine, String memberId, String round, DataTable curDataTable ) {
			DataRow boatPathRow;
			Int16 passNum;

			try {
				Boolean writeHeader = true;
				foreach ( DataRow curRow in curDataTable.Rows ) {
					passNum = (byte)curRow["PassNum"];
					boatPathRow = WscHandler.getBoatPath( "Jump", memberId, round, passNum.ToString(), (decimal)0, (byte)curRow["BoatSpeedKph"] );
					if ( boatPathRow == null ) continue;

					if ( writeHeader ) {
						writeHeader = false;
						myOutBuffer.WriteLine( outLine.ToString() );

						outLine = new StringBuilder( "Boat Path Monitoring Data" );
						myOutBuffer.WriteLine( outLine.ToString() );

						outLine = new StringBuilder( "" );
						outLine.Append( "Pass" + tabDelim );
						outLine.Append( "Desc" + tabDelim );
						outLine.Append( "Gate" + tabDelim );
						outLine.Append( "52M" + tabDelim );
						outLine.Append( "82M" + tabDelim );
						outLine.Append( "41M" + tabDelim );
						outLine.Append( "Exit" + tabDelim );
						myOutBuffer.WriteLine( outLine.ToString() );
					}

					outLine = new StringBuilder( ( (byte)boatPathRow["PassNumber"] ).ToString() + tabDelim + "Time" + tabDelim );
					for ( int rowIdx = 1; rowIdx <= 4; rowIdx++ ) {
						outLine.Append( tabDelim + ( (Decimal)boatPathRow["boatTimeBuoy" + rowIdx] ).ToString() );
					}
					myOutBuffer.WriteLine( outLine.ToString() );

					outLine = new StringBuilder( tabDelim + "Dev" );
					for ( int rowIdx = 0; rowIdx <= 3; rowIdx++ ) {
						outLine.Append( tabDelim + ( (Decimal)boatPathRow["PathDevBuoy" + rowIdx] ).ToString() );
					}
					myOutBuffer.WriteLine( outLine.ToString() );

					outLine = new StringBuilder( tabDelim + "Cum" );
					for ( int rowIdx = 0; rowIdx <= 3; rowIdx++ ) {
						outLine.Append( tabDelim + ( (Decimal)boatPathRow["PathDevCum" + rowIdx] ).ToString() );
					}
					myOutBuffer.WriteLine( outLine.ToString() );
				}

			} catch ( Exception ex ) {
				MessageBox.Show( "Exception encountered in ExportRecordAppData exportBoatPathDataExcel method"
					+ "\n\nException: " + ex.Message
				);
			}
		}

		private StreamWriter getExportFile(String inFileName) {
            StreamWriter outBuffer = null;

            SaveFileDialog myFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            myFileDialog.InitialDirectory = curPath;
            myFileDialog.FileName = inFileName;

            try {
                if (myFileDialog.ShowDialog() == DialogResult.OK) {
                    String myFileName = myFileDialog.FileName;
                    if (myFileName != null) {
                        int delimPos = myFileName.LastIndexOf( '\\' );
                        String curFileName = myFileName.Substring( delimPos + 1 );
                        if (curFileName.IndexOf( '.' ) < 0) {
                            myFileName += ".wsp";
                        }
                        outBuffer = File.CreateText( myFileName );
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
        }

        private String cleanLabel(String inValue) {
            String curReturnValue = inValue;
            curReturnValue = curReturnValue.Replace( '\n', ' ' );
            curReturnValue = curReturnValue.Replace( '\r', ' ' );
            curReturnValue = curReturnValue.Replace( '\t', ' ' );
            return curReturnValue;
        }
        
        private String transformName(String inSkierName) {
            try {
                int curDelim = inSkierName.IndexOf( ',' );
                String curFirstName = inSkierName.Substring( curDelim + 1 );
                String curLastName = inSkierName.Substring( 0, curDelim );
                return curFirstName.Trim() + " " + curLastName.Trim();
            
            } catch {
                return inSkierName;
            }
        }
        
        private DataRow getTourData(String inSanctionId) {
            DataRow curDataRow = null;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct T.SanctionId, T.Name, T.Class, T.Federation, T.TourDataLoc, T.LastUpdateDate" );
            curSqlStmt.Append( ", T.SlalomRounds, T.TrickRounds, T.JumpRounds, T.Rules, T.EventDates, T.EventLocation" );
            curSqlStmt.Append( ", T.HcapSlalomBase, T.HcapTrickBase, T.HcapJumpBase, T.HcapSlalomPct, T.HcapTrickPct, T.HcapJumpPct " );
            curSqlStmt.Append( ", T.RopeHandlesSpecs, T.SlalomRopesSpecs, T.JumpRopesSpecs, T.SlalomCourseSpecs, T.JumpCourseSpecs, T.TrickCourseSpecs, T.BuoySpecs" );
            curSqlStmt.Append( ", T.SafetyDirPerfReport, T.RuleExceptions, T.RuleExceptQ1, T.RuleExceptQ2, T.RuleExceptQ3, T.RuleExceptQ4" );
            curSqlStmt.Append( ", T.RuleInterpretations, T.RuleInterQ1, T.RuleInterQ2, T.RuleInterQ3, T.RuleInterQ4" );
            curSqlStmt.Append( ", T.ContactMemberId, TourRegCO.SkierName AS ContactName, T.ContactPhone, T.ContactEmail, T.ContactAddress" );
            curSqlStmt.Append( ", T.ChiefJudgeMemberId, TourRegCJ.SkierName AS ChiefJudgeName, T.ChiefJudgeAddress, T.ChiefJudgePhone, T.ChiefJudgeEmail" );
            curSqlStmt.Append( ", T.ChiefDriverMemberId, TourRegCD.SkierName AS ChiefDriverName, T.ChiefDriverAddress, T.ChiefDriverPhone, T.ChiefDriverEmail" );
            curSqlStmt.Append( ", T.SafetyDirMemberId, TourRegCS.SkierName AS ChiefSafetyName, T.SafetyDirAddress, T.SafetyDirPhone, T.SafetyDirEmail" );
            curSqlStmt.Append( ", T.ChiefScorerMemberId, TourRegCC.SkierName AS ChiefScorerName, T.ChiefScorerAddress, T.ChiefScorerPhone, T.ChiefScorerEmail " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCC ON T.SanctionId = TourRegCC.SanctionId AND T.ChiefScorerMemberId = TourRegCC.MemberId " );
            curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCJ ON T.SanctionId = TourRegCJ.SanctionId AND T.ChiefJudgeMemberId = TourRegCJ.MemberId " );
            curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCD ON T.SanctionId = TourRegCD.SanctionId AND T.ChiefDriverMemberId = TourRegCD.MemberId " );
            curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCS ON T.SanctionId = TourRegCS.SanctionId AND T.SafetyDirMemberId = TourRegCS.MemberId " );
            curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCO ON T.SanctionId = TourRegCO.SanctionId AND T.ContactMemberId = TourRegCO.MemberId " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
            curSqlStmt.Append( "ORDER BY T.SanctionId " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                curDataRow = curDataTable.Rows[0];
            }

            return curDataRow;
        }

        private DataRow getTechController(String inSanctionId) {
            DataRow curDataRow = null;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT T.SanctionId, T.MemberId, T.SkierName as ChiefTechName, T.State, T.City, O.TechOfficialRating " );
            curSqlStmt.Append( "FROM OfficialWork O " );
            curSqlStmt.Append( "     INNER JOIN TourReg T ON T.MemberId = O.MemberId AND T.SanctionId = O.SanctionId " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
            curSqlStmt.Append( "  AND O.TechChief = 'Y' " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count > 0) {
                curDataRow = curDataTable.Rows[0];
            }

            return curDataRow;
        }

        private DataTable getEventOfficials(String inSanctionId, String inEvent, String inEventGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round" );
            curSqlStmt.Append( ", O.WorkAsgmt, O.StartTime, O.EndTime, O.Notes " );
            curSqlStmt.Append( "FROM OfficialWorkAsgmt O " );
            curSqlStmt.Append( "     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt " );
            curSqlStmt.Append( "WHERE O.SanctionId = '" + inSanctionId + "' " );
            curSqlStmt.Append( "  AND O.Event = '" + inEvent + "' " );
            curSqlStmt.Append( "  AND O.EventGroup = '" + inEventGroup + "' " );
            curSqlStmt.Append( "  AND O.Round = " + inRound + " " );
            curSqlStmt.Append( "ORDER BY O.Event, O.Round, O.EventGroup, O.WorkAsgmt" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataRow getOfficialInfo(String inSanctionId, String inMemberId) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT distinct O.PK, O.SanctionId, O.MemberId, T.SkierName" );
            curSqlStmt.Append( ", O.JudgeSlalomRating, O.JudgeJumpRating, O.JudgeTrickRating" );
            curSqlStmt.Append( ", O.ScorerSlalomRating, O.ScorerTrickRating, O.ScorerJumpRating" );
            curSqlStmt.Append( ", O.DriverSlalomRating, O.DriverTrickRating, O.DriverJumpRating" );
            curSqlStmt.Append( ", O.SafetyOfficialRating, O.TechOfficialRating, O.AnncrOfficialRating, O.Note " );
            curSqlStmt.Append( "FROM OfficialWork O " );
            curSqlStmt.Append( "	INNER JOIN TourReg T ON T.MemberId = O.MemberId AND T.SanctionId = O.SanctionId " );
            curSqlStmt.Append( "WHERE O.SanctionId = '" + inSanctionId + "' " );
            curSqlStmt.Append( "  AND O.MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( "ORDER BY T.SkierName, O.MemberId " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count > 0) {
                return curDataTable.Rows[0];
            } else {
                return null;
            }
        }

        private DataTable getSkierInfo(String inSanctionId, String inEvent, String inMemberId, String inDiv, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct E.Event, E.MemberId, T.SkierName, E.AgeGroup as Div, T.SkiYearAge, E.TeamCode" );
            curSqlStmt.Append( ", T.Federation, T.State, T.City, COALESCE(S.EventClass, E.EventClass) as EventClass " );
            if (inEvent.Equals( "Slalom" )) {
                curSqlStmt.Append( ", Round, S.Score, S.FinalSpeedMPH, S.FinalSpeedKph, S.FinalLen, S.FinalLenOff, S.FinalPassScore, S.InsertDate " );
                curSqlStmt.Append( "FROM EventReg E " );
                curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                curSqlStmt.Append( "     INNER JOIN SlalomScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
            } else if (inEvent.Equals( "Trick" )) {
                curSqlStmt.Append( ", Round, S.Score, S.ScorePass1, S.ScorePass2, S.InsertDate " );
                curSqlStmt.Append( "FROM EventReg E " );
                curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                curSqlStmt.Append( "     INNER JOIN TrickScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
            } else if (inEvent.Equals( "Jump" )) {
                curSqlStmt.Append( ", Round, S.ScoreFeet as Score, S.ScoreFeet, S.ScoreMeters, S.RampHeight, S.BoatSpeed as BoatSpeedKph, MinValue as BoatSpeedMph, S.InsertDate " );
                curSqlStmt.Append( "FROM EventReg E " );
                curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                curSqlStmt.Append( "     INNER JOIN JumpScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
				curSqlStmt.Append( "     INNER JOIN CodeValueList ON ListName = 'JumpSpeeds' AND MaxValue = S.BoatSpeed " );
            }
            curSqlStmt.Append( "WHERE E.SanctionId = '" + inSanctionId + "' AND E.MemberId = '" + inMemberId + "' AND E.AgeGroup = '" + inDiv + "' AND E.Event = '" + inEvent + "' " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private String getSkierName( String inSanctionId, String inMemberId, String inDiv ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SkierName " );
            curSqlStmt.Append( "FROM TourReg " );
            curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' AND MemberId = '" + inMemberId + "' AND AgeGroup = '" + inDiv + "' " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return "Skier Not Found";
            return transformName(HelperFunctions.getDataRowColValue( curDataTable.Rows[0], "SkierName", "" ));
        }

        private int getSlalomLineData( decimal inLinelength ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue, SortSeq" );
            curSqlStmt.Append( ", MinValue as LineLengthOff, MaxValue as LineLengthMeters, CodeValue as LineLengthOffDesc " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName = 'SlalomLines' AND ListCodeNum = " + inLinelength );
            curSqlStmt.Append( "ORDER BY SortSeq" );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return -1;
            return Convert.ToInt32( HelperFunctions.getDataRowColValueDecimal( curDataTable.Rows[0], "SortSeq", 0 ) ) - 1;
        }

        private DataTable getSkierSlalomScore(String inSanctionId, String inMemberId, String inAgeGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MemberId, AgeGroup as Div, Round, EventClass" );
            curSqlStmt.Append( ", Score, NopsScore, StartLen, StartSpeed" );
            curSqlStmt.Append( ", FinalSpeedMph, FinalSpeedKph, FinalLen, FinalLenOff, FinalPassScore, Note " );
            curSqlStmt.Append( "FROM SlalomScore S " );
            curSqlStmt.Append( String.Format( "WHERE SanctionId = '{0}' AND MemberId = '{1}' AND AgeGroup = '{2}' AND Round = {3} ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
            curSqlStmt.Append( "ORDER BY SanctionId, MemberId" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getSkierSlalomRecap(String inSanctionId, String inMemberId, String inAgeGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SkierRunNum, LastUpdateDate, PassLineLength, PassSpeedKph, Score, BoatTime, TimeInTol" );
            curSqlStmt.Append( ", Judge1Score, Judge2Score, Judge3Score, Judge4Score, Judge5Score" );
            //curSqlStmt.Append( ", EntryGate1, EntryGate2, EntryGate3, ExitGate1, ExitGate2, ExitGate3" );
            curSqlStmt.Append( ", Reride, RerideReason, ScoreProt, Note " );
            curSqlStmt.Append( "FROM SlalomRecap " );
            curSqlStmt.Append( String.Format( "WHERE SanctionId = '{0}' AND MemberId = '{1}' AND AgeGroup = '{2}' AND Round = {3} ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
            curSqlStmt.Append( "ORDER BY SanctionId, MemberId, AgeGroup, Round, SkierRunNum" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getSkierTrickScore(String inSanctionId, String inMemberId, String inAgeGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MemberId, AgeGroup as Div, Round, EventClass" );
            curSqlStmt.Append( ", Score, ScorePass1, ScorePass2, NopsScore, Boat, Note " );
            curSqlStmt.Append( "FROM TrickScore " );
            curSqlStmt.Append( String.Format( "WHERE SanctionId = '{0}' AND MemberId = '{1}' AND AgeGroup = '{2}' AND Round = {3} ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
            curSqlStmt.Append( "ORDER BY SanctionId, MemberId" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getSkierTrickPass(String inSanctionId, String inMemberId, String inAgeGroup, String inRound, String inPass) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PassNum, Skis, Seq, Code, Results, Score, Note " );
            curSqlStmt.Append( "FROM TrickPass " );
            curSqlStmt.Append( String.Format( "WHERE SanctionId = '{0}' AND MemberId = '{1}' AND AgeGroup = '{2}' AND Round = {3} AND PassNum = {4} "
                , inSanctionId, inMemberId, inAgeGroup, inRound, inPass ) );
            curSqlStmt.Append( "ORDER BY SanctionId, MemberId, AgeGroup, Round, PassNum, Seq" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getSkierJumpScore(String inSanctionId, String inMemberId, String inAgeGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MemberId, AgeGroup as Div, Round, EventClass" );
            curSqlStmt.Append( ", ScoreFeet as Score, ScoreFeet, ScoreMeters, NopsScore, BoatSpeed, RampHeight, Boat, Note " );
            curSqlStmt.Append( "FROM JumpScore S" );
            curSqlStmt.Append( String.Format( "WHERE S.SanctionId = '{0}' AND S.MemberId = '{1}' AND S.AgeGroup = '{2}' AND S.Round = {3} ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
            curSqlStmt.Append( "ORDER BY SanctionId, MemberId" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getSkierJumpRecap(String inSanctionId, String inMemberId, String inAgeGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT R.PassNum, R.LastUpdateDate, R.RampHeight, R.BoatSpeed as BoatSpeedKph, MinValue as BoatSpeedMph" );
            curSqlStmt.Append( ", R.ScoreFeet, R.ScoreMeters, Results" );
            curSqlStmt.Append( ", BoatSplitTime2 as Split82Time, BoatEndTime as Split41Time, BoatSplitTime as Split52Time" );
            curSqlStmt.Append( ", SkierBoatPath, ReturnToBase, ScoreProt, Reride, RerideReason, R.Note as RideNote, S.Note as ScoreNote " );
            curSqlStmt.Append( "FROM JumpScore S " );
            curSqlStmt.Append( "  INNER JOIN JumpRecap R on R.SanctionId = S.SanctionId AND R.MemberId = S.MemberId AND R.AgeGroup = S.AgeGroup AND R.Round = S.Round " );
            curSqlStmt.Append( "             AND R.ScoreFeet = S.ScoreFeet AND R.ScoreMeters = S.ScoreMeters " );
			curSqlStmt.Append( "  INNER JOIN CodeValueList ON ListName = 'JumpSpeeds' AND MaxValue = R.BoatSpeed " );
            curSqlStmt.Append( String.Format( "WHERE S.SanctionId = '{0}' AND S.MemberId = '{1}' AND S.AgeGroup = '{2}' AND S.Round = {3} ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
            curSqlStmt.Append( "ORDER BY S.SanctionId, S.MemberId, S.AgeGroup, S.Round, PassNum" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getJumpMeterSetup(String inSanctionId) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM JumpMeterSetup WHERE SanctionId = '" + inSanctionId + "' " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getSkierBoatInfo( String inSanctionId, String inMemberId, String inAgeGroup, String inRound, String inEvent ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT S.MemberId, S.AgeGroup as Div, S.Round" );
            curSqlStmt.Append( ", B.BoatModel as BoatModel, B.ModelYear, B.SpeedControlVersion " );
            curSqlStmt.Append( "FROM " + inEvent + "Score S " );
            curSqlStmt.Append( "LEFT OUTER JOIN TourBoatUse B ON B.SanctionId = S.SanctionId AND B.HullId = S.Boat " );
            curSqlStmt.Append( String.Format( "WHERE S.SanctionId = '{0}' AND S.MemberId = '{1}' AND S.AgeGroup = '{2}' AND S.Round = {3} "
                , inSanctionId, inMemberId, inAgeGroup, inRound ) );
            curSqlStmt.Append( "ORDER BY S.SanctionId, S.MemberId" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private void setCellValue(int inRowIdx, String inColumnIdx, object inValue) {
            try {
                //Select the desired cell
                myExcelRange = myExcelActiveSheet.GetType().InvokeMember( "Cells", BindingFlags.GetProperty, null, myExcelActiveSheet, new object[2] { inRowIdx, inColumnIdx } );
                //Set the value of the specified cell to the value provided
                myExcelRange.GetType().InvokeMember( "Value", BindingFlags.SetProperty, null, myExcelRange, new object[] { inValue } );
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData:setCellValue" + "\n\nException: " + ex.Message );
            }
        }
        private void setCellValue(int inRowIdx, int inColumnIdx, object inValue) {
            try {
                //Select the desired cell
                myExcelRange = myExcelActiveSheet.GetType().InvokeMember( "Cells", BindingFlags.GetProperty, null, myExcelActiveSheet, new object[2] { inRowIdx, inColumnIdx } );
                //Set the value of the specified cell to the value provided
                myExcelRange.GetType().InvokeMember( "Value", BindingFlags.SetProperty, null, myExcelRange, new object[] { inValue } );
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData:setCellValue" + "\n\nException: " + ex.Message );
            }
        }

        private void SetColumnWdth(Object[] inRange, double wdth) {
            try {
                //Select the desired range of cells
                myExcelRange = myExcelActiveSheet.GetType().InvokeMember( "Range", BindingFlags.GetProperty, null, myExcelActiveSheet, inRange );
                //Set the width of all the columns in the selected range
                myExcelRange.GetType().InvokeMember( "Columnwdth", BindingFlags.SetProperty, null, myExcelRange, new object[] { wdth } );
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData:SetColumnWdth" + "\n\nException: " + ex.Message );
            }
        }

        private void setColumnFontBold(Object[] inRange) {
            try {
                //Select the desired range of cells
                myExcelRange = myExcelActiveSheet.GetType().InvokeMember( "Range", BindingFlags.GetProperty, null, myExcelActiveSheet, inRange );
                //Select the font object of the selected cells
                Object cellFont = myExcelRange.GetType().InvokeMember( "Font", BindingFlags.GetProperty, null, myExcelRange, null );
                //Set the style to bold
                cellFont.GetType().InvokeMember( "Bold", BindingFlags.SetProperty, null, cellFont, new object[] { true } );
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData:setColumnFontBold" + "\n\nException: " + ex.Message );
            }

            //entireRow.GetType().InvokeMember( "HorizontalAlignment", BindingFlags.SetProperty, null, entireRow, new object[] { 3 } );
        }

        private void setColumnAlignment(Object[] inRange, Object[] inAlignmentArgs) {
            try {
                //Select the desired range of cells
                myExcelRange = myExcelActiveSheet.GetType().InvokeMember( "Range", BindingFlags.GetProperty, null, myExcelActiveSheet, inRange );
                //Select the alignment of the selected cells
                myExcelRange.GetType().InvokeMember( "HorizontalAlignment", BindingFlags.SetProperty, null, myExcelRange, inAlignmentArgs );
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData:setColumnAlignment" + "\n\nException: " + ex.Message );
            }
        }

        private void setColumnBorders(Object[] inRange, Object[] inLineStyleArgs, Object[] inLIneStyleWeightArgs) {
            try {
                //Select the desired range of cells
                myExcelRange = myExcelActiveSheet.GetType().InvokeMember( "Range", BindingFlags.GetProperty, null, myExcelActiveSheet, inRange );
                //Select the borders object for the selected range of cells
                Object curBorders = myExcelRange.GetType().InvokeMember( "Borders", BindingFlags.GetProperty, null, myExcelRange, null );
                //Set the border line style and weight for the all the borders in the selected range
                curBorders.GetType().InvokeMember( "LineStyle", BindingFlags.SetProperty, null, curBorders, inLineStyleArgs );
                curBorders.GetType().InvokeMember( "Weight", BindingFlags.SetProperty, null, curBorders, inLIneStyleWeightArgs );
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData:setColumnBorders" + "\n\nException: " + ex.Message );
            }
        }

        private void setColumnBorder(Object[] inRange, Object[] inBorderEdgeArgs, Object[] inLineStyleArgs) {
            //Example of removing right edge border
            //setColumnBorder( new object[2] { "J" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new Object[] { 10 }, new Object[] { 0 } );
            try {
                //Select the desired range of cells
                myExcelRange = myExcelActiveSheet.GetType().InvokeMember( "Range", BindingFlags.GetProperty, null, myExcelActiveSheet, inRange );
                //Select the borders of the selected range
                Object curBorders = myExcelRange.GetType().InvokeMember( "Borders", BindingFlags.GetProperty, null, myExcelRange, null );
                //Select the border edge as indicated by the first border edge argument
                Object curObject = curBorders.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, curBorders, inBorderEdgeArgs );
                //Set the line style for the selected border edge, note that a value of zero will remove the border
                curObject.GetType().InvokeMember( "LineStyle", BindingFlags.SetProperty, null, curObject, inLineStyleArgs );

            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData:setColumnBorder" + "\n\nException: " + ex.Message );
            }
        }

        private void setColumnMerge(Object[] inRange) {
            try {
                //Select the desired range of cells
                myExcelRange = myExcelActiveSheet.GetType().InvokeMember( "Range", BindingFlags.GetProperty, null, myExcelActiveSheet, inRange );
                //Merge the selected cells 
                myExcelRange.GetType().InvokeMember( "MergeCells", BindingFlags.SetProperty, null, myExcelRange, new object[] { true } );
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData:setColumnMerge" + "\n\nException: " + ex.Message );
            }
        }

    }
}
