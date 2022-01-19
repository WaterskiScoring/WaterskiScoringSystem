using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Tools {
    class ExportRecordAppData {
        private String tabDelim = "\t";
        private String myRecordAppFormFileName = "RecordApplicationFormTemplate.xlsx";
        private String myExcelSheetName_IDSheet = "ID & CERTIFICATION PAGE";
        private String myExcelSheetName_EventSheet = "EventRecordData";
        private String myDeploymentDirectory = "";

        private const String myExcelAppUID = "Excel.Application";
        private object myExcelApp = null;
        private object myExcelWorkBooks, myExcelWorkBook, myExcelSheets, myExcelActiveSheet, myExcelRange;

        private StreamWriter myOutBuffer = null;
        private TourProperties myTourProperties;

        public ExportRecordAppData() {
            try {
                //Create Excel application object
                Type curAppType = Type.GetTypeFromProgID( myExcelAppUID );
                if (curAppType == null) {
                    MessageBox.Show( "Excel not available" );
                } else {
                    myExcelApp = Activator.CreateInstance( curAppType );
                    MessageBox.Show( "ExcelApp instance created of type: " + myExcelApp.GetType() );
                    // make it visible
                    myExcelApp.GetType().InvokeMember( "Visible", BindingFlags.SetProperty, null, myExcelApp, new object[] { true } );

                    try {
                        myDeploymentDirectory = Application.StartupPath;
                        if (myDeploymentDirectory == null) { myDeploymentDirectory = ""; }
                        if (myDeploymentDirectory.Length < 1) {
                            myDeploymentDirectory = "C:\\Users\\AllenFamily\\Documents\\Visual Studio 2010\\Projects\\WaterskiScoringSystem\\WaterskiScoringSystem\\\bin\\Debug";
                        }
                    } catch (Exception ex) {
                        if (myDeploymentDirectory == null) { myDeploymentDirectory = ""; }
                        if (myDeploymentDirectory.Length < 1) {
                            myDeploymentDirectory = "C:\\Users\\AllenFamily\\Documents\\Visual Studio 2010\\Projects\\WaterskiScoringSystem\\WaterskiScoringSystem\\\bin\\Debug";
                        }
                    }
                }
            } catch (Exception ex) {
                myExcelApp = null;
                MessageBox.Show( "Error initializing record application export processing" + "\n\nError: " + ex.Message );
            }
        }

        public bool ExportData(String inSanctionId, String inEvent, String inMemberId, String inDiv, String inEventGroup, Int16 inRound) {
            String curMethodName = "ExportRecordAppData:ExportData";
            String curMsg = "";

            try {
                StringBuilder outLine = new StringBuilder( "" );
                String curFilename = "RecordData_" + inEvent + "_" + inMemberId + "_" + inDiv + "_" + inRound + ".txt";
                myOutBuffer = getExportFile( curFilename );
				if ( myOutBuffer == null ) return false;

				Log.WriteFile( "Export Record Data: " + curFilename );

				DataRow curTourRow = getTourData( inSanctionId );
				if ( curTourRow == null ) {
					curMsg = "Tournament data not found, export bypassed";
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + ":conplete: " + curMsg );
					return true;
				}

				writeTourData( outLine, curTourRow );
				writeEventOfficials( outLine, curTourRow["SanctionId"].ToString(), inEvent, inEventGroup, inRound.ToString() );
				writeSkierInfo( outLine, curTourRow["SanctionId"].ToString(), inEvent, inMemberId, inDiv, inRound.ToString() );
				writePerfData( outLine, curTourRow["SanctionId"].ToString(), inEvent, inMemberId, inDiv, inRound.ToString() );
				myOutBuffer.Close();
				curMsg = curMethodName + " data successfully completed ";

				if ( myExcelApp != null ) {
					String curExcelTemplateFileName = myDeploymentDirectory + "\\" + myRecordAppFormFileName;
					String curExcelFileName = Properties.Settings.Default.ExportDirectory + "\\RecordData_" + inEvent + "_" + inMemberId + "_" + inDiv + "_" + inRound;
					MessageBox.Show( "ExcelFileName: " + curExcelFileName );

					//Get a new workbook.
					myExcelWorkBooks = myExcelApp.GetType().InvokeMember( "Workbooks", BindingFlags.GetProperty, null, myExcelApp, null );
					myExcelWorkBook = myExcelWorkBooks.GetType().InvokeMember( "Open", BindingFlags.InvokeMethod, null, myExcelWorkBooks, new object[] { curExcelTemplateFileName, true } );
					myExcelSheets = myExcelWorkBook.GetType().InvokeMember( "Worksheets", BindingFlags.GetProperty, null, myExcelWorkBook, null );
					myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { 1 } );
					// Range = WorkSheet.GetType().InvokeMember("Range",BindingFlags.GetProperty,null,WorkSheet,new object[1] { "A1" });

					myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { myExcelSheetName_IDSheet } );
					myExcelActiveSheet.GetType().InvokeMember( "Activate", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );
					myExcelWorkBook.GetType().InvokeMember( "SaveAs", BindingFlags.InvokeMethod, null, myExcelWorkBook, new object[] { curExcelFileName } );

					writeTourDataExcel( curTourRow );
					writeEventOfficialsExcel( curTourRow["SanctionId"].ToString(), inEvent, inEventGroup, inRound.ToString() );
					setCellValue( 14, "D", inEvent );
					setCellValue( 14, "B", inDiv );
					setCellValue( 16, "G", inRound );

					writeSkierInfoExcel( curTourRow["SanctionId"].ToString(), inEvent, inMemberId, inDiv, inRound.ToString() );
					writePerfDataExcel( curTourRow["SanctionId"].ToString(), inEvent, inMemberId, inDiv, inRound.ToString() );

					myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { myExcelSheetName_IDSheet } );
					myExcelActiveSheet.GetType().InvokeMember( "Activate", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );

					myExcelWorkBook.GetType().InvokeMember( "Save", BindingFlags.InvokeMethod, null, myExcelWorkBook, null );
					myExcelApp.GetType().InvokeMember( "DisplayAlerts", BindingFlags.SetProperty, null, myExcelApp, new object[] { false } );
					myExcelWorkBook.GetType().InvokeMember( "Close", BindingFlags.InvokeMethod, null, myExcelWorkBook, new object[] { true } );
					myExcelApp.GetType().InvokeMember( "DisplayAlerts", BindingFlags.SetProperty, null, myExcelApp, new object[] { true } );
					myExcelApp.GetType().InvokeMember( "Quit", System.Reflection.BindingFlags.InvokeMethod, null, myExcelApp, null );
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + ":conplete: " + curMsg );
					return true;
				}
				
			} catch (Exception ex) {
                MessageBox.Show( "Error:" + curMethodName + " Exception encountered in ExportRecordAppData processing:\n\n" + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            
			}
			return false;
		}

		//Write tournament information to Excel spreadsheet
		private bool writeTourDataExcel( DataRow inTourRow) {
            try {
                DataRow curOfficialRow = null;
                DataRow curTechRow = getTechController( inTourRow["SanctionId"].ToString() );

                setCellValue(18, "C", inTourRow["Name"].ToString());
                setCellValue(18, "G", inTourRow["SanctionId"].ToString());
                setCellValue(20, "B", inTourRow["EventLocation"].ToString());

                curOfficialRow = getOfficialInfo( inTourRow["SanctionId"].ToString(), inTourRow["ChiefJudgeMemberId"].ToString() );
				if ( curOfficialRow  != null ) {
					setCellValue( 44, "B", transformName( inTourRow["ChiefJudgeName"].ToString() ) );
					setCellValue( 44, "H", curOfficialRow["JudgeSlalomRating"].ToString() );
					setCellValue( 44, "G", inTourRow["ChiefJudgePhone"].ToString() );
					setCellValue( 44, "E", inTourRow["ChiefJudgeEmail"].ToString() );
				}

				curOfficialRow = getOfficialInfo( inTourRow["SanctionId"].ToString(), inTourRow["ChiefDriverMemberId"].ToString() );
				if ( curOfficialRow != null ) {
					setCellValue( 50, "B", transformName( inTourRow["ChiefDriverName"].ToString() ) );
					setCellValue( 50, "H", curOfficialRow["DriverSlalomRating"].ToString() );
					setCellValue( 50, "G", inTourRow["ChiefDriverPhone"].ToString() );
					setCellValue( 50, "E", inTourRow["ChiefDriverEmail"].ToString() );
				}

				curOfficialRow = getOfficialInfo( inTourRow["SanctionId"].ToString(), inTourRow["ChiefScorerMemberId"].ToString() );
				if ( curOfficialRow != null ) {
					setCellValue( 48, "B", transformName( inTourRow["ChiefScorerName"].ToString() ) );
					setCellValue( 48, "H", curOfficialRow["ScorerSlalomRating"].ToString() );
					setCellValue( 48, "G", inTourRow["ChiefScorerPhone"].ToString() );
					setCellValue( 48, "E", inTourRow["ChiefScorerEmail"].ToString() );
				}

				if (curTechRow == null) {
                } else {
                    setCellValue(46, "B", transformName( curTechRow["ChiefTechName"].ToString() ));
                    setCellValue(46, "H", curTechRow["TechOfficialRating"].ToString());
                }

                return true;

			} catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData writeTourData method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private bool writeEventOfficialsExcel(String inSanctionId, String inEvent, String inEventGroup, String inRound) {
            DataRow curRow = null, curOfficialRow = null;
            int curExcelRow = 54;
            try {
                setCellValue(curExcelRow, "A", "Boat Judge");
                DataTable curDataTable = getEventOfficials( inSanctionId, inEvent, inEventGroup, inRound );
                DataRow[] curRowsFound = curDataTable.Select( "WorkAsgmt = 'Boat Judge'" );
                if (curRowsFound.Length > 0) {
                    for (int curIdx = 0; curIdx < curRowsFound.Length; curIdx++) {
                        curRow = curRowsFound[curIdx];
                        curOfficialRow = getOfficialInfo( (String)curRow["SanctionId"], (String)curRow["MemberId"] );
                        if (curOfficialRow != null) {
                            setCellValue(curExcelRow, "B", transformName( curOfficialRow["SkierName"].ToString() ));
                            if (inEvent.Equals( "Slalom" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["JudgeSlalomRating"].ToString());
                            } else if (inEvent.Equals( "Trick" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["JudgeTrickRating"].ToString());
                            } else if (inEvent.Equals( "Jump" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["JudgeJumpRating"].ToString());
                            }
                            curExcelRow++;
                        }
                    }
                } else {
                    curExcelRow++;
                }
                curExcelRow++;

                setCellValue(curExcelRow, "A", "Driver");
                curRowsFound = curDataTable.Select( "WorkAsgmt = 'Driver'" );
                if (curRowsFound.Length > 0) {
                    for (int curIdx = 0; curIdx < curRowsFound.Length; curIdx++) {
                        curRow = curRowsFound[curIdx];
                        curOfficialRow = getOfficialInfo( (String)curRow["SanctionId"], (String)curRow["MemberId"] );
                        if (curOfficialRow != null) {
                            setCellValue(curExcelRow, "B", transformName( curOfficialRow["SkierName"].ToString() ));
                            if (inEvent.Equals( "Slalom" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["DriverSlalomRating"].ToString());
                            } else if (inEvent.Equals( "Trick" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["DriverTrickRating"].ToString());
                            } else if (inEvent.Equals( "Jump" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["DriverJumpRating"].ToString());
                            }
                            curExcelRow++;
                        }
                    }
                } else {
                    curExcelRow++;
                }
                curExcelRow++;

                setCellValue(curExcelRow, "A", "Event Judge");
                curRowsFound = curDataTable.Select("WorkAsgmt = 'Event Judge' OR WorkAsgmt = 'End Course Official'");
                if (curRowsFound.Length > 0) {
                    for (int curIdx = 0; curIdx < curRowsFound.Length; curIdx++) {
                        curRow = curRowsFound[curIdx];
                        curOfficialRow = getOfficialInfo( (String)curRow["SanctionId"], (String)curRow["MemberId"] );
                        if (curOfficialRow != null) {
                            setCellValue(curExcelRow, "B", transformName( curOfficialRow["SkierName"].ToString() ));
                            if (inEvent.Equals( "Slalom" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["JudgeSlalomRating"].ToString());
                            } else if (inEvent.Equals( "Trick" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["JudgeTrickRating"].ToString());
                            } else if (inEvent.Equals( "Jump" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["JudgeJumpRating"].ToString());
                            }
                        }
                        curExcelRow++;
                    }
                } else {
                    curExcelRow++;
                }
                curExcelRow++;

                setCellValue(curExcelRow, "A", "Scorer");
                curRowsFound = curDataTable.Select( "WorkAsgmt = 'Scorer'" );
                if (curRowsFound.Length > 0) {
                    for (int curIdx = 0; curIdx < curRowsFound.Length; curIdx++) {
                        curRow = curRowsFound[curIdx];
                        curOfficialRow = getOfficialInfo( (String)curRow["SanctionId"], (String)curRow["MemberId"] );
                        if (curOfficialRow != null) {
                            setCellValue(curExcelRow, "B", transformName( curOfficialRow["SkierName"].ToString() ));
                            if (inEvent.Equals( "Slalom" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["ScorerSlalomRating"].ToString());
                            } else if (inEvent.Equals( "Trick" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["ScorerTrickRating"].ToString());
                            } else if (inEvent.Equals( "Jump" )) {
                                setCellValue(curExcelRow, "D", curOfficialRow["ScorerJumpRating"].ToString());
                            }
                            curExcelRow++;
                        }
                    }
                } else {
                    curExcelRow++;
                }
                curExcelRow++;

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData writeEventOfficials method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private bool writeSkierInfoExcel(String inSanctionId, String inEvent, String inMemberId, String inDiv, String inRound) {
            DataRow curRow = null;
            
            try {
                DataTable curDataTable = getSkierInfo( inSanctionId, inEvent, inMemberId, inDiv, inRound );
                if (curDataTable.Rows.Count > 0) {
                    curRow = curDataTable.Rows[0];
                    setCellValue(23, "B", transformName( curRow["SkierName"].ToString() ));
                    setCellValue(23, "F", curRow["MemberId"].ToString() );
					setCellValue( 27, "F", curRow["SkiYearAge"].ToString() );
					setCellValue( 29, "B", curRow["City"].ToString() + ", " + curRow["State"].ToString() );
                    if (inEvent.Equals( "Slalom" )) {
                        String curValue = ( (decimal) curRow["Score"]).ToString("#0.00")
                            + " : " + ( (decimal) curRow["FinalPassScore"]).ToString("0.00")
                            + " @ "+ (String)curRow["FinalLenOff"]
                            + " " + curRow["FinalSpeedMPH"].ToString() + "mph " 
                            + " (" 
                            + curRow["FinalSpeedKph"].ToString() + "kph " 
                            + (String) curRow["FinalLen"] + "M"
                            + ")" ;
                        setCellValue( 14, "G", curValue );

					} else if (inEvent.Equals( "Jump" )) {
                        String curValue = ( (decimal) curRow["Score"]).ToString("#00")
                            + " feet (" + ( (decimal) curRow["ScoreMeters"]).ToString( "00.0" ) + "M)"
                            + " @ " + curRow["BoatSpeedKph"].ToString() + "kph"
							+ " ( " + ( (decimal) curRow["BoatSpeedMph"]).ToString( "00.0" ) + "mph)"
							+ " " + curRow["RampHeight"].ToString() + " ramp";
                        setCellValue( 14, "G", curValue );

					} else {
                        setCellValue( 14, "G", curRow["Score"].ToString() );
                    }
                    setCellValue(16, "C", curRow["LastUpdateDate"].ToString() );

                    try {
                        setCellValue(34, "B", curRow["BoatModel"].ToString() );
                    } catch {
                    }
                    try {
                        setCellValue(35, "B", curRow["ModelYear"].ToString() );
                    } catch {
                    }
                    try {
                        String curValue = curRow["SpeedControlVersion"].ToString();
                        if ( curValue.Contains("-") ) {
                            String[] curAttrList = curValue.Split('-');
                            setCellValue(34, "F", curAttrList[0]);
                            setCellValue(35, "F", curAttrList[1]);
                        } else {
                            setCellValue(34, "F", curRow["SpeedControlVersion"].ToString());
                            setCellValue(35, "F", curRow["SpeedControlVersion"].ToString());
                        }


                    } catch {
                    }

                }

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData writeEventOfficials method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private bool writePerfDataExcel(String inSanctionId, String inEvent, String inMemberId, String inDiv, String inRound) {
            try {
                //Excel._Worksheet curExcelSheet = (Excel._Worksheet)myExcelWorkbook.Sheets[myExcelSheetName_EventSheet];
                myExcelActiveSheet = myExcelSheets.GetType().InvokeMember( "Item", BindingFlags.GetProperty, null, myExcelSheets, new object[] { myExcelSheetName_EventSheet } );
                myExcelActiveSheet.GetType().InvokeMember( "Activate", BindingFlags.InvokeMethod, null, myExcelActiveSheet, null );

                int curExcelRow = 1;

                if (inEvent.Equals( "Slalom" )) {
                    writeSlalomDataDetail( curExcelRow, inSanctionId, inMemberId, inDiv, inRound );
                } else if (inEvent.Equals( "Trick" )) {
                    writeTrickDataDetail( inSanctionId, inMemberId, inDiv, inRound );
                } else if (inEvent.Equals( "Jump" )) {
                    writeJumpDataDetail( curExcelRow, inSanctionId, inMemberId, inDiv, inRound );
                }
                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData writeEventOfficials method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private int writeSlalomDataDetail(int inExcelRow, String inSanctionId, String inMemberId, String inDiv, String inRound) {
            int curExcelRow = inExcelRow;

            DataTable curDataTable = getSkierSlalomRecap( inSanctionId, inMemberId, inDiv, inRound );
            curExcelRow = exportDataTableExcel( curExcelRow, curDataTable );
			curExcelRow = exportBoatPathSlalomDataExcel( curExcelRow, inMemberId, inRound, curDataTable );

			curExcelRow = curExcelRow + 2;
            setCellValue( curExcelRow, "A", "Equipment Check:" );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "A" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "A", "1. Rope:" );
            setCellValue( curExcelRow, "B", "Note: wait at least 30 minutes after last use to measure the rope." );
            curExcelRow++;
            setColumnBorders( new object[2] { "B" + curExcelRow.ToString(), "K" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setCellValue( curExcelRow, "B", "'23:00" );
            setCellValue( curExcelRow, "C", "'18.25" );
            setCellValue( curExcelRow, "D", "'16:00" );
            setCellValue( curExcelRow, "E", "'14:25" );
            setCellValue( curExcelRow, "F", "'13:00" );
            setCellValue( curExcelRow, "G", "'12:00" );
            setCellValue( curExcelRow, "H", "'11:25" );
            setCellValue( curExcelRow, "I", "'10:75" );
            setCellValue( curExcelRow, "J", "'10:25" );
            setCellValue( curExcelRow, "K", "'9:75" );
            curExcelRow++;
            setCellValue( curExcelRow, "A", "Length:" );
            setColumnBorders( new object[2] { "B" + curExcelRow.ToString(), "K" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "2. Ski: " );
            curExcelRow++;
            setCellValue( curExcelRow, "A", "Does the ski qualify under W. Rule 10.03 ?" );
            setColumnBorders( new object[2] { "J" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "3. Slalom  Course: " );
            curExcelRow++;
            setCellValue( curExcelRow, "A", "Date of the course survey: " );
            setColumnMerge( new object[2] { "J" + curExcelRow.ToString(), "K" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "J" + curExcelRow.ToString(), "K" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "Do the skier and gate buoys meet the requirements of W. Rule 14.06? " );
            setColumnBorders( new object[2] { "J" + curExcelRow.ToString(), "J" + curExcelRow.ToString() },new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "4.  All Data/Video on Check List Available to be submitted: " );
            setColumnBorders( new object[2] { "J" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "AWSA slalom " );

            return curExcelRow;
        }

        private int writeTrickDataDetail(String inSanctionId, String inMemberId, String inDiv, String inRound) {
            int curExcelRow = 1;

            setCellValue( curExcelRow, "A", "TRICK RECORD DATA" );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
            setColumnMerge( new object[2] { "A" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
            setColumnAlignment( new object[2] { "A" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new object[] { 3 } );

            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "Note:" );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "A" + curExcelRow.ToString() } );
            setCellValue( curExcelRow, "B", "Attach to this form, copies of the trick run called by the the judges' sheets." );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Judges must sign and date the reviewed Judges Sheets (Pink Sheets)" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );

            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "Score:" );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "A" + curExcelRow.ToString() } );

            curExcelRow++;
            setCellValue( curExcelRow, "B", "FIRST PASS" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setCellValue( curExcelRow, "F", "SECOND PASS" );
            setColumnMerge( new object[2] { "F" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "A", "#" );
            setCellValue( curExcelRow, "B", "Skis" );
            setCellValue( curExcelRow, "C", "Trick" );
            setCellValue( curExcelRow, "D", "Status" );
            setCellValue( curExcelRow, "E", "Points" );
            setCellValue( curExcelRow, "G", "Skis" );
            setCellValue( curExcelRow, "H", "Trick" );
            setCellValue( curExcelRow, "I", "Status" );
            setCellValue( curExcelRow, "J", "Points" );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
            setColumnAlignment( new object[2] { "A" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new object[] { 3 } );

            DataTable curDataTable1 = getSkierTrickPass( inSanctionId, inMemberId, inDiv, inRound, "1" );
            DataTable curDataTable2 = getSkierTrickPass( inSanctionId, inMemberId, inDiv, inRound, "2" );

            curExcelRow++;
            try {
                int tmpExcelRow = curExcelRow;
                int tmpMaxRows = curDataTable1.Rows.Count;
                int curTotalPass1 = 0, curTotalPass2 = 0;
                if (curDataTable2.Rows.Count > tmpMaxRows) {
                    tmpMaxRows = curDataTable2.Rows.Count;
                }
                for (int tmpCount = 1; tmpCount <= tmpMaxRows; tmpCount++) {
                    setCellValue( tmpExcelRow, "A", "T" + tmpCount.ToString() );
                    tmpExcelRow++;
                }

                tmpExcelRow = curExcelRow;
                foreach (DataRow curRow in curDataTable1.Rows) {
                    setCellValue( tmpExcelRow, "B", curRow["Skis"].ToString() );
                    setCellValue( tmpExcelRow, "C", curRow["Code"].ToString() );
                    setCellValue( tmpExcelRow, "D", curRow["Results"].ToString() );
                    setCellValue( tmpExcelRow, "E", curRow["Score"].ToString() );
                    curTotalPass1 = curTotalPass1 + (Int16)curRow["Score"];
                    tmpExcelRow++;
                }

                tmpExcelRow = curExcelRow;
                foreach (DataRow curRow in curDataTable2.Rows) {
                    setCellValue( tmpExcelRow, "G", curRow["Skis"].ToString() );
                    setCellValue( tmpExcelRow, "H", curRow["Code"].ToString() );
                    setCellValue( tmpExcelRow, "I", curRow["Results"].ToString() );
                    setCellValue( tmpExcelRow, "J", curRow["Score"].ToString() );
                    curTotalPass2 = curTotalPass2 + (Int16)curRow["Score"];
                    tmpExcelRow++;
                }

                curExcelRow = curExcelRow + tmpMaxRows;
                setCellValue( curExcelRow, "B", "TOTAL PASS 1" );
                setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
                setColumnAlignment( new object[2] { "B" + curExcelRow.ToString(), "B" + curExcelRow.ToString() }, new object[] { 4 } );
                setCellValue( curExcelRow, "E", curTotalPass1.ToString() );
                setColumnBorder( new object[2] { "B" + curExcelRow.ToString(), "E" + curExcelRow.ToString() }, new Object[] { 8 }, new Object[] { 1 } );
                setColumnBorder( new object[2] { "E" + curExcelRow.ToString(), "E" + curExcelRow.ToString() }, new Object[] { 9 }, new Object[] { 1 } );
                setCellValue( curExcelRow, "G", "TOTAL PASS 2" );
                setColumnMerge( new object[2] { "G" + curExcelRow.ToString(), "I" + curExcelRow.ToString() } );
                setColumnAlignment( new object[2] { "G" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 4 } );
                setCellValue( curExcelRow, "J", curTotalPass2.ToString() );
                setColumnBorder( new object[2] { "G" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new Object[] { 8 }, new Object[] { 1 } );
                setColumnBorder( new object[2] { "J" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new Object[] { 9 }, new Object[] { 1 } );

                curExcelRow++;
                curExcelRow++;
                setCellValue( curExcelRow, "H", "TOTALS" );
                setColumnMerge( new object[2] { "H" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
                setColumnFontBold( new object[2] { "H" + curExcelRow.ToString(), "H" + curExcelRow.ToString() } );
                curExcelRow++;
                setCellValue( curExcelRow, "H", "First pass:" );
                setColumnMerge( new object[2] { "H" + curExcelRow.ToString(), "I" + curExcelRow.ToString() } );
                setCellValue( curExcelRow, "J", curTotalPass1.ToString() );
                curExcelRow++;
                setCellValue( curExcelRow, "H", "Second pass:" );
                setColumnMerge( new object[2] { "H" + curExcelRow.ToString(), "I" + curExcelRow.ToString() } );
                setCellValue( curExcelRow, "J", curTotalPass2.ToString() );
                curExcelRow++;
                setCellValue( curExcelRow, "H", "SCORE :" );
                setColumnMerge( new object[2] { "H" + curExcelRow.ToString(), "I" + curExcelRow.ToString() } );
                setCellValue( curExcelRow, "J", ( curTotalPass1 + curTotalPass2 ).ToString() );
                setColumnFontBold( new object[2] { "G" + curExcelRow.ToString(), "H" + curExcelRow.ToString() } );
                setColumnBorder( new object[2] { "J" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new Object[] { 9 }, new Object[] { -4119 } );
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in writeTrickDataDetail:TrickPass " + "\n\nException: " + ex.Message );
            }

            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "1. Ski: " );
            curExcelRow++;
            setCellValue(curExcelRow, "B", "Do the skis meet the requirements of AWSA Rule 8.3 ?");
            setColumnBorders( new object[2] { "J" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "2. Trick  Course: " );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Do the course meet the requirements of AWSA Rule 11.16(a) ?" );
            setColumnBorders( new object[2] { "J" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "3.  All Data/Video on Check List Available to be submitted: " );
            setColumnBorders( new object[2] { "J" + curExcelRow.ToString(), "J" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Video Review:" );
            setColumnFontBold( new object[2] { "B" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Each judge must review the video tape or video clip in regular speed only to ascertain that" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "his submitted judge's score (pink) sheet is correct. " );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "J" + curExcelRow.ToString() } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "AWSA trick " );

            return curExcelRow;
        }

        private int writeJumpDataDetail(int inExcelRow, String inSanctionId, String inMemberId, String inDiv, String inRound) {
            int curExcelRow = inExcelRow;

            DataTable curDataTable = getSkierJumpRecap( inSanctionId, inMemberId, inDiv, inRound );
            DataRow curRow = curDataTable.Rows[0];

            setCellValue( curExcelRow, "A", "JUMP RECORD DATA" );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "G" + curExcelRow.ToString() } );
            setColumnMerge( new object[2] { "A" + curExcelRow.ToString(), "G" + curExcelRow.ToString() } );
            setColumnAlignment( new object[2] { "A" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );

            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "Calculation of the jump" );
            setColumnMerge( new object[2] { "A" + curExcelRow.ToString(), "G" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Distance:" );
            setCellValue( curExcelRow, "D", curRow["ScoreFeet"].ToString() );
            setColumnBorders( new object[2] { "D" + curExcelRow.ToString(), "D" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setCellValue( curExcelRow, "E", "Feet" );
            setCellValue( curExcelRow, "F", curRow["ScoreMeters"].ToString() );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "F" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setCellValue( curExcelRow, "G", "Meters" );

            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "Timing ( in .01 seconds )" );
            setColumnMerge( new object[2] { "A" + curExcelRow.ToString(), "G" + curExcelRow.ToString() } );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "G" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "1st Segment:" );
            setCellValue( curExcelRow, "D", ((Decimal)curRow["Split82Time"]).ToString("##.00") );
            setColumnBorders( new object[2] { "D" + curExcelRow.ToString(), "D" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setCellValue( curExcelRow, "E", "seconds" );
            setCellValue( curExcelRow, "F", ( (Decimal)curRow["Split41Time"] ).ToString( "##.00" ) );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "F" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setCellValue( curExcelRow, "G", "seconds" );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Optional timing 2nd segment method used:" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "F" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "G" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );

			curExcelRow++;
			curExcelRow++;
			curExcelRow = exportBoatPathJumpDataExcel( curExcelRow, inMemberId, inRound, curDataTable );

			curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "Video Jump" );
            setColumnMerge( new object[2] { "A" + curExcelRow.ToString(), "G" + curExcelRow.ToString() } );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "G" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Manufacturer:" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "C" + curExcelRow.ToString() } );
            setColumnMerge( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );

            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Test Buoy:" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "coordinates:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setCellValue( curExcelRow, "F", "X:" );
            setCellValue( curExcelRow, "G", "Y:" );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "survey:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "video system:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Grid System:" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "coordinates:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setCellValue( curExcelRow, "F", "X:" );
            setCellValue( curExcelRow, "G", "Y:" );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Jump:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            setCellValue( curExcelRow, "F", "0" );
            setCellValue( curExcelRow, "G", "0" );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Upper Left:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Upper Right:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Lower Left:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Lower Left:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Ramp check:" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Measure again the height and length of the ramp before changing its position" );
            curExcelRow++;
            setCellValue( curExcelRow, "E", "Height" );
            setCellValue( curExcelRow, "F", "Length" );
            setCellValue( curExcelRow, "G", "Ratio" );
            setColumnAlignment( new object[2] { "E" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Left side:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setCellValue( curExcelRow, "G", "=E" + curExcelRow.ToString() + " / F" + curExcelRow.ToString() );
            setColumnBorders( new object[2] { "E" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "E" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Right side:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setCellValue( curExcelRow, "G", "=E" + curExcelRow.ToString() + " / F" + curExcelRow.ToString() );
            setColumnBorders( new object[2] { "E" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "E" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "F", "Top" );
            setCellValue( curExcelRow, "G", "Bottom" );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Width of ramp:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "D" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "F" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Maximum deviation from plane:" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "G" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Diagonal string measurement:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "E" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "G" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "G" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );
            curExcelRow++;
            setCellValue( curExcelRow, "C", "Center string measurement:" );
            setColumnMerge( new object[2] { "C" + curExcelRow.ToString(), "E" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "G" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            setColumnAlignment( new object[2] { "G" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new object[] { 3 } );

            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "Equipment Check:" );
            setColumnFontBold( new object[2] { "A" + curExcelRow.ToString(), "A" + curExcelRow.ToString() } );
            curExcelRow++;
            setCellValue( curExcelRow, "A", "1. Rope:" );
            setCellValue( curExcelRow, "B", "Note: wait at least 30 minutes after last use to measure the rope." );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Length:" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "F" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "G" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "2. Ski: " );
            curExcelRow++;
            setCellValue( curExcelRow, "B", "Does the ski qualify under W. Rule 10.03 ?" );
            setColumnMerge( new object[2] { "B" + curExcelRow.ToString(), "F" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "G" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "3.  All Data/Video on Check List Avaliable to be submitted:" );
            setColumnMerge( new object[2] { "A" + curExcelRow.ToString(), "F" + curExcelRow.ToString() } );
            setColumnBorders( new object[2] { "G" + curExcelRow.ToString(), "G" + curExcelRow.ToString() }, new Object[] { 1 }, new Object[] { 3 } );
            curExcelRow++;
            curExcelRow++;
            setCellValue( curExcelRow, "A", "AWSA jump " );

            return curExcelRow;
        }

        //Write tournament information
        private bool writeTourData(StringBuilder outLine, DataRow inTourRow) {
            try {
                DataRow curOfficialRow = null;
                DataRow curTechRow = getTechController( inTourRow["SanctionId"].ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Tournament summary as of " + DateTime.Now.ToString( "HH:MM:ss  MM-dd-yyyy" ) );
                myOutBuffer.WriteLine( outLine.ToString() );
                
                outLine = new StringBuilder( "" );
                outLine.Append( "SanctionId" + tabDelim + "Name" + tabDelim + "Class" + tabDelim + "Rules" + tabDelim + "EventDates" + tabDelim + "EventLocation" + tabDelim + "Tour Director" );
                outLine.Append( tabDelim + "Slalom Rds" + tabDelim + "Trick Rds" + tabDelim + "Jump Rds" );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( inTourRow["SanctionId"].ToString() + tabDelim + inTourRow["Name"].ToString() + tabDelim + inTourRow["Class"].ToString() );
                outLine.Append( tabDelim + inTourRow["Rules"].ToString() + tabDelim + inTourRow["EventDates"].ToString() + tabDelim + inTourRow["EventLocation"].ToString() );
                outLine.Append( tabDelim + transformName( inTourRow["ContactName"].ToString() ) );
                outLine.Append( tabDelim + ( (byte)inTourRow["SlalomRounds"] ).ToString( "#0" ) );
                outLine.Append( tabDelim + ( (byte)inTourRow["TrickRounds"] ).ToString( "#0" ) );
                outLine.Append( tabDelim + ( (byte)inTourRow["JumpRounds"] ).ToString( "#0" ) );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Chief Judge" + tabDelim + transformName( inTourRow["ChiefJudgeName"].ToString() ) );
                curOfficialRow = getOfficialInfo( inTourRow["SanctionId"].ToString(), inTourRow["ChiefJudgeMemberId"].ToString() );
				if ( curOfficialRow  != null ) {
					outLine.Append( tabDelim + curOfficialRow["JudgeSlalomRating"].ToString() );
					outLine.Append( tabDelim + inTourRow["ChiefJudgeAddress"].ToString() );
					outLine.Append( tabDelim + inTourRow["ChiefJudgePhone"].ToString() );
					outLine.Append( tabDelim + inTourRow["ChiefJudgeEmail"].ToString() );
					myOutBuffer.WriteLine( outLine.ToString() );
				}

				outLine = new StringBuilder( "" );
                outLine.Append( "Chief Driver" + tabDelim + transformName( inTourRow["ChiefDriverName"].ToString() ) );
                curOfficialRow = getOfficialInfo( inTourRow["SanctionId"].ToString(), inTourRow["ChiefDriverMemberId"].ToString() );
				if ( curOfficialRow != null ) {
					outLine.Append( tabDelim + curOfficialRow["DriverSlalomRating"].ToString() );
					outLine.Append( tabDelim + inTourRow["ChiefDriverAddress"].ToString() );
					outLine.Append( tabDelim + inTourRow["ChiefDriverPhone"].ToString() );
					outLine.Append( tabDelim + inTourRow["ChiefDriverEmail"].ToString() );
					myOutBuffer.WriteLine( outLine.ToString() );
				}

				outLine = new StringBuilder( "" );
                outLine.Append( "Chief Scorer" + tabDelim + transformName( inTourRow["ChiefScorerName"].ToString() ) );
                curOfficialRow = getOfficialInfo( inTourRow["SanctionId"].ToString(), inTourRow["ChiefScorerMemberId"].ToString() );
				if ( curOfficialRow != null ) {
					outLine.Append( tabDelim + curOfficialRow["ScorerSlalomRating"].ToString() );
					outLine.Append( tabDelim + inTourRow["ChiefScorerAddress"].ToString() );
					outLine.Append( tabDelim + inTourRow["ChiefScorerPhone"].ToString() );
					outLine.Append( tabDelim + inTourRow["ChiefScorerEmail"].ToString() );
					myOutBuffer.WriteLine( outLine.ToString() );
				}

				outLine = new StringBuilder( "" );
                outLine.Append( "Safety Director" + tabDelim + transformName( inTourRow["ChiefSafetyName"].ToString() ) );
                curOfficialRow = getOfficialInfo( inTourRow["SanctionId"].ToString(), inTourRow["SafetyDirMemberId"].ToString() );
				if ( curOfficialRow != null ) {
					outLine.Append( tabDelim + curOfficialRow["SafetyOfficialRating"].ToString() );
					outLine.Append( tabDelim + inTourRow["SafetyDirAddress"].ToString() );
					outLine.Append( tabDelim + inTourRow["SafetyDirPhone"].ToString() );
					outLine.Append( tabDelim + inTourRow["SafetyDirEmail"].ToString() );
					myOutBuffer.WriteLine( outLine.ToString() );
				}

				outLine = new StringBuilder( "" );
                outLine.Append( "Tech Controller" );
                if (curTechRow == null) {
                    outLine.Append( tabDelim + "" + tabDelim + "" );
                } else {
                    outLine.Append( tabDelim + transformName( curTechRow["ChiefTechName"].ToString() ) );
                    outLine.Append( tabDelim + curTechRow["TechOfficialRating"].ToString() );
                }
                myOutBuffer.WriteLine( outLine.ToString() );

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData writeTourData method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private bool writeEventOfficials(StringBuilder outLine, String inSanctionId, String inEvent, String inEventGroup, String inRound) {
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
                    curOfficialRow = getOfficialInfo( (String)curRow["SanctionId"], (String)curRow["MemberId"] );
                    if (curOfficialRow != null) {
                        outLine = new StringBuilder( "" );
                        outLine.Append( curRow["WorkAsgmt"].ToString() + tabDelim + transformName( curOfficialRow["SkierName"].ToString() ) + tabDelim );
                        if (inEvent.Equals( "Slalom" )) {
                            outLine.Append( curOfficialRow["JudgeSlalomRating"].ToString() );
                        } else if (inEvent.Equals( "Trick" )) {
                            outLine.Append( curOfficialRow["JudgeTrickRating"].ToString() );
                        } else if (inEvent.Equals( "Jump" )) {
                            outLine.Append( curOfficialRow["JudgeJumpRating"].ToString() );
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
                    curOfficialRow = getOfficialInfo( (String)curRow["SanctionId"], (String)curRow["MemberId"] );
                    if (curOfficialRow != null) {
                        outLine = new StringBuilder( "" );
                        outLine.Append( curRow["WorkAsgmt"].ToString() + tabDelim + transformName(curOfficialRow["SkierName"].ToString()) + tabDelim );
                        if (inEvent.Equals( "Slalom" )) {
                            outLine.Append( curOfficialRow["DriverSlalomRating"].ToString() );
                        } else if (inEvent.Equals( "Trick" )) {
                            outLine.Append( curOfficialRow["DriverTrickRating"].ToString() );
                        } else if (inEvent.Equals( "Jump" )) {
                            outLine.Append( curOfficialRow["DriverJumpRating"].ToString() );
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
                            curOfficialRow = getOfficialInfo( (String)curRow["SanctionId"], (String)curRow["MemberId"] );
                            if (curOfficialRow != null) {
                                outLine.Append( curRow["WorkAsgmt"].ToString() + tabDelim + transformName(curOfficialRow["SkierName"].ToString()) + tabDelim );
                                if (inEvent.Equals( "Slalom" )) {
                                    outLine.Append( curOfficialRow["JudgeSlalomRating"].ToString() );
                                } else if (inEvent.Equals( "Trick" )) {
                                    outLine.Append( curOfficialRow["JudgeTrickRating"].ToString() );
                                } else if (inEvent.Equals( "Jump" )) {
                                    outLine.Append( curOfficialRow["JudgeJumpRating"].ToString() );
                                }
                            }
                        } else {
                            outLine.Append( "Event Judge" + tabDelim + " " + tabDelim + " " );
                        }
                        myOutBuffer.WriteLine( outLine.ToString() );
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
                    curOfficialRow = getOfficialInfo( (String)curRow["SanctionId"], (String)curRow["MemberId"] );
                    if (curOfficialRow != null) {
                        outLine = new StringBuilder( "" );
                        outLine.Append( curRow["WorkAsgmt"].ToString() + tabDelim + transformName(curOfficialRow["SkierName"].ToString()) + tabDelim );
                        if (inEvent.Equals( "Slalom" )) {
                            outLine.Append( curOfficialRow["JudgeSlalomRating"].ToString() );
                        } else if (inEvent.Equals( "Trick" )) {
                            outLine.Append( curOfficialRow["JudgeTrickRating"].ToString() );
                        } else if (inEvent.Equals( "Jump" )) {
                            outLine.Append( curOfficialRow["JudgeJumpRating"].ToString() );
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
                MessageBox.Show( "Exception encountered in ExportRecordAppData writeEventOfficials method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private bool writeSkierInfo(StringBuilder outLine, String inSanctionId, String inEvent, String inMemberId, String inDiv, String inRound) {
            try {
                exportDataTable( outLine, getSkierInfo( inSanctionId, inEvent, inMemberId, inDiv, inRound ) );
                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData writeEventOfficials method"
                    + "\n\nException: " + ex.Message
                );
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

        private int exportDataTableExcel(int inStartRow, DataTable inDataTable) {
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

                return curExcelRow;
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportRecordAppData exportDataTableExcel method"
					+ "\n\nException: " + ex.Message
                );
                return 0;
            }
        }

        private bool exportDataTable(StringBuilder outLine, DataTable inDataTable) {
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
                MessageBox.Show( "Exception encountered in ExportRecordAppData exportDataTable method"
					+ "\n\nException: " + ex.Message
                );
                return false;
            }
        }

		private int exportBoatPathSlalomDataExcel( int inStartRow, String memberId, String round, DataTable curDataTable ) {
			int curExcelRow = inStartRow;
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
						curExcelRow += 2;
						setCellValue( curExcelRow, 1, "Boat Path Monitoring Data" );

						curExcelRow++;
						setCellValue( curExcelRow, 1, "Pass" );
						setCellValue( curExcelRow, 2, "Desc" );
						setCellValue( curExcelRow, 3, "Gate" );
						setCellValue( curExcelRow, 4, "Buoy 1" );
						setCellValue( curExcelRow, 5, "Buoy 2" );
						setCellValue( curExcelRow, 6, "Buoy 3" );
						setCellValue( curExcelRow, 7, "Buoy 4" );
						setCellValue( curExcelRow, 8, "Buoy 5" );
						setCellValue( curExcelRow, 9, "Buoy 6" );
						setCellValue( curExcelRow, 10, "Exit" );
					}

					curExcelRow++;
					int curColNum = 1;
					setCellValue( curExcelRow, curColNum, ( (byte)boatPathRow["PassNumber"] ).ToString() );
					curColNum++;
					setCellValue( curExcelRow, curColNum, "Time" );
					curColNum = 4;
					for ( int rowIdx = 1; rowIdx <= 7; rowIdx++ ) {
						if ( passScore < ( rowIdx - 1 ) ) break;
						setCellValue( curExcelRow, curColNum, ( (Decimal)boatPathRow["boatTimeBuoy" + rowIdx] ).ToString() );
						curColNum++;
					}
					curExcelRow++;
					curColNum = 2;
					setCellValue( curExcelRow, 2, "Dev" );
					curColNum = 3;
					for ( int rowIdx = 0; rowIdx <= 6; rowIdx++ ) {
						if ( passScore < ( rowIdx - 1 ) ) break;
						setCellValue( curExcelRow, curColNum, ( (Decimal)boatPathRow["PathDevBuoy" + rowIdx] ).ToString() );
						curColNum++;
					}

					curExcelRow++;
					curColNum = 2;
					setCellValue( curExcelRow, 2, "Cum" );
					curColNum = 3;
					for ( int rowIdx = 0; rowIdx <= 6; rowIdx++ ) {
						if ( passScore < ( rowIdx - 1 ) ) break;
						setCellValue( curExcelRow, curColNum, ( (Decimal)boatPathRow["PathDevCum" + rowIdx] ).ToString() );
						curColNum++;
					}
				}

			} catch ( Exception ex ) {
				MessageBox.Show( "Exception encountered in ExportRecordAppData exportBoatPathDataExcel method"
					+ "\n\nException: " + ex.Message
				);
			}

			return curExcelRow;
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

		private int exportBoatPathJumpDataExcel( int inStartRow, String memberId, String round, DataTable curDataTable ) {
			int curExcelRow = inStartRow;
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
						curExcelRow++;
						setCellValue( curExcelRow, 1, "Boat Path Monitoring Data" );

						curExcelRow++;
						setCellValue( curExcelRow, 1, "Pass" );
						setCellValue( curExcelRow, 2, "Desc" );
						setCellValue( curExcelRow, 3, "Gate" );
						setCellValue( curExcelRow, 4, "52M" );
						setCellValue( curExcelRow, 5, "82M" );
						setCellValue( curExcelRow, 6, "Exit" );
					}

					curExcelRow++;
					int curColNum = 1;
					setCellValue( curExcelRow, curColNum, ( (byte)boatPathRow["PassNumber"] ).ToString() );
					curColNum++;
					setCellValue( curExcelRow, curColNum, "Time" );
					curColNum = 4;
					for ( int rowIdx = 1; rowIdx <= 4; rowIdx++ ) {
						setCellValue( curExcelRow, curColNum, ( (Decimal)boatPathRow["boatTimeBuoy" + rowIdx] ).ToString() );
						curColNum++;
					}
					curExcelRow++;
					curColNum = 2;
					setCellValue( curExcelRow, 2, "Dev" );
					curColNum = 3;
					for ( int rowIdx = 0; rowIdx <= 3; rowIdx++ ) {
						setCellValue( curExcelRow, curColNum, ( (Decimal)boatPathRow["PathDevBuoy" + rowIdx] ).ToString() );
						curColNum++;
					}

					curExcelRow++;
					curColNum = 2;
					setCellValue( curExcelRow, 2, "Cum" );
					curColNum = 3;
					for ( int rowIdx = 0; rowIdx <= 3; rowIdx++ ) {
						setCellValue( curExcelRow, curColNum, ( (Decimal)boatPathRow["PathDevCum" + rowIdx] ).ToString() );
						curColNum++;
					}
				}

			} catch ( Exception ex ) {
				MessageBox.Show( "Exception encountered in ExportRecordAppData exportBoatPathDataExcel method"
					+ "\n\nException: " + ex.Message
				);
			}

			return curExcelRow;
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
            String curReturnValue = "";
            try {
                int curDelim = inSkierName.IndexOf( ',' );
                String curFirstName = inSkierName.Substring( curDelim + 1 );
                String curLastName = inSkierName.Substring( 0, curDelim );
                curReturnValue = curFirstName + " " + curLastName;
            } catch {
                curReturnValue = inSkierName;
            }
            return curReturnValue;
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
            DataTable curDataTable = getData( curSqlStmt.ToString() );
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
            DataTable curDataTable = getData( curSqlStmt.ToString() );
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
            return getData( curSqlStmt.ToString() );
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
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count > 0) {
                return curDataTable.Rows[0];
            } else {
                return null;
            }
        }

        private DataTable getSkierInfo(String inSanctionId, String inEvent, String inMemberId, String inDiv, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct E.Event, E.MemberId, T.SkierName, E.AgeGroup as Div, T.SkiYearAge, E.TeamCode" );
            curSqlStmt.Append( ", T.State, T.City, COALESCE(S.EventClass, E.EventClass) as EventClass " );
            curSqlStmt.Append( ", B.BoatModel as BoatModel, B.ModelYear, B.SpeedControlVersion " );
            if (inEvent.Equals( "Slalom" )) {
                curSqlStmt.Append( ", Round, S.Score, S.FinalSpeedMPH, S.FinalSpeedKph, S.FinalLen, S.FinalLenOff, S.FinalPassScore, S.LastUpdateDate " );
                curSqlStmt.Append( "FROM EventReg E " );
                curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                curSqlStmt.Append( "     INNER JOIN SlalomScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
            } else if (inEvent.Equals( "Trick" )) {
                curSqlStmt.Append( ", Round, S.Score, S.ScorePass1, S.ScorePass2, S.LastUpdateDate " );
                curSqlStmt.Append( "FROM EventReg E " );
                curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                curSqlStmt.Append( "     INNER JOIN TrickScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
            } else if (inEvent.Equals( "Jump" )) {
                curSqlStmt.Append( ", Round, S.ScoreFeet as Score, S.ScoreFeet, S.ScoreMeters, S.RampHeight, S.BoatSpeed as BoatSpeedKph, MinValue as BoatSpeedMph, S.LastUpdateDate " );
                curSqlStmt.Append( "FROM EventReg E " );
                curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                curSqlStmt.Append( "     INNER JOIN JumpScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
				curSqlStmt.Append( "     INNER JOIN CodeValueList ON ListName = 'JumpSpeeds' AND MaxValue = S.BoatSpeed" );
            }
            curSqlStmt.Append( "     LEFT OUTER JOIN TourBoatUse B ON B.SanctionId = T.SanctionId AND B.HullId = S.Boat " );
            curSqlStmt.Append( "WHERE E.SanctionId = '" + inSanctionId + "' AND E.MemberId = '" + inMemberId + "' AND E.AgeGroup = '" + inDiv + "' AND E.Event = '" + inEvent + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSkierSlalomScore(String inSanctionId, String inMemberId, String inAgeGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MemberId, AgeGroup as Div, Round, EventClass" );
            curSqlStmt.Append( ", Score, NopsScore, StartLen, StartSpeed, Boat" );
            curSqlStmt.Append( ", FinalSpeedMph, FinalSpeedKph, FinalLen, FinalLenOff, FinalPassScore, Note " );
            curSqlStmt.Append( " FROM SlalomScore " );
            curSqlStmt.Append( " WHERE SanctionId = '" + inSanctionId + "' AND MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "   AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound );
            curSqlStmt.Append( " ORDER BY SanctionId, MemberId" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSkierSlalomRecap(String inSanctionId, String inMemberId, String inAgeGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SkierRunNum, LastUpdateDate, PassLineLength, PassSpeedKph, Score, BoatTime, TimeInTol" );
            curSqlStmt.Append( ", Judge1Score, Judge2Score, Judge3Score, Judge4Score, Judge5Score" );
            //curSqlStmt.Append( ", EntryGate1, EntryGate2, EntryGate3, ExitGate1, ExitGate2, ExitGate3" );
            curSqlStmt.Append( ", Reride, RerideReason, ScoreProt, Note " );
            curSqlStmt.Append( "FROM SlalomRecap " );
            curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' AND MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound );
            curSqlStmt.Append( " ORDER BY SanctionId, MemberId, AgeGroup, Round, SkierRunNum" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSkierTrickScore(String inSanctionId, String inMemberId, String inAgeGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MemberId, AgeGroup as Div, Round, EventClass" );
            curSqlStmt.Append( ", Score, ScorePass1, ScorePass2, NopsScore, Boat, Note " );
            curSqlStmt.Append( "FROM TrickScore " );
            curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' AND MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound );
            curSqlStmt.Append( "ORDER BY SanctionId, MemberId" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSkierTrickPass(String inSanctionId, String inMemberId, String inAgeGroup, String inRound, String inPass) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PassNum, Skis, Seq, Code, Results, Score, Note " );
            curSqlStmt.Append( "FROM TrickPass " );
            curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' AND MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound + " AND PassNum = " + inPass + " " );
            curSqlStmt.Append( "ORDER BY SanctionId, MemberId, AgeGroup, Round, PassNum, Seq" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSkierJumpScore(String inSanctionId, String inMemberId, String inAgeGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MemberId, AgeGroup as Div, Round, EventClass" );
            curSqlStmt.Append( ", ScoreFeet as Score, ScoreFeet, ScoreMeters, NopsScore, BoatSpeed, RampHeight, Boat, Note " );
            curSqlStmt.Append( "FROM JumpScore " );
            curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' AND MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound + " " );
            curSqlStmt.Append( "ORDER BY SanctionId, MemberId" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSkierJumpRecap(String inSanctionId, String inMemberId, String inAgeGroup, String inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT R.PassNum, R.LastUpdateDate, R.RampHeight, R.BoatSpeed as BoatSpeedKph, MinValue as BoatSpeedMph" );
            curSqlStmt.Append( ", R.ScoreFeet, R.ScoreMeters, Results" );
            curSqlStmt.Append( ", BoatSplitTime2 as Split82Time, BoatEndTime as Split41Time, BoatSplitTime as Split52Time" );
            curSqlStmt.Append( ", ReturnToBase, ScoreProt, Reride, RerideReason, R.Note as RideNote, S.Note as ScoreNote " );
            curSqlStmt.Append( "FROM JumpScore S " );
            curSqlStmt.Append( "  INNER JOIN JumpRecap R on R.SanctionId = S.SanctionId AND R.MemberId = S.MemberId AND R.AgeGroup = S.AgeGroup AND R.Round = S.Round " );
            curSqlStmt.Append( "             AND R.ScoreFeet = S.ScoreFeet AND R.ScoreMeters = S.ScoreMeters " );
			curSqlStmt.Append( "  INNER JOIN CodeValueList ON ListName = 'JumpSpeeds' AND MaxValue = R.BoatSpeed " );
			curSqlStmt.Append( "WHERE S.SanctionId = '" + inSanctionId + "' AND S.MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "  AND S.AgeGroup = '" + inAgeGroup + "' AND S.Round = " + inRound + " " );
            curSqlStmt.Append( "ORDER BY S.SanctionId, S.MemberId, S.AgeGroup, S.Round, PassNum" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getJumpMeterSetup(String inSanctionId) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM JumpMeterSetup WHERE SanctionId = '" + inSanctionId + "' " );
            return getData( curSqlStmt.ToString() );
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

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
